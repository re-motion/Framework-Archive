using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Mapping;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class PropertyIndexerTest : ClientTransactionBaseTest
  {
    [Test]
    public void WorksForExistingProperty()
    {
      PropertyIndexer indexer = new PropertyIndexer (IndustrialSector.NewObject());
      Assert.IsNotNull (indexer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"]);
      Assert.AreSame (
          MappingConfiguration.Current.ClassDefinitions[typeof (IndustrialSector)]
              .GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"),
          indexer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name"].PropertyDefinition);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object type Rubicon.Data.DomainObjects.UnitTests.TestDomain."
        + "IndustrialSector does not have a mapping property named 'Bla'.\r\nParameter name: propertyName")]
    public void ThrowsForNonExistingProperty ()
    {
      PropertyIndexer indexer = new PropertyIndexer (IndustrialSector.NewObject ());
      object o = indexer["Bla"];
    }

    [Test]
    public void Count ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (6, order.Properties.Count);

      OrderItem orderItem = OrderItem.NewObject ();
      Assert.AreEqual (3, orderItem.Properties.Count);

      ClassWithAllDataTypes cwadt = ClassWithAllDataTypes.NewObject ();
      Assert.AreEqual (41, cwadt.Properties.Count);
    }

    [Test]
    public void GetEnumeratorGeneric ()
    {
      Order order = Order.NewObject();
      List<string> propertyNames = new List<string> ();
      foreach (PropertyAccessor propertyAccessor in (IEnumerable<PropertyAccessor>)order.Properties)
      {
        propertyNames.Add (propertyAccessor.PropertyIdentifier);
      }

      Assert.That (propertyNames, Is.EquivalentTo (new string[] {
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"
      }));
    }

    [Test]
    public void GetEnumeratorNonGeneric ()
    {
      Order order = Order.NewObject ();
      List<string> propertyNames = new List<string> ();
      foreach (PropertyAccessor propertyAccessor in (IEnumerable)order.Properties)
      {
        propertyNames.Add (propertyAccessor.PropertyIdentifier);
      }

      Assert.That (propertyNames, Is.EquivalentTo (new string[] {
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer",
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems"
      }));
    }

    [Test]
    public void Contains ()
    {
      Order order = Order.NewObject ();
      Assert.IsTrue (order.Properties.Contains ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"));
      Assert.IsTrue (order.Properties.Contains ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"));
      Assert.IsTrue (order.Properties.Contains ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket"));
      Assert.IsFalse (order.Properties.Contains ("OrderTicket"));
      Assert.IsFalse (order.Properties.Contains ("Bla"));
    }

    [Test]
    public void ShortNameAndType ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (order.Properties["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
          order.Properties[typeof (Order), "OrderNumber"]);
    }

    [Test]
    public void ShortNameAndTypeWithShadowedProperties ()
    {
      DerivedClassWithMixedProperties classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();

      PropertyIndexer indexer = new PropertyIndexer(classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer[typeof (DerivedClassWithMixedProperties), "String"]);
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer[typeof (ClassWithMixedProperties), "String"]);
    }

    [Test]
    public void Find ()
    {
      Order order = Order.NewObject ();
      Assert.AreEqual (order.Properties["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"],
        order.Properties.Find (typeof (Order), "OrderNumber"));
    }

    [Test]
    public void FindFromDerivedType ()
    {
      Distributor distributor = Distributor.NewObject ();
      Assert.IsFalse (distributor.Properties.Contains (typeof (Distributor).FullName + ".ContactPerson"));
      Assert.AreEqual (distributor.Properties[typeof (Partner), "ContactPerson"], distributor.Properties.Find (typeof (Distributor), "ContactPerson"));
    }

    [Test]
    public void FindWithShadowedProperty ()
    {
      DerivedClassWithMixedProperties classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();
      
      PropertyIndexer indexer = new PropertyIndexer (classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer.Find (typeof (DerivedClassWithMixedProperties), "String"));
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer.Find (typeof (ClassWithMixedProperties), "String"));
    }

    [Test]
    public void FindWithShadowedPropertyAndInferredType ()
    {
      DerivedClassWithMixedProperties classWithMixedProperties =
          (DerivedClassWithMixedProperties) RepositoryAccessor.NewObject (typeof (DerivedClassWithMixedProperties)).With();

      PropertyIndexer indexer = new PropertyIndexer (classWithMixedProperties);
      Assert.AreEqual (indexer[typeof (DerivedClassWithMixedProperties).FullName + ".String"],
          indexer.Find (classWithMixedProperties, "String"));
      Assert.AreEqual (indexer[typeof (ClassWithMixedProperties).FullName + ".String"],
          indexer.Find ((ClassWithMixedProperties) classWithMixedProperties, "String"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The domain object type Rubicon.Data.DomainObjects.UnitTests.TestDomain."
        + "Distributor does not have or inherit a mapping property with the short name 'Frobbers'.", MatchType = MessageMatch.Contains)]
    public void FindNonExistingProperty ()
    {
      Distributor distributor = Distributor.NewObject ();
      distributor.Properties.Find (typeof (Distributor), "Frobbers");
    }
  }
}
