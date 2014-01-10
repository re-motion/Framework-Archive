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
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;
using Remotion.Collections;
using Remotion.FunctionalProgramming;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.ServiceLocation
{
  /// <summary>
  /// Provides a default implementation of the <see cref="IServiceLocator"/> interface based on the <see cref="ImplementationForAttribute"/>.
  /// The <see cref="SafeServiceLocator"/> uses (and registers) an instance of this class unless an application registers its own service locator via 
  /// <see cref="ServiceLocator.SetLocatorProvider"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// This implementation of <see cref="IServiceLocator"/> uses the <see cref="ImplementationForAttribute"/> to resolve implementations of
  /// "service types" (usually interfaces or abstract classes). When the <see cref="DefaultServiceLocator"/> is asked to get an instance of a specific 
  /// service type for the first time, that type is checked for a <see cref="ImplementationForAttribute"/>, which is then inspected to determine 
  /// the actual concrete type to be instantiated, its lifetime, and similar properties. An instance is then returned that fulfills the properties 
  /// defined by the <see cref="ImplementationForAttribute"/>. After the first resolution of a service type, the instance (or a factory, 
  /// depending on the <see cref="LifetimeKind"/> associated with the type) is cached, so subsequent lookups for the same type are very fast.
  /// </para>
  /// <para>
  /// The <see cref="DefaultServiceLocator"/> also provides a set of <see cref="DefaultServiceLocator.Register(ServiceConfigurationEntry)"/> methods 
  /// that allow to registration of custom 
  /// implementations or factories for service types even if those types do not have the <see cref="ImplementationForAttribute"/> applied. 
  /// Applications can use this to override the configuration defined by the <see cref="ImplementationForAttribute"/> and to register 
  /// implementations of service types that do not have the <see cref="ImplementationForAttribute"/> applied. Custom implementations or factories
  /// must be registered before an instance of the respective service type is retrieved for the first time.
  /// </para>
  /// <para>
  /// In order to be instantiable by the <see cref="DefaultServiceLocator"/>, a concrete type indicated by the 
  /// <see cref="ImplementationForAttribute"/> must have exactly one public constructor. The constructor may have parameters, in which case
  /// the <see cref="DefaultServiceLocator"/> will try to get an instance for each of the parameters using the same <see cref="IServiceLocator"/>
  /// methods. If a parameter cannot be resolved (because the parameter type has no <see cref="ImplementationForAttribute"/> applied and no
  /// custom implementation or factory was manually registered), an exception is thrown. Dependency cycles are not detected and will lead to a 
  /// <see cref="StackOverflowException"/> or infinite loop. Use the <see cref="Register(ServiceConfigurationEntry)"/> method to manually 
  /// register a factory for types that do not apply to these constructor rules.
  /// </para>
  /// <para>
  /// In order to have a custom service locator use the same defaults as the <see cref="DefaultServiceLocator"/>, the 
  /// <see cref="DefaultServiceConfigurationDiscoveryService"/> can be used to extract those defaults from a set of types.
  /// </para>
  /// </remarks>
  /// <threadsafety static="true" instance="true" />
  public class DefaultServiceLocator : IServiceLocator
  {
    private class Registration
    {
      public readonly Func<object> SingleFactory;
      public readonly Func<object> CompoundFactory;
      public readonly IReadOnlyCollection<Func<object>> MultipleFactories;

      public Registration (
          Type serviceType,
          Func<object> singleFactory,
          Func<object> compoundFactory,
          IReadOnlyCollection<Func<object>> multipleFactories)
      {
        if (singleFactory != null && multipleFactories.Any())
        {
          throw new ArgumentException (
              string.Format ("Service type '{0}': Single and Multiple registration types are mutually exclusive.", serviceType));
        }

        if (singleFactory != null && compoundFactory != null)
        {
          throw new ArgumentException (
              string.Format ("Service type '{0}': Single and Compound registration types are mutually exclusive.", serviceType));
        }

        SingleFactory = singleFactory;
        CompoundFactory = compoundFactory;
        MultipleFactories = multipleFactories;
      }
    }

    private readonly IServiceConfigurationDiscoveryService _serviceConfigurationDiscoveryService;

    public static DefaultServiceLocator Create ()
    {
      return new DefaultServiceLocator (DefaultServiceConfigurationDiscoveryService.Create());
    }

    private static readonly MethodInfo s_resolveIndirectDependencyMethod =
        MemberInfoFromExpressionUtility.GetMethod ((DefaultServiceLocator sl) => sl.ResolveIndirectDependency<object> (null))
        .GetGenericMethodDefinition();
    private static readonly MethodInfo s_resolveIndirectCollectionDependencyMethod =
        MemberInfoFromExpressionUtility.GetMethod ((DefaultServiceLocator sl) => sl.ResolveIndirectCollectionDependency<object> (null))
        .GetGenericMethodDefinition();

    private readonly IDataStore<Type, Registration> _dataStore = DataStoreFactory.CreateWithLocking<Type, Registration>();

    private DefaultServiceLocator (IServiceConfigurationDiscoveryService serviceConfigurationDiscoveryService)
    {
      _serviceConfigurationDiscoveryService = serviceConfigurationDiscoveryService;
      Register (typeof(ILogManager), typeof(Log4NetLogManager), LifetimeKind.Singleton);
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ImplementationForAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The 
    /// <see cref="ImplementationForAttribute"/> could not be found on the <paramref name="serviceType"/>, or the concrete implementation could
    /// not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public object GetInstance (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      var instance = GetInstanceOrNull (serviceType);
      if (instance == null)
      {
        string message = string.Format (
            "Cannot get a concrete implementation of type '{0}': Expected 'ConcreteImplementationAttribute' could not be found.",
            serviceType.FullName);
        throw new ActivationException (message);
      }

      return instance;
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ImplementationForAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <param name="key">The name the object was registered with. This parameter is ignored by this implementation of <see cref="IServiceLocator"/>.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The 
    /// <see cref="ImplementationForAttribute"/> could not be found on the <paramref name="serviceType"/>, or the concrete implementation could
    /// not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public object GetInstance (Type serviceType, string key)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      return GetInstance (serviceType);
    }

    /// <summary>
    /// Get all instance of the given <paramref name="serviceType"/>, or an empty sequence if no instance could be found.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>
    /// A sequence of instances of the requested <paramref name="serviceType"/>. The <paramref name="serviceType"/> must either have a 
    /// <see cref="ImplementationForAttribute"/>, or a concrete implementation or factory must have been registered using one of the 
    /// <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instances: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<object> GetAllInstances (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      var registration = GetOrCreateRegistration (serviceType);

      var errorMessageFormat =
          "Invalid ConcreteImplementationAttribute configuration for service type '{0}'. "
          + "The service has implementations registered with RegistrationType.{1}. Use GetInstance() to retrieve the implementations.";

      if (registration.SingleFactory != null)
        throw new ActivationException (string.Format (errorMessageFormat, serviceType, RegistrationType.Single));

      // TODO TT: Enable, support for compound indirect resolution, and Test
      //if (registration.CompoundFactory != null)
      //  throw new ActivationException (string.Format (errorMessageFormat, serviceType, RegistrationType.Compound));

      return registration.MultipleFactories.Select (factory => SafeInvokeInstanceFactory (factory, serviceType));
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ImplementationForAttribute"/>, 
    /// or a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    ///<typeparam name="TService">The type of object requested.</typeparam>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The 
    /// <see cref="ImplementationForAttribute"/> could not be found on the <typeparamref name="TService"/>, type or the concrete implementation 
    /// could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public TService GetInstance<TService> ()
    {
      return (TService) GetInstance (typeof (TService));
    }

    /// <summary>
    /// Get an instance of the given <typeparamref name="TService"/> type. The type must either have a <see cref="ImplementationForAttribute"/>,
    /// or a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods.
    /// </summary>
    /// <typeparam name="TService">The type of object requested.</typeparam>
    /// <param name="key">The name the object was registered with. This parameter is ignored by this implementation of <see cref="IServiceLocator"/>.</param>
    /// <returns>The requested service instance.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The
    /// <see cref="ImplementationForAttribute"/> could not be found on the <typeparamref name="TService"/>, type or the concrete implementation
    /// could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public TService GetInstance<TService> (string key)
    {
      return (TService) GetInstance (typeof (TService), key);
    }

    /// <summary>
    /// Get all instance of the given <typeparamref name="TService"/> type, or an empty sequence if no instance could be found.
    /// </summary>
    /// <typeparam name="TService">The type of object requested.</typeparam>
    /// <returns>
    /// A sequence of instances of the requested <typeparamref name="TService"/> type. The <typeparamref name="TService"/> type must either have a 
    /// <see cref="ImplementationForAttribute"/>, or a concrete implementation or factory must have been registered using one of the 
    /// <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, the sequence is empty.
    /// </returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instances: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    public IEnumerable<TService> GetAllInstances<TService> ()
    {
      return GetAllInstances (typeof (TService)).Cast<TService> ();
    }

    /// <summary>
    /// Get an instance of the given <paramref name="serviceType"/>. The type must either have a <see cref="ImplementationForAttribute"/>, or
    /// a concrete implementation or factory must have been registered using one of the <see cref="Register(ServiceConfigurationEntry)"/> methods. Otherwise, 
    /// the method returns <see langword="null"/>.
    /// </summary>
    /// <param name="serviceType">The type of object requested.</param>
    /// <returns>The requested service instance, or <see langword="null" /> if no instance could be found.</returns>
    /// <exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">There was an error resolving the service instance: The concrete 
    /// implementation could not be instantiated. Inspect the <see cref="Exception.InnerException"/> property for the reason of the exception.</exception>
    object IServiceProvider.GetService (Type serviceType)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);

      return GetInstanceOrNull (serviceType);
    }

    //TODO TT: register api ideas: 
    //registerSingle(this IServiceConfigurationRegistry registry, Type serviceType, Type implementationType, Type[] decoratorTypes = null)
    //registerMultiple(this IServiceConfigurationRegistry registry, Type serviceType, Type[] implementationType, Type[] decoratorTypes = null)
    //registerCompound(this IServiceConfigurationRegistry registry, Type serviceType, Type compoundType, Type[] implementationTypes, Type[] decoratorTypes = null)
    //registerXXX(this IServiceConfigurationRegistry registry, Type serviceType, Func<object>[] instanceFactories, Type[] decoratorTypes)

    //as extension for IServiceConfigurationRegistry, or drop if did not exist before feature branch or if no longer needed. Obsolete if old API was used in many projects
    /// <summary>
    /// Registers factories for the specified <paramref name="serviceType"/>. 
    /// The factories are subsequently invoked whenever instances for the <paramref name="serviceType"/> is requested.
    /// </summary>
    /// <param name="serviceType">The service type to register the factories for.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <paramref name="serviceType"/>. These factories
    /// must return non-null instances implementing the <paramref name="serviceType"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <paramref name="serviceType"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <paramref name="serviceType"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, params Func<object>[] instanceFactories)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("instanceFactories", instanceFactories);

      Register (serviceType, (IEnumerable<Func<object>>) instanceFactories);
    }

    //as extension for IServiceConfigurationRegistry, or drop if did not exist before feature branch or if no longer needed. Obsolete if old API was used in many projects
    /// <summary>
    /// Registers factories for the specified <paramref name="serviceType"/>. 
    /// The factories are subsequently invoked whenever instances for the <paramref name="serviceType"/> is requested.
    /// </summary>
    /// <param name="serviceType">The service type to register the factories for.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <paramref name="serviceType"/>. These factories
    /// must return non-null instances implementing the <paramref name="serviceType"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <paramref name="serviceType"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <paramref name="serviceType"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, params Tuple<Func<object>, RegistrationType>[] instanceFactories)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("instanceFactories", instanceFactories);

      Register (serviceType, (IEnumerable<Tuple<Func<object>, RegistrationType>>) instanceFactories);
    }

    //as extension for IServiceConfigurationRegistry, or drop if did not exist before feature branch or if no longer needed. Obsolete if old API was used in many projects
    /// <summary>
    /// Registers factories for the specified <paramref name="serviceType"/>. 
    /// The factories are subsequently invoked whenever instances for the <paramref name="serviceType"/> is requested.
    /// </summary>
    /// <param name="serviceType">The service type to register the factories for.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <paramref name="serviceType"/>. These factories
    /// must return non-null instances implementing the <paramref name="serviceType"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <paramref name="serviceType"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <paramref name="serviceType"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, IEnumerable<Func<object>> instanceFactories)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("instanceFactories", instanceFactories);

      Register (serviceType, instanceFactories.Select (f => Tuple.Create (f, RegistrationType.Single)));
    }

    //as extension for IServiceConfigurationRegistry, or drop if did not exist before feature branch or if no longer needed. Obsolete if old API was used in many projects
    /// <summary>
    /// Registers factories for the specified <paramref name="serviceType"/>. 
    /// The factories are subsequently invoked whenever instances for the <paramref name="serviceType"/> is requested.
    /// </summary>
    /// <param name="serviceType">The service type to register the factories for.</param>
    /// <param name="instanceFactories">The instance factories to use when resolving instances for the <paramref name="serviceType"/>. These factories
    /// must return non-null instances implementing the <paramref name="serviceType"/>, otherwise an <see cref="ActivationException"/> is thrown
    /// when an instance of <paramref name="serviceType"/> is requested.</param>
    /// <exception cref="InvalidOperationException">Factories have already been registered or an instance of the <paramref name="serviceType"/> has 
    /// already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, IEnumerable<Tuple<Func<object>, RegistrationType>> instanceFactories)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("instanceFactories", instanceFactories);

      var groupedFactories = instanceFactories.ToLookup (f => f.Item2, f => f.Item1);
      var registration = new Registration (
          serviceType,
          singleFactory: groupedFactories[RegistrationType.Single].FirstOrDefault(), //TODO TT: Exeption for many
          compoundFactory: groupedFactories[RegistrationType.Compound].FirstOrDefault(), //TODO TT: Exeption for many
          multipleFactories: groupedFactories[RegistrationType.Multiple].ToArray());

      var registeredValue = _dataStore.GetOrCreateValue (serviceType, key => registration);
      if (!ReferenceEquals (registration, registeredValue))
      {
        var message = string.Format ("Register cannot be called twice or after GetInstance for service type: '{0}'.", serviceType.Name);
        throw new InvalidOperationException (message);
      }
    }

    //leave this register method:

    /// <summary>
    /// Registers a concrete implementation for the specified <paramref name="serviceType"/>.
    /// </summary>
    /// <param name="serviceType">The service type to register a concrete implementation for.</param>
    /// <param name="concreteImplementationType">The type of the concrete implementation to be instantiated when an instance of the 
    /// <paramref name="serviceType"/> is retrieved.</param>
    /// <param name="lifetime">The lifetime of the instances.</param>
    /// <param name="registrationType">The registration type of the implementation.</param>
    /// <exception cref="InvalidOperationException">An instance of the <paramref name="serviceType"/> has already been retrieved. Registering factories
    /// or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (Type serviceType, Type concreteImplementationType, LifetimeKind lifetime, RegistrationType registrationType = RegistrationType.Single)
    {
      ArgumentUtility.CheckNotNull ("serviceType", serviceType);
      ArgumentUtility.CheckNotNull ("concreteImplementationType", concreteImplementationType);

      var serviceImplemetation = new ServiceImplementationInfo (concreteImplementationType, lifetime, registrationType);
      ServiceConfigurationEntry serviceConfigurationEntry;
      try
      {
        serviceConfigurationEntry = new ServiceConfigurationEntry (serviceType, serviceImplemetation);
      }
      catch (ArgumentException ex)
      {
        throw new ArgumentException ("Implementation type must implement service type.", "concreteImplementationType", ex);
      }

      //TODO TT: use different overload
      Register (serviceConfigurationEntry);
    }

    // var registryStub = new FakeRegistry
    // registryStub.RegisterSinge()
    // Assert.that (registryStub.Entry.ServiceType, Is.EqualTo (...)
    // Assert.that (registryStub.Entry.ImplInfos, ...

    //Declared in interface IServiceConfigurationRegistry
    /// <summary>
    /// Registers a concrete implementation.
    /// </summary>
    /// <param name="serviceConfigurationEntry">A <see cref="ServiceConfigurationEntry"/> describing the concrete implementation to be registered.</param>
    /// <exception cref="InvalidOperationException">An instance of the service type described by the <paramref name="serviceConfigurationEntry"/>
    /// has already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      ArgumentUtility.CheckNotNull ("serviceConfigurationEntry", serviceConfigurationEntry);

      var registration = CreateRegistration (serviceConfigurationEntry);
      var registeredValue = _dataStore.GetOrCreateValue (serviceConfigurationEntry.ServiceType, key => registration);
      if (!ReferenceEquals (registration, registeredValue))
      {
        var message = string.Format (
            "Register cannot be called twice or after GetInstance for service type: '{0}'.",
            serviceConfigurationEntry.ServiceType.Name);
        throw new InvalidOperationException (message);
      }
    }

    private Registration GetOrCreateRegistration (Type serviceType)
    {
      return _dataStore.GetOrCreateValue (
          serviceType,
          t =>
          {
            var serviceConfigurationEntry = GetServiceConfigurationEntry (t);
            return CreateRegistration (serviceConfigurationEntry);
          });
    }

    private ServiceConfigurationEntry GetServiceConfigurationEntry (Type serviceType)
    {
      try
      {
        return _serviceConfigurationDiscoveryService.GetDefaultConfiguration (serviceType);
      }
      
      catch (InvalidOperationException ex)
      {
        // This exception is part of the IServiceLocator contract.
        var message = string.Format ("Invalid ConcreteImplementationAttribute configuration for service type '{0}'. {1}", serviceType, ex.Message);
        throw new ActivationException (message, ex);
      }
    }

    private Registration CreateRegistration (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      var isCompound = serviceConfigurationEntry.ImplementationInfos.Any (i => i.RegistrationType == RegistrationType.Compound);
      Func<Func<object>, object> noDecorators = f => f();

      var decoratorChain = CreateDecoratorChain (
          serviceConfigurationEntry.ServiceType,
          serviceConfigurationEntry.ImplementationInfos.Where (i => i.RegistrationType == RegistrationType.Decorator));

      //TODO TT: Exceptions
      return new Registration (
          serviceConfigurationEntry.ServiceType,
          singleFactory:
              serviceConfigurationEntry.ImplementationInfos.Where (i => i.RegistrationType == RegistrationType.Single)
                  .Select (i => CreateInstanceFactory (serviceConfigurationEntry.ServiceType, i, decoratorChain))
                  .SingleOrDefault (
                      () => new InvalidOperationException (
                          string.Format (
                              "Cannot register multiple implementations for service type '{0}' when registration type if set to '{1}'.",
                              serviceConfigurationEntry.ServiceType,
                              RegistrationType.Single))),
          compoundFactory:
              serviceConfigurationEntry.ImplementationInfos.Where (i => i.RegistrationType == RegistrationType.Compound)
                  .Select (i => CreateInstanceFactory (serviceConfigurationEntry.ServiceType,i, decoratorChain))
                  .SingleOrDefault (() => new InvalidOperationException ("compound")),
          multipleFactories:
              serviceConfigurationEntry.ImplementationInfos.Where (i => i.RegistrationType == RegistrationType.Multiple)
                  .Select (i => CreateInstanceFactory (serviceConfigurationEntry.ServiceType,i, isCompound ? noDecorators : decoratorChain))
                  .ToArray());
    }

    private object GetInstanceOrNull (Type serviceType)
    {
      //TODO TT: During registration, validate that single and compound only have one registration. Then change from FirstOrDefault to SingleOrDefault
      var registration = GetOrCreateRegistration (serviceType);
      if (registration.SingleFactory != null && registration.MultipleFactories.Any())
      {
        throw new ActivationException (
            string.Format ("Multiple implemetations are configured for service type '{0}'. Consider using 'GetAllInstances'.", serviceType)); //TODO TT: better message because compound?
      }

      var factory = registration.SingleFactory ?? registration.CompoundFactory;
      if (factory == null)
        return null; // TODO TT: Is this really possible?

      return SafeInvokeInstanceFactory (factory, serviceType);
    }

    private object SafeInvokeInstanceFactory (Func<object> factory, Type serviceType)
    {
      object instance;
      try
      {
        instance = factory();
      }
      catch (ActivationException ex)
      {
        var message = string.Format ("Could not resolve type '{0}': {1}", serviceType, ex.Message);
        throw new ActivationException (message, ex);
      }
      catch (Exception ex)
      {
        var message = string.Format ("{0}: {1}", ex.GetType().Name, ex.Message);
        throw new ActivationException (message, ex);
      }

      if (instance == null)
      {
        var message = string.Format (
            "The registered factory returned null instead of an instance implementing the requested service type '{0}'.",
            serviceType);
        throw new ActivationException (message);
      }

      if (!serviceType.IsInstanceOfType (instance))
      {
        var message = string.Format (
            "The instance returned by the registered factory does not implement the requested type '{0}'. (Instance type: '{1}'.)",
            serviceType,
            instance.GetType());
        throw new ActivationException (message);
      }

      return instance;
    }

    private Func<Func<object>, object> CreateDecoratorChain (Type serviceType, IEnumerable<ServiceImplementationInfo> decorators)
    {
      Func<Func<object>, object> activator = (innerActivator) => innerActivator();

      //TODO TT: Write test asserting that the first decorator is the innermost decorator.
      //TODO TT: Refactor to simple expression tree without closures etc
      // arg => new DecoratorType3 (
      //            new DecoratorType2 (
      //                new DecoratorType1 ((ServiceType) arg)))

      foreach (var decoratorImplementationInfo in decorators)
      {
        var constructor = GetSingleConstructor (decoratorImplementationInfo, serviceType);

        var parameterExpression = Expression.Parameter (typeof (object));

        // arg => new DecoratorType ((ServiceType) arg)
        var activationExpression = Expression.Lambda<Func<object, object>> (
            Expression.New (
                constructor,
                Expression.Convert (parameterExpression, serviceType)),
            parameterExpression);
        var compiledExpression = activationExpression.Compile();

        var previousActivator = activator;

        activator = (innerActivator) => compiledExpression (previousActivator (innerActivator));
      }
      return activator;
    }

    private ConstructorInfo GetSingleConstructor (ServiceImplementationInfo serviceImplementationInfo, Type expectedParameterType)
    {
      var argumentTypesDoNotMatchMessage = string.Format (
          " The public constructor must at least accept an argument of type '{0}'.",
          expectedParameterType);

      var exceptionMessage = string.Format (
          "Type '{0}' cannot be instantiated. The type must have exactly one public constructor. {1}",
          serviceImplementationInfo.ImplementationType,
          expectedParameterType == null ? "" : argumentTypesDoNotMatchMessage);

      var constructors = serviceImplementationInfo.ImplementationType.GetConstructors();
      if (constructors.Length != 1)
        throw new ActivationException (exceptionMessage);

      var constructor = constructors.First();
      if (expectedParameterType != null && constructor.GetParameters().Select (p => p.ParameterType == expectedParameterType).Count() != 1)
        throw new ActivationException (exceptionMessage);

      return constructor;
    }

    private Func<object> CreateInstanceFactory (Type serviceType, ServiceImplementationInfo serviceImplementationInfo, Func<Func<object>, object> decoratorChain)
    {
      // TODO TT: Write test to also decorate original factory
      if (serviceImplementationInfo.Factory != null)
        return serviceImplementationInfo.Factory; // return () => decoratorChain (serviceImplementationInfo.Factory)

      var expectedParameterType = serviceImplementationInfo.RegistrationType == RegistrationType.Compound ? typeof (IEnumerable<>).MakeGenericType (serviceType) : null;
      var ctorInfo = GetSingleConstructor (serviceImplementationInfo, expectedParameterType);
      var instanceFactory = CreateInstanceFactory (ctorInfo);
      Func<object> decoratedFactory = () => decoratorChain (instanceFactory);

      switch (serviceImplementationInfo.Lifetime)
      {
        case LifetimeKind.Singleton:
          var factoryContainer = new DoubleCheckedLockingContainer<object> (decoratedFactory);
          return () => factoryContainer.Value;
        default:
          return decoratedFactory;
      }
    }

    private Func<object> CreateInstanceFactory (ConstructorInfo ctorInfo)
    {
      var serviceLocator = Expression.Constant (this);

      var parameterInfos = ctorInfo.GetParameters();
      var ctorArgExpressions = parameterInfos.Select (x => GetIndirectResolutionCall (serviceLocator, x));

      var factoryLambda = Expression.Lambda<Func<object>> (Expression.New (ctorInfo, ctorArgExpressions));
      return factoryLambda.Compile();
    }

    private Expression GetIndirectResolutionCall (Expression serviceLocator, ParameterInfo parameterInfo)
    {
      MethodInfo resolutionMethod;
      if (parameterInfo.ParameterType.IsGenericType && parameterInfo.ParameterType.GetGenericTypeDefinition() == typeof (IEnumerable<>))
      {
        var elementType = parameterInfo.ParameterType.GetGenericArguments().Single();
        resolutionMethod = s_resolveIndirectCollectionDependencyMethod.MakeGenericMethod (elementType);
      }
      else
        resolutionMethod = s_resolveIndirectDependencyMethod.MakeGenericMethod (parameterInfo.ParameterType);

      var context = string.Format (
          "constructor parameter '{0}' of type '{1}'",
          parameterInfo.Name,
          parameterInfo.Member.DeclaringType);
      return Expression.Call (serviceLocator, resolutionMethod, Expression.Constant (context));
    }

    private T ResolveIndirectDependency<T> (string context)
    {
      try
      {
        return GetInstance<T>();
      }
      catch (ActivationException ex)
      {
        var message = string.Format ("Error resolving indirect dependendency of {0}: {1}", context, ex.Message);
        throw new ActivationException (message, ex);
      }
    }

    private IEnumerable<T> ResolveIndirectCollectionDependency<T> (string context)
    {
      // To keep the lazy sequence semantics of GetAllInstances, and still be able to catch the ActivationException, we need to manually iterate
      // the input sequence from within a try/catch block and yield return from outside the try/catch block. (Yield return is not allowed to stand
      // within a try/catch block.)
      using (var enumerator = GetAllInstances<T>().GetEnumerator())
      {
        while (true)
        {
          T current;
          try
          {
            if (!enumerator.MoveNext())
              yield break;
            current = enumerator.Current;
          }
          catch (ActivationException ex)
          {
            var message = string.Format ("Error resolving indirect collection dependendency of {0}: {1}", context, ex.Message);
            throw new ActivationException (message, ex);
          }
          yield return current;
        }
      }
    }
  }
}