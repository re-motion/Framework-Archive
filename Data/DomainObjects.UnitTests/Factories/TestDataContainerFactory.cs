using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Resources;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
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
    private DomainObjectIDs _domainObjectIDs;

    // construction and disposing

    public TestDataContainerFactory (ClientTransactionMock clientTransactionMock)
    {
      ArgumentUtility.CheckNotNull ("clientTransactionMock", clientTransactionMock);
      _clientTransactionMock = clientTransactionMock;

      _domainObjectIDs = StandardConfiguration.Instance.GetDomainObjectIDs();
    }

    // methods and properties

    public DataContainer CreateCustomer1DataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.Customer1, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name"), "Kunde 1"));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince"), new NaDateTime (new DateTime (2000, 1, 1))));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type"), Customer.CustomerType.Standard));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"), _domainObjectIDs.IndustrialSector1));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateClassWithAllDataTypesDataContainer ()
    {
      ObjectID id = new ObjectID ("ClassWithAllDataTypes", new Guid ("{3F647D79-0CAF-4a53-BAA7-A56831F8CE2D}"));

      DataContainer dataContainer = DataContainer.CreateForExisting (id, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty"], false));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty"], (byte) 85));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty"], new DateTime (2005, 1, 1)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty"], new DateTime (2005, 1, 1, 17, 0, 0)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty"], (decimal) 123456.789));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty"], 987654.321));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty"], ClassWithAllDataTypes.EnumType.Value1));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty"], new Guid ("{236C2DCE-43BD-45ad-BDE6-15F8C05C4B29}")));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property"], (short) 32767));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property"], 2147483647));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property"], (long) 9223372036854775807));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty"], (float) 6789.321));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty"], "abcdef���"));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength"], "12345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890"));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty"], ResourceManager.GetImage1 ()));

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty"], new NaBoolean (true)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty"], new NaByte (78)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty"], new NaDateTime (new DateTime (2005, 2, 1))));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty"], new NaDateTime (new DateTime (2005, 2, 1, 5, 0, 0))));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty"], new NaDecimal (new decimal (765.098))));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty"], new NaDouble (654321.789)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty"], new NaGuid (new Guid ("{19B2DFBE-B7BB-448e-8002-F4DBF6032AE8}"))));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property"], new NaInt16 (12000)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property"], new NaInt32 (-2147483647)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property"], new NaInt64 (3147483647)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty"], new NaSingle (12.456F)));

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty"], NaBoolean.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty"], NaByte.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty"], NaDateTime.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty"], NaDateTime.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty"], NaDecimal.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty"], NaDouble.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty"], NaGuid.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty"], NaInt16.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty"], NaInt32.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty"], NaInt64.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty"], NaSingle.Null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty"], null));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty"], null));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreatePartner1DataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.Partner1, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name"), "Partner 1"));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"), _domainObjectIDs.Person1));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"), _domainObjectIDs.IndustrialSector1));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateDistributor2DataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.Distributor2, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name"), "H�ndler 2"));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson"), _domainObjectIDs.Person6));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector"), _domainObjectIDs.IndustrialSector1));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition.GetPropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.NumberOfShops"), 10));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrder1DataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.Order1, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], 1));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"], new DateTime (2005, 1, 1)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"], _domainObjectIDs.Official1));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"], _domainObjectIDs.Customer1));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrder2DataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.Order2, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], 3));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"], new DateTime (2005, 3, 1)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"], _domainObjectIDs.Official1));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"], _domainObjectIDs.Customer3));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrderWithoutOrderItemDataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.OrderWithoutOrderItem, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber"], 2));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate"], new DateTime (2005, 2, 1)));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official"], _domainObjectIDs.Official1));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer"], _domainObjectIDs.Customer1));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrderTicket1DataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.OrderTicket1, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName"], @"C:\order1.png"));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"], _domainObjectIDs.Order1));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateOrderTicket2DataContainer ()
    {
      DataContainer dataContainer = DataContainer.CreateForExisting (_domainObjectIDs.OrderTicket2, null);
      ClassDefinition classDefinition = dataContainer.ClassDefinition;

      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName"], @"C:\order2.png"));
      dataContainer.PropertyValues.Add (new PropertyValue (classDefinition["Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order"], _domainObjectIDs.OrderWithoutOrderItem));

      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }

    public DataContainer CreateClassWithGuidKeyDataContainer ()
    {
      ObjectID id = new ObjectID ("ClassWithGuidKey", new Guid ("{7D1F5F2E-D111-433b-A675-300B55DC4756}"));

      DataContainer dataContainer = DataContainer.CreateForExisting (id, null);
      _clientTransactionMock.SetClientTransaction (dataContainer);

      return dataContainer;
    }
  }
}
