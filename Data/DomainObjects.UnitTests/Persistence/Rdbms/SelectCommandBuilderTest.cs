using System;
using System.Data;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Mixins;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SelectCommandBuilderTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateWithOrderClause ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      Provider.Connect ();
      SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, orderDefinition, orderDefinition.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"), DomainObjectIDs.Customer1);

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
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];
      SelectCommandBuilder.CreateForRelatedIDLookup (
          Provider, 
          orderDefinition, 
          orderDefinition.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"), 
          DomainObjectIDs.Customer1);
    }

    [Test]
    public void WhereClauseBuilder_CanBeMixed ()
    {
      ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];

      Provider.Connect ();
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (WhereClauseBuilder)).Clear().AddMixins (typeof (WhereClauseBuilderMixin)).EnterScope())
      {
        SelectCommandBuilder builder = SelectCommandBuilder.CreateForRelatedIDLookup (
            Provider,
            orderDefinition,
            orderDefinition.GetMandatoryPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"),
            DomainObjectIDs.Customer1);

        using (IDbCommand command = builder.Create())
        {
          Assert.IsTrue (command.CommandText.Contains ("Mixed!"));
        }
      }
    }
  }
}
