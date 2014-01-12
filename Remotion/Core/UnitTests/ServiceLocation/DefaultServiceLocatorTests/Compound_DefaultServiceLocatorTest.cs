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

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Register_DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void GetInstance_Compound_InstantiatesImplementationsInOrderOfRegistration ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestCompound),
          new[] { typeof (TestImplementation1), typeof (TestImplementation2) });
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestCompound>());
      var compoundRegistration = (TestCompound) instance;
      Assert.That (
          compoundRegistration.CompoundRegistrations.Select (c => c.GetType()),
          Is.EqualTo (new[] { typeof (TestImplementation1), typeof (TestImplementation2) }));
    }

    [Test]
    public void GetInstance_CompoundWithEmptyImplementationsList_InstantiatesEmptyCompound ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestCompound),
          new Type[0]);
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestCompound>());
      var compoundRegistration = (TestCompound) instance;
      Assert.That (compoundRegistration.CompoundRegistrations, Is.Empty);
    }

    [Test]
    public void GetInstance_CompoundIsInstance_ImplementationIsSingleton_ImplementationsBehaveAsSingleton ()
    {
      var compound = new ServiceImplementationInfo (typeof (TestCompound), LifetimeKind.Instance, RegistrationType.Compound);
      var implementation = new ServiceImplementationInfo (typeof (TestImplementation1), LifetimeKind.Singleton, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestType), compound, implementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (((TestCompound) instance1).CompoundRegistrations, Is.EqualTo (((TestCompound) instance2).CompoundRegistrations));
    }

    [Test]
    public void GetInstance_CompoundIsSingleton_ImplementationIsInstance_ImplementationsBehaveAsSingleton()
    {
      var compound = new ServiceImplementationInfo (typeof (TestCompound), LifetimeKind.Singleton, RegistrationType.Compound);
      var implementation = new ServiceImplementationInfo (typeof (TestImplementation1), LifetimeKind.Instance, RegistrationType.Multiple);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestType), compound, implementation);

      _serviceLocator.Register (serviceConfigurationEntry);

      var instance1 = _serviceLocator.GetInstance (typeof (ITestType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance1, Is.SameAs (instance2));
      Assert.That (((TestCompound) instance1).CompoundRegistrations, Is.EqualTo (((TestCompound) instance2).CompoundRegistrations));
    }

    [Test]
    public void GetInstance_CompoundWithAdditionalConstructorParameters_PerformsIndirectResolutionCalls ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestCompoundWithAdditionalConstructorParameters),
          new[] { typeof (TestImplementation1) });
      _serviceLocator.Register (serviceConfigurationEntry);

      _serviceLocator.Register (
          new ServiceConfigurationEntry (
              typeof (StubService),
              new ServiceImplementationInfo (typeof (StubService), LifetimeKind.Singleton, RegistrationType.Single)));

      var instance = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestCompoundWithAdditionalConstructorParameters>());
      var compoundRegistration = (TestCompoundWithAdditionalConstructorParameters) instance;
      Assert.That (compoundRegistration.CompoundRegistrations, Is.Not.Empty);
      Assert.That (compoundRegistration.StubService, Is.TypeOf<StubService>());
    }

    [Test]
    public void GetAllInstances_ThrowsActivationException ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestCompound),
          new Type[0]);
      _serviceLocator.Register (serviceConfigurationEntry);

      Assert.That (
          () => _serviceLocator.GetInstance<ITestType>(),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "A compound implemetation is configured for service type 'Remotion.UnitTests.ServiceLocation.ServiceLocatorTests.TestDomain.ITestType'. "
              + "Consider using 'GetInstance'."));
    }

    [Test]
    public void Register_CompoundWithNoPublicConstructor_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestCompoundWithErrors),
          typeof (TestCompoundWithoutPublicConstructor),
          new Type[0]);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestCompoundWithoutPublicConstructor' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithConstructorWithoutArguments_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestCompoundWithErrors),
          typeof (TestCompoundWithConstructorWithoutArguments),
          new Type[0]);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestCompoundWithConstructorWithoutArguments' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithConstructorWithoutMatchingArgument_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestCompoundWithErrors),
          typeof (TestCompoundWithConstructorWithoutMatchingArgument),
          new Type[0]);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestCompoundWithConstructorWithoutMatchingArgument' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'System.Collections.Generic.IEnumerable`1[Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors]'."));
    }

    [Test]
    public void Register_CompoundWithMultipleRegistrations_ThrowsInvalidOperationException ()
    {
      var compound1 = new ServiceImplementationInfo (
          typeof (TestCompoundWithMultipleRegistrations1),
          LifetimeKind.Instance,
          RegistrationType.Compound);
      var compound2 = new ServiceImplementationInfo (
          typeof (TestCompoundWithMultipleRegistrations2),
          LifetimeKind.Instance,
          RegistrationType.Compound);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestCompoundWithErrors), compound1, compound2);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Cannot register multiple implementations with registration type 'Compound' "
              + "for service type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestCompoundWithErrors'."));
    }

    private ServiceConfigurationEntry CreateServiceConfigurationEntry (
        Type serviceType,
        Type compoundType,
        Type[] implementationTypes,
        LifetimeKind lifetimeKind = LifetimeKind.Instance)
    {
      var implementations = new[] { new ServiceImplementationInfo (compoundType, lifetimeKind, RegistrationType.Compound) }
          .Concat (implementationTypes.Select (t => new ServiceImplementationInfo (t, lifetimeKind, RegistrationType.Multiple)));
      return new ServiceConfigurationEntry (serviceType, implementations);
    }
  }
}