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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Cannot get a concrete implementation of type 'Microsoft.Practices.ServiceLocation.IServiceLocator': " +
        "Expected 'ConcreteImplementationAttribute' could not be found.")]
    public void GetInstance_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      _serviceLocator.GetInstance (typeof (IServiceLocator));
    }

    [Test]
    public void GetInstance_TypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (
          SafeServiceLocator.Current.GetInstance<ITestInstanceConcreteImplementationAttributeType>(),
          Is.InstanceOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage = "InvalidOperationException: This exception comes from the ctor.")]
    public void GetInstance_ConstructorThrowingException ()
    {
      _serviceLocator.GetInstance (typeof (ITestRegistrationTypeSingleTypeThrowingExceptionInCtor));
    }

    [Test]
    public void GetInstance_InstanceNotCompatibleWithServiceType ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), () => new object ());
      Assert.That (
          () => _serviceLocator.GetInstance (typeof (ISomeInterface)),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The instance returned by the registered factory does not implement the requested type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'. (Instance type: 'System.Object'.)"));
    }

    [Test]
    public void GetInstance_ServiceTypeWithNullImplementation ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), () => null);
      Assert.That (
          () => _serviceLocator.GetInstance (typeof (ISomeInterface)),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'."));
    }

    [Test]
    public void GetInstance_InstanceLifeTime_ReturnsNotSameInstancesForAServiceType ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (instance1, Is.Not.SameAs (instance2));
    }

    [Test]
    public void GetInstance_SingletonLifeTime_ReturnsSameInstancesForAServiceType ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));

      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    public void GetInstance_WithKeyParameter_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType), "Test");

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_ServiceWithRegistrationTypeMultiple_ThrowsActivationException ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestRegistrationTypeMultiple1), LifetimeKind.Singleton, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestRegistrationTypeMultiple), implementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      Assert.That (
          () => _serviceLocator.GetInstance<ITestRegistrationTypeMultiple>(),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Multiple implemetations are configured for service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestRegistrationTypeMultiple'. Consider using 'GetAllInstances'."));
    }

    [Test]
    public void Register_WithMultipleServiceImplementationsForRegistrationTypeSingle_ThrowsInvalidOperationException ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestRegistrationTypeSingle1), LifetimeKind.Singleton, RegistrationType.Single);
      var implementation2 = new ServiceImplementationInfo (typeof (TestRegistrationTypeSingle2), LifetimeKind.Singleton, RegistrationType.Single);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestRegistrationTypeSingle), implementation1, implementation2);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot register multiple implementations for service type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestRegistrationTypeSingle' when registration type if set to 'Single'."));
    }

    [Test]
    public void GetInstance_TypeWithGenericServiceInterface ()
    {
      Assert.That (_serviceLocator.GetInstance (typeof (ITestOpenGenericService<int>)), Is.TypeOf (typeof (TestOpenGenericIntImplementation)));
      Assert.That (_serviceLocator.GetInstance (typeof (ITestOpenGenericService<string>)), Is.TypeOf (typeof (TestOpenGenericStringImplementation)));
      Assert.That (
          SafeServiceLocator.Current.GetInstance<ITestOpenGenericService<int>>(),
          Is.InstanceOf (typeof (TestOpenGenericIntImplementation)));
    }

    [Test]
    public void GetInstance_NotImplementedGenericServiceInterface ()
    {
      Assert.That (() => _serviceLocator.GetInstance (typeof (ITestOpenGenericService<object>)), 
        Throws.Exception.InstanceOf<ActivationException>());
    }

    [Test]
    public void GetAllInstances ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (ITestImplementationForAttributeType)).ToArray();

      Assert.That (result, Has.Length.EqualTo (1));
      Assert.That (result.Single(), Is.TypeOf (typeof (TestImplementationForAttributeType)));
    }

    [Test]
    public void GetAllInstances_ServiceWithRegistrationTypeMultiple ()
    {
      var instances = _serviceLocator.GetAllInstances (typeof (ITestRegistrationTypeMultiple)).ToArray();

      Assert.That (
          instances.Select (i => i.GetType()),
          Is.EqualTo (new[] { typeof (TestRegistrationTypeMultiple1), typeof (TestRegistrationTypeMultiple2) }));
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage =
        "Invalid ConcreteImplementationAttribute configuration for service type "
        + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestRegistrationTypeSingle'. "
        + "The service has implementations registered with RegistrationType.Single. Use GetInstance() to retrieve the implementations.")]
    public void GetAllInstances_ServiceWithRegistrationTypeSingle ()
    {
      _serviceLocator.Register (
          new ServiceConfigurationEntry (
              typeof (ITestRegistrationTypeSingle),
              new ServiceImplementationInfo (typeof (TestRegistrationTypeSingle1), LifetimeKind.Instance)));

      _serviceLocator.GetAllInstances (typeof (ITestRegistrationTypeSingle)).ToArray();
    }

    [Test]
    [ExpectedException (typeof (ActivationException), ExpectedMessage = "InvalidOperationException: This exception comes from the ctor.")]
    public void GetAllInstances_ConstructorThrowingException ()
    {
      _serviceLocator.GetAllInstances (typeof (ITestRegistrationTypeMultipleTypeThrowingExceptionInCtor)).ToArray();
    }

    [Test]
    public void GetAllInstances_InstanceNotCompatibleWithServiceType ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), Tuple.Create ((Func<object>) (() => new object()), RegistrationType.Multiple));
      Assert.That (
          () => _serviceLocator.GetAllInstances (typeof (ISomeInterface)).ToArray (),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The instance returned by the registered factory does not implement the requested type " 
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'. (Instance type: 'System.Object'.)"));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithNullImplementation ()
    {
      _serviceLocator.Register (typeof (ISomeInterface), Tuple.Create ((Func<object>) (() => null), RegistrationType.Multiple));
      Assert.That (
          () => _serviceLocator.GetAllInstances (typeof (ISomeInterface)).ToArray(),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTest+ISomeInterface'."));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithMultipleConcreteImplementationAttributes ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (ITestMultipleConcreteImplementationAttributesType)).ToArray ();

      Assert.That (result, Has.Length.EqualTo (3));
      Assert.That (result[0], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType2)));
      Assert.That (result[1], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType3)));
      Assert.That (result[2], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType1)));
    }

    [Test]
    public void GetAllInstances_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (IServiceLocator));

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetInstance_Generic ()
    {
      var result = _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType>();

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_Generic_WithKeyParameter_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType> ("Test");

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_Generic_ServiceTypeWithUnresolvableAndResolveImplementationTypes ()
    {
      var result = _serviceLocator.GetInstance<ITestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypes> ();

      Assert.That (result, Is.TypeOf<TestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypesExisting> ());
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetAllInstances<IServiceLocator>();

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetAllInstances_Generic_ServiceTypeWithMultipleConcreteImplementationAttributes ()
    {
      var result = _serviceLocator.GetAllInstances<ITestMultipleConcreteImplementationAttributesType> ().ToArray();

      Assert.That (result, Has.Length.EqualTo(3));
      Assert.That (result[0], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType2)));
      Assert.That (result[1], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType3)));
      Assert.That (result[2], Is.TypeOf (typeof (TestMultipleConcreteImplementationAttributesType1)));
    }

    [Test]
    public void GetInstance_ConstructorInjection_OneParameter ()
    {
      var result = _serviceLocator.GetInstance<ITestConstructorInjectionWithOneParameter>();

      Assert.That (result, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (((TestConstructorInjectionWithOneParameter) result).Param, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_ConstructorInjection_ThreeParametersRecursive ()
    {
      var result = _serviceLocator.GetInstance<ITestConstructorInjectionWithThreeParameters>();

      Assert.That (result, Is.TypeOf (typeof (TestConstructorInjectionWithThreeParameters)));

      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param1, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (
          ((TestConstructorInjectionWithOneParameter) ((TestConstructorInjectionWithThreeParameters) result).Param1).Param,
          Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param2, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (
          ((TestConstructorInjectionWithOneParameter) ((TestConstructorInjectionWithThreeParameters) result).Param2).Param,
          Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param3, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_ConstructorInjection_InstanceLifeTime_ReturnsNotSameInstances_ForServiceParameter_WithInstanceLifetime ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithInstanceLifetime));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithInstanceLifetime));

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (
          ((TestConstructorInjectionWithOneParameterWithInstanceLifetime) instance1).Param,
          Is.Not.SameAs (((TestConstructorInjectionWithOneParameterWithInstanceLifetime) instance2).Param));
    }

    [Test]
    public void GetInstance_ConstructorInjection_InstanceLifeTime_ReturnsNotSameInstances_ForServiceParameter_WithSingletonLifetime ()
    {
      var instance1 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithSingletonLifetime));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestConstructorInjectionWithOneParameterWithSingletonLifetime));

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (
          ((TestConstructorInjectionWithOneParameterWithSingletonLifetime) instance1).Param,
          Is.SameAs (((TestConstructorInjectionWithOneParameterWithSingletonLifetime) instance2).Param));
    }

    [Test]
    public void Register_TypeWithTooManyPublicCtors_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestTypeWithTooManyPublicConstructors), LifetimeKind.Singleton, RegistrationType.Single);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestTypeErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestTypeWithTooManyPublicConstructors' cannot be instantiated. " +
              "The type must have exactly one public constructor."));
    }

    [Test]
    public void Register_TypeWithOnlyNonPublicCtor_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestTypeWithOnlyNonPublicConstructor), LifetimeKind.Singleton, RegistrationType.Single);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestTypeErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestTypeWithOnlyNonPublicConstructor' cannot be instantiated. " +
              "The type must have exactly one public constructor."));
    }

    [Test]
    public void GetService_TypeWithConcreteImplementationAttribute ()
    {
      var result = ((IServiceLocator) _serviceLocator).GetService (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetService_TypeWithoutConcreteImplementatioAttribute ()
    {
      var result = ((IServiceLocator) _serviceLocator).GetService (typeof (string));

      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetService_ServiceTypeWithUnresolvableAndResolveImplementationTypes ()
    {
      var result =
        ((IServiceLocator) _serviceLocator).GetService (typeof (ITestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypes));

      Assert.That (result, Is.TypeOf<TestConcreteImplementationAttributeWithUnresolvableAndResolvableImplementationTypesExisting> ());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Register cannot be called twice or after GetInstance for service type: 'ITestInstanceConcreteImplementationAttributeType'.")
    ]
    public void Register_Factory_ServiceAlreadyExists ()
    {
      _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType>();
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), () => new object());
    }

    [Test]
    public void Register_Factory_ServiceIsAdded ()
    {
      var instance = new TestConcreteImplementationAttributeType ();
      Func<object> instanceFactory = () => instance;

      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory);

      Assert.That (_serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType)), Is.SameAs (instance));
    }

    [Test]
    public void Register_MultipleFactories_ServiceIsAdded ()
    {
      var instance1 = new object ();
      var instance2 = new object ();
      Func<object> instanceFactory1 = () => instance1;
      Func<object> instanceFactory2 = () => instance2;

      _serviceLocator.Register (
          typeof (object),
          new[]
          {
              Tuple.Create (instanceFactory1, RegistrationType.Multiple),
              Tuple.Create (instanceFactory2, RegistrationType.Multiple),
          });

      Assert.That (_serviceLocator.GetAllInstances (typeof (object)), Is.EqualTo (new[] { instance1, instance2 }));
    }

    [Test]
    public void Register_NoFactories_OverridesAttributes ()
    {
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (_serviceLocator.GetAllInstances (typeof (object)), Is.Empty);
    }

    [Test]
    public void Register_Twice_ExceptionIsThrown ()
    {
      Func<object> instanceFactory = () => new object();
      _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory);

      Assert.That (
          () => _serviceLocator.Register (typeof (ITestInstanceConcreteImplementationAttributeType), instanceFactory),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Register cannot be called twice or after GetInstance for service type: 'ITestInstanceConcreteImplementationAttributeType'."));
    }

    [Test]
    public void Register_ConcreteImplementation_ServiceAlreadyExists_ThrowsException ()
    {
      _serviceLocator.GetInstance<ITestSingletonConcreteImplementationAttributeType>();
      Assert.That (
          () => _serviceLocator.Register (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Register cannot be called twice or after GetInstance for service type: 'ITestSingletonConcreteImplementationAttributeType'."));
    }

    [Test]
    public void Register_ConcreteImplementation_ImplementationTypeDoesNotImplementServiceType_ThrowsException ()
    {
      Assert.That (
          () => _serviceLocator.Register (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (object), LifetimeKind.Singleton),
          Throws.ArgumentException.With.Message.EqualTo (
              "Implementation type must implement service type.\r\nParameter name: concreteImplementationType"));
    }

    [Test]
    public void Register_ConcreteImplementation_ServiceIsAdded ()
    {
      _serviceLocator.Register (
          typeof (ITestSingletonConcreteImplementationAttributeType), typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    public void Register_SingletonServiceIsLazyInitialized ()
    {
      _serviceLocator.Register (
          typeof (TestConstructorInjectionForServiceWithoutConcreteImplementationAttribute),
          typeof (TestConstructorInjectionForServiceWithoutConcreteImplementationAttribute),
          LifetimeKind.Singleton);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Register cannot be called twice or after GetInstance for service type: 'ITestSingletonConcreteImplementationAttributeType'.")
    ]
    public void Register_ServiceConfigurationEntry_ServiceAlreadyExists_ThrowsException ()
    {
      _serviceLocator.GetInstance<ITestSingletonConcreteImplementationAttributeType>();
      var serviceImplementation = new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestSingletonConcreteImplementationAttributeType), serviceImplementation);
      _serviceLocator.Register (serviceConfigurationEntry);
    }

    [Test]
    public void Register_ServiceConfigurationEntry_ServiceAdded ()
    {
      var serviceImplementation = new ServiceImplementationInfo (
          typeof (TestConcreteImplementationAttributeType), LifetimeKind.Singleton);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType), serviceImplementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (instance2));
    }

    [Test]
    public void Register_ServiceConfigurationEntry_MultipleServices ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType1), LifetimeKind.Singleton, RegistrationType.Multiple);
      var implementation2 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType2), LifetimeKind.Singleton, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestMultipleRegistrationsType), implementation1, implementation2);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instances = _serviceLocator.GetAllInstances<ITestMultipleRegistrationsType> ().ToArray ();
      Assert.That (instances, Has.Length.EqualTo (2));
      Assert.That (instances[0], Is.TypeOf<TestMultipleRegistrationType1> ());
      Assert.That (instances[1], Is.TypeOf<TestMultipleRegistrationType2> ());
    }

    [Test]
    public void Register_ServiceConfigurationEntry_ImplementationInfoWithFactory ()
    {
      var expectedInstance = new TestConcreteImplementationAttributeType();
      var serviceImplementation = ServiceImplementationInfo.Create (() => expectedInstance);

      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType),
          serviceImplementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (expectedInstance));
    }

    [Test]
    public void GetInstance_IEnumerable ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType1), LifetimeKind.Singleton, RegistrationType.Multiple);
      var implementation2 = new ServiceImplementationInfo (typeof (TestMultipleRegistrationType2), LifetimeKind.Singleton, RegistrationType.Multiple);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestMultipleRegistrationsType), implementation1, implementation2));
      _serviceLocator.Register (typeof (DomainType), typeof (DomainType), LifetimeKind.Singleton);

      var instances = ((DomainType) _serviceLocator.GetInstance (typeof (DomainType))).AllInstances.ToArray();

      Assert.That (instances, Has.Length.EqualTo (2));
      Assert.That (instances[0], Is.TypeOf<TestMultipleRegistrationType1> ());
      Assert.That (instances[1], Is.TypeOf<TestMultipleRegistrationType2> ());
    }

    [Test]
    public void GetInstance_Compound ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestCompoundImplementation1),LifetimeKind.Instance, RegistrationType.Multiple);
      var implementation2 = new ServiceImplementationInfo (typeof (TestCompoundImplementation2),LifetimeKind.Instance, RegistrationType.Multiple);
      var compound = new ServiceImplementationInfo (typeof (TestCompoundRegistration), LifetimeKind.Instance, RegistrationType.Compound);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestCompoundRegistration), implementation1, implementation2, compound));

      var instance = _serviceLocator.GetInstance (typeof (ITestCompoundRegistration));

      Assert.That (instance, Is.InstanceOf<TestCompoundRegistration>());
      var compoundRegistration = (TestCompoundRegistration) instance;
      Assert.That (compoundRegistration.CompoundRegistrations, Is.Not.Empty);
      Assert.That (
          compoundRegistration.CompoundRegistrations.Select (c => c.GetType()),
          Is.EqualTo (new[] { typeof (TestCompoundImplementation1), typeof (TestCompoundImplementation2) }));
    }

    [Test]
    public void GetInstance_CompoundWithEmptyImplementationsList ()
    {
      var compound = new ServiceImplementationInfo (typeof (TestCompoundRegistration), LifetimeKind.Instance, RegistrationType.Compound);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestCompoundRegistration), compound));

      var instance = _serviceLocator.GetInstance (typeof (ITestCompoundRegistration));

      Assert.That (instance, Is.InstanceOf<TestCompoundRegistration>());
      var compoundRegistration = (TestCompoundRegistration) instance;
      Assert.That (compoundRegistration.CompoundRegistrations, Is.Empty);
    }

    [Test]
    public void Register_CompoundWithNoPublicConstructor_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (
          typeof (TestCompoundWithoutPublicConstructor),
          LifetimeKind.Instance,
          RegistrationType.Compound);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestCompoundWithErrors), implementation);

      Assert.That (
          () =>  _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestCompoundWithoutPublicConstructor' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithConstructorWithoutArguments_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (
          typeof (TestCompoundWithConstructorWithoutArguments),
          LifetimeKind.Instance,
          RegistrationType.Compound);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestCompoundWithErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestCompoundWithConstructorWithoutArguments' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithConstructorWithoutMatchingArgument_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (
          typeof (TestCompoundWithConstructorWithoutMatchingArgument),
          LifetimeKind.Instance,
          RegistrationType.Compound);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestCompoundWithErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestCompoundWithConstructorWithoutMatchingArgument' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void GetInstance_Decorator ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestDecoratorRegistrationDecoratedObject1),LifetimeKind.Instance, RegistrationType.Single);
      var decorator = new ServiceImplementationInfo (typeof (TestDecoratorRegistrationDecorator), LifetimeKind.Instance, RegistrationType.Decorator);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestDecoratorRegistration), implementation, decorator));

      var instance = _serviceLocator.GetInstance (typeof (ITestDecoratorRegistration));

      Assert.That (instance, Is.InstanceOf<TestDecoratorRegistrationDecorator>());
      var decoratorInstance = (TestDecoratorRegistrationDecorator) instance;
      Assert.That (decoratorInstance.DecoratedObject, Is.InstanceOf<TestDecoratorRegistrationDecoratedObject1>());
    }

    [Test]
    public void Register_DecoratorWithoutPublicConstructor_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (
          typeof (TestDecoratorWithoutPublicConstructor),
          LifetimeKind.Instance,
          RegistrationType.Decorator);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestDecoratorWithErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestDecoratorWithoutPublicConstructor' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestDecoratorWithErrors'."));
    }

    [Test]
    public void Register_DecoratorWithConstructorWithoutArguments_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (
          typeof (TestDecoratorWithConstructorWithoutArguments),
          LifetimeKind.Instance,
          RegistrationType.Decorator);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestDecoratorWithErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestDecoratorWithConstructorWithoutArguments' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestDecoratorWithErrors'."));
    }

    [Test]
    public void Register_DecoratorWithConstructorWithoutMatchingArgument_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (
          typeof (TestDecoratorWithConstructorWithoutMatchingArgument),
          LifetimeKind.Instance,
          RegistrationType.Decorator);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestDecoratorWithErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.TestDomain.TestDecoratorWithConstructorWithoutMatchingArgument' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ITestDecoratorWithErrors'."));
    }

    [Test]
    public void GetInstance_StackedDecorators ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestStackedDecoratorsObject1),LifetimeKind.Instance, RegistrationType.Single);
      var decorator1 = new ServiceImplementationInfo (typeof (TestStackedDecoratorsDecorator1), LifetimeKind.Instance, RegistrationType.Decorator);
      var decorator2 = new ServiceImplementationInfo (typeof (TestStackedDecoratorsDecorator2), LifetimeKind.Instance, RegistrationType.Decorator);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestStackedDecorators), implementation1, decorator1, decorator2));

      var instance = _serviceLocator.GetInstance (typeof (ITestStackedDecorators));

      Assert.That (instance, Is.InstanceOf<TestStackedDecoratorsDecorator2>());
      var decoratorInstance2 = (TestStackedDecoratorsDecorator2) instance;
      Assert.That (decoratorInstance2.DecoratedObject, Is.InstanceOf<TestStackedDecoratorsDecorator1>());
      var decoratorInstance1 = (TestStackedDecoratorsDecorator1) decoratorInstance2.DecoratedObject;
      Assert.That (decoratorInstance1.DecoratedObject, Is.InstanceOf<TestStackedDecoratorsObject1>());
    }

    [Test]
    public void GetInstance_DecoratedCompound ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (ITestDecoratedCompoundObject1),LifetimeKind.Instance, RegistrationType.Multiple);
      var implementation2 = new ServiceImplementationInfo (typeof (ITestDecoratedCompoundObject2),LifetimeKind.Instance, RegistrationType.Multiple);
      var decorator = new ServiceImplementationInfo (typeof (ITestDecoratedCompoundDecorator), LifetimeKind.Instance, RegistrationType.Decorator);
      var compound = new ServiceImplementationInfo (typeof (ITestDecoratedCompoundCompound), LifetimeKind.Instance, RegistrationType.Compound);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (IITestDecoratedCompound), implementation1, implementation2, decorator, compound));

      var instance = _serviceLocator.GetInstance (typeof (IITestDecoratedCompound));

      Assert.That (instance, Is.InstanceOf<ITestDecoratedCompoundDecorator>());
      var decoratorInstance = (ITestDecoratedCompoundDecorator) instance;
      Assert.That (decoratorInstance.DecoratedObject, Is.InstanceOf<ITestDecoratedCompoundCompound>());
      var compoundInstance = (ITestDecoratedCompoundCompound) decoratorInstance.DecoratedObject;
      Assert.That (compoundInstance.InnerObjects, Is.Not.Empty);
    }
    
    [Test]
    public void GetInstance_Compound_ReturnsFirstCompoundImplementation_OrderedByPosition ()
    {
      var compound = new ServiceImplementationInfo (typeof (TestCompoundRegistration), LifetimeKind.Instance, RegistrationType.Compound);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestCompoundRegistration), compound));

      var instance = _serviceLocator.GetInstance (typeof (ITestCompoundRegistration));

      Assert.That (instance, Is.InstanceOf<TestCompoundRegistration>());
    }

    [Test]
    public void GetInstance_IndirectActivationException_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      Assert.That (
          () => _serviceLocator.GetInstance<IInterfaceWithIndirectActivationException>(),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.TestDomain.IInterfaceWithIndirectActivationException': "
              + "Error resolving indirect dependendency of constructor parameter 'innerDependency' of type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ClassWithIndirectActivationException': Cannot get a concrete implementation of type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.IInterfaceWithoutImplementation': "
              + "Expected 'ConcreteImplementationAttribute' could not be found."));
    }

    [Test]
    public void GetInstance_IndirectActivationException_ForCollectionParameter_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      Assert.That (
          () => _serviceLocator.GetInstance<IInterfaceWithIndirectActivationExceptionForCollectionParameter>(),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.TestDomain.IInterfaceWithIndirectActivationExceptionForCollectionParameter': "
              + "Error resolving indirect collection dependendency of constructor parameter 'innerDependency' of type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.ClassWithIndirectActivationExceptionForCollectionParameter': "
              + "InvalidOperationException: This exception comes from the ctor."));
    }

    [Test]
    public void GetInstance_ExceptionDuringImplictRegistration_ThrowsActivationException_WithOriginalExceptionAsInnerException ()
    {
      Assert.Fail ("TODO Implement");
      Assert.That (
          () => _serviceLocator.GetInstance<IInterfaceWithIndirectActivationExceptionForCollectionParameter>(),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.TestDomain.IX': "
              + "Error resolving type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.X': "
              + "InvalidOperationException: This exception comes from the Registration."));
    }

    class DomainType
    {
      public readonly IEnumerable<ITestMultipleRegistrationsType> AllInstances;

      public DomainType (IEnumerable<ITestMultipleRegistrationsType> allInstances)
      {
        AllInstances = allInstances;
      }
    }

    public interface ISomeInterface { }
  }
}