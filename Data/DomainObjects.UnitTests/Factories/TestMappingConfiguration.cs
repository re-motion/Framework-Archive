using System;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.DomainObjects;
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
            s_current = new TestMappingConfiguration ();

          return s_current;
        }
      }
    }

    public static void Reset ()
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

    private TestMappingConfiguration ()
    {
      _classDefinitions = CreateClassDefinitions ();
      _relationDefinitions = CreateRelationDefinitions ();
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

    private ClassDefinitionCollection CreateClassDefinitions ()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection ();

      ClassDefinition testDomainBase = CreateTestDomainBaseDefinition();
      ClassDefinition storageProviderStubDomainBase = CreateStorageProviderStubDomainBaseDefinition ();

      ClassDefinition company = CreateCompanyDefinition (testDomainBase);
      classDefinitions.Add (company);

      ClassDefinition customer = CreateCustomerDefinition (company);
      classDefinitions.Add (customer);

      ClassDefinition partner = CreatePartnerDefinition (company);
      classDefinitions.Add (partner);

      ClassDefinition supplier = CreateSupplierDefinition (partner);
      classDefinitions.Add (supplier);

      ClassDefinition distributor = CreateDistributorDefinition (partner);
      classDefinitions.Add (distributor);

      classDefinitions.Add (CreateOrderDefinition (testDomainBase));
      classDefinitions.Add (CreateOrderTicketDefinition (testDomainBase));
      classDefinitions.Add (CreateOrderItemDefinition (testDomainBase));

      //TODO: reset to storageProviderStubDomainBase
      ClassDefinition officialDefinition = CreateOfficialDefinition (testDomainBase);
      classDefinitions.Add (officialDefinition);
      classDefinitions.Add (CreateSpecialOfficialDefinition (officialDefinition));

      classDefinitions.Add (CreateCeoDefinition (testDomainBase));
      classDefinitions.Add (CreatePersonDefinition (testDomainBase));

      classDefinitions.Add (CreateClientDefinition (testDomainBase));
      classDefinitions.Add (CreateLocationDefinition (testDomainBase));

      ClassDefinition fileSystemItemDefinition = CreateFileSystemItemDefinition (testDomainBase);
      classDefinitions.Add (fileSystemItemDefinition);
      classDefinitions.Add (CreateFolderDefinition (fileSystemItemDefinition));
      classDefinitions.Add (CreateFileDefinition (fileSystemItemDefinition));

      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithAllDataTypesDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithGuidKeyDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithInvalidKeyTypeDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithoutIDColumnDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithoutClassIDColumnDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithoutTimestampColumnDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithValidRelationsDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithInvalidRelationDefinition (testDomainBase));
      classDefinitions.Add (CreateIndustrialSectorDefinition (testDomainBase));
      classDefinitions.Add (CreateEmployeeDefinition (testDomainBase));
      classDefinitions.Add (CreateComputerDefinition (testDomainBase));
      classDefinitions.Add (CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (testDomainBase));


      return classDefinitions;
    }

    private ClassDefinition CreateTestDomainBaseDefinition ()
    {
      ClassDefinition testDomainBase = new ClassDefinition (
          "TestDomainBase", null, DatabaseTest.c_testDomainProviderID, typeof (TestDomainBase));

      return testDomainBase;
    }

    private ClassDefinition CreateStorageProviderStubDomainBaseDefinition ()
    {
      ClassDefinition storageProviderStubDomainBase = new ClassDefinition (
          "StorageProviderStubDomainBase", null, DatabaseTest.c_unitTestStorageProviderStubID, typeof (StorageProviderStubDomainBase));

      return storageProviderStubDomainBase;
    }

    private ClassDefinition CreateCompanyDefinition (ClassDefinition baseClass)
    {
      ClassDefinition company = new ClassDefinition (
          "Company", "Company", DatabaseTest.c_testDomainProviderID, typeof (Company), baseClass);

      company.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.Name", "Name", "string", 100));
      company.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", "IndustrialSectorID", TypeInfo.ObjectIDMappingTypeName));

      return company;
    }

    private ClassDefinition CreateCustomerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition customer = new ClassDefinition (
          "Customer", null, DatabaseTest.c_testDomainProviderID, typeof (Customer), baseClass);

      customer.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.CustomerSince", "CustomerSince", "dateTime", true));

      customer.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Type",
          "CustomerType",
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer+CustomerType, Rubicon.Data.DomainObjects.UnitTests"));

      return customer;
    }

    private ClassDefinition CreatePartnerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition partner = new ClassDefinition (
          "Partner", null, DatabaseTest.c_testDomainProviderID, typeof (Partner), baseClass);

      partner.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", "ContactPersonID", TypeInfo.ObjectIDMappingTypeName));

      return partner;
    }

    private ClassDefinition CreateSupplierDefinition (ClassDefinition baseClass)
    {
      ClassDefinition supplier = new ClassDefinition (
          "Supplier", null, DatabaseTest.c_testDomainProviderID, typeof (Supplier), baseClass);

      supplier.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Supplier.SupplierQuality", "SupplierQuality", "int32"));

      return supplier;
    }

    private ClassDefinition CreateDistributorDefinition (ClassDefinition baseClass)
    {
      ClassDefinition distributor = new ClassDefinition (
          "Distributor", null, DatabaseTest.c_testDomainProviderID, typeof (Distributor), baseClass);

      distributor.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Distributor.NumberOfShops", "NumberOfShops", "int32"));

      return distributor;
    }

    private ClassDefinition CreateOrderDefinition (ClassDefinition baseClass)
    {
      ClassDefinition order = new ClassDefinition (
          "Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order), baseClass);

      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.OrderNumber", "OrderNo", "int32"));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.DeliveryDate", "DeliveryDate", "dateTime"));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Official", "OfficialID", TypeInfo.ObjectIDMappingTypeName));

      return order;
    }

    //TODO: reset to c_unitTestStorageProviderStubID
    private ClassDefinition CreateOfficialDefinition (ClassDefinition baseClass)
    {
      ClassDefinition official = new ClassDefinition (
          "Official", "Official", DatabaseTest.c_testDomainProviderID, typeof (Official), baseClass);

      official.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Official.Name", "Name", "string", 100));

      return official;
    }

    //TODO: reset to c_unitTestStorageProviderStubID
    private ClassDefinition CreateSpecialOfficialDefinition (ClassDefinition officialDefinition)
    {
      return new ClassDefinition (
          "SpecialOfficial", null, DatabaseTest.c_testDomainProviderID, typeof (SpecialOfficial), officialDefinition);
    }

    private ClassDefinition CreateOrderTicketDefinition (ClassDefinition baseClass)
    {
      ClassDefinition orderTicket = new ClassDefinition (
          "OrderTicket", "OrderTicket", DatabaseTest.c_testDomainProviderID, typeof (OrderTicket), baseClass);

      orderTicket.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.FileName", "FileName", "string", 255));
      orderTicket.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderTicket.Order", "OrderID", TypeInfo.ObjectIDMappingTypeName));

      return orderTicket;
    }

    private ClassDefinition CreateOrderItemDefinition (ClassDefinition baseClass)
    {
      ClassDefinition orderItem = new ClassDefinition (
          "OrderItem", "OrderItem", DatabaseTest.c_testDomainProviderID, typeof (OrderItem), baseClass);

      orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Order", "OrderID", TypeInfo.ObjectIDMappingTypeName));
      orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Position", "Position", "int32"));
      orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.OrderItem.Product", "Product", "string", 100));

      return orderItem;
    }

    private ClassDefinition CreateCeoDefinition (ClassDefinition baseClass)
    {
      ClassDefinition order = new ClassDefinition (
          "Ceo", "Ceo", DatabaseTest.c_testDomainProviderID, typeof (Ceo), baseClass);

      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Name", "Name", "string", 100));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Ceo.Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName));

      return order;
    }

    private ClassDefinition CreatePersonDefinition (ClassDefinition baseClass)
    {
      ClassDefinition order = new ClassDefinition (
          "Person", "Person", DatabaseTest.c_testDomainProviderID, typeof (Person), baseClass);

      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.Name", "Name", "string", 100));

      return order;
    }

    private ClassDefinition CreateClientDefinition (ClassDefinition baseClass)
    {
      ClassDefinition clientClass = new ClassDefinition (
          "Client", "Client", DatabaseTest.c_testDomainProviderID, typeof (Client), baseClass);

      clientClass.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient", "ParentClientID", TypeInfo.ObjectIDMappingTypeName));

      return clientClass;
    }

    private ClassDefinition CreateLocationDefinition (ClassDefinition baseClass)
    {
      ClassDefinition location = new ClassDefinition (
          "Location", "Location", DatabaseTest.c_testDomainProviderID, typeof (Location), baseClass);

      location.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", "ClientID", TypeInfo.ObjectIDMappingTypeName));

      return location;
    }

    private ClassDefinition CreateFileSystemItemDefinition (ClassDefinition baseClass)
    {
      ClassDefinition fileSystemItem = new ClassDefinition (
          "FileSystemItem", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (FileSystemItem), baseClass);

      fileSystemItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder", "ParentFolderID", TypeInfo.ObjectIDMappingTypeName));

      return fileSystemItem;
    }

    private ClassDefinition CreateFolderDefinition (ClassDefinition baseClass)
    {
      ClassDefinition folder = new ClassDefinition (
          "Folder", null, DatabaseTest.c_testDomainProviderID, typeof (Folder), baseClass);

      return folder;
    }

    private ClassDefinition CreateFileDefinition (ClassDefinition baseClass)
    {
      ClassDefinition file = new ClassDefinition (
          "File", null, DatabaseTest.c_testDomainProviderID, typeof (File), baseClass);

      return file;
    }

    //TODO: remove Date and NaDate properties
    private ClassDefinition CreateClassWithAllDataTypesDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classWithAllDataTypes = new ClassDefinition (
          "ClassWithAllDataTypes", "TableWithAllDataTypes", DatabaseTest.c_testDomainProviderID, typeof (ClassWithAllDataTypes), baseClass);

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", "Boolean", "boolean"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.ByteProperty", "Byte", "byte"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateProperty", "Date", "dateTime"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty", "DateTime", "dateTime"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DecimalProperty", "Decimal", "decimal"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DoubleProperty", "Double", "double"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty",
          "Enum",
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes+EnumType, Rubicon.Data.DomainObjects.UnitTests"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.GuidProperty", "Guid", "guid"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int16Property", "Int16", "int16"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int32Property", "Int32", "int32"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.Int64Property", "Int64", "int64"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.SingleProperty", "Single", "single"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty", "String", "string", 100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength", "StringWithoutMaxLength", "string"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty", "Binary", "binary"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty", "NaBoolean", "boolean", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteProperty", "NaByte", "byte", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateProperty", "NaDate", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeProperty", "NaDateTime", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalProperty", "NaDecimal", "decimal", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleProperty", "NaDouble", "double", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidProperty", "NaGuid", "guid", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16Property", "NaInt16", "int16", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32Property", "NaInt32", "int32", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64Property", "NaInt64", "int64", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleProperty", "NaSingle", "single", true));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty", "StringWithNullValue", "string", true, true, 100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanWithNullValueProperty", "NaBooleanWithNullValue", "boolean", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaByteWithNullValueProperty", "NaByteWithNullValue", "byte", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateWithNullValueProperty", "NaDateWithNullValue", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDateTimeWithNullValueProperty", "NaDateTimeWithNullValue", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDecimalWithNullValueProperty", "NaDecimalWithNullValue", "decimal", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaDoubleWithNullValueProperty", "NaDoubleWithNullValue", "double", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaGuidWithNullValueProperty", "NaGuidWithNullValue", "guid", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt16WithNullValueProperty", "NaInt16WithNullValue", "int16", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt32WithNullValueProperty", "NaInt32WithNullValue", "int32", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaInt64WithNullValueProperty", "NaInt64WithNullValue", "int64", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaSingleWithNullValueProperty", "NaSingleWithNullValue", "single", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty", "NullableBinary", "binary", true, true, 1000000));

      return classWithAllDataTypes;
    }

    private ClassDefinition CreateClassWithGuidKeyDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithGuidKey", "TableWithGuidKey",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithGuidKey), baseClass);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithInvalidKeyTypeDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithKeyOfInvalidType", "TableWithKeyOfInvalidType",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithKeyOfInvalidType), baseClass);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutIDColumnDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutIDColumn", "TableWithoutIDColumn",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutIDColumn), baseClass);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutClassIDColumnDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithoutClassIDColumn", "TableWithoutClassIDColumn", DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutClassIDColumn), baseClass);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutTimestampColumnDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutTimestampColumn", "TableWithoutTimestampColumn",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutTimestampColumn), baseClass);

      return classDefinition;
    }

    private ClassDefinition CreateClassWithValidRelationsDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithValidRelations", "TableWithValidRelations",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithValidRelations), baseClass);

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional", "TableWithGuidKeyOptionalID", TypeInfo.ObjectIDMappingTypeName));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional", "TableWithGuidKeyNonOptionalID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithInvalidRelationDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithInvalidRelation", "TableWithInvalidRelation", DatabaseTest.c_testDomainProviderID, typeof (ClassWithInvalidRelation), baseClass);

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey", "TableWithGuidKeyID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithoutRelatedClassIDColumn",
          "TableWithoutRelatedClassIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumn),
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor", "DistributorID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass",
          "TableWithOptionalOneToOneRelationAndOppositeDerivedClass",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass),
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithoutRelatedClassIDColumnAndDerivation",
          "TableWithoutRelatedClassIDColumnAndDerivation",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumnAndDerivation), 
          baseClass);

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateIndustrialSectorDefinition (ClassDefinition baseClass)
    {
      ClassDefinition industrialSector = new ClassDefinition (
        "IndustrialSector", "IndustrialSector", DatabaseTest.c_testDomainProviderID, typeof (IndustrialSector), baseClass);

      industrialSector.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Name", "Name", "string", 100));

      return industrialSector;
    }

    private ClassDefinition CreateEmployeeDefinition (ClassDefinition baseClass)
    {
      ClassDefinition employee = new ClassDefinition (
          "Employee", "Employee", DatabaseTest.c_testDomainProviderID, typeof (Employee), baseClass);

      employee.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Name", "Name", "string", 100));
      employee.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", "SupervisorID", TypeInfo.ObjectIDMappingTypeName));

      return employee;
    }

    private ClassDefinition CreateComputerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition computer = new ClassDefinition (
          "Computer", "Computer", DatabaseTest.c_testDomainProviderID, typeof (Computer), baseClass);

      computer.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.SerialNumber", "SerialNumber", "string", 20));
      computer.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Computer.Employee", "EmployeeID", TypeInfo.ObjectIDMappingTypeName));

      return computer;
    }

    private ClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (ClassDefinition baseClass)
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithRelatedClassIDColumnAndNoInheritance", "TableWithRelatedClassIDColumnAndNoInheritance",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithRelatedClassIDColumnAndNoInheritance), baseClass);

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey", "TableWithGuidKeyID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    #endregion

    #region Methods for creating relation definitions

    private RelationDefinitionCollection CreateRelationDefinitions ()
    {
      RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection ();

      relationDefinitions.Add (CreateCustomerToOrderRelationDefinition ());
      relationDefinitions.Add (CreateOrderToOrderItemRelationDefinition ());
      relationDefinitions.Add (CreateOrderToOrderTicketRelationDefinition ());
      relationDefinitions.Add (CreateOrderToOfficialRelationDefinition ());
      relationDefinitions.Add (CreateCompanyToCeoRelationDefinition ());
      relationDefinitions.Add (CreatePartnerToPersonRelationDefinition ());
      relationDefinitions.Add (CreateClientToLocationRelationDefinition ());
      relationDefinitions.Add (CreateParentClientToChildClientRelationDefinition ());
      relationDefinitions.Add (CreateFolderToFileSystemItemRelationDefinition ());

      relationDefinitions.Add (CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ());
      relationDefinitions.Add (CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition ());
      relationDefinitions.Add (CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition ());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsOptional ());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithInvalidRelation ());
      relationDefinitions.Add (CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation ());
      relationDefinitions.Add (CreateIndustrialSectorToCompanyRelationDefinition ());
      relationDefinitions.Add (CreateSupervisorToSubordinateRelationDefinition ());
      relationDefinitions.Add (CreateEmployeeToComputerRelationDefinition ());

      return relationDefinitions;
    }

    private RelationDefinition CreateCustomerToOrderRelationDefinition ()
    {
      ClassDefinition customer = _classDefinitions[typeof (Customer)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          customer, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer.Orders", false, CardinalityType.Many, typeof (OrderCollection), "OrderNo asc");

      ClassDefinition orderClass = _classDefinitions[typeof (Order)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Order.Customer", endPoint1, endPoint2);

      customer.MyRelationDefinitions.Add (relation);
      orderClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderTicketRelationDefinition ()
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

    private RelationDefinition CreateOrderToOrderItemRelationDefinition ()
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

    private RelationDefinition CreateOrderToOfficialRelationDefinition ()
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

    private RelationDefinition CreateCompanyToCeoRelationDefinition ()
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

    private RelationDefinition CreatePartnerToPersonRelationDefinition ()
    {
      ClassDefinition partnerClass = _classDefinitions[typeof (Partner)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          partnerClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", true);

      ClassDefinition personClass = _classDefinitions[typeof (Person)];
      VirtualRelationEndPointDefinition endPoint2 = new VirtualRelationEndPointDefinition (
          personClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Person.AssociatedPartnerCompany", false, CardinalityType.One, typeof (Partner));

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Partner.ContactPerson", endPoint1, endPoint2);

      personClass.MyRelationDefinitions.Add (relation);
      partnerClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateParentClientToChildClientRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];

      NullRelationEndPointDefinition endPoint1 = new NullRelationEndPointDefinition (clientClass);
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (clientClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient", false);
      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Client.ParentClient", endPoint1, endPoint2);

      clientClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClientToLocationRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];
      ClassDefinition locationClass = _classDefinitions[typeof (Location)];

      NullRelationEndPointDefinition endPoint1 = new NullRelationEndPointDefinition (clientClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          locationClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", true);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Location.Client", endPoint1, endPoint2);

      locationClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateFolderToFileSystemItemRelationDefinition ()
    {
      ClassDefinition fileSystemItemClass = _classDefinitions[typeof (FileSystemItem)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          fileSystemItemClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder", false);

      ClassDefinition folderClass = _classDefinitions[typeof (Folder)];
      VirtualRelationEndPointDefinition endPoint2 = new VirtualRelationEndPointDefinition (
          folderClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Folder.FileSystemItems", false, CardinalityType.Many, typeof (ObjectList<FileSystemItem>));

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.FileSystemItem.ParentFolder", endPoint1, endPoint2);

      folderClass.MyRelationDefinitions.Add (relation);
      fileSystemItemClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition ()
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

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumn.Distributor", endPoint1, endPoint2);

      distributorClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ()
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
          classWithoutRelatedClassIDColumnAndDerivation, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithoutRelatedClassIDColumnAndDerivation.Company", endPoint1, endPoint2);

      companyClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnAndDerivation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];
      ClassDefinition classWithOptionalOneToOneRelationAndOppositeDerivedClass = _classDefinitions[typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass)];

      NullRelationEndPointDefinition endPoint1 = new NullRelationEndPointDefinition (companyClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithOptionalOneToOneRelationAndOppositeDerivedClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company", endPoint1, endPoint2);

      classWithOptionalOneToOneRelationAndOppositeDerivedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsOptional ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithValidRelationsOptional", false, CardinalityType.One, typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions[typeof (ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyOptional",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ()
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

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithValidRelations.ClassWithGuidKeyNonOptional",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithInvalidRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithInvalidRelation", false, CardinalityType.One, typeof (ClassWithInvalidRelation));

      ClassDefinition classWithInvalidRelation = _classDefinitions[typeof (ClassWithInvalidRelation)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithInvalidRelation, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithInvalidRelation.ClassWithGuidKey",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithInvalidRelation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithGuidKey.ClassWithRelatedClassIDColumnAndNoInheritance", false, CardinalityType.One,
          typeof (ClassWithRelatedClassIDColumnAndNoInheritance));

      ClassDefinition classWithRelatedClassIDColumnAndNoInheritance = _classDefinitions[typeof (ClassWithRelatedClassIDColumnAndNoInheritance)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithRelatedClassIDColumnAndNoInheritance, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithRelatedClassIDColumnAndNoInheritance.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateIndustrialSectorToCompanyRelationDefinition ()
    {
      ClassDefinition industrialSectorClass = _classDefinitions[typeof (IndustrialSector)];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          industrialSectorClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.IndustrialSector.Companies", true, CardinalityType.Many, typeof (ObjectList<Company>));

      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          companyClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Company.IndustrialSector", endPoint1, endPoint2);

      industrialSectorClass.MyRelationDefinitions.Add (relation);
      companyClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateSupervisorToSubordinateRelationDefinition ()
    {
      ClassDefinition employeeClass = _classDefinitions[typeof (Employee)];
      
      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          employeeClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Subordinates", false, CardinalityType.Many, typeof (ObjectList<Employee>));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          employeeClass, "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", false);

      RelationDefinition relation = new RelationDefinition ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.Employee.Supervisor", endPoint1, endPoint2);

      employeeClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateEmployeeToComputerRelationDefinition ()
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
