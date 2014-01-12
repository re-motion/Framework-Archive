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
  public class Errors_DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }
    
    [Test]
    public void Register_TypeWithTooManyPublicCtors_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestTypeWithTooManyPublicConstructors), LifetimeKind.Singleton, RegistrationType.Single);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestTypeWithErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithTooManyPublicConstructors' cannot be instantiated. "
              + "The type must have exactly one public constructor."));
    }

    [Test]
    public void Register_TypeWithOnlyNonPublicCtor_ThrowsInvalidOperationException ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestTypeWithOnlyNonPublicConstructor), LifetimeKind.Singleton, RegistrationType.Single);
      var serviceConfigurationEntry = new ServiceConfigurationEntry (typeof (ITestTypeWithErrors), implementation);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithOnlyNonPublicConstructor' cannot be instantiated. " 
              + "The type must have exactly one public constructor."));
    }

    [Test]
    public void Register_Twice_ExceptionIsThrown ()
    {
      var serviceConfigurationEntry = new ServiceConfigurationEntry (
          typeof (ITestInstanceConcreteImplementationAttributeType),
          new ServiceImplementationInfo (typeof (TestConcreteImplementationAttributeType), LifetimeKind.Instance, RegistrationType.Single));
      _serviceLocator.Register (serviceConfigurationEntry);

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Register cannot be called twice or after GetInstance for service type: 'ITestInstanceConcreteImplementationAttributeType'."));
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
  }
}