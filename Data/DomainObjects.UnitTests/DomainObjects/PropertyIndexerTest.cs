using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Mapping;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Data.DomainObjects.UnitTests.DomainObjects
{
  [TestFixture]
  public class PropertyIndexerTest
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
		public void GetEnumerator ()
		{
			Order order = Order.NewObject();
			List<string> propertyNames = new List<string> ();
			foreach (PropertyAccessor propertyAccessor in order.Properties)
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
  }
}
