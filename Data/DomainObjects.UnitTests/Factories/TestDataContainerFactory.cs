using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
public sealed class TestDataContainerFactory
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  private TestDataContainerFactory ()
  {
  }

  // methods and properties

  public static DataContainer CreateCustomer1DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Customer1, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Name"), "Kunde 1"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("CustomerSince"), new NaDateTime (new DateTime (2000, 1, 1))));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("CustomerType"), Customer.CustomerType.Standard));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("IndustrialSector"), DomainObjectIDs.IndustrialSector1));

    return dataContainer;
  }

  public static DataContainer CreateClassWithAllDataTypesDataContainer ()
  {
    ObjectID id = new ObjectID (DatabaseTest.c_testDomainProviderID, 
        "ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    DataContainer container = DataContainer.CreateForExisting (id, null);
    ClassDefinition classDefinition = container.ClassDefinition;
    
    container.PropertyValues.Add (new PropertyValue (classDefinition["BooleanProperty"], false));
    container.PropertyValues.Add (new PropertyValue (classDefinition["ByteProperty"], (byte) 85));
    container.PropertyValues.Add (new PropertyValue (classDefinition["CharProperty"], 'a'));
    container.PropertyValues.Add (new PropertyValue (classDefinition["DateTimeProperty"], new DateTime (2005, 1, 1)));
    container.PropertyValues.Add (new PropertyValue (classDefinition["DecimalProperty"], (decimal) 123456.789));
    container.PropertyValues.Add (new PropertyValue (classDefinition["DoubleProperty"], 987654.321));
    container.PropertyValues.Add (new PropertyValue (classDefinition["EnumProperty"], ClassWithAllDataTypes.EnumType.Value1));
    container.PropertyValues.Add (new PropertyValue (classDefinition["GuidProperty"], new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}")));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Int16Property"], (short) 32767));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Int32Property"], 2147483647));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Int64Property"], (long) 9223372036854775807));
    container.PropertyValues.Add (new PropertyValue (classDefinition["SingleProperty"], (float) 6789.321));
    container.PropertyValues.Add (new PropertyValue (classDefinition["StringProperty"], "abcdeföäü"));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaBooleanProperty"], new NaBoolean (true)));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaDateTimeProperty"], new NaDateTime (new DateTime (2005, 2, 1))));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaDoubleProperty"], new NaDouble (654321.789)));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaInt32Property"], new NaInt32 (-2147483647)));
    container.PropertyValues.Add (new PropertyValue (classDefinition["StringWithNullValueProperty"], null));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaBooleanWithNullValueProperty"], NaBoolean.Null));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaDateTimeWithNullValueProperty"], NaDateTime.Null));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaDoubleWithNullValueProperty"], NaDouble.Null));
    container.PropertyValues.Add (new PropertyValue (classDefinition["NaInt32WithNullValueProperty"], NaInt32.Null));

    return container;
  }

  public static DataContainer CreatePartner1DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Partner1, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Name"), "Partner 1"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("ContactPerson"), DomainObjectIDs.Person1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("IndustrialSector"), DomainObjectIDs.IndustrialSector1));

    return dataContainer;
  }

  public static DataContainer CreateDistributor2DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Distributor2, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Name"), "Händler 2"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("ContactPerson"), DomainObjectIDs.Person6));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("IndustrialSector"), DomainObjectIDs.IndustrialSector1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("NumberOfShops"), 10));

    return dataContainer;
  }

  public static DataContainer CreateOrder1DataContainer ()
  {
    DataContainer container = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null);
    ClassDefinition classDefinition = container.ClassDefinition;
    
    container.PropertyValues.Add (new PropertyValue (classDefinition["OrderNumber"], 1));
    container.PropertyValues.Add (new PropertyValue (classDefinition["DeliveryDate"], new DateTime (2005, 1, 1)));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Official"], DomainObjectIDs.Official1));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Customer"], DomainObjectIDs.Customer1));

    return container;
  }

  public static DataContainer CreateOrder2DataContainer ()
  {
    DataContainer container = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null);
    ClassDefinition classDefinition = container.ClassDefinition;
    
    container.PropertyValues.Add (new PropertyValue (classDefinition["OrderNumber"], 3));
    container.PropertyValues.Add (new PropertyValue (classDefinition["DeliveryDate"], new DateTime (2005, 3, 1)));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Official"], DomainObjectIDs.Official1));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Customer"], DomainObjectIDs.Customer3));

    return container;
  }

  public static DataContainer CreateOrderWithoutOrderItemDataContainer ()
  {
    DataContainer container = DataContainer.CreateForExisting (DomainObjectIDs.OrderWithoutOrderItem, null);
    ClassDefinition classDefinition = container.ClassDefinition;
    
    container.PropertyValues.Add (new PropertyValue (classDefinition["OrderNumber"], 2));
    container.PropertyValues.Add (new PropertyValue (classDefinition["DeliveryDate"], new DateTime (2005, 2, 1)));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Official"], DomainObjectIDs.Official1));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Customer"], DomainObjectIDs.Customer1));

    return container;
  }

  public static DataContainer CreateNewOrderDataContainer ()
  {
    ObjectID newID = new ObjectID (DatabaseTest.c_testDomainProviderID, "Order", Guid.NewGuid ());
    DataContainer container = DataContainer.CreateNew (newID);
    ClassDefinition classDefinition = container.ClassDefinition;
    
    container["OrderNumber"] = 10;
    container["DeliveryDate"] = new DateTime (2006, 1, 1);
    container["Official"] = DomainObjectIDs.Official1;
    container["Customer"] = DomainObjectIDs.Customer1;

    return container;
  }

  public static DataContainer CreateOrderTicket1DataContainer ()
  {
    DataContainer container = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null);
    ClassDefinition classDefinition = container.ClassDefinition;
    
    container.PropertyValues.Add (new PropertyValue (classDefinition["FileName"], @"C:\order1.png"));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Order"], DomainObjectIDs.Order1));

    return container;
  }

  public static DataContainer CreateOrderTicket2DataContainer ()
  {
    DataContainer container = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket2, null);
    ClassDefinition classDefinition = container.ClassDefinition;
    
    container.PropertyValues.Add (new PropertyValue (classDefinition["FileName"], @"C:\order2.png"));
    container.PropertyValues.Add (new PropertyValue (classDefinition["Order"], DomainObjectIDs.OrderWithoutOrderItem));

    return container;
  }

  public static DataContainer CreateClassWithGuidKeyDataContainer ()
  {
    ObjectID id = new ObjectID (
        DatabaseTest.c_testDomainProviderID, "ClassWithGuidKey", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

    return DataContainer.CreateForExisting (id, null);
  }
}
}
