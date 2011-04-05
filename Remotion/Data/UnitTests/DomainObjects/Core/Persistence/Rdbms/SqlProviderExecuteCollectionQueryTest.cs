// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderExecuteCollectionQueryTest : SqlProviderBaseTest
  {
    [Test]
    public void ExecuteCollectionQuery ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1.Value);

      var orderContainerIDs = Provider.ExecuteCollectionQuery (query).Select (dc => dc.ID).ToArray();

      Assert.IsTrue (orderContainerIDs.Contains (DomainObjectIDs.Order1));
      Assert.IsTrue (orderContainerIDs.Contains (DomainObjectIDs.OrderWithoutOrderItem));
    }

    [Test]
    public void ExecuteCollectionQuery_WithNullIDs ()
    {
      var query = QueryFactory.CreateCollectionQuery (
          "test",
          Provider.StorageProviderDefinition,
          "SELECT NULL AS [ID] FROM [Order] WHERE [Order].[OrderNo] IN (1, 2)",
          new QueryParameterCollection (),
          typeof (DomainObjectCollection));

      var orderContainers = Provider.ExecuteCollectionQuery (query);
      Assert.That (orderContainers, Is.EqualTo (new DataContainer[] { null, null }));
    }

    [Test]
    public void AllDataTypes ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("QueryWithAllDataTypes");
      query.Parameters.Add ("@boolean", false);
      query.Parameters.Add ("@byte", (byte) 85);
      query.Parameters.Add ("@date", new DateTime (2005, 1, 1));
      query.Parameters.Add ("@dateTime", new DateTime (2005, 1, 1, 17, 0, 0));
      query.Parameters.Add ("@decimal", (decimal) 123456.789);
      query.Parameters.Add ("@doubleLowerBound", 987654D);
      query.Parameters.Add ("@doubleUpperBound", 987655D);
      query.Parameters.Add ("@enum", ClassWithAllDataTypes.EnumType.Value1);
      query.Parameters.Add ("@extensibleEnum", Color.Values.Red());
      query.Parameters.Add ("@flags", ClassWithAllDataTypes.FlagsType.Flag1|ClassWithAllDataTypes.FlagsType.Flag2);
      query.Parameters.Add ("@guid", new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}"));
      query.Parameters.Add ("@int16", (short) 32767);
      query.Parameters.Add ("@int32", 2147483647);
      query.Parameters.Add ("@int64", 9223372036854775807L);
      query.Parameters.Add ("@singleLowerBound", (float) 6789);
      query.Parameters.Add ("@singleUpperBound", (float) 6790);
      query.Parameters.Add ("@string", "abcdef���");

      query.Parameters.Add ("@naBoolean", true);
      query.Parameters.Add ("@naByte", (byte) 78);
      query.Parameters.Add ("@naDate", new DateTime (2005, 2, 1));
      query.Parameters.Add ("@naDateTime", new DateTime (2005, 2, 1, 5, 0, 0));
      query.Parameters.Add ("@naDecimal", 765.098m);
      query.Parameters.Add ("@naDoubleLowerBound", 654321D);
      query.Parameters.Add ("@naDoubleUpperBound", 654322D);
      query.Parameters.Add ("@naEnum", ClassWithAllDataTypes.EnumType.Value2);
      query.Parameters.Add ("@naFlags", ClassWithAllDataTypes.FlagsType.Flag0);
      query.Parameters.Add ("@naGuid", new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"));
      query.Parameters.Add ("@naInt16", (short) 12000);
      query.Parameters.Add ("@naInt32", -2147483647);
      query.Parameters.Add ("@naInt64", 3147483647L);
      query.Parameters.Add ("@naSingleLowerBound", 12F);
      query.Parameters.Add ("@naSingleUpperBound", 13F);

      query.Parameters.Add ("@extensibleEnumWithNullValue", null);
      query.Parameters.Add ("@naBooleanWithNullValue", null);
      query.Parameters.Add ("@naByteWithNullValue", null);
      query.Parameters.Add ("@naDateWithNullValue", null);
      query.Parameters.Add ("@naDateTimeWithNullValue", null);
      query.Parameters.Add ("@naDecimalWithNullValue", null);
      query.Parameters.Add ("@naEnumWithNullValue", null);
      query.Parameters.Add ("@naFlagsWithNullValue", null);
      query.Parameters.Add ("@naDoubleWithNullValue", null);
      query.Parameters.Add ("@naGuidWithNullValue", null);
      query.Parameters.Add ("@naInt16WithNullValue", null);
      query.Parameters.Add ("@naInt32WithNullValue", null);
      query.Parameters.Add ("@naInt64WithNullValue", null);
      query.Parameters.Add ("@naSingleWithNullValue", null);
      query.Parameters.Add ("@stringWithNullValue", null);

      var actualContainers = Provider.ExecuteCollectionQuery (query);

      Assert.IsNotNull (actualContainers);
      Assert.AreEqual (1, actualContainers.Length);

      DataContainer expectedContainer = TestDataContainerFactory.CreateClassWithAllDataTypesDataContainer ();
      var checker = new DataContainerChecker ();
      checker.Check (expectedContainer, actualContainers[0]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Expected query type is 'Collection', but was 'Scalar'.\r\nParameter name: query")]
    public void ScalarQuery ()
    {
      Provider.ExecuteCollectionQuery (QueryFactory.CreateQueryFromConfiguration ("OrderNoSumByCustomerNameQuery"));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void DifferentStorageProviderID ()
    {
      var definition = new QueryDefinition (
          "QueryWithDifferentStorageProviderID",
          UnitTestStorageProviderDefinition,
          "select 42",
          QueryType.Collection);

      Provider.ExecuteCollectionQuery (QueryFactory.CreateQuery (definition));
    }

    [Test]
    public void ObjectIDParameter ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderQuery");
      query.Parameters.Add ("@customerID", DomainObjectIDs.Customer1);

      var orderContainerIDs = Provider.ExecuteCollectionQuery (query).Select (dc => dc.ID);

      Assert.That (orderContainerIDs.ToArray(), Is.EquivalentTo (new[] {DomainObjectIDs.Order1, DomainObjectIDs.OrderWithoutOrderItem}));
    }

    [Test]
    public void ObjectIDOfDifferentStorageProvider ()
    {
      var query = QueryFactory.CreateQueryFromConfiguration ("OrderByOfficialQuery");
      query.Parameters.Add ("@officialID", DomainObjectIDs.Official1);

      var orderContainerIDs = Provider.ExecuteCollectionQuery (query).Select (dc => dc.ID);

      Assert.That (orderContainerIDs.ToArray(), Is.EquivalentTo (
          new[] { 
              DomainObjectIDs.Order1, 
              DomainObjectIDs.Order2, 
              DomainObjectIDs.Order3, 
              DomainObjectIDs.Order4, 
              DomainObjectIDs.OrderWithoutOrderItem}));

    }
  }
}
