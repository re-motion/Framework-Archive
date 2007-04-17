using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Persistence.Rdbms;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class QueryCommandBuilderTest: SqlProviderBaseTest
  {
    [Test]
    public void UsesView()
    {
      QueryDefinition queryDefinition = new QueryDefinition ("TheQuery", "StorageProvider", "Statement", QueryType.Collection);
      
      Provider.Connect ();
      CommandBuilder commandBuilder = new QueryCommandBuilder (Provider, new Query (queryDefinition));
      Assert.That (commandBuilder.UsesView, Is.True);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Provider must be connected first.\r\nParameter name: provider")]
    public void ConstructorChecksForConnectedProvider ()
    {
      QueryDefinition queryDefinition = new QueryDefinition ("TheQuery", "StorageProvider", "Statement", QueryType.Collection);
      new QueryCommandBuilder (Provider, new Query (queryDefinition));
    }
  }
}