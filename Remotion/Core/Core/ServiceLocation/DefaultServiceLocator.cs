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
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Security.AccessControl;
using System.Text;
using JetBrains.Annotations;
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

    private readonly IDataStore<Tuple<Type, RegistrationType>, Tuple<Func<object>, RegistrationType>[]> _dataStore =
        DataStoreFactory.CreateWithLocking<Tuple<Type, RegistrationType>, Tuple<Func<object>, RegistrationType>[]>();

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

      var factories = GetOrCreateFactories (serviceType, RegistrationType.Multiple);

      //if (factories.Any (f => f.Item2 == RegistrationType.Single))
      //{
      //  throw new ActivationException (
      //      string.Format ("Invalid ConcreteImplementationAttribute configuration for service type '{0}'. ", serviceType)
      //      + "The service has implementations registered with RegistrationType.Single. Use GetInstance() to retrieve the implementations.");
      //}

      return factories.Select (factory => SafeInvokeInstanceFactory (factory, serviceType));
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

      foreach (var group in instanceFactories.GroupBy (f => f.Item2))
      {
        //TODO TT: TEST
        if (group.Key != RegistrationType.Single && group.Key != RegistrationType.Multiple)
          throw new ArgumentException ("Register is supported only for Single or Multiple registration types.");

        var factoryArray = group.ToArray();
        var factories = _dataStore.GetOrCreateValue (Tuple.Create(serviceType, group.Key), t => factoryArray);
        if (factories != factoryArray)
        {
          var message = string.Format ("Register cannot be called twice or after GetInstance for service type: '{0}'.", serviceType.Name);
          throw new InvalidOperationException (message);
        }
      }
    }

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

      Register (serviceConfigurationEntry);
    }

    /// <summary>
    /// Registers a concrete implementation.
    /// </summary>
    /// <param name="serviceConfigurationEntry">A <see cref="ServiceConfigurationEntry"/> describing the concrete implementation to be registered.</param>
    /// <exception cref="InvalidOperationException">An instance of the service type described by the <paramref name="serviceConfigurationEntry"/>
    /// has already been retrieved. Registering factories or concrete implementations can only be done before any instances are retrieved.</exception>
    public void Register (ServiceConfigurationEntry serviceConfigurationEntry)
    {
      ArgumentUtility.CheckNotNull ("serviceConfigurationEntry", serviceConfigurationEntry);

      foreach (var group in serviceConfigurationEntry.ImplementationInfos.GroupBy (i => i.RegistrationType))
      {
        var factories = CreateInstanceFactories (serviceConfigurationEntry.ServiceType, group);
        Register (serviceConfigurationEntry.ServiceType, factories);
      }
    }

    private Tuple<Func<object>, RegistrationType>[] GetOrCreateFactories (Type serviceType, RegistrationType registrationType)
    {
      return _dataStore.GetOrCreateValue (Tuple.Create (serviceType, registrationType), t => CreateInstanceFactories (t.Item1, t.Item2).ToArray());
    }

    private object GetInstanceOrNull (Type serviceType)
    {
      //if (decorator exists)
      //return decorator instance
      //if (compound factories exist)
      //return safeInvokeInstanceFactory (factory.Where (r= compound).first());

      //TODO TT: During registration, validate that single and compound only have one registration. Then change from FirstOrDefault to SingleOrDefault
      var factory = GetOrCreateFactories (serviceType, RegistrationType.Single).FirstOrDefault()
                    ?? GetOrCreateFactories (serviceType, RegistrationType.Compound).FirstOrDefault();
      if (factory == null)
        return null;

      //if (factories.Any (f => f.Item2 == RegistrationType.Multiple))
      //{
      //  throw new ActivationException (
      //      string.Format ("Invalid ConcreteImplementationAttribute configuration for service type '{0}'. ", serviceType)
      //      + "The service has implementations registered with RegistrationType.Multiple. Use GetAllInstances() to retrieve the implementations.");
      //}

      return SafeInvokeInstanceFactory (factory, serviceType);
    }

    private object SafeInvokeInstanceFactory (Tuple<Func<object>, RegistrationType> factory, Type serviceType)
    {
      object instance;
      try
      {
        instance = factory.Item1();
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

    private IEnumerable<Tuple<Func<object>, RegistrationType>> CreateInstanceFactories (Type serviceType, RegistrationType registrationType)
    {
      ServiceConfigurationEntry entry;
      try
      {
        entry = _serviceConfigurationDiscoveryService.GetDefaultConfiguration (serviceType);
      }
      catch (InvalidOperationException ex)
      {
        // This exception is part of the IServiceLocator contract.
        var message = string.Format ("Invalid ConcreteImplementationAttribute configuration for service type '{0}'. {1}", serviceType, ex.Message);
        throw new ActivationException (message, ex);
      }

      return CreateInstanceFactories (entry.ServiceType, entry.ImplementationInfos.Where (i => i.RegistrationType == registrationType));
    }

    private IEnumerable<Tuple<Func<object>, RegistrationType>> CreateInstanceFactories (
        Type serviceType,
        IEnumerable<ServiceImplementationInfo> implementationInfos)
    {
      //var implementationInfosByType = implementationInfos.ToLookup (i => i.RegistrationType);

      //if (implementationInfosByType.Contains (RegistrationType.Decorator))
      //  return CreateDecoratedInstanceFactory (serviceType, implementationInfosByType);

      //if (implementationInfosByType.Contains (RegistrationType.Compound))
      //  return CreateCompoundInstanceFactory (serviceType, implementationInfosByType);

      return implementationInfos.Select (CreateInstanceFactory);
    }

    private IEnumerable<Tuple<Func<object>, RegistrationType>> CreateDecoratedInstanceFactory (
        Type serviceType,
        ILookup<RegistrationType, ServiceImplementationInfo> implementationInfosByType)
    {
      var decorators = implementationInfosByType[RegistrationType.Decorator];

      var factoriesToBeDecorated = CreateInstanceFactories (
          serviceType,
          implementationInfosByType.SelectMany (i => i).Where (i => i.RegistrationType != RegistrationType.Decorator))
          .ToArray();

      Func<Func<object>, object> activator = (innerActivator) => innerActivator();

      foreach (var decoratorImplementationInfo in decorators)
      {
        var constructor = GetSingleConstructor(decoratorImplementationInfo, serviceType);

        var parameterExpression = Expression.Parameter (typeof (object));
        var activationExpression = Expression.Lambda<Func<object, object>> (
            Expression.New (
                constructor,
                Expression.Convert (parameterExpression, serviceType)),
            parameterExpression);
        var compiledExpression = activationExpression.Compile();
        var previousActivator = activator;

        activator = (innerActivator) => compiledExpression (previousActivator(innerActivator));
      }

      return factoriesToBeDecorated.Select (
          i => Tuple.Create ((Func<object>) (() => activator (i.Item1)), RegistrationType.Decorator)).ToArray();
    }

    private IEnumerable<Tuple<Func<object>, RegistrationType>> CreateCompoundInstanceFactory (
        Type serviceType,
        ILookup<RegistrationType, ServiceImplementationInfo> implementationInfosByType)
    {
      var compoundImplementation = implementationInfosByType[RegistrationType.Compound].First();
      var constructor = GetSingleConstructor(compoundImplementation, typeof (IEnumerable<>).MakeGenericType (serviceType));

      var wrappedImplementations = implementationInfosByType[RegistrationType.Multiple];

      var castMethod = typeof (Enumerable).GetMethod ("Cast", BindingFlags.Public | BindingFlags.Static)
        .MakeGenericMethod (serviceType);

      var parameterExpression = Expression.Parameter (typeof (IEnumerable<object>));
      var lambda = Expression.Lambda<Func<IEnumerable<object>, object>> (
          Expression.New (
              constructor,
              Expression.Call (null, castMethod, parameterExpression)),
          parameterExpression);

      var compiledConstructor = lambda.Compile();
      var factories = wrappedImplementations.Select (CreateInstanceFactory);
      var compoundFactory = (Func<object>) (() => compiledConstructor (factories.Select (f => f.Item1()).ToArray()));
      return new[] { Tuple.Create (compoundFactory, RegistrationType.Compound) };
    }

    private ConstructorInfo GetSingleConstructor (ServiceImplementationInfo compoundImplementation, Type expectedParameterType)
    {
      var exceptionMessage = string.Format (
          "Type '{0}' cannot be instantiated. {1} implementations must have a single public constructor accepting a single argument of type '{2}'.",
          compoundImplementation.ImplementationType,
          compoundImplementation.RegistrationType,
          expectedParameterType);

      var constructors = compoundImplementation.ImplementationType.GetConstructors();
      if (constructors.Length != 1)
        throw new ActivationException (exceptionMessage);

      var constructor = constructors.First();
      if (constructor.GetParameters().Length != 1 || constructor.GetParameters().First().ParameterType != expectedParameterType)
        throw new ActivationException (exceptionMessage);

      return constructor;
    }

    private Tuple<Func<object>, RegistrationType> CreateInstanceFactory (ServiceImplementationInfo serviceImplementationInfo)
    {
      if (serviceImplementationInfo.Factory != null)
        return Tuple.Create(serviceImplementationInfo.Factory, serviceImplementationInfo.RegistrationType);

      var publicCtors = serviceImplementationInfo.ImplementationType.GetConstructors();
      if (publicCtors.Length != 1)
      {
        throw new ActivationException (
            string.Format (
                "Type '{0}' has not exactly one public constructor and cannot be instantiated.", serviceImplementationInfo.ImplementationType.Name));
      }

      var ctorInfo = publicCtors.Single();
      Tuple<Func<object>, RegistrationType> factory = CreateInstanceFactory (ctorInfo, serviceImplementationInfo);

      switch (serviceImplementationInfo.Lifetime)
      {
        case LifetimeKind.Singleton:
          var factoryContainer = new DoubleCheckedLockingContainer<object> (factory.Item1);
          return Tuple.Create((Func<object>) (() => factoryContainer.Value), factory.Item2);
        default:
          return factory;
      }
    }

    private Tuple<Func<object>, RegistrationType> CreateInstanceFactory (ConstructorInfo ctorInfo, ServiceImplementationInfo serviceImplementationInfo)
    {
      var serviceLocator = Expression.Constant (this);

      var parameterInfos = ctorInfo.GetParameters();
      var ctorArgExpressions = parameterInfos.Select (x => GetIndirectResolutionCall (serviceLocator, x));

      var factoryLambda = Expression.Lambda<Func<object>> (Expression.New (ctorInfo, ctorArgExpressions));
      return Tuple.Create (factoryLambda.Compile(), serviceImplementationInfo.RegistrationType);
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