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

      ClassDefinition company = CreateCompanyDefinition ();
      classDefinitions.Add (company);

      ClassDefinition customer = CreateCustomerDefinition (company);
      classDefinitions.Add (customer);

      ClassDefinition partner = CreatePartnerDefinition (company);
      classDefinitions.Add (partner);

      ClassDefinition supplier = CreateSupplierDefinition (partner);
      classDefinitions.Add (supplier);

      ClassDefinition distributor = CreateDistributorDefinition (partner);
      classDefinitions.Add (distributor);

      classDefinitions.Add (CreateOrderDefinition ());
      classDefinitions.Add (CreateOrderTicketDefinition ());
      classDefinitions.Add (CreateOrderItemDefinition ());

      ClassDefinition officialDefinition = CreateOfficialDefinition ();
      classDefinitions.Add (officialDefinition);
      classDefinitions.Add (CreateSpecialOfficialDefinition (officialDefinition));

      classDefinitions.Add (CreateCeoDefinition ());
      classDefinitions.Add (CreatePersonDefinition ());

      classDefinitions.Add (CreateClientDefinition ());
      classDefinitions.Add (CreateLocationDefinition ());

      ClassDefinition fileSystemItemDefinition = CreateFileSystemItemDefinition ();
      classDefinitions.Add (fileSystemItemDefinition);
      classDefinitions.Add (CreateFolderDefinition (fileSystemItemDefinition));
      classDefinitions.Add (CreateFileDefinition (fileSystemItemDefinition));

      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition ());
      classDefinitions.Add (CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition ());
      classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnDefinition ());
      classDefinitions.Add (CreateClassWithAllDataTypesDefinition ());
      classDefinitions.Add (CreateClassWithGuidKeyDefinition ());
      classDefinitions.Add (CreateClassWithInvalidKeyTypeDefinition ());
      classDefinitions.Add (CreateClassWithoutIDColumnDefinition ());
      classDefinitions.Add (CreateClassWithoutClassIDColumnDefinition ());
      classDefinitions.Add (CreateClassWithoutTimestampColumnDefinition ());
      classDefinitions.Add (CreateClassWithValidRelationsDefinition ());
      classDefinitions.Add (CreateClassWithInvalidRelationDefinition ());
      classDefinitions.Add (CreateIndustrialSectorDefinition ());
      classDefinitions.Add (CreateEmployeeDefinition ());
      classDefinitions.Add (CreateComputerDefinition ());
      classDefinitions.Add (CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition ());


      return classDefinitions;
    }

    private ClassDefinition CreateCompanyDefinition ()
    {
      ClassDefinition company = new ClassDefinition (
          "Company", "Company", DatabaseTest.c_testDomainProviderID, typeof (Company));

      company.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
      company.MyPropertyDefinitions.Add (new PropertyDefinition (
          "IndustrialSector", "IndustrialSectorID", TypeInfo.ObjectIDMappingTypeName));

      return company;
    }

    private ClassDefinition CreateCustomerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition customer = new ClassDefinition (
          "Customer", "Company", DatabaseTest.c_testDomainProviderID, typeof (Customer), baseClass);

      customer.MyPropertyDefinitions.Add (new PropertyDefinition ("CustomerSince", "CustomerSince", "dateTime", true));

      customer.MyPropertyDefinitions.Add (
          new PropertyDefinition (
              "Type",
              "CustomerType",
              "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer+CustomerType, Rubicon.Data.DomainObjects.UnitTests"));

      return customer;
    }

    private ClassDefinition CreatePartnerDefinition (ClassDefinition baseClass)
    {
      ClassDefinition partner = new ClassDefinition (
          "Partner", "Company", DatabaseTest.c_testDomainProviderID, typeof (Partner), baseClass);

      partner.MyPropertyDefinitions.Add (new PropertyDefinition ("ContactPerson", "ContactPersonID", TypeInfo.ObjectIDMappingTypeName));

      return partner;
    }

    private ClassDefinition CreateSupplierDefinition (ClassDefinition baseClass)
    {
      ClassDefinition supplier = new ClassDefinition (
          "Supplier", "Company", DatabaseTest.c_testDomainProviderID, typeof (Supplier), baseClass);

      supplier.MyPropertyDefinitions.Add (new PropertyDefinition ("SupplierQuality", "SupplierQuality", "int32"));

      return supplier;
    }

    private ClassDefinition CreateDistributorDefinition (ClassDefinition baseClass)
    {
      ClassDefinition distributor = new ClassDefinition (
          "Distributor", "Company", DatabaseTest.c_testDomainProviderID, typeof (Distributor), baseClass);

      distributor.MyPropertyDefinitions.Add (new PropertyDefinition ("NumberOfShops", "NumberOfShops", "int32"));

      return distributor;
    }

    private ClassDefinition CreateOrderDefinition ()
    {
      ClassDefinition order = new ClassDefinition (
          "Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order));

      order.MyPropertyDefinitions.Add (new PropertyDefinition ("OrderNumber", "OrderNo", "int32"));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("DeliveryDate", "DeliveryDate", "dateTime"));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Official", "OfficialID", TypeInfo.ObjectIDMappingTypeName));

      return order;
    }

    private ClassDefinition CreateOfficialDefinition ()
    {
      ClassDefinition official = new ClassDefinition (
          "Official", "Official", DatabaseTest.c_unitTestStorageProviderStubID, typeof (Official));

      official.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

      return official;
    }

    private ClassDefinition CreateSpecialOfficialDefinition (ClassDefinition officialDefinition)
    {
      return new ClassDefinition (
          "SpecialOfficial", "Official", DatabaseTest.c_unitTestStorageProviderStubID, typeof (SpecialOfficial), officialDefinition);
    }

    private ClassDefinition CreateOrderTicketDefinition ()
    {
      ClassDefinition orderTicket = new ClassDefinition (
          "OrderTicket", "OrderTicket", DatabaseTest.c_testDomainProviderID, typeof (OrderTicket));

      orderTicket.MyPropertyDefinitions.Add (new PropertyDefinition ("FileName", "FileName", "string", 255));
      orderTicket.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", TypeInfo.ObjectIDMappingTypeName));

      return orderTicket;
    }

    private ClassDefinition CreateOrderItemDefinition ()
    {
      ClassDefinition orderItem = new ClassDefinition (
          "OrderItem", "OrderItem", DatabaseTest.c_testDomainProviderID, typeof (OrderItem));

      orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", TypeInfo.ObjectIDMappingTypeName));
      orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Position", "Position", "int32"));
      orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Product", "Product", "string", 100));

      return orderItem;
    }

    private ClassDefinition CreateCeoDefinition ()
    {
      ClassDefinition order = new ClassDefinition (
          "Ceo", "Ceo", DatabaseTest.c_testDomainProviderID, typeof (Ceo));

      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName));

      return order;
    }

    private ClassDefinition CreatePersonDefinition ()
    {
      ClassDefinition order = new ClassDefinition (
          "Person", "Person", DatabaseTest.c_testDomainProviderID, typeof (Person));

      order.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

      return order;
    }

    private ClassDefinition CreateClientDefinition ()
    {
      ClassDefinition clientClass = new ClassDefinition (
          "Client", "Client", DatabaseTest.c_testDomainProviderID, typeof (Client));

      clientClass.MyPropertyDefinitions.Add (new PropertyDefinition ("ParentClient", "ParentClientID", TypeInfo.ObjectIDMappingTypeName));

      return clientClass;
    }

    private ClassDefinition CreateLocationDefinition ()
    {
      ClassDefinition location = new ClassDefinition (
          "Location", "Location", DatabaseTest.c_testDomainProviderID, typeof (Location));

      location.MyPropertyDefinitions.Add (new PropertyDefinition ("Client", "ClientID", TypeInfo.ObjectIDMappingTypeName));

      return location;
    }

    private ClassDefinition CreateFileSystemItemDefinition ()
    {
      ClassDefinition fileSystemItem = new ClassDefinition (
          "FileSystemItem", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (FileSystemItem));

      fileSystemItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
      fileSystemItem.MyPropertyDefinitions.Add (new PropertyDefinition ("ParentFolder", "ParentFolderID", TypeInfo.ObjectIDMappingTypeName));

      return fileSystemItem;
    }

    private ClassDefinition CreateFolderDefinition (ClassDefinition baseClass)
    {
      ClassDefinition folder = new ClassDefinition (
          "Folder", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (Folder), baseClass);

      return folder;
    }

    private ClassDefinition CreateFileDefinition (ClassDefinition baseClass)
    {
      ClassDefinition file = new ClassDefinition (
          "File", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (File), baseClass);

      return file;
    }

    private ClassDefinition CreateClassWithAllDataTypesDefinition ()
    {
      ClassDefinition classWithAllDataTypes = new ClassDefinition (
          "ClassWithAllDataTypes", "TableWithAllDataTypes", DatabaseTest.c_testDomainProviderID, typeof (ClassWithAllDataTypes));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("BooleanProperty", "Boolean", "boolean"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("ByteProperty", "Byte", "byte"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("DateProperty", "Date", "date"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("DateTimeProperty", "DateTime", "dateTime"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("DecimalProperty", "Decimal", "decimal"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("DoubleProperty", "Double", "double"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition (
          "EnumProperty",
          "Enum",
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes+EnumType, Rubicon.Data.DomainObjects.UnitTests"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("GuidProperty", "Guid", "guid"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Int16Property", "Int16", "int16"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Int32Property", "Int32", "int32"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("Int64Property", "Int64", "int64"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("SingleProperty", "Single", "single"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("StringProperty", "String", "string", 100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("StringPropertyWithoutMaxLength", "StringWithoutMaxLength", "string"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("BinaryProperty", "Binary", "binary"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaBooleanProperty", "NaBoolean", "boolean", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaByteProperty", "NaByte", "byte", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDateProperty", "NaDate", "date", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDateTimeProperty", "NaDateTime", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDecimalProperty", "NaDecimal", "decimal", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDoubleProperty", "NaDouble", "double", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaGuidProperty", "NaGuid", "guid", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaInt16Property", "NaInt16", "int16", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaInt32Property", "NaInt32", "int32", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaInt64Property", "NaInt64", "int64", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaSingleProperty", "NaSingle", "single", true));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("StringWithNullValueProperty", "StringWithNullValue", "string", true, true, 100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaBooleanWithNullValueProperty", "NaBooleanWithNullValue", "boolean", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaByteWithNullValueProperty", "NaByteWithNullValue", "byte", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDateWithNullValueProperty", "NaDateWithNullValue", "date", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDateTimeWithNullValueProperty", "NaDateTimeWithNullValue", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDecimalWithNullValueProperty", "NaDecimalWithNullValue", "decimal", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaDoubleWithNullValueProperty", "NaDoubleWithNullValue", "double", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaGuidWithNullValueProperty", "NaGuidWithNullValue", "guid", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaInt16WithNullValueProperty", "NaInt16WithNullValue", "int16", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaInt32WithNullValueProperty", "NaInt32WithNullValue", "int32", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaInt64WithNullValueProperty", "NaInt64WithNullValue", "int64", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NaSingleWithNullValueProperty", "NaSingleWithNullValue", "single", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("NullableBinaryProperty", "NullableBinary", "binary", true, true, 1000000));

      return classWithAllDataTypes;
    }

    private ClassDefinition CreateClassWithGuidKeyDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithGuidKey", "TableWithGuidKey",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithGuidKey));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithInvalidKeyTypeDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithKeyOfInvalidType", "TableWithKeyOfInvalidType",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithKeyOfInvalidType));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutIDColumnDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutIDColumn", "TableWithoutIDColumn",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutIDColumn));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutClassIDColumnDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithoutClassIDColumn", "TableWithoutClassIDColumn", DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutClassIDColumn));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutTimestampColumnDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutTimestampColumn", "TableWithoutTimestampColumn",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutTimestampColumn));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithValidRelationsDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition ("ClassWithValidRelations", "TableWithValidRelations",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithValidRelations));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "ClassWithGuidKeyOptional", "TableWithGuidKeyOptionalID", TypeInfo.ObjectIDMappingTypeName));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "ClassWithGuidKeyNonOptional", "TableWithGuidKeyNonOptionalID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithInvalidRelationDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithInvalidRelation", "TableWithInvalidRelation", DatabaseTest.c_testDomainProviderID, typeof (ClassWithInvalidRelation));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "ClassWithGuidKey", "TableWithGuidKeyID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithoutRelatedClassIDColumn",
          "TableWithoutRelatedClassIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumn));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Distributor", "DistributorID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass",
          "TableWithOptionalOneToOneRelationAndOppositeDerivedClass",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithoutRelatedClassIDColumnAndDerivation",
          "TableWithoutRelatedClassIDColumnAndDerivation",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumnAndDerivation));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
          "Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName));

      return classDefinition;
    }

    private ClassDefinition CreateIndustrialSectorDefinition ()
    {
      ClassDefinition industrialSector = new ClassDefinition (
        "IndustrialSector", "IndustrialSector", DatabaseTest.c_testDomainProviderID, typeof (IndustrialSector));

      industrialSector.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

      return industrialSector;
    }

    private ClassDefinition CreateEmployeeDefinition ()
    {
      ClassDefinition employee = new ClassDefinition (
          "Employee", "Employee", DatabaseTest.c_testDomainProviderID, typeof (Employee));

      employee.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
      employee.MyPropertyDefinitions.Add (new PropertyDefinition ("Supervisor", "SupervisorID", TypeInfo.ObjectIDMappingTypeName));

      return employee;
    }

    private ClassDefinition CreateComputerDefinition ()
    {
      ClassDefinition computer = new ClassDefinition (
          "Computer", "Computer", DatabaseTest.c_testDomainProviderID, typeof (Computer));

      computer.MyPropertyDefinitions.Add (new PropertyDefinition ("SerialNumber", "SerialNumber", "string", 20));
      computer.MyPropertyDefinitions.Add (new PropertyDefinition ("Employee", "EmployeeID", TypeInfo.ObjectIDMappingTypeName));

      return computer;
    }

    private static ClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition ()
    {
      ClassDefinition classDefinition = new ClassDefinition (
          "ClassWithRelatedClassIDColumnAndNoInheritance", "TableWithRelatedClassIDColumnAndNoInheritance",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithRelatedClassIDColumnAndNoInheritance));

      classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("ClassWithGuidKey", "TableWithGuidKeyID", TypeInfo.ObjectIDMappingTypeName));

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
      ClassDefinition customer = _classDefinitions["Customer"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          customer, "Orders", false, CardinalityType.Many, typeof (OrderCollection), "OrderNo asc");

      ClassDefinition orderClass = _classDefinitions["Order"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Customer", true);

      RelationDefinition relation = new RelationDefinition ("CustomerToOrder", endPoint1, endPoint2);

      customer.MyRelationDefinitions.Add (relation);
      orderClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderTicketRelationDefinition ()
    {
      ClassDefinition orderClass = _classDefinitions["Order"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          orderClass, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      ClassDefinition orderTicketClass = _classDefinitions["OrderTicket"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderTicketClass, "Order", true);

      RelationDefinition relation = new RelationDefinition ("OrderToOrderTicket", endPoint1, endPoint2);

      orderClass.MyRelationDefinitions.Add (relation);
      orderTicketClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderItemRelationDefinition ()
    {
      ClassDefinition orderClass = _classDefinitions["Order"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          orderClass, "OrderItems", true, CardinalityType.Many, typeof (DomainObjectCollection));

      ClassDefinition orderItemClass = _classDefinitions["OrderItem"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderItemClass, "Order", true);

      RelationDefinition relation = new RelationDefinition ("OrderToOrderItem", endPoint1, endPoint2);

      orderClass.MyRelationDefinitions.Add (relation);
      orderItemClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateOrderToOfficialRelationDefinition ()
    {
      ClassDefinition officialClass = _classDefinitions["Official"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          officialClass, "Orders", false, CardinalityType.Many, typeof (DomainObjectCollection));

      ClassDefinition orderClass = _classDefinitions["Order"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Official", true);

      RelationDefinition relation = new RelationDefinition ("OfficialToOrder", endPoint1, endPoint2);

      officialClass.MyRelationDefinitions.Add (relation);
      orderClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToCeoRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions["Company"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          companyClass, "Ceo", true, CardinalityType.One, typeof (Ceo));

      ClassDefinition ceoClass = _classDefinitions["Ceo"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          ceoClass, "Company", true);

      RelationDefinition relation = new RelationDefinition ("CompanyToCeo", endPoint1, endPoint2);

      companyClass.MyRelationDefinitions.Add (relation);
      ceoClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreatePartnerToPersonRelationDefinition ()
    {
      ClassDefinition personClass = _classDefinitions["Person"];

      ClassDefinition partnerClass = _classDefinitions["Partner"];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          partnerClass, "ContactPerson", true);

      VirtualRelationEndPointDefinition endPoint2 = new VirtualRelationEndPointDefinition (
          personClass, "AssociatedPartnerCompany", false, CardinalityType.One, typeof (Partner));

      RelationDefinition relation = new RelationDefinition ("PartnerToPerson", endPoint1, endPoint2);

      personClass.MyRelationDefinitions.Add (relation);
      partnerClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateParentClientToChildClientRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions["Client"];

      NullRelationEndPointDefinition endPoint1 = new NullRelationEndPointDefinition (clientClass);
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (clientClass, "ParentClient", false);
      RelationDefinition relation = new RelationDefinition ("ParentClientToChildClient", endPoint1, endPoint2);

      clientClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClientToLocationRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions["Client"];
      ClassDefinition locationClass = _classDefinitions["Location"];

      NullRelationEndPointDefinition endPoint1 = new NullRelationEndPointDefinition (clientClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          locationClass, "Client", true);

      RelationDefinition relation = new RelationDefinition ("ClientToLocation", endPoint1, endPoint2);

      locationClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateFolderToFileSystemItemRelationDefinition ()
    {
      ClassDefinition folderClass = _classDefinitions["Folder"];
      ClassDefinition fileSystemItemClass = _classDefinitions["FileSystemItem"];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          fileSystemItemClass, "ParentFolder", false);

      VirtualRelationEndPointDefinition endPoint2 = new VirtualRelationEndPointDefinition (
          folderClass, "FileSystemItems", false, CardinalityType.Many, typeof (DomainObjectCollection));

      RelationDefinition relation = new RelationDefinition ("FolderToFileSystemItem", endPoint1, endPoint2);

      folderClass.MyRelationDefinitions.Add (relation);
      fileSystemItemClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition ()
    {
      ClassDefinition distributorClass = _classDefinitions["Distributor"];

      ClassDefinition classWithoutRelatedClassIDColumnClass = _classDefinitions["ClassWithoutRelatedClassIDColumn"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          distributorClass,
          "ClassWithoutRelatedClassIDColumn",
          false,
          CardinalityType.One,
          typeof (ClassWithoutRelatedClassIDColumn));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnClass, "Distributor", false);

      RelationDefinition relation = new RelationDefinition ("DistributorToClassWithoutRelatedClassIDColumn", endPoint1, endPoint2);

      distributorClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions["Company"];

      ClassDefinition classWithoutRelatedClassIDColumnAndDerivation = _classDefinitions["ClassWithoutRelatedClassIDColumnAndDerivation"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          companyClass,
          "ClassWithoutRelatedClassIDColumnAndDerivation",
          false,
          CardinalityType.One,
          typeof (ClassWithoutRelatedClassIDColumnAndDerivation));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnAndDerivation, "Company", false);

      RelationDefinition relation = new RelationDefinition (
          "CompanyToClassWithoutRelatedClassIDColumnAndDerivation", endPoint1, endPoint2);

      companyClass.MyRelationDefinitions.Add (relation);
      classWithoutRelatedClassIDColumnAndDerivation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions["Company"];
      ClassDefinition classWithOptionalOneToOneRelationAndOppositeDerivedClass = _classDefinitions["ClassWithOptionalOneToOneRelationAndOppositeDerivedClass"];

      NullRelationEndPointDefinition endPoint1 = new NullRelationEndPointDefinition (companyClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithOptionalOneToOneRelationAndOppositeDerivedClass, "Company", false);

      RelationDefinition relation = new RelationDefinition ("CompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClass", endPoint1, endPoint2);

      classWithOptionalOneToOneRelationAndOppositeDerivedClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsOptional ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions["ClassWithGuidKey"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey, "ClassWithValidRelationsOptional", false, CardinalityType.One, typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions["ClassWithValidRelations"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "ClassWithGuidKeyOptional", false);

      RelationDefinition relation = new RelationDefinition ("ClassWithGuidKeyToClassWithValidRelationsOptional",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions["ClassWithGuidKey"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey,
          "ClassWithValidRelationsNonOptional",
          true,
          CardinalityType.One,
          typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions["ClassWithValidRelations"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "ClassWithGuidKeyNonOptional", true);

      RelationDefinition relation = new RelationDefinition ("ClassWithGuidKeyToClassWithValidRelationsNonOptional",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithValidRelations.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithInvalidRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions["ClassWithGuidKey"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey, "ClassWithInvalidRelation", false, CardinalityType.One, typeof (ClassWithInvalidRelation));

      ClassDefinition classWithInvalidRelation = _classDefinitions["ClassWithInvalidRelation"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithInvalidRelation, "ClassWithGuidKey", false);

      RelationDefinition relation = new RelationDefinition ("ClassWithGuidKeyToClassWithInvalidRelation",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithInvalidRelation.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions["ClassWithGuidKey"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          classWithGuidKey, "ClassWithRelatedClassIDColumnAndNoInheritance", false, CardinalityType.One,
          typeof (ClassWithRelatedClassIDColumnAndNoInheritance));

      ClassDefinition classWithRelatedClassIDColumnAndNoInheritance = _classDefinitions["ClassWithRelatedClassIDColumnAndNoInheritance"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithRelatedClassIDColumnAndNoInheritance, "ClassWithGuidKey", false);

      RelationDefinition relation = new RelationDefinition ("ClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritance",
          endPoint1, endPoint2);

      classWithGuidKey.MyRelationDefinitions.Add (relation);
      classWithRelatedClassIDColumnAndNoInheritance.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateIndustrialSectorToCompanyRelationDefinition ()
    {
      ClassDefinition industrialSectorClass = _classDefinitions["IndustrialSector"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          industrialSectorClass, "Companies", true, CardinalityType.Many, typeof (DomainObjectCollection));

      ClassDefinition companyClass = _classDefinitions["Company"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          companyClass, "IndustrialSector", false);

      RelationDefinition relation = new RelationDefinition ("IndustrialSectorToCompany", endPoint1, endPoint2);

      industrialSectorClass.MyRelationDefinitions.Add (relation);
      companyClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateSupervisorToSubordinateRelationDefinition ()
    {
      ClassDefinition employeeClass = _classDefinitions["Employee"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          employeeClass, "Subordinates", false, CardinalityType.Many, typeof (DomainObjectCollection));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          employeeClass, "Supervisor", false);

      RelationDefinition relation = new RelationDefinition ("SupervisorToSubordinate", endPoint1, endPoint2);

      employeeClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateEmployeeToComputerRelationDefinition ()
    {
      ClassDefinition employeeClass = _classDefinitions["Employee"];

      VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
          employeeClass, "Computer", false, CardinalityType.One, typeof (Computer));

      ClassDefinition computerClass = _classDefinitions["Computer"];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          computerClass, "Employee", false);

      RelationDefinition relation = new RelationDefinition ("EmployeeToComputer", endPoint1, endPoint2);

      employeeClass.MyRelationDefinitions.Add (relation);
      computerClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    #endregion
  }
}
