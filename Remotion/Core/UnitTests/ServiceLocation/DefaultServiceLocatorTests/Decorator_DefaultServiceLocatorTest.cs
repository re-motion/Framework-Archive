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
using Remotion.ServiceLocation;
using Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain;
using Remotion.UnitTests.ServiceLocation.TestDomain;

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Decorator_DefaultServiceLocatorTest
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = DefaultServiceLocator.Create();
    }

    [Test]
    public void Register_DecoratorWithoutPublicConstructor_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestDecoratorWithErrors),
          new[] { typeof (TestDecoratorWithoutPublicConstructor) },
          typeof (TestDecoratorWithErrorsImplementation));

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestDecoratorWithoutPublicConstructor' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestDecoratorWithErrors'."));
    }

    [Test]
    public void Register_DecoratorWithConstructorWithoutArguments_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestDecoratorWithErrors),
          new[] { typeof (TestDecoratorWithConstructorWithoutArguments) },
          typeof (TestDecoratorWithErrorsImplementation));

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestDecoratorWithConstructorWithoutArguments' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestDecoratorWithErrors'."));
    }

    [Test]
    public void Register_DecoratorWithConstructorWithoutMatchingArgument_ThrowsInvalidOperationException ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestDecoratorWithErrors),
          new[] { typeof (TestDecoratorWithConstructorWithoutMatchingArgument) },
          typeof (TestDecoratorWithErrorsImplementation));

      Assert.That (
          () => _serviceLocator.Register (serviceConfigurationEntry),
          Throws.InvalidOperationException.With.Message.EqualTo (
              "Type 'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.TestDecoratorWithConstructorWithoutMatchingArgument' cannot be instantiated. "
              + "The type must have exactly one public constructor. The public constructor must at least accept an argument of type "
              + "'Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests.TestDomain.ITestDecoratorWithErrors'."));
    }

    [Test]
    public void GetInstance_Decorator ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestDecoratorRegistration),
          new[] { typeof (TestDecoratorRegistrationDecorator) },
          typeof (TestDecoratorRegistrationDecoratedObject1));
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestDecoratorRegistration));

      Assert.That (instance, Is.InstanceOf<TestDecoratorRegistrationDecorator>());
      var decoratorInstance = (TestDecoratorRegistrationDecorator) instance;
      Assert.That (decoratorInstance.DecoratedObject, Is.InstanceOf<TestDecoratorRegistrationDecoratedObject1>());
    }
    
    [Test]
    public void GetInstance_StackedDecorators ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestStackedDecorators),
          new[] { typeof (TestStackedDecoratorsDecorator1), typeof (TestStackedDecoratorsDecorator2) },
          typeof (TestStackedDecoratorsObject1));
      _serviceLocator.Register (serviceConfigurationEntry);

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
    
    private ServiceConfigurationEntry CreateServiceConfigurationEntry (
        Type serviceType,
        Type[] decoratorTypes,
        Type implementationType,
        LifetimeKind lifetimeKind = LifetimeKind.Instance)
    {
      var implementations = decoratorTypes.Select (t => new ServiceImplementationInfo (t, lifetimeKind, RegistrationType.Decorator))
          .Concat (new[] { new ServiceImplementationInfo (implementationType, lifetimeKind, RegistrationType.Single) });
      return new ServiceConfigurationEntry (serviceType, implementations);
    }
  }
}