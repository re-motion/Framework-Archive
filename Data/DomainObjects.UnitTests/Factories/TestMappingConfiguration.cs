using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public class TestMappingConfiguration
  {
    // types

    // static members and constants

    private static TestMappingConfiguration s_current;

    public static TestMappingConfiguration Current
    {
      get
      {
        lock (typeof (TestMappingConfiguration))
        {
          if (s_current == null)
            s_current = new TestMappingConfiguration();

          return s_current;
        }
      }
    }

    public static void Reset()
    {
      lock (typeof (TestMappingConfiguration))
      {
        s_current = null;
      }
    }

    // member fields

    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;

    // construction and disposing

    private TestMappingConfiguration()
    {
      _classDefinitions = CreateClassDefinitions();
      _relationDefinitions = CreateRelationDefinitions();
    }

    // methods and properties

    public ClassDefinitionCollection ClassDefinitions
    {
      get { return _classDefinitions; }
    }

    public RelationDefinitionCollection RelationDefinitions
    {
      get { return _relationDefinitions; }
    }

    #region Methods for creating class definitions

    private ClassDefinitionCollection CreateClassDefinitions()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();

      ReflectionBasedClassDefinition testDomainBase = CreateTestDomainBaseDefinition();
      classDefinitions.Add (testDomainBase);

      ReflectionBasedClassDefinition storageProviderStubDomainBase = CreateStorageProviderStubDomainBaseDefinition();
      classDefinitions.Add (storageProviderStubDomainBase);

      ReflectionBasedClassDefinition company = CreateCompanyDefinition (null);
      classDefinitions.Add (company);

      ReflectionBasedClassDefinition customer = CreateCustomerDefinition (company);
      classDefinitions.Add (customer);

      ReflectionBasedClassDefinition partner = CreatePartnerDefinition (company);
      classDefinitions.Add (partner);

      ReflectionBasedClassDefinition supplier = CreateSupplierDefinition (partner);
      classDefinitions.Add (supplier);

      ReflectionBasedClassDefinition distributor = CreateDistributorDefinition (partner);
      classDefinitions.Add (distributor);

      classDefinitions.Add (CreateOrderDefinition (null));
      classDefinitions.Add (CreateOrderTicketDefinition (null));
      classDefinitions.Add (CreateOrderItemDefinition (null));

      ReflectionBasedClassDefinition officialDefinition = CreateOfficialDefinition (storageProviderStubDomainBase);
      classDefinitions.Add (officialDefinition);
      classDefinitions.Add (CreateSpecialOfficialDefinition (officialDefinition));

      classDefinitions.Add (CreateCeoDefinition (null));
      classDefinitions.Add (CreatePersonDefinition (null));

      classDefinitions.Add (CreateClientDefinition (null));
      classDefinitions.Add (CreateLocationDefinition (null));

      ReflectionBasedClassDefinition fileSystemItemDefinition = CreateFileSystemItemDefinition (null);
      classDefinitions.Add (fileSystemItemDefinition);
      classDefinitions.Add (CreateFolderDefinition (fileSystemItemDefinition));
      classDefinitions.Add (CreateFileDefinition (fileSystemItemDefinition));

      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition (null));
      classDefinitions.Add (CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (null));
      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnDefinition (null));
      classDefinitions.Add (CreateClassWithAllDataTypesDefinition (null));
      classDefinitions.Add (CreateClassWithGuidKeyDefinition (null));
      classDefinitions.Add (CreateClassWithInvalidKeyTypeDefinition (null));
      classDefinitions.Add (CreateClassWithoutIDColumnDefinition (null));
      classDefinitions.Add (CreateClassWithoutClassIDColumnDefinition (null));
      classDefinitions.Add (CreateClassWithoutTimestampColumnDefinition (null));
      classDefinitions.Add (CreateClassWithValidRelationsDefinition (null));
      classDefinitions.Add (CreateClassWithInvalidRelationDefinition (null));
      classDefinitions.Add (CreateIndustrialSectorDefinition (null));
      classDefinitions.Add (CreateEmployeeDefinition (null));
      classDefinitions.Add (CreateComputerDefinition (null));
      classDefinitions.Add (CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (null));


      return classDefinitions;
    }

    private ReflectionBasedClassDefinition CreateTestDomainBaseDefinition()
    {
      ReflectionBasedClassDefinition testDomainBase = new ReflectionBasedClassDefinition (
          "TestDomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (TestDomainBase), true);

      return testDomainBase;
    }

    private ReflectionBasedClassDefinition CreateStorageProviderStubDomainBaseDefinition()
    {
      ReflectionBasedClassDefinition storageProviderStubDomainBase = new ReflectionBasedClassDefinition (
          "StorageProviderStubDomainBase", null, DatabaseTest.c_unitTestStorageProviderStubID, typeof (StorageProviderStubDomainBase), true);

      return storageProviderStubDomainBase;
    }

    private ReflectionBasedClassDefinition CreateCompanyDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition company = new ReflectionBasedClassDefinition (
          "Company", "Company", DatabaseTest.c_testDomainProviderID, typeof (Company), false, baseClass);

      company.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              company, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name", "Name", typeof (string), 100));
      company.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              company, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", "IndustrialSectorID", typeof (ObjectID), true));

      return company;
    }

    private ReflectionBasedClassDefinition CreateCustomerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition customer = new ReflectionBasedClassDefinition (
          "Customer", null, DatabaseTest.c_testDomainProviderID, typeof (Customer), false, baseClass);

      customer.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince", "CustomerSince", typeof (DateTime?)));

      customer.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              customer,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type",
              "CustomerType",
              typeof (Customer.CustomerType)));

      return customer;
    }

    private ReflectionBasedClassDefinition CreatePartnerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition partner = new ReflectionBasedClassDefinition (
          "Partner", null, DatabaseTest.c_testDomainProviderID, typeof (Partner), false, baseClass);

      partner.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              partner, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", "ContactPersonID", typeof (ObjectID), true));

      return partner;
    }

    private ReflectionBasedClassDefinition CreateSupplierDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition supplier = new ReflectionBasedClassDefinition (
          "Supplier", null, DatabaseTest.c_testDomainProviderID, typeof (Supplier), false, baseClass);

      supplier.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              supplier, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Supplier.SupplierQuality", "SupplierQuality", typeof (int)));

      return supplier;
    }

    private ReflectionBasedClassDefinition CreateDistributorDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition distributor = new ReflectionBasedClassDefinition (
          "Distributor", null, DatabaseTest.c_testDomainProviderID, typeof (Distributor), false, baseClass);

      distributor.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              distributor, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.NumberOfShops", "NumberOfShops", typeof (int)));

      return distributor;
    }

    private ReflectionBasedClassDefinition CreateOrderDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition order = new ReflectionBasedClassDefinition (
          "Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order), false, baseClass);

      order.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              order, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber", "OrderNo", typeof (int)));
      order.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              order, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate", "DeliveryDate", typeof (DateTime)));
      order.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              order, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", "CustomerID", typeof (ObjectID), true));
      order.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              order, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official", "OfficialID", typeof (ObjectID), true));

      return order;
    }

    private ReflectionBasedClassDefinition CreateOfficialDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition official = new ReflectionBasedClassDefinition (
          "Official", "Official", DatabaseTest.c_unitTestStorageProviderStubID, typeof (Official), false, baseClass);

      official.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              official, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Name", "Name", typeof (string), 100));

      return official;
    }

    private ReflectionBasedClassDefinition CreateSpecialOfficialDefinition (ReflectionBasedClassDefinition officialDefinition)
    {
      return new ReflectionBasedClassDefinition (
          "SpecialOfficial", null, DatabaseTest.c_unitTestStorageProviderStubID, typeof (SpecialOfficial), false, officialDefinition);
    }

    private ReflectionBasedClassDefinition CreateOrderTicketDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition orderTicket = new ReflectionBasedClassDefinition (
          "OrderTicket", "OrderTicket", DatabaseTest.c_testDomainProviderID, typeof (OrderTicket), false, baseClass);

      orderTicket.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName", "FileName", typeof (string), 255));
      orderTicket.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              orderTicket, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", "OrderID", typeof (ObjectID), true));

      return orderTicket;
    }

    private ReflectionBasedClassDefinition CreateOrderItemDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition orderItem = new ReflectionBasedClassDefinition (
          "OrderItem", "OrderItem", DatabaseTest.c_testDomainProviderID, typeof (OrderItem), false, baseClass);

      orderItem.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              orderItem, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", "OrderID", typeof (ObjectID), true));
      orderItem.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              orderItem, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Position", "Position", typeof (int)));
      orderItem.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              orderItem, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product", "Product", typeof (string), 100));

      return orderItem;
    }

    private ReflectionBasedClassDefinition CreateCeoDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition ceo = new ReflectionBasedClassDefinition (
          "Ceo", "Ceo", DatabaseTest.c_testDomainProviderID, typeof (Ceo), false, baseClass);

      ceo.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              ceo, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Name", "Name", typeof (string), 100));
      ceo.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              ceo, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", "CompanyID", typeof (ObjectID), true));

      return ceo;
    }

    private ReflectionBasedClassDefinition CreatePersonDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition person = new ReflectionBasedClassDefinition (
          "Person", "Person", DatabaseTest.c_testDomainProviderID, typeof (Person), false, baseClass);

      person.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              person, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.Name", "Name", typeof (string), 100));

      return person;
    }

    private ReflectionBasedClassDefinition CreateClientDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition client = new ReflectionBasedClassDefinition (
          "Client", "Client", DatabaseTest.c_testDomainProviderID, typeof (Client), false, baseClass);

      client.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              client, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient", "ParentClientID", typeof (ObjectID), true));

      return client;
    }

    private ReflectionBasedClassDefinition CreateLocationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition location = new ReflectionBasedClassDefinition (
          "Location", "Location", DatabaseTest.c_testDomainProviderID, typeof (Location), false, baseClass);

      location.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              location, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", "ClientID", typeof (ObjectID), true));

      return location;
    }

    private ReflectionBasedClassDefinition CreateFileSystemItemDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition fileSystemItem = new ReflectionBasedClassDefinition (
          "FileSystemItem", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (FileSystemItem), false, baseClass);

      fileSystemItem.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              fileSystemItem, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder", "ParentFolderID", typeof (ObjectID), true));

      return fileSystemItem;
    }

    private ReflectionBasedClassDefinition CreateFolderDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition folder = new ReflectionBasedClassDefinition (
          "Folder", null, DatabaseTest.c_testDomainProviderID, typeof (Folder), false, baseClass);

      return folder;
    }

    private ReflectionBasedClassDefinition CreateFileDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition file = new ReflectionBasedClassDefinition (
          "File", null, DatabaseTest.c_testDomainProviderID, typeof (File), false, baseClass);

      return file;
    }

    //TODO: remove Date and NaDate properties
    private ReflectionBasedClassDefinition CreateClassWithAllDataTypesDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classWithAllDataTypes = new ReflectionBasedClassDefinition (
          "ClassWithAllDataTypes", "TableWithAllDataTypes", DatabaseTest.c_testDomainProviderID, typeof (ClassWithAllDataTypes), false, baseClass);

      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", "Boolean", typeof (bool)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty", "Byte", typeof (byte)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty", "Date", typeof (DateTime)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty",
              "DateTime",
              typeof (DateTime)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty",
              "Decimal",
              typeof (Decimal)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty", "Double", typeof (double)));

      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty",
              "Enum",
              typeof (ClassWithAllDataTypes.EnumType)));

      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty", "Guid", typeof (Guid)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property", "Int16", typeof (short)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property", "Int32", typeof (int)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property", "Int64", typeof (long)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty", "Single", typeof (float)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty",
              "String",
              typeof (string),
              100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength",
              "StringPropertyWithoutMaxLength",
              typeof (string),
              false));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty", "Binary", typeof (byte[])));

      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty",
              "NaBoolean",
              typeof (bool?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty", "NaByte", typeof (byte?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty",
              "NaDate",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty",
              "NaDateTime",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty",
              "NaDecimal",
              typeof (Decimal?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty",
              "NaDouble",
              typeof (double?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty", "NaGuid", typeof (Guid?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property",
              "NaInt16",
              typeof (short?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property", "NaInt32", typeof (int?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property",
              "NaInt64",
              typeof (long?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty",
              "NaSingle",
              typeof (float?)));

      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty",
              "StringWithNullValue",
              typeof (string),
              true,
              100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty",
              "NaBooleanWithNullValue",
              typeof (bool?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty",
              "NaByteWithNullValue",
              typeof (byte?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty",
              "NaDateWithNullValue",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty",
              "NaDateTimeWithNullValue",
              typeof (DateTime?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty",
              "NaDecimalWithNullValue",
              typeof (Decimal?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty",
              "NaDoubleWithNullValue",
              typeof (double?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty",
              "NaGuidWithNullValue",
              typeof (Guid?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty",
              "NaInt16WithNullValue",
              typeof (short?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty",
              "NaInt32WithNullValue",
              typeof (int?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty",
              "NaInt64WithNullValue",
              typeof (long?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty",
              "NaSingleWithNullValue",
              typeof (float?)));
      classWithAllDataTypes.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classWithAllDataTypes,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty",
              "NullableBinary",
              typeof (byte[]),
              true,
              1000000));

      return classWithAllDataTypes;
    }

    private ReflectionBasedClassDefinition CreateClassWithGuidKeyDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithGuidKey",
          "TableWithGuidKey",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithGuidKey),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithInvalidKeyTypeDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithKeyOfInvalidType",
          "TableWithKeyOfInvalidType",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithKeyOfInvalidType),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithoutIDColumn",
          "TableWithoutIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutIDColumn),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutClassIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithoutClassIDColumn",
          "TableWithoutClassIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutClassIDColumn),
          false,
          baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutTimestampColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          new ReflectionBasedClassDefinition (
              "ClassWithoutTimestampColumn",
              "TableWithoutTimestampColumn",
              DatabaseTest.c_testDomainProviderID,
              typeof (ClassWithoutTimestampColumn),
              false,
              baseClass);

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithValidRelationsDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithValidRelations",
          "TableWithValidRelations",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithValidRelations),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional",
              "TableWithGuidKeyOptionalID",
              typeof (ObjectID),
              true));

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional",
              "TableWithGuidKeyNonOptionalID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithInvalidRelationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithInvalidRelation",
          "TableWithInvalidRelation",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithInvalidRelation),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey",
              "TableWithGuidKeyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithoutRelatedClassIDColumn",
          "TableWithoutRelatedClassIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumn),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor",
              "DistributorID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass",
          "TableWithOptionalOneToOneRelationAndOppositeDerivedClass",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company",
              "CompanyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (
        ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithoutRelatedClassIDColumnAndDerivation",
          "TableWithoutRelatedClassIDColumnAndDerivation",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumnAndDerivation),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company",
              "CompanyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateIndustrialSectorDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition industrialSector = new ReflectionBasedClassDefinition (
          "IndustrialSector", "IndustrialSector", DatabaseTest.c_testDomainProviderID, typeof (IndustrialSector), false, baseClass);

      industrialSector.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              industrialSector, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name", "Name", typeof (string), 100));

      return industrialSector;
    }

    private ReflectionBasedClassDefinition CreateEmployeeDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition employee = new ReflectionBasedClassDefinition (
          "Employee", "Employee", DatabaseTest.c_testDomainProviderID, typeof (Employee), false, baseClass);

      employee.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              employee, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Name", "Name", typeof (string), 100));
      employee.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              employee, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", "SupervisorID", typeof (ObjectID), true));

      return employee;
    }

    private ReflectionBasedClassDefinition CreateComputerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition computer = new ReflectionBasedClassDefinition (
          "Computer", "Computer", DatabaseTest.c_testDomainProviderID, typeof (Computer), false, baseClass);

      computer.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              computer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.SerialNumber", "SerialNumber", typeof (string), 20));
      computer.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              computer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee", "EmployeeID", typeof (ObjectID), true));

      return computer;
    }

    private ReflectionBasedClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithRelatedClassIDColumnAndNoInheritance",
          "TableWithRelatedClassIDColumnAndNoInheritance",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithRelatedClassIDColumnAndNoInheritance),
          false,
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (
          new ReflectionBasedPropertyDefinition (
              classDefinition,
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey",
              "TableWithGuidKeyID",
              typeof (ObjectID),
              true));

      return classDefinition;
    }

    #endregion

    #region Methods for creating relation definitions

    private RelationDefinitionCollection CreateRelationDefinitions()
    {
      RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection();

      relationDefinitions.Add (CreateCustomerToOrderRelationDefinition());
      relationDefinitions.Add (CreateOrderToOrderItemRelationDefinition());
      relationDefinitions.Add (CreateOrderToOrderTicketRelationDefinition());
      relationDefinitions.Add (CreateOrderToOfficialRelationDefinition());
      relationDefinitions.Add (CreateCompanyToCeoRelationDefinition());
      relationDefinitions.Add (CreatePartnerToPersonRelationDefinition());
      relationDefinitions.Add (CreateClientToLocationRelationDefinition());
      relationDefinitions.Add (CreateParentClientToChildClientRelationDefinition());
      relationDefinitions.Add (CreateFolderToFileSystemItemRelationDefinition());

      relationDefinitions.Add (CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition());
      relationDefinitions.Add (CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition());
      relationDefinitions.Add (CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsOptional());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsNonOptional());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithInvalidRelation());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation());
      relationDefinitions.Add (CreateIndustrialSectorToCompanyRelationDefinition());
      relationDefinitions.Add (CreateSupervisorToSubordinateRelationDefinition());
      relationDefinitions.Add (CreateEmployeeToComputerRelationDefinition());

      return relationDefinitions;
    }

    private RelationDefinition CreateCustomerToOrderRelationDefinition()
    {
      ClassDefinition customer = _classDefinitions[typeof (Customer)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          customer,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders",
          false,
          CardinalityType.Many,
          typeof (OrderCollection),
          "OrderNo asc");

      ClassDefinition orderClass = _classDefinitions[typeof (Order)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", endPoint1, endPoint2);

      customer.MyRelationDefinitions.Add (relation);
      orderClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderTicketRelationDefinition()
    {
      ClassDefinition orderClass = _classDefinitions[typeof (Order)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          orderClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      ClassDefinition orderTicketClass = _classDefinitions[typeof (OrderTicket)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderTicketClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", endPoint1, endPoint2);

      orderClass.MyRelationDefinitions.Add (relation);
      orderTicketClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderItemRelationDefinition()
    {
      ClassDefinition orderClass = _classDefinitions[typeof (Order)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          orderClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderItems", true, CardinalityType.Many, typeof (ObjectList<OrderItem>));

      ClassDefinition orderItemClass = _classDefinitions[typeof (OrderItem)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderItemClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", endPoint1, endPoint2);

      orderClass.MyRelationDefinitions.Add (relation);
      orderItemClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOfficialRelationDefinition()
    {
      ClassDefinition officialClass = _classDefinitions[typeof (Official)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          officialClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Orders", false, CardinalityType.Many, typeof (ObjectList<Order>));

      ClassDefinition orderClass = _classDefinitions[typeof (Order)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official", endPoint1, endPoint2);

      officialClass.MyRelationDefinitions.Add (relation);
      orderClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToCeoRelationDefinition()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          companyClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Ceo", true, CardinalityType.One, typeof (Ceo));

      ClassDefinition ceoClass = _classDefinitions[typeof (Ceo)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          ceoClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", endPoint1, endPoint2);

      companyClass.MyRelationDefinitions.Add (relation);
      ceoClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreatePartnerToPersonRelationDefinition()
    {
      ClassDefinition partnerClass = _classDefinitions[typeof (Partner)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          partnerClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", true);

      ClassDefinition personClass = _classDefinitions[typeof (Person)];
      VirtualRelationEndPointDefinition endPoint2 = new VirtualRelationEndPointDefinition (
          personClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", false, CardinalityType.One, typeof (Partner));

      RelationDefinition relation =
          new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", endPoint1, endPoint2);

      personClass.MyRelationDefinitions.Add (relation);
      partnerClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateParentClientToChildClientRelationDefinition()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);
      RelationEndPointDefinition endPoint2 =
          new RelationEndPointDefinition (clientClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient", false);
      RelationDefinition relation =
          new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient", endPoint1, endPoint2);

      clientClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClientToLocationRelationDefinition()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];
      ClassDefinition locationClass = _classDefinitions[typeof (Location)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          locationClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", endPoint1, endPoint2);

      locationClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateFolderToFileSystemItemRelationDefinition()
    {
      ClassDefinition fileSystemItemClass = _classDefinitions[typeof (FileSystemItem)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          fileSystemItemClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder", false);

      ClassDefinition folderClass = _classDefinitions[typeof (Folder)];
      VirtualRelationEndPointDefinition endPoint2 = new VirtualRelationEndPointDefinition (
          folderClass,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems",
          false,
          CardinalityType.Many,
          typeof (ObjectList<FileSystemItem>));

      RelationDefinition relation =
          new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder", endPoint1, endPoint2);

      folderClass.MyRelationDefinitions.Add (relation);
      fileSystemItemClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition()
    {
      ClassDefinition distributorClass = _classDefinitions[typeof (Distributor)];

      ClassDefinition classWithoutRelatedClassIDColumnClass = _classDefinitions[typeof (ClassWithoutRelatedClassIDColumn)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          distributorClass,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.ClassWithoutRelatedClassIDColumn",
          false,
          CardinalityType.One,
          typeof (ClassWithoutRelatedClassIDColumn));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor", false);

      RelationDefinition relation =
          new RelationDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor", endPoint1, endPoint2);

      distributorClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      ClassDefinition classWithoutRelatedClassIDColumnAndDerivation = _classDefinitions[typeof (ClassWithoutRelatedClassIDColumnAndDerivation)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          companyClass,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.ClassWithoutRelatedClassIDColumnAndDerivation",
          false,
          CardinalityType.One,
          typeof (ClassWithoutRelatedClassIDColumnAndDerivation));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnAndDerivation,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company", endPoint1, endPoint2);

      companyClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnAndDerivation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];
      ClassDefinition classWithOptionalOneToOneRelationAndOppositeDerivedClass =
          _classDefinitions[typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (companyClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithOptionalOneToOneRelationAndOppositeDerivedClass,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company", endPoint1, endPoint2);

      classWithOptionalOneToOneRelationAndOppositeDerivedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsOptional()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional",
          false,
          CardinalityType.One,
          typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions[typeof (ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional", false);

      RelationDefinition relation =
          new RelationDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsNonOptional()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsNonOptional",
          true,
          CardinalityType.One,
          typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions[typeof (ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional", true);

      RelationDefinition relation =
          new RelationDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithInvalidRelation()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithInvalidRelation",
          false,
          CardinalityType.One,
          typeof (ClassWithInvalidRelation));

      ClassDefinition classWithInvalidRelation = _classDefinitions[typeof (ClassWithInvalidRelation)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithInvalidRelation, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey", false);

      RelationDefinition relation =
          new RelationDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithInvalidRelation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithRelatedClassIDColumnAndNoInheritance",
          false,
          CardinalityType.One,
          typeof (ClassWithRelatedClassIDColumnAndNoInheritance));

      ClassDefinition classWithRelatedClassIDColumnAndNoInheritance = _classDefinitions[typeof (ClassWithRelatedClassIDColumnAndNoInheritance)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithRelatedClassIDColumnAndNoInheritance,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey",
              endPoint1,
              endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithRelatedClassIDColumnAndNoInheritance.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateIndustrialSectorToCompanyRelationDefinition()
    {
      ClassDefinition industrialSectorClass = _classDefinitions[typeof (IndustrialSector)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          industrialSectorClass,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies",
          true,
          CardinalityType.Many,
          typeof (ObjectList<Company>));

      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          companyClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", false);

      RelationDefinition relation =
          new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", endPoint1, endPoint2);

      industrialSectorClass.MyRelationDefinitions.Add (relation);
      companyClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateSupervisorToSubordinateRelationDefinition()
    {
      ClassDefinition employeeClass = _classDefinitions[typeof (Employee)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          employeeClass,
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates",
          false,
          CardinalityType.Many,
          typeof (ObjectList<Employee>));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          employeeClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", false);

      RelationDefinition relation =
          new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", endPoint1, endPoint2);

      employeeClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateEmployeeToComputerRelationDefinition()
    {
      ClassDefinition employeeClass = _classDefinitions[typeof (Employee)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          employeeClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Computer", false, CardinalityType.One, typeof (Computer));

      ClassDefinition computerClass = _classDefinitions[typeof (Computer)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          computerClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee", endPoint1, endPoint2);

      employeeClass.MyRelationDefinitions.Add (relation);
      computerClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    #endregion
  }
}