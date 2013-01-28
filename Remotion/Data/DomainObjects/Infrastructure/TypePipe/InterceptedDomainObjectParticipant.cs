// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Scripting.Ast;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  public class InterceptedDomainObjectParticipant : IParticipant
  {
    private static readonly MethodInfo s_getPublicDomainObjectTypeImplementation = GetInfrastructureHook ("GetPublicDomainObjectTypeImplementation");
    private static readonly MethodInfo s_performConstructorCheck = GetInfrastructureHook ("PerformConstructorCheck");

    private static readonly PropertyInfo s_properties = MemberInfoFromExpressionUtility.GetProperty ((DomainObject o) => o.Properties);
    private static readonly MethodInfo s_getPropertyAccessor = MemberInfoFromExpressionUtility.GetMethod ((PropertyIndexer i) => i["propertyName"]);

    private static readonly MethodInfo s_propertyGetValue =
        MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((PropertyAccessor o) => o.GetValue<object>());
    private static readonly MethodInfo s_propertySetValue =
        MemberInfoFromExpressionUtility.GetGenericMethodDefinition ((PropertyAccessor o) => o.SetValue<object> (null));

    private static readonly MethodInfo s_preparePropertyAccess =
        MemberInfoFromExpressionUtility.GetMethod (() => CurrentPropertyManager.PreparePropertyAccess ("propertyName"));
    private static readonly MethodInfo s_propertyAccessFinished =
        MemberInfoFromExpressionUtility.GetMethod (() => CurrentPropertyManager.PropertyAccessFinished());

    private static MethodInfo GetInfrastructureHook (string name)
    {
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      var method = typeof (DomainObject).GetMethod (name, bindingFlags);
      Assertion.IsNotNull (method);

      return method;
    }

    private readonly ITypeDefinitionProvider _typeDefinitionProvider;
    private readonly IInterceptedPropertyFinder _interceptedPropertyFinder;
    private readonly IRelatedMethodFinder _relatedMethodFinder;

    public InterceptedDomainObjectParticipant (
        ITypeDefinitionProvider typeDefinitionProvider, IInterceptedPropertyFinder interceptedPropertyFinder, IRelatedMethodFinder relatedMethodFinder)
    {
      ArgumentUtility.CheckNotNull ("typeDefinitionProvider", typeDefinitionProvider);
      ArgumentUtility.CheckNotNull ("interceptedPropertyFinder", interceptedPropertyFinder);
      ArgumentUtility.CheckNotNull ("relatedMethodFinder", relatedMethodFinder);

      _typeDefinitionProvider = typeDefinitionProvider;
      _interceptedPropertyFinder = interceptedPropertyFinder;
      _relatedMethodFinder = relatedMethodFinder;
    }

    public ICacheKeyProvider PartialCacheKeyProvider { get; private set; }

    public void ModifyType (ProxyType proxyType)
    {
      ArgumentUtility.CheckNotNull ("proxyType", proxyType);
      Assertion.IsTrue (typeof (DomainObject).IsAssignableFromFast (proxyType));

      // TODO 5370: This will change when TypePipe is integrated with re-mix.
      var concreteBaseType = proxyType.BaseType;
      var domainObjectType = _typeDefinitionProvider.GetPublicDomainObjectType (concreteBaseType);

      // Add marker interface.
      proxyType.AddInterface (typeof (IInterceptedDomainObject));

      // Override infrastructure hooks on DomainObject.
      OverridePerformConstructorCheck (proxyType);
      OverrideGetPublicDomainObjectType (proxyType, domainObjectType);

      // Intercept properties.
      var properties = _interceptedPropertyFinder.GetProperties (domainObjectType);
      ProcessProperties (proxyType, properties);

      // Implement ISerializable (see TypeGenerator).
      // For now, serialization is not supported.
      // TODO 5370: Use TypePipe serialization capabilities, after TypePipe is integrated with re-mix.
    }

    private void OverridePerformConstructorCheck (ProxyType proxyType)
    {
      proxyType.GetOrAddOverride (s_performConstructorCheck).SetBody (ctx => Expression.Empty());
    }

    private void OverrideGetPublicDomainObjectType (ProxyType proxyType, Type publicDomainObjectType)
    {
      proxyType.GetOrAddOverride (s_getPublicDomainObjectTypeImplementation).SetBody (ctx => Expression.Constant (publicDomainObjectType));
    }

    private void ProcessProperties (ProxyType proxyType, IEnumerable<Tuple<PropertyInfo, string>> properties)
    {
      foreach (var property in properties)
        ProcessProperty (proxyType, property.Item1, property.Item2);
    }

    private void ProcessProperty (ProxyType proxyType, PropertyInfo property, string propertyIdentifier)
    {
      //var getMethod = property.GetGetMethod (true);
      //var setMethod = property.GetSetMethod (true);
      var getMethod = property.GetGetMethod ();
      var setMethod = property.GetSetMethod ();

      // TODO 5370: Check if this is still needed (probably not!) as proxyType.BaseType directly is the DoaminObject subclass created by the programmer.
      var mostDerivedGetOverride = getMethod != null ? _relatedMethodFinder.GetMostDerivedOverride (getMethod.GetBaseDefinition(), proxyType) : null;
      var mostDerivedSetOverride = setMethod != null ? _relatedMethodFinder.GetMostDerivedOverride (setMethod.GetBaseDefinition(), proxyType) : null;

      if (_interceptedPropertyFinder.IsOverridable (mostDerivedGetOverride))
      {
        var getter = proxyType.GetOrAddOverride (mostDerivedGetOverride);
        if (_interceptedPropertyFinder.IsAutomaticPropertyAccessor (mostDerivedGetOverride))
          getter.SetBody (ctx => ImplementByCalling (ctx.This, property.PropertyType, propertyIdentifier, s_propertyGetValue));
        else
          getter.SetBody (ctx => WrapAccessorBody (ctx.PreviousBody, propertyIdentifier));
      }

      if (_interceptedPropertyFinder.IsOverridable (mostDerivedSetOverride))
      {
        var setter = proxyType.GetOrAddOverride (setMethod);
        if (_interceptedPropertyFinder.IsAutomaticPropertyAccessor (mostDerivedSetOverride))
            // TODO: Last? (are multi-indexed properties really supported?)
          setter.SetBody (ctx => ImplementByCalling (ctx.This, property.PropertyType, propertyIdentifier, s_propertySetValue, ctx.Parameters.Last()));
        else
          setter.SetBody (ctx => WrapAccessorBody (ctx.PreviousBody, propertyIdentifier));
      }
    }

    private Expression ImplementByCalling (
        Expression @this, Type propertyType, string propertyIdentifier, MethodInfo propertyAccessorMethod, params Expression[] arguments)
    {
      var propertyIndexer = Expression.Property (@this, s_properties);
      var propertyAccessor = Expression.Call (propertyIndexer, s_getPropertyAccessor, Expression.Constant (propertyIdentifier));
      var body = Expression.Call (propertyAccessor, propertyAccessorMethod.MakeGenericMethod (propertyType), arguments);

      return WrapAccessorBody (body, propertyIdentifier);
    }

    private Expression WrapAccessorBody (Expression body, string propertyIdentifier)
    {
      return Expression.Block (
          Expression.Call (s_preparePropertyAccess, Expression.Constant (propertyIdentifier)),
          Expression.TryFinally (
              body,
              Expression.Call (s_propertyAccessFinished)));
    }
  }
}