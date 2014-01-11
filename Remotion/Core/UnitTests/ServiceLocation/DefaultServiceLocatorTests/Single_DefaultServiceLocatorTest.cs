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
using System.Linq;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Single_DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
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
    public void Register_ServiceConfigurationEntry_ImplementationInfoWithFactory ()
    {
      var expectedInstance = new TestConcreteImplementationAttributeType();
      var serviceImplementation = ServiceImplementationInfo.CreateSingle (() => expectedInstance);

      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType),
          serviceImplementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (expectedInstance));
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
    public void GetInstance_ServiceTypeWithNullImplementation ()
    {
      _serviceLocator.RegisterSingle<DefaultServiceLocatorTest.ISomeInterface> (() => null);
      Assert.That (
          () => _serviceLocator.GetInstance (typeof (DefaultServiceLocatorTest.ISomeInterface)),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.DefaultServiceLocatorTest+ISomeInterface'."));
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

  }
}