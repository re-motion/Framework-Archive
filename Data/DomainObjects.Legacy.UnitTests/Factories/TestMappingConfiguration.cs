using System;
using Remotion.Data.DomainObjects.Legacy.Mapping;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.Factories
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

      XmlBasedClassDefinition company = CreateCompanyDefinition ();
      classDefinitions.Add (company);

      XmlBasedClassDefinition customer = CreateCustomerDefinition (company);
      classDefinitions.Add (customer);

      XmlBasedClassDefinition partner = CreatePartnerDefinition (company);
      classDefinitions.Add (partner);

      XmlBasedClassDefinition supplier = CreateSupplierDefinition (partner);
      classDefinitions.Add (supplier);

      XmlBasedClassDefinition distributor = CreateDistributorDefinition (partner);
      classDefinitions.Add (distributor);

      classDefinitions.Add (CreateOrderDefinition ());
      classDefinitions.Add (CreateOrderTicketDefinition ());
      classDefinitions.Add (CreateOrderItemDefinition ());

      XmlBasedClassDefinition officialDefinition = CreateOfficialDefinition ();
      classDefinitions.Add (officialDefinition);
      classDefinitions.Add (CreateSpecialOfficialDefinition (officialDefinition));

      classDefinitions.Add (CreateCeoDefinition ());
      classDefinitions.Add (CreatePersonDefinition ());

      classDefinitions.Add (CreateClientDefinition ());
      classDefinitions.Add (CreateLocationDefinition ());

      XmlBasedClassDefinition fileSystemItemDefinition = CreateFileSystemItemDefinition ();
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

    private XmlBasedClassDefinition CreateCompanyDefinition ()
    {
      XmlBasedClassDefinition company = new XmlBasedClassDefinition (
          "Company", "Company", DatabaseTest.c_testDomainProviderID, typeof (Company));

      company.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (company, "Name", "Name", "string", 100));
      company.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          company, "IndustrialSector", "IndustrialSectorID", TypeInfo.ObjectIDMappingTypeName, true));

      return company;
    }

    private XmlBasedClassDefinition CreateCustomerDefinition (XmlBasedClassDefinition baseClass)
    {
      XmlBasedClassDefinition customer = new XmlBasedClassDefinition (
          "Customer", "Company", DatabaseTest.c_testDomainProviderID, typeof (Customer), baseClass);

      customer.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (customer, "CustomerSince", "CustomerSince", "dateTime", true));

      customer.MyPropertyDefinitions.Add (
          new XmlBasedPropertyDefinition (
              customer,
              "Type",
              "CustomerType",
              "Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain.Customer+CustomerType, Remotion.Data.DomainObjects.Legacy.UnitTests"));

      return customer;
    }

    private XmlBasedClassDefinition CreatePartnerDefinition (XmlBasedClassDefinition baseClass)
    {
      XmlBasedClassDefinition partner = new XmlBasedClassDefinition (
          "Partner", "Company", DatabaseTest.c_testDomainProviderID, typeof (Partner), baseClass);

      partner.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (partner, "ContactPerson", "ContactPersonID", TypeInfo.ObjectIDMappingTypeName, true));

      return partner;
    }

    private XmlBasedClassDefinition CreateSupplierDefinition (XmlBasedClassDefinition baseClass)
    {
      XmlBasedClassDefinition supplier = new XmlBasedClassDefinition (
          "Supplier", "Company", DatabaseTest.c_testDomainProviderID, typeof (Supplier), baseClass);

      supplier.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (supplier, "SupplierQuality", "SupplierQuality", "int32"));

      return supplier;
    }

    private XmlBasedClassDefinition CreateDistributorDefinition (XmlBasedClassDefinition baseClass)
    {
      XmlBasedClassDefinition distributor = new XmlBasedClassDefinition (
          "Distributor", "Company", DatabaseTest.c_testDomainProviderID, typeof (Distributor), baseClass);

      distributor.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (distributor, "NumberOfShops", "NumberOfShops", "int32"));

      return distributor;
    }

    private XmlBasedClassDefinition CreateOrderDefinition ()
    {
      XmlBasedClassDefinition order = new XmlBasedClassDefinition (
          "Order", "Order", DatabaseTest.c_testDomainProviderID, typeof (Order));

      order.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (order, "OrderNumber", "OrderNo", "int32"));
      order.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (order, "DeliveryDate", "DeliveryDate", "dateTime"));
      order.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (order, "Customer", "CustomerID", TypeInfo.ObjectIDMappingTypeName, true));
      order.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (order, "Official", "OfficialID", TypeInfo.ObjectIDMappingTypeName, true));

      return order;
    }

    private XmlBasedClassDefinition CreateOfficialDefinition ()
    {
      XmlBasedClassDefinition official = new XmlBasedClassDefinition (
          "Official", "Official", DatabaseTest.c_unitTestStorageProviderStubID, typeof (Official));

      official.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (official, "Name", "Name", "string", 100));

      return official;
    }

    private XmlBasedClassDefinition CreateSpecialOfficialDefinition (XmlBasedClassDefinition officialDefinition)
    {
      return new XmlBasedClassDefinition (
          "SpecialOfficial", "Official", DatabaseTest.c_unitTestStorageProviderStubID, typeof (SpecialOfficial), officialDefinition);
    }

    private XmlBasedClassDefinition CreateOrderTicketDefinition ()
    {
      XmlBasedClassDefinition orderTicket = new XmlBasedClassDefinition (
          "OrderTicket", "OrderTicket", DatabaseTest.c_testDomainProviderID, typeof (OrderTicket));

      orderTicket.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (orderTicket, "FileName", "FileName", "string", 255));
      orderTicket.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (orderTicket, "Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, true));

      return orderTicket;
    }

    private XmlBasedClassDefinition CreateOrderItemDefinition ()
    {
      XmlBasedClassDefinition orderItem = new XmlBasedClassDefinition (
          "OrderItem", "OrderItem", DatabaseTest.c_testDomainProviderID, typeof (OrderItem));

      orderItem.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (orderItem, "Order", "OrderID", TypeInfo.ObjectIDMappingTypeName, true));
      orderItem.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (orderItem, "Position", "Position", "int32"));
      orderItem.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (orderItem, "Product", "Product", "string", 100));

      return orderItem;
    }

    private XmlBasedClassDefinition CreateCeoDefinition ()
    {
      XmlBasedClassDefinition ceo = new XmlBasedClassDefinition (
          "Ceo", "Ceo", DatabaseTest.c_testDomainProviderID, typeof (Ceo));

      ceo.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (ceo, "Name", "Name", "string", 100));
      ceo.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (ceo, "Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName, true));

      return ceo;
    }

    private XmlBasedClassDefinition CreatePersonDefinition ()
    {
      XmlBasedClassDefinition person = new XmlBasedClassDefinition (
          "Person", "Person", DatabaseTest.c_testDomainProviderID, typeof (Person));

      person.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (person, "Name", "Name", "string", 100));

      return person;
    }

    private XmlBasedClassDefinition CreateClientDefinition ()
    {
      XmlBasedClassDefinition clientClass = new XmlBasedClassDefinition (
          "Client", "Client", DatabaseTest.c_testDomainProviderID, typeof (Client));

      clientClass.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (clientClass, "ParentClient", "ParentClientID", TypeInfo.ObjectIDMappingTypeName, true));

      return clientClass;
    }

    private XmlBasedClassDefinition CreateLocationDefinition ()
    {
      XmlBasedClassDefinition location = new XmlBasedClassDefinition (
          "Location", "Location", DatabaseTest.c_testDomainProviderID, typeof (Location));

      location.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (location, "Client", "ClientID", TypeInfo.ObjectIDMappingTypeName, true));

      return location;
    }

    private XmlBasedClassDefinition CreateFileSystemItemDefinition ()
    {
      XmlBasedClassDefinition fileSystemItem = new XmlBasedClassDefinition (
          "FileSystemItem", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (FileSystemItem));

      fileSystemItem.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (fileSystemItem, "ParentFolder", "ParentFolderID", TypeInfo.ObjectIDMappingTypeName, true));

      return fileSystemItem;
    }

    private XmlBasedClassDefinition CreateFolderDefinition (XmlBasedClassDefinition baseClass)
    {
      XmlBasedClassDefinition folder = new XmlBasedClassDefinition (
          "Folder", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (Folder), baseClass);

      return folder;
    }

    private XmlBasedClassDefinition CreateFileDefinition (XmlBasedClassDefinition baseClass)
    {
      XmlBasedClassDefinition file = new XmlBasedClassDefinition (
          "File", "FileSystemItem", DatabaseTest.c_testDomainProviderID, typeof (File), baseClass);

      return file;
    }

    private XmlBasedClassDefinition CreateClassWithAllDataTypesDefinition ()
    {
      XmlBasedClassDefinition classWithAllDataTypes = new XmlBasedClassDefinition (
          "ClassWithAllDataTypes", "TableWithAllDataTypes", DatabaseTest.c_testDomainProviderID, typeof (ClassWithAllDataTypes));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "BooleanProperty", "Boolean", "boolean"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "ByteProperty", "Byte", "byte"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "DateProperty", "Date", "date"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "DateTimeProperty", "DateTime", "dateTime"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "DecimalProperty", "Decimal", "decimal"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "DoubleProperty", "Double", "double"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          classWithAllDataTypes, 
          "EnumProperty",
          "Enum",
          "Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain.ClassWithAllDataTypes+EnumType, Remotion.Data.DomainObjects.Legacy.UnitTests"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "GuidProperty", "Guid", "guid"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "Int16Property", "Int16", "int16"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "Int32Property", "Int32", "int32"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "Int64Property", "Int64", "int64"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "SingleProperty", "Single", "single"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "StringProperty", "String", "string", 100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "StringPropertyWithoutMaxLength", "StringWithoutMaxLength", "string"));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "BinaryProperty", "Binary", "binary"));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaBooleanProperty", "NaBoolean", "boolean", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaByteProperty", "NaByte", "byte", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDateProperty", "NaDate", "date", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDateTimeProperty", "NaDateTime", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDecimalProperty", "NaDecimal", "decimal", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDoubleProperty", "NaDouble", "double", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaGuidProperty", "NaGuid", "guid", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaInt16Property", "NaInt16", "int16", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaInt32Property", "NaInt32", "int32", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaInt64Property", "NaInt64", "int64", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaSingleProperty", "NaSingle", "single", true));

      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "StringWithNullValueProperty", "StringWithNullValue", "string", true, true, 100));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaBooleanWithNullValueProperty", "NaBooleanWithNullValue", "boolean", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaByteWithNullValueProperty", "NaByteWithNullValue", "byte", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDateWithNullValueProperty", "NaDateWithNullValue", "date", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDateTimeWithNullValueProperty", "NaDateTimeWithNullValue", "dateTime", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDecimalWithNullValueProperty", "NaDecimalWithNullValue", "decimal", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaDoubleWithNullValueProperty", "NaDoubleWithNullValue", "double", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaGuidWithNullValueProperty", "NaGuidWithNullValue", "guid", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaInt16WithNullValueProperty", "NaInt16WithNullValue", "int16", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaInt32WithNullValueProperty", "NaInt32WithNullValue", "int32", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaInt64WithNullValueProperty", "NaInt64WithNullValue", "int64", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NaSingleWithNullValueProperty", "NaSingleWithNullValue", "single", true));
      classWithAllDataTypes.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classWithAllDataTypes, "NullableBinaryProperty", "NullableBinary", "binary", true, true, 1000000));

      return classWithAllDataTypes;
    }

    private XmlBasedClassDefinition CreateClassWithGuidKeyDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassWithGuidKey", "TableWithGuidKey",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithGuidKey));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithInvalidKeyTypeDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassWithKeyOfInvalidType", "TableWithKeyOfInvalidType",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithKeyOfInvalidType));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithoutIDColumnDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassWithoutIDColumn", "TableWithoutIDColumn",
        DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutIDColumn));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithoutClassIDColumnDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "ClassWithoutClassIDColumn", "TableWithoutClassIDColumn", DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutClassIDColumn));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithoutTimestampColumnDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassWithoutTimestampColumn", "TableWithoutTimestampColumn",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithoutTimestampColumn));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithValidRelationsDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition ("ClassWithValidRelations", "TableWithValidRelations",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithValidRelations));

      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          classDefinition, "ClassWithGuidKeyOptional", "TableWithGuidKeyOptionalID", TypeInfo.ObjectIDMappingTypeName, true));

      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          classDefinition, "ClassWithGuidKeyNonOptional", "TableWithGuidKeyNonOptionalID", TypeInfo.ObjectIDMappingTypeName, true));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithInvalidRelationDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "ClassWithInvalidRelation", "TableWithInvalidRelation", DatabaseTest.c_testDomainProviderID, typeof (ClassWithInvalidRelation));

      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          classDefinition, "ClassWithGuidKey", "TableWithGuidKeyID", TypeInfo.ObjectIDMappingTypeName, true));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "ClassWithoutRelatedClassIDColumn",
          "TableWithoutRelatedClassIDColumn",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumn));

      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          classDefinition, "Distributor", "DistributorID", TypeInfo.ObjectIDMappingTypeName, true));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass",
          "TableWithOptionalOneToOneRelationAndOppositeDerivedClass",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass));

      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          classDefinition, "Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName, true));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "ClassWithoutRelatedClassIDColumnAndDerivation",
          "TableWithoutRelatedClassIDColumnAndDerivation",
          DatabaseTest.c_testDomainProviderID,
          typeof (ClassWithoutRelatedClassIDColumnAndDerivation));

      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (
          classDefinition, "Company", "CompanyID", TypeInfo.ObjectIDMappingTypeName, true));

      return classDefinition;
    }

    private XmlBasedClassDefinition CreateIndustrialSectorDefinition ()
    {
      XmlBasedClassDefinition industrialSector = new XmlBasedClassDefinition (
        "IndustrialSector", "IndustrialSector", DatabaseTest.c_testDomainProviderID, typeof (IndustrialSector));

      industrialSector.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (industrialSector, "Name", "Name", "string", 100));

      return industrialSector;
    }

    private XmlBasedClassDefinition CreateEmployeeDefinition ()
    {
      XmlBasedClassDefinition employee = new XmlBasedClassDefinition (
          "Employee", "Employee", DatabaseTest.c_testDomainProviderID, typeof (Employee));

      employee.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (employee, "Name", "Name", "string", 100));
      employee.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (employee, "Supervisor", "SupervisorID", TypeInfo.ObjectIDMappingTypeName, true));

      return employee;
    }

    private XmlBasedClassDefinition CreateComputerDefinition ()
    {
      XmlBasedClassDefinition computer = new XmlBasedClassDefinition (
          "Computer", "Computer", DatabaseTest.c_testDomainProviderID, typeof (Computer));

      computer.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (computer, "SerialNumber", "SerialNumber", "string", 20));
      computer.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (computer, "Employee", "EmployeeID", TypeInfo.ObjectIDMappingTypeName, true));

      return computer;
    }

    private static XmlBasedClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition ()
    {
      XmlBasedClassDefinition classDefinition = new XmlBasedClassDefinition (
          "ClassWithRelatedClassIDColumnAndNoInheritance", "TableWithRelatedClassIDColumnAndNoInheritance",
          DatabaseTest.c_testDomainProviderID, typeof (ClassWithRelatedClassIDColumnAndNoInheritance));

      classDefinition.MyPropertyDefinitions.Add (new XmlBasedPropertyDefinition (classDefinition, "ClassWithGuidKey", "TableWithGuidKeyID", TypeInfo.ObjectIDMappingTypeName, true));

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

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (clientClass, "ParentClient", false);
      RelationDefinition relation = new RelationDefinition ("ParentClientToChildClient", endPoint1, endPoint2);

      clientClass.MyRelationDefinitions.Add (relation);

      return relation;
    }

    private RelationDefinition CreateClientToLocationRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions["Client"];
      ClassDefinition locationClass = _classDefinitions["Location"];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);

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

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (companyClass);

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
