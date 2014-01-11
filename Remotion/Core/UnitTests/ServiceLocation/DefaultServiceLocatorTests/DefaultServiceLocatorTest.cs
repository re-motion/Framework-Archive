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