using System;
using System.Data;
using NUnit.Framework;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class SqlProviderExecuteCollectionQueryTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public SqlProviderExecuteCollectionQueryTest ()
  {
  }

  // methods and properties

  [Test]
  public void ExecuteCollectionQuery ()
  {
    Query query = new Query ("OrderQuery");
    query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);

    DataContainerCollection orderContainers = Provider.ExecuteCollectionQuery (query);

    Assert.IsNotNull (orderContainers);
    Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.Order1));
    Assert.IsTrue (orderContainers.Contains (DomainObjectIDs.OrderWithoutOrderItem));
  }

  [Test]
  public void ExecuteCollectionQueryWithAllDataTypes ()
  {
    Query query = new Query ("QueryWithAllDataTypes");
    query.Parameters.Add ("@boolean", false);
    query.Parameters.Add ("@byte", (byte) 85);
    query.Parameters.Add ("@date", new DateTime (2005, 1 ,1));
    query.Parameters.Add ("@dateTime", new DateTime (2005, 1, 1, 17, 0, 0));
    query.Parameters.Add ("@decimal", (decimal) 123456.789);
    query.Parameters.Add ("@double", 987654.321D);
    query.Parameters.Add ("@enum", ClassWithAllDataTypes.EnumType.Value1);
    query.Parameters.Add ("@guid", new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"));
    query.Parameters.Add ("@int16", (short) 32767);
    query.Parameters.Add ("@int32", 2147483647);
    query.Parameters.Add ("@int64", (long) 9223372036854775807);
    query.Parameters.Add ("@singleLowerBound", (float) 6789);
    query.Parameters.Add ("@singleUpperBound", (float) 6790);
    query.Parameters.Add ("@string", "abcdeföäü");
    query.Parameters.Add ("@naBoolean", new NaBoolean (true));
    query.Parameters.Add ("@naDate", new NaDateTime (new DateTime (2005, 2, 1)));
    query.Parameters.Add ("@naDateTime", new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0)));
    query.Parameters.Add ("@naDouble", new NaDouble (654321.789));
    query.Parameters.Add ("@naInt32", new NaInt32 (-2147483647));
    query.Parameters.Add ("@stringWithNullValue", (string) null);
    query.Parameters.Add ("@naBooleanWithNullValue", NaBoolean.Null);
    query.Parameters.Add ("@naDateWithNullValue", NaDateTime.Null);
    query.Parameters.Add ("@naDateTimeWithNullValue", NaDateTime.Null);
    query.Parameters.Add ("@naDoubleWithNullValue", NaDouble.Null);
    query.Parameters.Add ("@naInt32WithNullValue", NaInt32.Null);

    DataContainerCollection actualContainers = Provider.ExecuteCollectionQuery (query);

    Assert.IsNotNull (actualContainers);
    Assert.AreEqual (1, actualContainers.Count);

    DataContainer expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
    DataContainerChecker checker = new DataContainerChecker ();
    checker.Check (expectedContainer, actualContainers[0]);
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void ExecuteCollectionQueryWithScalarQuery ()
  {
    Provider.ExecuteCollectionQuery (new Query ("OrderNoSumByCustomerNameQuery"));
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void ExecuteCollectionQueryWithDifferentStorageProviderID ()
  {
    QueryDefinition definition = new QueryDefinition (
        "QueryWithDifferentStorageProviderID", 
        "DifferentStorageProviderID",
        "select 42", 
        QueryType.Collection);

    Provider.ExecuteCollectionQuery (new Query (definition));
  }
}
}
