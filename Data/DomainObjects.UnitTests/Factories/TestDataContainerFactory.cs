using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Data.DomainObjects.UnitTests.Transaction;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
public class TestDataContainerFactory
{
  // types

  // members and constants

  // member fields

  private ClientTransactionMock _clientTransactionMock;

  // construction and disposing

  public TestDataContainerFactory (ClientTransactionMock clientTransactionMock)
  {
    ArgumentUtility.CheckNotNull ("clientTransactionMock", clientTransactionMock);
    _clientTransactionMock = clientTransactionMock;
  }

  // methods and properties

  public DataContainer CreateCustomer1DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Customer1, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Name"), "Kunde 1"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("CustomerSince"), new NaDateTime (new DateTime (2000, 1, 1))));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("CustomerType"), Customer.CustomerType.Standard));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("IndustrialSector"), DomainObjectIDs.IndustrialSector1));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateClassWithAllDataTypesDataContainer ()
  {
    ObjectID id = new ObjectID (DatabaseTest.c_testDomainProviderID, 
        "ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

    DataContainer dataContainer = DataContainer.CreateForExisting (id, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["BooleanProperty"], false));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["ByteProperty"], (byte) 85));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["DateProperty"], new DateTime (2005, 1, 1)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["DateTimeProperty"], new DateTime (2005, 1, 1, 17, 0, 0)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["DecimalProperty"], (decimal) 123456.789));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["DoubleProperty"], 987654.321));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["EnumProperty"], ClassWithAllDataTypes.EnumType.Value1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["GuidProperty"], new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}")));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Int16Property"], (short) 32767));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Int32Property"], 2147483647));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Int64Property"], (long) 9223372036854775807));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["SingleProperty"], (float) 6789.321));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["StringProperty"], "abcdeföäü"));

    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaBooleanProperty"], new NaBoolean (true)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaByteProperty"], new NaByte (78)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDateProperty"], new NaDateTime (new DateTime (2005, 2, 1))));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDateTimeProperty"], new NaDateTime (new DateTime (2005, 2, 1, 5, 0 , 0))));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDecimalProperty"], new NaDecimal (new decimal (765.098))));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDoubleProperty"], new NaDouble (654321.789)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaGuidProperty"], new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"))));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaInt16Property"], new NaInt16 (12000)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaInt32Property"], new NaInt32 (-2147483647)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaInt64Property"], new NaInt64 (3147483647)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaSingleProperty"], new NaSingle (12.456F)));

    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaBooleanWithNullValueProperty"], NaBoolean.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaByteWithNullValueProperty"], NaByte.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDateWithNullValueProperty"], NaDateTime.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDateTimeWithNullValueProperty"], NaDateTime.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDecimalWithNullValueProperty"], NaDecimal.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaDoubleWithNullValueProperty"], NaDouble.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaGuidWithNullValueProperty"], NaGuid.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaInt16WithNullValueProperty"], NaInt16.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaInt32WithNullValueProperty"], NaInt32.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaInt64WithNullValueProperty"], NaInt64.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["NaSingleWithNullValueProperty"], NaSingle.Null));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["StringWithNullValueProperty"], null));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreatePartner1DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Partner1, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Name"), "Partner 1"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("ContactPerson"), DomainObjectIDs.Person1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("IndustrialSector"), DomainObjectIDs.IndustrialSector1));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateDistributor2DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Distributor2, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Name"), "Händler 2"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("ContactPerson"), DomainObjectIDs.Person6));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("IndustrialSector"), DomainObjectIDs.IndustrialSector1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("NumberOfShops"), 10));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateOrder1DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["OrderNumber"], 1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["DeliveryDate"], new DateTime (2005, 1, 1)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Official"], DomainObjectIDs.Official1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Customer"], DomainObjectIDs.Customer1));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateOrder2DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order2, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["OrderNumber"], 3));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["DeliveryDate"], new DateTime (2005, 3, 1)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Official"], DomainObjectIDs.Official1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Customer"], DomainObjectIDs.Customer3));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateOrderWithoutOrderItemDataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderWithoutOrderItem, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["OrderNumber"], 2));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["DeliveryDate"], new DateTime (2005, 2, 1)));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Official"], DomainObjectIDs.Official1));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Customer"], DomainObjectIDs.Customer1));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateNewOrderDataContainer ()
  {
    ObjectID newID = new ObjectID (DatabaseTest.c_testDomainProviderID, "Order", Guid.NewGuid ());
    DataContainer dataContainer = DataContainer.CreateNew (newID);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer["OrderNumber"] = 10;
    dataContainer["DeliveryDate"] = new DateTime (2006, 1, 1);
    dataContainer["Official"] = DomainObjectIDs.Official1;
    dataContainer["Customer"] = DomainObjectIDs.Customer1;

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateOrderTicket1DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket1, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["FileName"], @"C:\order1.png"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Order"], DomainObjectIDs.Order1));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateOrderTicket2DataContainer ()
  {
    DataContainer dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.OrderTicket2, null);
    ClassDefinition classDefinition = dataContainer.ClassDefinition;
    
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["FileName"], @"C:\order2.png"));
    dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Order"], DomainObjectIDs.OrderWithoutOrderItem));

    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }

  public DataContainer CreateClassWithGuidKeyDataContainer ()
  {
    ObjectID id = new ObjectID (
        DatabaseTest.c_testDomainProviderID, "ClassWithGuidKey", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

    DataContainer dataContainer = DataContainer.CreateForExisting (id, null);
    _clientTransactionMock.SetClientTransaction (dataContainer);

    return dataContainer;
  }
}
}
