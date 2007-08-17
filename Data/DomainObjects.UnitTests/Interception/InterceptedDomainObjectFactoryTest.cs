using System;
using System.IO;
using System.Runtime.Serialization;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Reflection;
using Rubicon.Utilities;
using File=System.IO.File;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception
{
  [TestFixture]
  public class InterceptedDomainObjectFactoryTest : ClientTransactionBaseTest
  {
    private InterceptedDomainObjectFactory _factory;
    private readonly string _assemblyDirectory = Path.Combine (Environment.CurrentDirectory, "Interception.InterceptedDomainObjectFactoryTest.Dlls");

    public override void SetUp ()
    {
      base.SetUp ();
      if (!Directory.Exists (_assemblyDirectory))
        Directory.CreateDirectory (_assemblyDirectory);
      _factory = new InterceptedDomainObjectFactory (_assemblyDirectory);
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsAssignableType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.IsTrue (typeof (Order).IsAssignableFrom (concreteType));
    }

    [Test]
    public void GetConcreteDomainObjectTypeReturnsDifferentType ()
    {
      Type concreteType = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreNotEqual (typeof (Order), concreteType);
    }

    [Test]
    public void FactoryCachesGeneratedTypes ()
    {
      Type concreteType1 = _factory.GetConcreteDomainObjectType (typeof (Order));
      Type concreteType2 = _factory.GetConcreteDomainObjectType (typeof (Order));
      Assert.AreSame (concreteType1, concreteType2);
    }

    [Test]
    public void SaveReturnsPathOfGeneratedAssemblySigned ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      string[] paths = _factory.SaveGeneratedAssemblies ();
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (_assemblyDirectory, "Rubicon.Data.DomainObjects.Generated.Signed.dll"), paths[0]);
      Assert.IsTrue (File.Exists (paths[0]));
    }

    [Test]
    public void CanContinueToGenerateTypesAfterSave ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      _factory.SaveGeneratedAssemblies ();
      _factory.GetConcreteDomainObjectType (typeof (OrderItem));
      _factory.SaveGeneratedAssemblies ();
      _factory.GetConcreteDomainObjectType (typeof (ClassWithAllDataTypes));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.TestDomain."
        + "AbstractClass as it is abstract; for classes with automatic properties, InstantiableAttribute must be used.\r\nParameter name: baseType")]
    public void AbstractWithoutInstantiableAttributeCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (AbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.SampleTypes.NonInstantiableAbstractClass as its member Foo is abstract (and not an "
        + "automatic property).\r\nParameter name: baseType")]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type "
        + "Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes.NonInstantiableAbstractClassWithProps, "
        + "property Foo is abstract but not defined in the mapping (assumed property id: "
        + "Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes.NonInstantiableAbstractClassWithProps.Foo)."
        + "\r\nParameter name: baseType")]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableAbstractClassWithProps));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.SampleTypes.NonInstantiableClassWithAutomaticRelatedCollectionSetter, automatic "
        + "properties for related object collections cannot have setters: property 'RelatedObjects', property id 'Rubicon.Data.DomainObjects."
        + "UnitTests.Interception.SampleTypes.NonInstantiableClassWithAutomaticRelatedCollectionSetter."
        + "RelatedObjects'.\r\nParameter name: baseType")]
    public void AbstractWithAutoCollectionSetterCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableClassWithAutomaticRelatedCollectionSetter));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.Interception."
        + "SampleTypes.NonInstantiableSealedClass as it is sealed.\r\nParameter name: baseType")]
    public void SealedCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableSealedClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void NonDomainCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (NonInstantiableNonDomainClass));
    }

    [Test]
    public void WasCreatedByFactory ()
    {
      Assert.IsTrue (_factory.WasCreatedByFactory (_factory.GetConcreteDomainObjectType (typeof (Order))));
      Assert.IsFalse (_factory.WasCreatedByFactory (typeof (Order)));
    }

    [Test]
    public void GetTypesafeConstructorInvoker ()
    {
      IFuncInvoker<Order> invoker = _factory.GetTypesafeConstructorInvoker<Order> (_factory.GetConcreteDomainObjectType (typeof (Order)));
      Order order = invoker.With ();
      Assert.IsNotNull (order);
      Assert.AreSame (_factory.GetConcreteDomainObjectType (typeof (Order)), ((object) order).GetType ());
    }

    [Test]
    public void GetTypesafeConstructorInvokerWithConstructors ()
    {
      IFuncInvoker<DOWithConstructors> invoker =
          _factory.GetTypesafeConstructorInvoker<DOWithConstructors> (
              _factory.GetConcreteDomainObjectType (typeof (DOWithConstructors)));
      DOWithConstructors instance = invoker.With ("17", "4");
      Assert.IsNotNull (instance);
      Assert.AreEqual ("17", instance.FirstArg);
      Assert.AreEqual ("4", instance.SecondArg);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The type Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order was not "
        + "created by InterceptedDomainObjectFactory.GetConcreteDomainObjectType.\r\nParameter name: type")]
    public void GetTypesafeConstructorInvokerThrowsOnTypeNotCreatedByFactory ()
    {
      _factory.GetTypesafeConstructorInvoker<Order> (typeof (Order));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException),
        ExpectedMessage = "Argument type is a .*Order_WithInterception.* which cannot be assigned to type .*OrderTicket.",
        MatchType = MessageMatch.Regex)]
    public void GetTypesafeConstructorInvokerThrowsOnInvalidTMinimimal ()
    {
      _factory.GetTypesafeConstructorInvoker<OrderTicket> (_factory.GetConcreteDomainObjectType (typeof (Order)));
    }

    [Test]
    public void PrepareUnconstructedInstance ()
    {
      Order order = (Order) FormatterServices.GetSafeUninitializedObject (_factory.GetConcreteDomainObjectType (typeof (Order)));
      _factory.PrepareUnconstructedInstance (order);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object's type "
        + "Rubicon.Data.DomainObjects.UnitTests.Interception.SampleTypes.DOWithConstructors was not "
        + "created by InterceptedDomainObjectFactory.GetConcreteDomainObjectType.\r\nParameter name: instance")]
    public void PrepareUnconstructedInstanceThrowsOnTypeNotCreatedByFactory ()
    {
      _factory.PrepareUnconstructedInstance (new DOWithConstructors (7));
    }
  }
}