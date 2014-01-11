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
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Multiple_DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
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
    public void GetAllInstances_ServiceTypeWithNullImplementation ()
    {
      var implementation = ServiceImplementationInfo.CreateMultiple<ISomeInterface> (() => null);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ISomeInterface), implementation);

      _serviceLocator.Register (serviceConfigurationEntry);
      Assert.That (
          () => _serviceLocator.GetAllInstances (typeof (ISomeInterface)).ToArray(),
          Throws.TypeOf<ActivationException> ().With.Message.EqualTo (
              "The registered factory returned null instead of an instance implementing the requested service type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.Multiple_DefaultServiceLocatorTest+ISomeInterface'."));
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
    public void Register_NoFactories_OverridesAttributes ()
    {
      _serviceLocator.RegisterMultiple<ITestInstanceConcreteImplementationAttributeType>();

      Assert.That (_serviceLocator.GetAllInstances (typeof (object)), Is.Empty);
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