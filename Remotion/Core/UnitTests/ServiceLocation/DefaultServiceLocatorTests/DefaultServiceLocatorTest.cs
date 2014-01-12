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
  public class DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void Register_Factory_ServiceIsAdded ()
    {
      var instance = new TestConcreteImplementationAttributeType();

      _serviceLocator.RegisterSingle<ITestInstanceConcreteImplementationAttributeType> (() => instance);

      Assert.That (_serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType)), Is.SameAs (instance));
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
      var serviceImplementation = ServiceImplementationInfo.CreateSingle (() => expectedInstance);

      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestSingletonConcreteImplementationAttributeType),
          serviceImplementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestSingletonConcreteImplementationAttributeType));
      Assert.That (instance1, Is.SameAs (expectedInstance));
    }

    [Test]
    public void GetInstance_TypeWithGenericServiceInterface ()
    {

      var serviceLocator = DefaultServiceLocator.Create();

      Assert.That (serviceLocator.GetInstance (typeof (ITestOpenGenericService<int>)), Is.TypeOf (typeof (TestOpenGenericIntImplementation)));
      Assert.That (serviceLocator.GetInstance (typeof (ITestOpenGenericService<string>)), Is.TypeOf (typeof (TestOpenGenericStringImplementation)));
      Assert.That (
          SafeServiceLocator.Current.GetInstance<ITestOpenGenericService<int>>(),
          Is.InstanceOf (typeof (TestOpenGenericIntImplementation)));
    }

    [Test]
    public void GetInstance_NotImplementedGenericServiceInterface ()
    {
      var serviceLocator = DefaultServiceLocator.Create();

      Assert.That (() => serviceLocator.GetInstance (typeof (ITestOpenGenericService<object>)), 
        Throws.Exception.InstanceOf<ActivationException>());
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