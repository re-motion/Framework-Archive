using System;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Factories;
using Remotion.Data.DomainObjects.Legacy.UnitTests.Resources;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;
using Remotion.NullableValueTypes;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Persistence.Rdbms
{
  [TestFixture]
  public class SqlProviderCreateNewDataContainerTest : SqlProviderBaseTest
  {
    [Test]
    public void CreateNewDataContainer ()
    {
      ClassDefinition orderClass = TestMappingConfiguration.Current.ClassDefinitions[typeof (Order)];
      DataContainer newContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (orderClass));

      Assert.IsNotNull (newContainer, "New DataContainer is null.");
      Assert.IsNotNull (newContainer.ID, "ObjectID of new DataContainer.");
      Assert.AreEqual (orderClass.ID, newContainer.ID.ClassID, "ClassID of ObjectID.");
      Assert.AreEqual (c_testDomainProviderID, newContainer.ID.StorageProviderID, "StorageProviderID of ObjectID.");
      Assert.AreEqual (typeof (Guid), newContainer.ID.Value.GetType (), "Type of ID value of ObjectID.");
      Assert.IsNull (newContainer.Timestamp, "Timestamp of new DataContainer.");
      Assert.AreEqual (StateType.New, newContainer.State, "State of new DataContainer.");
      Assert.AreEqual (4, newContainer.PropertyValues.Count, "PropertyValues.Count");
      Assert.AreEqual (int.MinValue, newContainer["OrderNumber"], "OrderNumber");
      Assert.AreEqual (DateTime.MinValue, newContainer["DeliveryDate"], "DeliveryDate");
      Assert.IsNull (newContainer["Official"], "Official");
      Assert.IsNull (newContainer["Customer"], "Customer");
    }

    [Test]
    public void CreateClassWithAllDataTypes ()
    {
      ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];
      DataContainer newContainer = DataContainer.CreateNew (Provider.CreateNewObjectID (classDefinition));

      Assert.AreEqual (false, newContainer["BooleanProperty"]);
      Assert.AreEqual (byte.MinValue, newContainer["ByteProperty"]);
      Assert.AreEqual (DateTime.MinValue, newContainer["DateProperty"]);
      Assert.AreEqual (DateTime.MinValue, newContainer["DateTimeProperty"]);
      Assert.AreEqual (decimal.MinValue, newContainer["DecimalProperty"]);
      Assert.AreEqual (double.MinValue, newContainer["DoubleProperty"]);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, newContainer["EnumProperty"]);
      Assert.AreEqual (Guid.Empty, newContainer["GuidProperty"]);
      Assert.AreEqual (short.MinValue, newContainer["Int16Property"]);
      Assert.AreEqual (int.MinValue, newContainer["Int32Property"]);
      Assert.AreEqual (long.MinValue, newContainer["Int64Property"]);
      Assert.AreEqual (float.MinValue, newContainer["SingleProperty"]);
      Assert.AreEqual (string.Empty, newContainer["StringProperty"]);
      Assert.AreEqual (string.Empty, newContainer["StringPropertyWithoutMaxLength"]);
      ResourceManager.IsEmptyImage ((byte[]) newContainer["BinaryProperty"]);

      Assert.AreEqual (NaBoolean.Null, newContainer["NaBooleanProperty"]);
      Assert.AreEqual (NaByte.Null, newContainer["NaByteProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["NaDateProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["NaDateTimeProperty"]);
      Assert.AreEqual (NaDecimal.Null, newContainer["NaDecimalProperty"]);
      Assert.AreEqual (NaDouble.Null, newContainer["NaDoubleProperty"]);
      Assert.AreEqual (NaGuid.Null, newContainer["NaGuidProperty"]);
      Assert.AreEqual (NaInt16.Null, newContainer["NaInt16Property"]);
      Assert.AreEqual (NaInt32.Null, newContainer["NaInt32Property"]);
      Assert.AreEqual (NaInt64.Null, newContainer["NaInt64Property"]);
      Assert.AreEqual (NaSingle.Null, newContainer["NaSingleProperty"]);

      Assert.AreEqual (NaBoolean.Null, newContainer["NaBooleanWithNullValueProperty"]);
      Assert.AreEqual (NaByte.Null, newContainer["NaByteWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["NaDateWithNullValueProperty"]);
      Assert.AreEqual (NaDateTime.Null, newContainer["NaDateTimeWithNullValueProperty"]);
      Assert.AreEqual (NaDecimal.Null, newContainer["NaDecimalWithNullValueProperty"]);
      Assert.AreEqual (NaDouble.Null, newContainer["NaDoubleWithNullValueProperty"]);
      Assert.AreEqual (NaGuid.Null, newContainer["NaGuidWithNullValueProperty"]);
      Assert.AreEqual (NaInt16.Null, newContainer["NaInt16WithNullValueProperty"]);
      Assert.AreEqual (NaInt32.Null, newContainer["NaInt32WithNullValueProperty"]);
      Assert.AreEqual (NaInt64.Null, newContainer["NaInt64WithNullValueProperty"]);
      Assert.AreEqual (NaSingle.Null, newContainer["NaSingleWithNullValueProperty"]);
      Assert.AreEqual (null, newContainer["StringWithNullValueProperty"]);
      Assert.AreEqual (null, newContainer["NullableBinaryProperty"]);
    }
  }
}
