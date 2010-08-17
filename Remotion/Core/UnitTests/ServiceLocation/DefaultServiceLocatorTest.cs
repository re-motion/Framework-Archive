// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Implementation;
using Remotion.ServiceLocation;

namespace Remotion.UnitTests.ServiceLocation
{
  [TestFixture]
  public class DefaultServiceLocatorTest
  {
    private IServiceLocator _serviceLocator;

    [SetUp]
    public void SetUp ()
    {
      _serviceLocator = new DefaultServiceLocator();
    }

    [Test]
    public void GetService_TypeWithoutConcreteImplementatioAttribute ()
    {
      var result = _serviceLocator.GetService (typeof (string));

      Assert.That (result, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (ActivationException),
        ExpectedMessage = "Cannot get a version-dependent implementation of type 'Microsoft.Practices.ServiceLocation.IServiceLocator': " +
                          "Expected 'ConcreteImplementationAttribute' could not be found.")]
    public void GetInstance_ServiceTypeWithoutConcreteImplementationAttribute ()
    {
      _serviceLocator.GetInstance (typeof (IServiceLocator));
    }

    [Test]
    [ExpectedException (typeof (InvalidCastException),
        ExpectedMessage =
            "Unable to cast object of type 'Remotion.UnitTests.ServiceLocation.TestConcreteImplementationAttributeType' to type 'Remotion.UnitTests.ServiceLocation.ITestConcreteImplementationAttributeTypeWithoutImplementation'."
        )]
    public void GetInstance_ServiceTypeWithoutConcreteImplementationType ()
    {
      _serviceLocator.GetInstance<ITestConcreteImplementationAttributeTypeWithoutImplementation>();
    }

    [Test]
    public void GetService_TypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetService (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_TypeWithConcreteImplementationAttribute ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
      Assert.IsInstanceOfType (
          typeof (TestConcreteImplementationAttributeType), SafeServiceLocator.Current.GetInstance<ITestInstanceConcreteImplementationAttributeType>());
    }

    [Test]
    public void GetInstance_GetInstancesFromCache ()
    {
      var testableSerciceLocator = new TestableDefaultServiceLocator();
      testableSerciceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (
          testableSerciceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType)),
          Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));

      Func<object> instanceCreator;
      Assert.That (testableSerciceLocator.Cache.TryGetValue (typeof (ITestInstanceConcreteImplementationAttributeType), out instanceCreator), Is.True);
      Assert.That (instanceCreator(), Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
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
    public void GetInstanceWithKeyParamete_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance (typeof (ITestInstanceConcreteImplementationAttributeType), "Test");

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetAllInstances ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (ITestInstanceConcreteImplementationAttributeType));

      Assert.That (result.ToArray().Length, Is.EqualTo (1));
      Assert.That (result.ToArray()[0], Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetAllInstances_ConreteImplementationAttributeIsNotDefined ()
    {
      var result = _serviceLocator.GetAllInstances (typeof (IServiceLocator));

      Assert.That (result.ToArray().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetInstanceWithGenericType ()
    {
      var result = _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType>();

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstanceWithGenericTypeAndKeyParameter_KeyIsIgnored ()
    {
      var result = _serviceLocator.GetInstance<ITestInstanceConcreteImplementationAttributeType> ("Test");

      Assert.That (result, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetAllInstancesWithGenericType_ConcreteImplementationAttributeIsNotDefined ()
    {
      var result = _serviceLocator.GetAllInstances<IServiceLocator>();

      Assert.That (result.ToArray().Length, Is.EqualTo (0));
    }

    [Test]
    public void GetInstance_ConstructorInjection_OneParameter ()
    {
      var result = _serviceLocator.GetInstance<ITestConstructorInjectionWithOneParameter>();

      Assert.That(result, Is.TypeOf(typeof(TestConstructorInjectionWithOneParameter)));
      Assert.That (((TestConstructorInjectionWithOneParameter) result).Param, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    public void GetInstance_ConstructorInjection_ThreeParametersRecursive ()
    {
      var result = _serviceLocator.GetInstance<ITestConstructorInjectionWithThreeParameters> ();

      Assert.That (result, Is.TypeOf (typeof (TestConstructorInjectionWithThreeParameters)));
      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param1, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param2, Is.TypeOf (typeof (TestConstructorInjectionWithOneParameter)));
      Assert.That (((TestConstructorInjectionWithThreeParameters) result).Param3, Is.TypeOf (typeof (TestConcreteImplementationAttributeType)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), 
      ExpectedMessage = "Type 'TestTypeWithNotExactOnePublicConstructor' has not exact one public constructor and cannot be instantiated.")]
    public void GetInstance_TypeHasNotExactOnePublicConstructor ()
    {
      _serviceLocator.GetInstance<ITestTypeWithNotExactOnePublicConstructor>();
    }

  }

  [ConcreteImplementation ("Remotion.UnitTests.ServiceLocation.TestConcreteImplementationAttributeType, Remotion.UnitTests, Version = <version>",
      LifeTime = LifetimeKind.Instance)]
  internal interface ITestInstanceConcreteImplementationAttributeType
  {
  }

  [ConcreteImplementation ("Remotion.UnitTests.ServiceLocation.TestConcreteImplementationAttributeType, Remotion.UnitTests, Version = <version>",
      LifeTime = LifetimeKind.Singleton)]
  internal interface ITestSingletonConcreteImplementationAttributeType { }

  [ConcreteImplementation ("Remotion.UnitTests.ServiceLocation.TestConcreteImplementationAttributeType, Remotion.UnitTests, Version = <version>")]
  internal interface ITestConcreteImplementationAttributeTypeWithoutImplementation { }

  public class TestConcreteImplementationAttributeType
      : ITestInstanceConcreteImplementationAttributeType, ITestSingletonConcreteImplementationAttributeType
  {
    public TestConcreteImplementationAttributeType ()
    {
    }
  }

  [ConcreteImplementation ("Remotion.UnitTests.ServiceLocation.TestConstructorInjectionWithOneParameter, Remotion.UnitTests, Version = <version>")]
  internal interface ITestConstructorInjectionWithOneParameter { }

  internal class TestConstructorInjectionWithOneParameter : ITestConstructorInjectionWithOneParameter
  {
    public readonly ITestSingletonConcreteImplementationAttributeType Param;

    public TestConstructorInjectionWithOneParameter (ITestSingletonConcreteImplementationAttributeType param)
    {
      Param = param;
    }
  }

   [ConcreteImplementation ("Remotion.UnitTests.ServiceLocation.TestConstructorInjectionWithThreeParameters, Remotion.UnitTests, Version = <version>")]
  internal interface ITestConstructorInjectionWithThreeParameters { }

  internal class TestConstructorInjectionWithThreeParameters : ITestConstructorInjectionWithThreeParameters
  {
    public readonly ITestConstructorInjectionWithOneParameter Param1;
    public readonly ITestConstructorInjectionWithOneParameter Param2;
    public readonly ITestSingletonConcreteImplementationAttributeType Param3;
    
    public TestConstructorInjectionWithThreeParameters (
        ITestConstructorInjectionWithOneParameter param1,
        ITestConstructorInjectionWithOneParameter param2,
        ITestSingletonConcreteImplementationAttributeType param3)
    {
      Param1 = param1;
      Param2 = param2;
      Param3 = param3;
    }
  }

  [ConcreteImplementation ("Remotion.UnitTests.ServiceLocation.TestTypeWithNotExactOnePublicConstructor, Remotion.UnitTests, Version = <version>")]
  internal interface ITestTypeWithNotExactOnePublicConstructor { }

  internal class TestTypeWithNotExactOnePublicConstructor : ITestTypeWithNotExactOnePublicConstructor
  {
    public TestTypeWithNotExactOnePublicConstructor () { }
    public TestTypeWithNotExactOnePublicConstructor (ITestSingletonConcreteImplementationAttributeType param) { }
  }
}