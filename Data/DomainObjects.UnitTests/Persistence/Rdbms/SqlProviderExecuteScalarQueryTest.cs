using System;
using System.Data;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
[TestFixture]
public class SqlProviderExecuteScalarQueryTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderExecuteScalarQueryTest ()
  {
  }

  // methods and properties

  [Test]
  public void ScalarQueryWithoutParameter ()
  {
    Assert.AreEqual (42, Provider.ExecuteScalarQuery (new Query ("QueryWithoutParameter")));
  }

  [Test]
  [ExpectedException (typeof (StorageProviderException))]
  public void InvalidScalarQuery ()
  {
    QueryDefinition definition = new QueryDefinition (
        "InvalidQuery", 
        c_testDomainProviderID, 
        "This is not T-SQL",
        QueryType.Scalar);

    Provider.ExecuteScalarQuery (new Query (definition));
  }

  [Test]
  public void ScalarQueryWithParameter ()
  {
    Query query = new Query ("OrderNoSumByCustomerNameQuery");
    query.Parameters.Add ("@customerName", "Kunde 1");

    Assert.AreEqual (3, Provider.ExecuteScalarQuery (query));
  }

  [Test]
  public void ParameterWithTextReplacement ()
  {
    Query query = new Query ("OrderNoSumForMultipleCustomers");
    query.Parameters.Add ("{companyNames}", "'Kunde 1', 'Kunde 3'", QueryParameterType.Text);

    Assert.AreEqual (6, Provider.ExecuteScalarQuery (query));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void CollectionQuery ()
  {
    Provider.ExecuteScalarQuery (new Query ("OrderQuery"));
  }

  [Test]
  public void BulkUpdateQuery ()
  {
    Query query = new Query ("BulkUpdateQuery");
    query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);

    Assert.AreEqual (2, Provider.ExecuteScalarQuery (query));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void DifferentStorageProviderID ()
  {
    QueryDefinition definition = new QueryDefinition (
        "QueryWithDifferentStorageProviderID", 
        "DifferentStorageProviderID",
        "select 42", 
        QueryType.Scalar);

    Provider.ExecuteScalarQuery (new Query (definition));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException), 
      "The query parameter '@customerID' is of type 'Rubicon.Data.DomainObjects.ObjectID'."
      + " The value of this parameter is of type 'System.String', but only 'System.Guid' is supported.\r\nParameter name: query")]
  public void InvalidObjectIDValue ()
  {
    ObjectID invalidCustomerID = new ObjectID (DomainObjectIDs.Customer1.ClassID, DomainObjectIDs.Customer1.Value.ToString ());

    Query query = new Query ("BulkUpdateQuery");
    query.Parameters.Add ("@customerID", invalidCustomerID);

    Provider.ExecuteScalarQuery (query);
  }
}
}
