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
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;
using Remotion.UnitTests.ServiceLocation.TestDomain;
using Rhino.Mocks;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Errors_DefaultServiceLocatorTest : TestBase
  {
    [Test]
    public void Register_TypeWithTooManyPublicCtors_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestTypeWithErrors),
          typeof (TestTypeWithTooManyPublicConstructors));

      var serviceLocator = CreateServiceLocator();

      Assert.That (
          () => serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithTooManyPublicConstructors' cannot be instantiated. "
              + "The type must have exactly one public constructor."));
    }

    [Test]
    public void Register_TypeWithOnlyNonPublicCtor_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestTypeWithErrors),
          typeof (TestTypeWithOnlyNonPublicConstructor));

      var serviceLocator = CreateServiceLocator();

      Assert.That (
          () => serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithOnlyNonPublicConstructor' cannot be instantiated. "
              + "The type must have exactly one public constructor."));
    }

    [Test]
    public void Register_Twice_ExceptionIsThrown ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementation1));

      var serviceLocator = CreateServiceLocator();
      serviceLocator.Register (serviceConfigurationEntry);

      Assert.That (
          () => serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Register cannot be called twice or after GetInstance for service type: 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType'."));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringDependencyResolution_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementationWithMultipleConstructorParameters));

      var expectedException = new Exception ("Expected Exception Message");
      var serviceConfigurationDiscoveryServiceStub = MockRepository.GenerateStrictMock<IServiceConfigurationDiscoveryService>();
      serviceConfigurationDiscoveryServiceStub.Stub (_ => _.GetDefaultConfiguration (typeof (InstanceService))).Throw (expectedException);

      var serviceLocator = CreateServiceLocator (serviceConfigurationDiscoveryServiceStub);
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateMultipleService());
      serviceLocator.Register (CreateSingletonService());

      Assert.That (
          () => serviceLocator.GetInstance (typeof (ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect dependency of constructor parameter 'instanceService1' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestImplementationWithMultipleConstructorParameters': "
              + "Error resolving service Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.InstanceService': "
              + "Expected Exception Message"));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringConstructor_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestTypeWithConstructorThrowingSingleDependency));

      var serviceConfigurationEntryForError = CreateSingleServiceConfigurationEntry (
          typeof (ITestTypeWithErrors),
          typeof (TestTypeWithConstructorThrowingException));

      var serviceLocator = CreateServiceLocator ();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (serviceConfigurationEntryForError);

      Assert.That (
          () => serviceLocator.GetInstance (typeof (ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect dependency of constructor parameter 'param' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithConstructorThrowingSingleDependency': "
              + "ApplicationException: This exception comes from the ctor."));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringDependencyResolution_ForCollectionParameter_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestImplementationWithMultipleConstructorParameters));

      var expectedException = new Exception ("Expected Exception Message");
      var serviceConfigurationDiscoveryServiceStub = MockRepository.GenerateStrictMock<IServiceConfigurationDiscoveryService>();
      serviceConfigurationDiscoveryServiceStub.Stub (_ => _.GetDefaultConfiguration (typeof (MultipleService))).Throw (expectedException);

      var serviceLocator = CreateServiceLocator (serviceConfigurationDiscoveryServiceStub);
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (CreateInstanceService());
      serviceLocator.Register (CreateSingletonService());

      Assert.That (
          () => serviceLocator.GetInstance (typeof (ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect collection dependency of constructor parameter 'multipleService' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestImplementationWithMultipleConstructorParameters': "
              + "Error resolving service Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.MultipleService': "
              + "Expected Exception Message"));
    }

    [Test]
    public void GetInstance_IndirectActivationExceptionDuringConstructor_ForCollectionParameter_ThrowsActivationException_CausesFullMessageToBeBuilt ()
    {
      var serviceConfigurationEntry = CreateSingleServiceConfigurationEntry (
          typeof (ITestType),
          typeof (TestTypeWithConstructorThrowingMultipleDependency));

      var serviceConfigurationEntryForError = CreateMultipleServiceConfigurationEntry (
          typeof (ITestTypeWithErrors),
          new[] { typeof (TestTypeWithConstructorThrowingException) });

      var serviceLocator = CreateServiceLocator ();
      serviceLocator.Register (serviceConfigurationEntry);
      serviceLocator.Register (serviceConfigurationEntryForError);

      Assert.That (
          () => serviceLocator.GetInstance (typeof (ITestType)),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestType': "
              + "Error resolving indirect collection dependency of constructor parameter 'param' "
              + "of type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestTypeWithConstructorThrowingMultipleDependency': "
              + "ApplicationException: This exception comes from the ctor."));
    }

    [Test]
    public void GetInstance_ExceptionDuringImplictRegistration_ThrowsActivationException_WithOriginalExceptionAsInnerException ()
    {
      var serviceLocator = CreateServiceLocator();

      Assert.Fail ("TODO Implement");
      Assert.That (
          () => serviceLocator.GetInstance<IInterfaceWithIndirectActivationExceptionForCollectionParameter>(),
          Throws.TypeOf<ActivationException>().With.Message.EqualTo (
              "Could not resolve type 'Remotion.UnitTests.ServiceLocation.TestDomain.IX': "
              + "Error resolving type "
              + "'Remotion.UnitTests.ServiceLocation.TestDomain.X': "
              + "InvalidOperationException: This exception comes from the Registration."));
    }
  }
}