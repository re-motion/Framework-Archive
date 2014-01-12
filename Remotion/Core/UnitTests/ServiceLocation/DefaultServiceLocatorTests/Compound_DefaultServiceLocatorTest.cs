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
using NUnit.Framework;
using Remotion.FunctionalProgramming;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;
using Remotion.UnitTests.ServiceLocation.TestDomain;

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
    public void GetInstance_Compound ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestCompoundRegistration),
          typeof (TestCompoundRegistration),
          new[]{typeof (TestCompoundImplementation1), typeof (TestCompoundImplementation2)});
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestCompoundRegistration));

      Assert.That (instance, Is.InstanceOf<TestCompoundRegistration>());
      var compoundRegistration = (TestCompoundRegistration) instance;
      Assert.That (compoundRegistration.CompoundRegistrations, Is.Not.Empty);
      Assert.That (
          compoundRegistration.CompoundRegistrations.Select (c => c.GetType()),
          Is.EqualTo (new[] { typeof (TestCompoundImplementation1), typeof (TestCompoundImplementation2) }));
    }
        
    [Test]
    public void GetInstance_Compound_ReturnsFirstCompoundImplementation_OrderedByPosition ()
    {
      //TODO TT: Exception message
      var compound1 = new ServiceImplementationInfo (typeof (TestCompoundRegistration), LifetimeKind.Instance, RegistrationType.Compound);
      var compound2 = new ServiceImplementationInfo (typeof (TestCompoundRegistration2), LifetimeKind.Instance, RegistrationType.Compound);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestCompoundRegistration), compound1, compound2);
      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo ("compound"));

      var instance = _serviceLocator.GetInstance (typeof (ITestCompoundRegistration));

      Assert.That (instance, Is.InstanceOf<TestCompoundRegistration>());
    }

    [Test]
    public void GetInstance_CompoundWithEmptyImplementationsList ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestCompoundRegistration),
          typeof (TestCompoundRegistration),
          new Type[0]);
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestCompoundRegistration));

      Assert.That (instance, Is.InstanceOf<TestCompoundRegistration>());
      var compoundRegistration = (TestCompoundRegistration) instance;
      Assert.That (compoundRegistration.CompoundRegistrations, Is.Empty);
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