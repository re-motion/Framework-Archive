/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;
using Remotion.Utilities;
using System.Reflection;

namespace Remotion.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Provides functionality for creating instances of DomainObjects which intercept property calls.
  /// </summary>
  public class InterceptedDomainObjectFactory : IDomainObjectFactory
  {
    private struct CodeGenerationKeys
    {
      public readonly Type PublicDomainObjectType;
      public readonly Type TypeToDeriveFrom;

      public CodeGenerationKeys (Type publicDomainObjectType, Type typeToDeriveFrom)
      {
        ArgumentUtility.CheckNotNull ("publicDomainObjectType", publicDomainObjectType);
        ArgumentUtility.CheckNotNull ("typeToDeriveFrom", typeToDeriveFrom);

        PublicDomainObjectType = publicDomainObjectType;
        TypeToDeriveFrom = typeToDeriveFrom;
      }
    }

    private readonly ModuleManager _scope;
    private readonly InterlockedCache<CodeGenerationKeys, Type> _typeCache = new InterlockedCache<CodeGenerationKeys, Type> ();

    /// <summary>
    /// Initializes a new instance of the <see cref="InterceptedDomainObjectFactory"/> class.
    /// </summary>
    public InterceptedDomainObjectFactory ()
        : this (Environment.CurrentDirectory)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="InterceptedDomainObjectFactory"/> class.
    /// </summary>
    /// <param name="assemblyDirectory">The directory to save the generated assemblies to. This directory is only used when
    /// <see cref="SaveGeneratedAssemblies"/> is used.</param>
    public InterceptedDomainObjectFactory (string assemblyDirectory)
    {
      _scope = new ModuleManager (assemblyDirectory);
    }

    /// <summary>
    /// Gets the <see cref="ModuleManager"/> scope used by this factory.
    /// </summary>
    /// <value>The scope used by this factory to generate code.</value>
    public ModuleManager Scope
    {
      get { return _scope; }
    }

    /// <summary>
    /// Saves the assemblies generated by the factory and returns the paths of the saved manifest modules.
    /// </summary>
    /// <returns>The paths of the manifest modules of the saved assemblies.</returns>
    public string[] SaveGeneratedAssemblies ()
    {
      return _scope.SaveAssemblies ();
    }

    /// <summary>
    /// Gets a domain object type assignable to the given base type which intercepts property calls.
    /// </summary>
    /// <param name="baseType">The base domain object type whose properties should be intercepted.</param>
    /// <returns>A domain object type which intercepts property calls.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="baseType"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="baseType"/> cannot be assigned to <see cref="DomainObject"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="baseType"/> is an abstract, non-instantiable type.</exception>
    /// <exception cref="MappingException">The given <paramref name="baseType"/> is not part of the mapping.</exception>
    public Type GetConcreteDomainObjectType (Type baseType)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("baseType", baseType, typeof (DomainObject));

      ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (baseType);
      Type concreteBaseType = DomainObjectMixinCodeGenerationBridge.GetConcreteType (baseType);
      
      return GetConcreteDomainObjectType (classDefinition, concreteBaseType, "baseType");
    }

    /// <summary>
    /// Gets a domain object type assignable to the given base type which intercepts property calls.
    /// </summary>
    /// <param name="baseTypeClassDefinition">The base domain object type whose properties should be intercepted.</param>
    /// <param name="concreteBaseType">The base domain object type whose properties should be intercepted.</param>
    /// <returns>A domain object type which intercepts property calls.</returns>
    /// <exception cref="ArgumentNullException">One of the parameters passed to this method is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="concreteBaseType"/> cannot be assigned to the type specified by <paramref name="baseTypeClassDefinition"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="baseTypeClassDefinition"/> denotes an abstract, non-instantiable type.</exception>
    public Type GetConcreteDomainObjectType (ClassDefinition baseTypeClassDefinition, Type concreteBaseType)
    {
      ArgumentUtility.CheckNotNull ("baseTypeClassDefinition", baseTypeClassDefinition);
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("concreteBaseType", concreteBaseType, baseTypeClassDefinition.ClassType);

      return GetConcreteDomainObjectType(baseTypeClassDefinition, concreteBaseType, "baseTypeClassDefinition");
    }

    private Type GetConcreteDomainObjectType (ClassDefinition baseTypeClassDefinition, Type concreteBaseType, string argumentNameForExceptions)
    {
      if (baseTypeClassDefinition.IsAbstract)
      {
        string message = string.Format (
          "Cannot instantiate type {0} as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.",
          baseTypeClassDefinition.ClassType.FullName);
        throw new ArgumentException (message, argumentNameForExceptions);
      }

      try
      {
        return _typeCache.GetOrCreateValue (new CodeGenerationKeys (baseTypeClassDefinition.ClassType, concreteBaseType),
            CreateConcreteDomainObjectType);
      }
      catch (NonInterceptableTypeException ex)
      {
        throw new ArgumentException (ex.Message, argumentNameForExceptions, ex);
      }
    }

    private Type CreateConcreteDomainObjectType (CodeGenerationKeys codeGenerationData)
    {
      TypeGenerator generator = _scope.CreateTypeGenerator (codeGenerationData.PublicDomainObjectType, codeGenerationData.TypeToDeriveFrom);
      return generator.BuildType ();
    }

    /// <summary>
    /// Checkes whether a given domain object type was created by this factory implementation (but not necessarily the same factory instance).
    /// </summary>
    /// <param name="type">The type to be checked.</param>
    /// <returns>True if <paramref name="type"/> was created by an instance of the <see cref="InterceptedDomainObjectFactory"/>; false otherwise.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="type"/> parameter was null.</exception>
    public bool WasCreatedByFactory (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      return typeof (IInterceptedDomainObject).IsAssignableFrom (type);
    }

    /// <summary>
    /// Returns a construction object that can be used to instantiate objects of a given interceptable type.
    /// </summary>
    /// <typeparam name="TStaticType">The type statically returned by the construction object.</typeparam>
    /// <param name="dynamicType">The exatct interceptable type to be constructed; this must be a type returned by <see cref="GetConcreteDomainObjectType(Type)"/>.
    /// <typeparamref name="TStaticType"/> must be assignable from this type.</param>
    /// <returns>A construction object, which instantiates <paramref name="dynamicType"/> and returns <typeparamref name="TStaticType"/>.</returns>
    /// <exception cref="ArgumentNullException">The <paramref name="dynamicType"/> argument is null.</exception>
    /// <exception cref="ArgumentTypeException"><paramref name="dynamicType"/> is not the same or a subtype of <typeparamref name="TStaticType"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="dynamicType"/> wasn't created by this kind of factory.</exception>
    public IFuncInvoker<TStaticType> GetTypesafeConstructorInvoker<TStaticType> (Type dynamicType)
        where TStaticType : DomainObject
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("dynamicType", dynamicType, typeof (TStaticType));
      if (!WasCreatedByFactory (dynamicType))
        throw new ArgumentException (
            string.Format ("The type {0} was not created by InterceptedDomainObjectFactory.GetConcreteDomainObjectType.", dynamicType.FullName), "dynamicType");
      return TypesafeDomainObjectActivator.CreateInstance<TStaticType> (dynamicType.BaseType, dynamicType, 
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
    }

    /// <summary>
    /// Prepares an instance which has not been created via <see cref="GetTypesafeConstructorInvoker{TStaticType}"/> for use. This operation
    /// is a no-op for this implementation of <see cref="IDomainObjectFactory"/>.
    /// </summary>
    /// <param name="instance">The instance to be prepared</param>
    /// <exception cref="ArgumentNullException">The <paramref name="instance"/> argument is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="instance"/> is not of a type created by this kind of factory.</exception>
    public void PrepareUnconstructedInstance (DomainObject instance)
    {
      ArgumentUtility.CheckNotNull ("instance", instance);
      Type type = ((object)instance).GetType ();
      if (!WasCreatedByFactory (type))
        throw new ArgumentException (
            string.Format ("The domain object's type {0} was not created by InterceptedDomainObjectFactory.GetConcreteDomainObjectType.",
              type.FullName), "instance");

      DomainObjectMixinCodeGenerationBridge.PrepareUnconstructedInstance (instance);
    }
  }
}
