using System;
using System.IO;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;

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
  public void QueryParameterTest ()
  {
    QueryParameter queryParameter = new QueryParameter ("name", "value", QueryParameterType.Text);

    QueryParameter deserializedQueryParameter = (QueryParameter) SerializeAndDeserialize (queryParameter);

    AreEqual (queryParameter, deserializedQueryParameter);
  }

  [Test]
  public void QueryParameterCollectionTest ()
  {
    QueryParameterCollection queryParameters = new QueryParameterCollection ();
    queryParameters.Add ("Text Parameter", "Value 1", QueryParameterType.Text);
    queryParameters.Add ("Value Parameter", "Value 2", QueryParameterType.Value);

    QueryParameterCollection deserializedQueryParameters = (QueryParameterCollection) SerializeAndDeserialize (queryParameters);

    AreEqual (queryParameters, deserializedQueryParameters);
  }

  [Test]
  public void QueryDefinitionTest ()
  {
    QueryDefinition queryDefinition = new QueryDefinition ("queryID", "TestDomain", "statement", QueryType.Collection, typeof (DomainObjectCollection));

    QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

    AreEqual (queryDefinition, deserializedQueryDefinition);
  }

  [Test]
  public void QueryDefinitionInQueryConfigurationTest ()
  {
    QueryDefinition queryDefinition = QueryConfiguration.Current["OrderQuery"];

    QueryDefinition deserializedQueryDefinition = (QueryDefinition) SerializeAndDeserialize (queryDefinition);

    Assert.AreSame (queryDefinition, deserializedQueryDefinition);
  }

  [Test]
  [ExpectedException (typeof (QueryConfigurationException), "QueryDefinition 'UnknownQuery' does not exist.")]
  public void UnknownQueryDefinitionInQueryConfigurationTest ()
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
    Assert.IsFalse (object.ReferenceEquals (expected, actual));
    Assert.AreEqual (expected.QueryID, actual.QueryID);
    Assert.AreEqual (expected.QueryType, actual.QueryType);
    Assert.AreEqual (expected.Statement, actual.Statement);
    Assert.AreEqual (expected.StorageProviderID, actual.StorageProviderID);
    Assert.AreEqual (expected.CollectionType, actual.CollectionType);
  }

}
}
