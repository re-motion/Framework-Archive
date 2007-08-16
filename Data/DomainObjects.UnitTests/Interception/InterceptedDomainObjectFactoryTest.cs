using System;
using System.IO;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Configuration;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using File=System.IO.File;

namespace Rubicon.Data.DomainObjects.UnitTests.Interception
{
  [TestFixture]
  public class InterceptedDomainObjectFactoryTest : ClientTransactionBaseTest
  {
    private InterceptedDomainObjectFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();
      _factory = new InterceptedDomainObjectFactory ();
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
    public void SaveReturnsPathOfGeneratedAssemblySigned ()
    {
      _factory.GetConcreteDomainObjectType (typeof (Order));
      string[] paths = _factory.SaveGeneratedAssemblies ();
      Assert.AreEqual (1, paths.Length);
      Assert.AreEqual (Path.Combine (Environment.CurrentDirectory, "Rubicon.Data.DomainObjects.Generated.Signed.dll"), paths[0]);
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
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyIntegrationTest+NonInstantiableAbstractClass as its member Foo is abstract (and not an "
        + "automatic property).\r\nParameter name: baseType")]
    public void AbstractWithMethodCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (InterceptedPropertyIntegrationTest.NonInstantiableAbstractClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type "
        + "Rubicon.Data.DomainObjects.UnitTests.Interception.InterceptedPropertyIntegrationTest+NonInstantiableAbstractClassWithProps, "
        + "property Foo is abstract but not defined in the mapping (assumed property id: "
        + "Rubicon.Data.DomainObjects.UnitTests.Interception.InterceptedPropertyIntegrationTest+NonInstantiableAbstractClassWithProps.Foo)."
        + "\r\nParameter name: baseType")]
    public void AbstractWithNonAutoPropertiesCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (InterceptedPropertyIntegrationTest.NonInstantiableAbstractClassWithProps));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests."
        + "Interception.InterceptedPropertyIntegrationTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter, automatic "
        + "properties for related object collections cannot have setters: property 'RelatedObjects', property id 'Rubicon.Data.DomainObjects."
        + "UnitTests.Interception.InterceptedPropertyIntegrationTest+NonInstantiableClassWithAutomaticRelatedCollectionSetter."
        + "RelatedObjects'.\r\nParameter name: baseType")]
    public void AbstractWithAutoCollectionSetterCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (InterceptedPropertyIntegrationTest.NonInstantiableClassWithAutomaticRelatedCollectionSetter));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Cannot instantiate type Rubicon.Data.DomainObjects.UnitTests.Interception."
        + "InterceptedPropertyIntegrationTest+NonInstantiableSealedClass as it is sealed.\r\nParameter name: baseType")]
    public void SealedCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (InterceptedPropertyIntegrationTest.NonInstantiableSealedClass));
    }

    [Test]
    [ExpectedException (typeof (ArgumentTypeException))]
    public void NonDomainCannotBeInstantiated ()
    {
      _factory.GetConcreteDomainObjectType (typeof (InterceptedPropertyIntegrationTest.NonInstantiableNonDomainClass));
    }
  }
}