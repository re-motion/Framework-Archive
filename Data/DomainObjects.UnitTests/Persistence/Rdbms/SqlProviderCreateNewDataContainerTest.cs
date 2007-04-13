using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderCreateNewDataContainerTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      DataContainer newContainer = Provider.CreateNewDataContainer (orderClass);

      Assert.IsNotNull (newContainer, "New DataContainer is null.");
      Assert.IsNotNull (newContainer.ID, "ObjectID of new DataContainer.");
      Assert.AreEqual (orderClass.ID, newContainer.ID.ClassID, "ClassID of ObjectID.");
      Assert.AreEqual (DatabaseTest.c_testDomainProviderID, newContainer.ID.StorageProviderID, "StorageProviderID of ObjectID.");
      Assert.AreEqual (typeof (Guid), newContainer.ID.Value.GetType (), "Type of ID value of ObjectID.");
      Assert.IsNull (newContainer.Timestamp, "Timestamp of new DataContainer.");
      Assert.AreEqual (StateType.New, newContainer.State, "State of new DataContainer.");
      Assert.AreEqual (4, newContainer.PropertyValues.Count, "PropertyValues.Count");
      Assert.AreEqual (int.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], "OrderNumber");
      Assert.AreEqual (DateTime.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"], "DeliveryDate");
      Assert.IsNull (newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"], "Official");
      Assert.IsNull (newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"], "Customer");
    }

    [Test]
    public void CreateClassWithAllDataTypes ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];
      DataContainer newContainer = Provider.CreateNewDataContainer (classDefinition);

      Assert.AreEqual (false, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"]);
      Assert.AreEqual (byte.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"]);
      Assert.AreEqual (DateTime.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"]);
      Assert.AreEqual (DateTime.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"]);
      Assert.AreEqual (decimal.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"]);
      Assert.AreEqual (double.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"]);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"]);
      Assert.AreEqual (Guid.Empty, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"]);
      Assert.AreEqual (short.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"]);
      Assert.AreEqual (int.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"]);
      Assert.AreEqual (long.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"]);
      Assert.AreEqual (float.MinValue, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"]);
      Assert.AreEqual (string.Empty, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"]);
      Assert.AreEqual (string.Empty, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEmptyImage ((byte[]) newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"]);

      Assert.AreEqual (NaBoolean.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"]);
      Assert.AreEqual (NaByte.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"]);
      Assert.AreEqual (NaDecimal.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"]);
      Assert.AreEqual (NaDouble.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"]);
      Assert.AreEqual (NaGuid.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"]);
      Assert.AreEqual (NaInt16.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"]);
      Assert.AreEqual (NaInt32.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"]);
      Assert.AreEqual (NaInt64.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"]);
      Assert.AreEqual (NaSingle.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"]);

      Assert.AreEqual (NaBoolean.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"]);
      Assert.AreEqual (NaByte.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"]);
      Assert.AreEqual (NaDecimal.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"]);
      Assert.AreEqual (NaDouble.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"]);
      Assert.AreEqual (NaGuid.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"]);
      Assert.AreEqual (NaInt16.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"]);
      Assert.AreEqual (NaInt32.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"]);
      Assert.AreEqual (NaInt64.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"]);
      Assert.AreEqual (NaSingle.Null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"]);
      Assert.AreEqual (null, newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"]);
      Assert.IsNull (newContainer["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"]);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage = "The StorageProviderID 'UnitTestStorageProviderStub' of the provided ClassDefinition does not match with this StorageProvider's ID 'TestDomain'.\r\nParameter name: classDefinition")]
    public void ClassDefinitionOfOtherStorageProvider ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (Official)];
      Provider.CreateNewDataContainer (classDefinition);
    }
  }
}
