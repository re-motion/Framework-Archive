using System;
using System.Data;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
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
    ClassDefinition orderDefinition = TestMappingConfiguration.Current.ClassDefinitions["Order"];
    
    SelectCommandBuilder builder = new SelectCommandBuilder (
        Provider, "*", orderDefinition, "CustomerID", DomainObjectIDs.Customer1, "OrderNumber desc");
    
    Provider.Connect ();
    using (IDbCommand command = builder.Create ())
    {
      Assert.AreEqual ("SELECT * FROM [Order] WHERE [CustomerID] = @CustomerID ORDER BY OrderNumber desc;", command.CommandText);
    }
  }
}
}
