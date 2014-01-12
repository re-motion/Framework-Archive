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

namespace Remotion.UnitTests.ServiceLocation.DefaultServiceLocatorTests
{
  [TestFixture]
  public class Decorator_DefaultServiceLocatorTest :TestBase
  {
    private DefaultServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = CreateServiceLocator();
    }

    [Test]
    public void GetInstance_SingleDecorator_DecoratesImplementation ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestType),
          new[] { typeof (TestDecorator1) },
          typeof (TestImplementation1));
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestDecorator1>());
      var decoratorInstance = (TestDecorator1) instance;
      Assert.That (decoratorInstance.DecoratedObject, Is.TypeOf<TestImplementation1>());
    }

    [Test]
    public void GetInstance_StackedDecorators_DecoratesImplementationWithFirstDecoratorBeingClosestToImplementation ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestType),
          new[] { typeof (TestDecorator1), typeof (TestDecorator2) },
          typeof (TestImplementation1));
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestDecorator2>());
      var decoratorInstance2 = (TestDecorator2) instance;

      Assert.That (decoratorInstance2.DecoratedObject, Is.TypeOf<TestDecorator1>());
      var decoratorInstance1 = (TestDecorator1) decoratorInstance2.DecoratedObject;

      Assert.That (decoratorInstance1.DecoratedObject, Is.TypeOf<TestImplementation1>());
    }

    [Test]
    public void GetInstance_DecoratorIsInstance_ImplementationIsSingleton_DecoratorBehaveAsSingleton ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestImplementation1), LifetimeKind.Singleton, RegistrationType.Single);
      var decorator = new ServiceImplementationInfo (typeof (TestDecorator1), LifetimeKind.Instance, RegistrationType.Decorator);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestType), implementation, decorator));

      var instance1 = _serviceLocator.GetInstance (typeof (ITestType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance1, Is.SameAs (instance2));
      Assert.That (((TestDecorator1) instance1).DecoratedObject, Is.SameAs (((TestDecorator1) instance2).DecoratedObject));
    }

    [Test]
    public void GetInstance_DecoratorIsSingleton_ImplementationIsInstance_DecoratorBehaveAsInstance ()
    {
      var implementation = new ServiceImplementationInfo (typeof (TestImplementation1), LifetimeKind.Instance, RegistrationType.Single);
      var decorator = new ServiceImplementationInfo (typeof (TestDecorator1), LifetimeKind.Singleton, RegistrationType.Decorator);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestType), implementation, decorator));

      var instance1 = _serviceLocator.GetInstance (typeof (ITestType));
      var instance2 = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance1, Is.Not.SameAs (instance2));
      Assert.That (((TestDecorator1) instance1).DecoratedObject, Is.Not.SameAs (((TestDecorator1) instance2).DecoratedObject));
    }

    [Test]
    public void GetInstance_DecoratorWithAdditionalConstructorParameters_PerformsIndirectResolutionCalls ()
    {
      var serviceConfigurationEntry = CreateServiceConfigurationEntry (
          typeof (ITestType),
          new[] { typeof (TestDecoratorWithAdditionalConstructorParameters) },
          typeof (TestImplementation1));
      _serviceLocator.Register (serviceConfigurationEntry);

      var instance = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestDecoratorWithAdditionalConstructorParameters>());
      var decoratorInstance = (TestDecoratorWithAdditionalConstructorParameters) instance;
      Assert.That (decoratorInstance.DecoratedObject, Is.TypeOf<TestImplementation1>());
      Assert.That (decoratorInstance.SingleService, Is.TypeOf<SingleService>());
      Assert.That (decoratorInstance.MultipleService, Is.Not.Empty);
      Assert.That (decoratorInstance.MultipleService, Has.All.TypeOf<MultipleService>());
    }

    [Test]
    public void GetAllInstances_DecoratesImplementations ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestImplementation1), LifetimeKind.Instance, RegistrationType.Multiple);
      var implementation2 = new ServiceImplementationInfo (typeof (TestImplementation2), LifetimeKind.Instance, RegistrationType.Multiple);
      var decorator = new ServiceImplementationInfo (typeof (TestDecorator1), LifetimeKind.Instance, RegistrationType.Decorator);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestType), implementation1, implementation2, decorator));

      var instances = _serviceLocator.GetAllInstances (typeof (ITestType)).ToArray();

      Assert.That (instances, Has.All.TypeOf<TestDecorator1>());

      var decoratorInstance1 = (TestDecorator1) instances[0];
      Assert.That (decoratorInstance1.DecoratedObject, Is.TypeOf<TestImplementation1>());

      var decoratorInstance2 = (TestDecorator1) instances[1];
      Assert.That (decoratorInstance2.DecoratedObject, Is.TypeOf<TestImplementation2>());
    }

    [Test]
    public void GetInstance_DecoratedCompound_DecoratesCompound_DoesNotDecorateImplementations ()
    {
      var implementation1 = new ServiceImplementationInfo (typeof (TestImplementation1), LifetimeKind.Instance, RegistrationType.Multiple);
      var implementation2 = new ServiceImplementationInfo (typeof (TestImplementation2), LifetimeKind.Instance, RegistrationType.Multiple);
      var decorator = new ServiceImplementationInfo (typeof (TestDecorator1), LifetimeKind.Instance, RegistrationType.Decorator);
      var compound = new ServiceImplementationInfo (typeof (TestCompound), LifetimeKind.Instance, RegistrationType.Compound);
      _serviceLocator.Register (new ServiceConfigurationEntry (typeof (ITestType), implementation1, implementation2, decorator, compound));

      var instance = _serviceLocator.GetInstance (typeof (ITestType));

      Assert.That (instance, Is.TypeOf<TestDecorator1>());
      var decoratorInstance = (TestDecorator1) instance;

      Assert.That (decoratorInstance.DecoratedObject, Is.TypeOf<TestCompound>());
      var compoundInstance = (TestCompound) decoratorInstance.DecoratedObject;

      Assert.That (compoundInstance.InnerObjects, Is.Not.Empty);
      Assert.That (
          compoundInstance.InnerObjects.Select (i => i.GetType()),
          Is.EqualTo (new[] { typeof (TestImplementation1), typeof (TestImplementation2) }));
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