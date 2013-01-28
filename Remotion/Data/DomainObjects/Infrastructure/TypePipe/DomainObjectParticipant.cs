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
using Remotion.TypePipe.MutableReflection.BodyBuilding;
using Remotion.TypePipe.MutableReflection.Implementation;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// A TypePipe <see cref="IParticipant"/> that specifies the code generation needs necessary for re-store.
  /// The proxy base type (i.e. requested type) is assumed to be a subclass of <see cref="DomainObject"/>.
  /// </summary>
  /// <remarks>
  /// This <see cref="IParticipant"/> applies the following modifications.
  /// <list type="bullet">
  ///   <item>Adds marker interface <see cref="IInterceptedDomainObject"/>.</item>
  ///   <item>
  ///     Overrides infrastructure methods <see cref="DomainObject.PerformConstructorCheck"/> and 
  ///     <see cref="DomainObject.GetPublicDomainObjectTypeImplementation"/> on <see cref="DomainObject"/>.
  ///   </item>
  ///   <item>Implements or wraps intercepted properties (i.e. properties returned by <see cref="IInterceptedPropertyFinder"/>).</item>
  /// </list>
  /// Note that serialization is currently not supported.
  /// </remarks>
  public class DomainObjectParticipant : IParticipant
  {
    private class CacheKeyProvider : ICacheKeyProvider
    {
      private readonly ITypeDefinitionProvider _typeDefinitionProvider;

      public CacheKeyProvider (ITypeDefinitionProvider typeDefinitionProvider)
      {
        _typeDefinitionProvider = typeDefinitionProvider;
      }

      public object GetCacheKey (Type requestedType)
      {
        ArgumentUtility.CheckNotNull ("requestedType", requestedType);

        return _typeDefinitionProvider.GetTypeDefinition (requestedType);
      }
    }

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

    public DomainObjectParticipant (
        ITypeDefinitionProvider typeDefinitionProvider, IInterceptedPropertyFinder interceptedPropertyFinder, IRelatedMethodFinder relatedMethodFinder)
    {
      ArgumentUtility.CheckNotNull ("typeDefinitionProvider", typeDefinitionProvider);
      ArgumentUtility.CheckNotNull ("interceptedPropertyFinder", interceptedPropertyFinder);
      ArgumentUtility.CheckNotNull ("relatedMethodFinder", relatedMethodFinder);

      _typeDefinitionProvider = typeDefinitionProvider;
      _interceptedPropertyFinder = interceptedPropertyFinder;
      _relatedMethodFinder = relatedMethodFinder;
    }

    public ICacheKeyProvider PartialCacheKeyProvider
    {
      get { return new CacheKeyProvider (_typeDefinitionProvider); }
    }

    public void ModifyType (ProxyType proxyType)
    {
      ArgumentUtility.CheckNotNull ("proxyType", proxyType);
      Assertion.IsTrue (typeof (DomainObject).IsAssignableFromFast (proxyType));

      // TODO 5370: This will change when TypePipe is integrated with re-mix.
      var concreteBaseType = proxyType.BaseType;
      var domainObjectType = _typeDefinitionProvider.GetPublicDomainObjectType (concreteBaseType);
      CheckClassDefinition (domainObjectType);

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
      foreach (var propertyMapping in properties)
      {
        var property = propertyMapping.Item1;
        var propertyIdentifier = propertyMapping.Item2;

        var getter = property.GetGetMethod (true);
        var setter = property.GetSetMethod (true);

        ProcessAccessor (proxyType, getter, property.PropertyType, propertyIdentifier, s_propertyGetValue, ctx => new Expression[0]);
        ProcessAccessor (proxyType, setter, property.PropertyType, propertyIdentifier, s_propertySetValue, ctx => new[] { ctx.Parameters.Last() });
        // TODO xxx: Last? (are multi-indexed properties really supported?)
      }
    }

    private void ProcessAccessor (
        ProxyType proxyType,
        MethodInfo accessor,
        Type propertyType,
        string propertyIdentifier,
        MethodInfo accessorImplementationMethod,
        Func<MethodBodyModificationContext, IEnumerable<Expression>> argumentProvider)
    {
      if (accessor == null)
        return;

      var mostDerivedAccessor = _relatedMethodFinder.GetMostDerivedOverride (accessor.GetBaseDefinition(), proxyType);

      if (_interceptedPropertyFinder.IsOverridable (mostDerivedAccessor))
      {
        var getter = proxyType.GetOrAddOverride (mostDerivedAccessor);
        if (_interceptedPropertyFinder.IsAutomaticPropertyAccessor (mostDerivedAccessor))
        {
          var instantiatedMethod = accessorImplementationMethod.MakeGenericMethod (propertyType);
          getter.SetBody (ctx => ImplementByCalling (ctx.This, propertyIdentifier, instantiatedMethod, argumentProvider (ctx)));
        }
        else
          getter.SetBody (ctx => WrapAccessorBody (ctx.PreviousBody, propertyIdentifier));
      }
    }

    private Expression ImplementByCalling (
        Expression @this, string propertyIdentifier, MethodInfo accessorImplementationMethod, IEnumerable<Expression> arguments)
    {
      var propertyIndexer = Expression.Property (@this, s_properties);
      var propertyAccessor = Expression.Call (propertyIndexer, s_getPropertyAccessor, Expression.Constant (propertyIdentifier));
      var body = Expression.Call (propertyAccessor, accessorImplementationMethod, arguments);

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

    private void CheckClassDefinition (Type domainObjectType)
    {
      // TODO xxx: Retrieve class definition just for checking that it is non-abstract?
      var classDefinition = _typeDefinitionProvider.GetTypeDefinition (domainObjectType);
      if (classDefinition.IsAbstract)
      {
        var message = string.Format (
            "Cannot instantiate type '{0}' as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.",
            classDefinition.ClassType.FullName);
        throw new NonInterceptableTypeException (message, classDefinition.ClassType);
      }
    }
  }
}