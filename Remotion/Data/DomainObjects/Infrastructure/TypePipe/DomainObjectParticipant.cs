﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.TypePipe.Dlr.Ast;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.TypePipe;
using Remotion.TypePipe.Caching;
using Remotion.TypePipe.MutableReflection;
using Remotion.TypePipe.TypeAssembly;
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
  ///   <item>
  ///     Implements or wraps intercepted properties (i.e., properties for which <see cref="IInterceptedPropertyFinder"/> returns interceptors).
  ///   </item>
  /// </list>
  /// Note that serialization is currently not supported.
  /// </remarks>
  public class DomainObjectParticipant : SimpleParticipantBase
  {
    private static readonly MethodInfo s_getPublicDomainObjectTypeImplementation = GetInfrastructureHook ("GetPublicDomainObjectTypeImplementation");
    private static readonly MethodInfo s_performConstructorCheck = GetInfrastructureHook ("PerformConstructorCheck");

    private static MethodInfo GetInfrastructureHook (string name)
    {
      var bindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;
      var method = typeof (DomainObject).GetMethod (name, bindingFlags);
      Assertion.IsNotNull (method);

      return method;
    }

    private readonly ITypeDefinitionProvider _typeDefinitionProvider;
    private readonly IInterceptedPropertyFinder _interceptedPropertyFinder;

    public DomainObjectParticipant (ITypeDefinitionProvider typeDefinitionProvider, IInterceptedPropertyFinder interceptedPropertyFinder)
    {
      ArgumentUtility.CheckNotNull ("typeDefinitionProvider", typeDefinitionProvider);
      ArgumentUtility.CheckNotNull ("interceptedPropertyFinder", interceptedPropertyFinder);

      _typeDefinitionProvider = typeDefinitionProvider;
      _interceptedPropertyFinder = interceptedPropertyFinder;
    }

    // Assuming a stable mapping, we do not need any additional keys.
    // Note: To support modifiable mappings, we could use the ClassDefinition as cache key. However, there is no good way to recreate a 
    // ClassDefinition within the generated code (without relying on a stable mapping) or to deserialize a ClassDefinition (without a stable mapping).
    public override ITypeIdentifierProvider PartialTypeIdentifierProvider
    {
      get { return null; }
    }

    public override void Participate (object id, IProxyTypeAssemblyContext proxyTypeAssemblyContext)
    {
      ArgumentUtility.CheckNotNull ("proxyTypeAssemblyContext", proxyTypeAssemblyContext);

      if (!typeof (DomainObject).IsTypePipeAssignableFrom (proxyTypeAssemblyContext.RequestedType))
        return;

      var proxyType = proxyTypeAssemblyContext.ProxyType;
      var domainObjectType = proxyTypeAssemblyContext.RequestedType;

      var classDefinition = _typeDefinitionProvider.GetTypeDefinition (domainObjectType);
      if (classDefinition == null)
        return;

      // Add marker interface.
      proxyType.AddInterface (typeof (IInterceptedDomainObject));

      // Override infrastructure hooks on DomainObject.
      OverridePerformConstructorCheck (proxyType);
      OverrideGetPublicDomainObjectType (proxyType, domainObjectType);

      // Intercept properties.
      InterceptProperties (proxyType, domainObjectType, classDefinition);
    }

    public override void HandleNonSubclassableType (Type requestedType)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      if (typeof (DomainObject).IsTypePipeAssignableFrom (requestedType))
      {
        var message = string.Format ("The requested type '{0}' is derived from DomainObject but cannot be subclassed.", requestedType.Name);
        throw new NotSupportedException (message);
      }
    }

    private void OverridePerformConstructorCheck (MutableType proxyType)
    {
      proxyType.GetOrAddOverride (s_performConstructorCheck).SetBody (ctx => Expression.Empty());
    }

    private void OverrideGetPublicDomainObjectType (MutableType proxyType, Type publicDomainObjectType)
    {
      proxyType.GetOrAddOverride (s_getPublicDomainObjectTypeImplementation).SetBody (ctx => Expression.Constant (publicDomainObjectType));
    }

    private void InterceptProperties (MutableType proxyType, Type domainObjectType, ClassDefinition classDefinition)
    {
      var accessorInterceptors = _interceptedPropertyFinder.GetPropertyInterceptors (classDefinition, domainObjectType);

      foreach (var interceptor in accessorInterceptors)
        interceptor.Intercept (proxyType);
    }
  }
}