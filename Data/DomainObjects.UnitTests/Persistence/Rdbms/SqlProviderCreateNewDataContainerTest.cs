using System;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.Persistence;

namespace Rubicon.Data.DomainObjects.UnitTests.Persistence
{
[TestFixture]
public class ProviderCreateNewDataContainerTest : SqlProviderBaseTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public ProviderCreateNewDataContainerTest ()
  {
  }

  // methods and properties

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
    Assert.AreEqual (0, newContainer["OrderNumber"], "OrderNumber");
    Assert.AreEqual (DateTime.MinValue, newContainer["DeliveryDate"], "DeliveryDate");
    Assert.IsNull (newContainer["Official"], "Official");
    Assert.IsNull (newContainer["Customer"], "Customer");
  }

  [Test]
  public void CreateClassWithAllDataTypes ()
  {
    ClassDefinition classDefinition = TestMappingConfiguration.Current.ClassDefinitions[typeof (ClassWithAllDataTypes)];
    DataContainer newContainer = Provider.CreateNewDataContainer (classDefinition);

    Assert.AreEqual (false, newContainer["BooleanProperty"]);
    Assert.AreEqual ((byte) 0, newContainer["ByteProperty"]);
    Assert.AreEqual (' ', newContainer["CharProperty"]);
    Assert.AreEqual (DateTime.MinValue, newContainer["DateProperty"]);
    Assert.AreEqual (DateTime.MinValue, newContainer["DateTimeProperty"]);
    Assert.AreEqual ((decimal) 0, newContainer["DecimalProperty"]);
    Assert.AreEqual ((double) 0, newContainer["DoubleProperty"]);
    Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, newContainer["EnumProperty"]);
    Assert.AreEqual (Guid.Empty, newContainer["GuidProperty"]);
    Assert.AreEqual ((short) 0, newContainer["Int16Property"]);
    Assert.AreEqual ((int) 0, newContainer["Int32Property"]);
    Assert.AreEqual ((long) 0, newContainer["Int64Property"]);
    Assert.AreEqual ((float) 0, newContainer["SingleProperty"]);
    Assert.AreEqual (string.Empty, newContainer["StringProperty"]);
    Assert.AreEqual (NaBoolean.Null, newContainer["NaBooleanProperty"]);
    Assert.AreEqual (NaDateTime.Null, newContainer["NaDateProperty"]);
    Assert.AreEqual (NaDateTime.Null, newContainer["NaDateTimeProperty"]);
    Assert.AreEqual (NaDouble.Null, newContainer["NaDoubleProperty"]);
    Assert.AreEqual (NaInt32.Null, newContainer["NaInt32Property"]);
    Assert.AreEqual (null, newContainer["StringWithNullValueProperty"]);
    Assert.AreEqual (NaBoolean.Null, newContainer["NaBooleanWithNullValueProperty"]);
    Assert.AreEqual (NaDateTime.Null, newContainer["NaDateWithNullValueProperty"]);
    Assert.AreEqual (NaDateTime.Null, newContainer["NaDateTimeWithNullValueProperty"]);
    Assert.AreEqual (NaDouble.Null, newContainer["NaDoubleWithNullValueProperty"]);
    Assert.AreEqual (NaInt32.Null, newContainer["NaInt32WithNullValueProperty"]);
  }
}
}
