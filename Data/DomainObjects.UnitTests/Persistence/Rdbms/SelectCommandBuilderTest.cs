using System;
using System.Data;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SelectCommandBuilderTest : SqlProviderBaseTest
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public SelectCommandBuilderTest ()
    {
    }

    // methods and properties

    [Test]
    public void CreateWithOrderClause ()
    {
      ClassDefinition orderDefinition = LegacyTestMappingConfiguration.Current.ClassDefinitions["Order"];

      Provider.Connect ();

      SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, orderDefinition, orderDefinition.GetMandatoryPropertyDefinition ("Customer"), DomainObjectIDs.Customer1);

      using (IDbCommand command = builder.Create ())
      {
        Assert.AreEqual (
            "SELECT * FROM [Order] WHERE [CustomerID] = @CustomerID ORDER BY OrderNo asc;",
            command.CommandText);
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      ClassDefinition orderDefinition = LegacyTestMappingConfiguration.Current.ClassDefinitions["Order"];
      using (StorageProviderManager manager = new StorageProviderManager ())
      {
        RdbmsProvider provider = (RdbmsProvider) manager.GetMandatory (c_testDomainProviderID);

        SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
            provider, orderDefinition, orderDefinition.GetMandatoryPropertyDefinition ("Customer"), DomainObjectIDs.Customer1);

      }
    }
  }
}
