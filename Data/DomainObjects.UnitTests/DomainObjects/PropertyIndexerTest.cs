using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.Mapping;

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
  }
}
