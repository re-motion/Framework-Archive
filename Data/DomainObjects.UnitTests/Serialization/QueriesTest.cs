using System;
using System.IO;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;

namespace Rubicon.Data.DomainObjects.UnitTests.Serialization
{
[TestFixture]
public class QueriesTest : SerializationBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public QueriesTest ()
  {
  }

  // methods and properties

  [Test]
  public void QueryParameter ()
  {
    QueryParameter queryParameter = new QueryParameter ("name", "value", QueryParameterType.Text);

    QueryParameter deserializedQueryParameter = (QueryParameter) SerializeAndDeserialize (queryParameter);

    AreEqual (queryParameter, deserializedQueryParameter);
  }

  [Test]
  public void QueryParameterCollection ()
  {
    QueryParameterCollection queryParameters = new QueryParameterCollection ();
    queryParameters.Add ("Text Parameter", "Value 1", QueryParameterType.Text);
    queryParameters.Add ("Value Parameter", "Value 2", QueryParameterType.Value);

    QueryParameterCollection deserializedQueryParameters = (QueryParameterCollection) SerializeAndDeserialize (queryParameters);

    AreEqual (queryParameters, deserializedQueryParameters);
  }

  [Test]
  public void QueryDefinition ()
  {
    QueryDefinition queryDefinition = new QueryDefinition ("queryID", "TestDomain", "statement", QueryType.Collection, typeof (DomainObjectCollection));

    QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

    Assert.IsFalse (object.ReferenceEquals (queryDefinition, deserializedQueryDefinition));
    AreEqual (queryDefinition, deserializedQueryDefinition);
  }

  [Test]
  public void QueryDefinitionInQueryConfiguration ()
  {
    QueryDefinition queryDefinition = QueryConfiguration.Current["OrderQuery"];

    QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

    Assert.AreSame (queryDefinition, deserializedQueryDefinition);
  }

  [Test]
  [ExpectedException (typeof (QueryConfigurationException), "QueryDefinition 'UnknownQuery' does not exist.")]
  public void UnknownQueryDefinitionInQueryConfiguration ()
  {
    QueryDefinition unknownQueryDefinition = new QueryDefinition ("UnknownQuery", "TestDomain", "select 42", QueryType.Scalar);
    QueryConfiguration.Current.QueryDefinitions.Add (unknownQueryDefinition);

    using (MemoryStream stream = new MemoryStream ())
    {
      Serialize (stream, unknownQueryDefinition);
      QueryConfiguration.SetCurrent (null);
      Deserialize (stream);
    }
  }

  [Test]
  public void QueryDefinitionCollection ()
  {
    QueryDefinitionCollection queryDefinitions = new QueryDefinitionCollection ();
    queryDefinitions.Add (QueryConfiguration.Current.QueryDefinitions[0]);
    queryDefinitions.Add (QueryConfiguration.Current.QueryDefinitions[1]);

    QueryDefinitionCollection deserializedQueryDefinitions = (QueryDefinitionCollection) SerializeAndDeserialize (queryDefinitions);
    AreEqual (queryDefinitions, deserializedQueryDefinitions);
    Assert.AreSame (deserializedQueryDefinitions[0], QueryConfiguration.Current.QueryDefinitions[0]);
    Assert.AreSame (deserializedQueryDefinitions[1], QueryConfiguration.Current.QueryDefinitions[1]);
  }

  [Test]
  public void Query ()
  {
    Query query = new Query ("OrderQuery");
    query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

    Query deserializedQuery = (Query) SerializeAndDeserialize (query);
    AreEqual (query, deserializedQuery);
    Assert.AreSame (QueryConfiguration.Current["OrderQuery"], deserializedQuery.Definition);
  }

  private void AreEqual (Query expected, Query actual)
  {
    Assert.IsFalse (object.ReferenceEquals (expected, actual));
    Assert.IsNotNull (actual);

    Assert.AreEqual (expected.QueryID, actual.QueryID);
    Assert.AreSame (expected.Definition, actual.Definition);
    AreEqual (expected.Parameters, actual.Parameters);
  }

  private void AreEqual (QueryDefinitionCollection expected, QueryDefinitionCollection actual)
  {
    Assert.IsFalse (object.ReferenceEquals (expected, actual));
    Assert.IsNotNull (actual);
    Assert.AreEqual (expected.Count, actual.Count);

    for (int i = 0; i < expected.Count; i++)
      AreEqual (expected[i], actual[i]);
  }

  private void AreEqual (QueryParameter expected, QueryParameter actual)
  {
    Assert.IsFalse (object.ReferenceEquals (expected, actual));
    Assert.AreEqual (expected.Name, actual.Name);
    Assert.AreEqual (expected.ParameterType, actual.ParameterType);
    Assert.AreEqual (expected.Value, actual.Value);
  }

  private void AreEqual (QueryParameterCollection expected, QueryParameterCollection actual)
  {
    Assert.IsFalse (object.ReferenceEquals (expected, actual));
    Assert.AreEqual (expected.Count, actual.Count);
    Assert.AreEqual (expected.IsReadOnly, actual.IsReadOnly);

    for (int i = 0; i < expected.Count; i++)
    {
      AreEqual (expected[i], actual[i]);

      // Check if Hashtable of CommonCollection is deserialized correctly
      QueryParameter actualQueryParameter = actual[i];
      Assert.AreSame (actualQueryParameter, actual[actualQueryParameter.Name]); 
    }
  }

  private void AreEqual (QueryDefinition expected, QueryDefinition actual)
  {
    Assert.AreEqual (expected.QueryID, actual.QueryID);
    Assert.AreEqual (expected.QueryType, actual.QueryType);
    Assert.AreEqual (expected.Statement, actual.Statement);
    Assert.AreEqual (expected.StorageProviderID, actual.StorageProviderID);
    Assert.AreEqual (expected.CollectionType, actual.CollectionType);
  }

}
}
