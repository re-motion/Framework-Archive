using System;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

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

    classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition ());
    classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnDefinition ());
    classDefinitions.Add (CreateClassWithAllDataTypesDefinition ());
    classDefinitions.Add (CreateClassWithGuidKeyDefinition ());
    classDefinitions.Add (CreateClassWithInvalidKeyTypeDefinition ());
    classDefinitions.Add (CreateClassWithoutIDPropertyDefinition ());
    classDefinitions.Add (CreateClassWithoutClassIDPropertyDefinition ());
    classDefinitions.Add (CreateClassWithoutTimestampPropertyDefinition ());
    classDefinitions.Add (CreateClassWithValidRelationsDefinition ());
    classDefinitions.Add (CreateClassWithInvalidRelationDefinition ());
    classDefinitions.Add (CreateIndustrialSectorDefinition ());
    classDefinitions.Add (CreateEmployeeDefinition ());
    classDefinitions.Add (CreateComputerDefinition ());
    classDefinitions.Add (CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition  ());


    return classDefinitions;
  }

  private ClassDefinition CreateCompanyDefinition ()
  {
    ClassDefinition company = new ClassDefinition (
        "Company", "Company", typeof (Company), DatabaseTest.c_testDomainProviderID);

    company.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
    company.MyPropertyDefinitions.Add (new PropertyDefinition (
        "IndustrialSector", "IndustrialSectorID", "objectID"));
    
    return company;
  }

  private ClassDefinition CreateCustomerDefinition (ClassDefinition baseClass)
  {
    ClassDefinition customer = new ClassDefinition (
        "Customer", "Company", typeof (Customer), DatabaseTest.c_testDomainProviderID, baseClass);
    
    customer.MyPropertyDefinitions.Add (new PropertyDefinition ("CustomerSince", "CustomerSince", "dateTime", true));

    customer.MyPropertyDefinitions.Add (
        new PropertyDefinition (
            "CustomerType", 
            "CustomerType", 
            "Rubicon.Data.DomainObjects.UnitTests.TestDomain.Customer+CustomerType, Rubicon.Data.DomainObjects.UnitTests"));

    return customer;
  }

  private ClassDefinition CreatePartnerDefinition (ClassDefinition baseClass)
  {
    ClassDefinition partner = new ClassDefinition (
        "Partner", "Company", typeof (Partner), DatabaseTest.c_testDomainProviderID, baseClass);

    partner.MyPropertyDefinitions.Add (new PropertyDefinition ("ContactPerson", "ContactPersonID", "objectID"));
    
    return partner;
  }

  private ClassDefinition CreateSupplierDefinition (ClassDefinition baseClass)
  {
    ClassDefinition supplier = new ClassDefinition (
        "Supplier", "Company", typeof (Supplier), DatabaseTest.c_testDomainProviderID, baseClass);

    supplier.MyPropertyDefinitions.Add (new PropertyDefinition ("SupplierQuality", "SupplierQuality", "int32"));
    
    return supplier;
  }

  private ClassDefinition CreateDistributorDefinition (ClassDefinition baseClass)
  {
    ClassDefinition distributor = new ClassDefinition (
        "Distributor", "Company", typeof (Distributor), DatabaseTest.c_testDomainProviderID, baseClass);

    distributor.MyPropertyDefinitions.Add (new PropertyDefinition ("NumberOfShops", "NumberOfShops", "int32"));
    
    return distributor;
  }

  private ClassDefinition CreateOrderDefinition ()
  {
    ClassDefinition order = new ClassDefinition (
        "Order", "Order", typeof (Order), DatabaseTest.c_testDomainProviderID);
    
    order.MyPropertyDefinitions.Add (new PropertyDefinition ("OrderNumber", "OrderNo", "int32"));
    order.MyPropertyDefinitions.Add (new PropertyDefinition ("DeliveryDate", "DeliveryDate", "dateTime"));
    order.MyPropertyDefinitions.Add (new PropertyDefinition ("Customer", "CustomerID", "objectID"));
    order.MyPropertyDefinitions.Add (new PropertyDefinition ("Official", "OfficialID", "objectID"));

    return order;
  }

  private ClassDefinition CreateOfficialDefinition ()
  {
    ClassDefinition official = new ClassDefinition (
        "Official", "Official", typeof (Official), DatabaseTest.c_unitTestStorageProviderStubID);
    
    official.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    return official;
  }

  private ClassDefinition CreateSpecialOfficialDefinition (ClassDefinition officialDefinition)
  {
    return new ClassDefinition (
        "SpecialOfficial", "Official", typeof (SpecialOfficial), DatabaseTest.c_unitTestStorageProviderStubID, officialDefinition);
  }

  private ClassDefinition CreateOrderTicketDefinition ()
  {
    ClassDefinition orderTicket = new ClassDefinition (
        "OrderTicket", "OrderTicket", typeof (OrderTicket), DatabaseTest.c_testDomainProviderID);
    
    orderTicket.MyPropertyDefinitions.Add (new PropertyDefinition ("FileName", "FileName", "string", 255));
    orderTicket.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID"));

    return orderTicket;
  }

  private ClassDefinition CreateOrderItemDefinition ()
  {
    ClassDefinition orderItem = new ClassDefinition (
        "OrderItem", "OrderItem", typeof (OrderItem), DatabaseTest.c_testDomainProviderID);
    
    orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID"));
    orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Position", "Position", "int32"));
    orderItem.MyPropertyDefinitions.Add (new PropertyDefinition ("Product", "Product", "string", 100));

    return orderItem;
  }

  private ClassDefinition CreateCeoDefinition ()
  {
    ClassDefinition order = new ClassDefinition (
        "Ceo", "Ceo", typeof (Ceo), DatabaseTest.c_testDomainProviderID);
    
    order.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
    order.MyPropertyDefinitions.Add (new PropertyDefinition ("Company", "CompanyID", "objectID"));

    return order;
  }

  private ClassDefinition CreatePersonDefinition ()
  {
    ClassDefinition order = new ClassDefinition (
        "Person", "Person", typeof (Person), DatabaseTest.c_testDomainProviderID);
    
    order.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    return order;
  }

  private ClassDefinition CreateClientDefinition ()
  {
    return new ClassDefinition (
        "Client", "Client", typeof (Client), DatabaseTest.c_testDomainProviderID);
  }

  private ClassDefinition CreateLocationDefinition ()
  {
    ClassDefinition location = new ClassDefinition (
        "Location", "Location", typeof (Location), DatabaseTest.c_testDomainProviderID);
    
    location.MyPropertyDefinitions.Add (new PropertyDefinition ("Client", "ClientID", "objectID"));

    return location;
  }

  private ClassDefinition CreateClassWithAllDataTypesDefinition ()
  {
    ClassDefinition classWithAllDataTypes = new ClassDefinition (
        "ClassWithAllDataTypes", "TableWithAllDataTypes", typeof (ClassWithAllDataTypes), DatabaseTest.c_testDomainProviderID);
    
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

    classWithAllDataTypes.MyPropertyDefinitions.Add (new PropertyDefinition ("StringWithNullValueProperty", "StringWithNullValue", "string", true, 100));
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

    return classWithAllDataTypes;
  }

  private ClassDefinition CreateClassWithGuidKeyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithGuidKey", "TableWithGuidKey",
        typeof (ClassWithGuidKey), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private ClassDefinition CreateClassWithInvalidKeyTypeDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithKeyOfInvalidType", "TableWithKeyOfInvalidType",
        typeof (ClassWithKeyOfInvalidType), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private ClassDefinition CreateClassWithoutIDPropertyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutIDProperty", "TableWithoutIDColumn",
        typeof (ClassWithoutIDProperty), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private ClassDefinition CreateClassWithoutClassIDPropertyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutClassIDProperty", "TableWithoutClassIDColumn",
        typeof (ClassWithoutClassIDProperty), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private ClassDefinition CreateClassWithoutTimestampPropertyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutTimestampProperty", "TableWithoutTimestampColumn",
        typeof (ClassWithoutTimestampProperty), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private ClassDefinition CreateClassWithValidRelationsDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithValidRelations", "TableWithValidRelations",
        typeof (ClassWithValidRelations), DatabaseTest.c_testDomainProviderID);

    classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
        "ClassWithGuidKeyOptional", "TableWithGuidKeyOptionalID", "objectID"));

    classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
        "ClassWithGuidKeyNonOptional", "TableWithGuidKeyNonOptionalID", "objectID"));

    return classDefinition;
  }

  private ClassDefinition CreateClassWithInvalidRelationDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithInvalidRelation", "TableWithInvalidRelation",
        typeof (ClassWithInvalidRelation), DatabaseTest.c_testDomainProviderID);

    classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
        "ClassWithGuidKey", "TableWithGuidKeyID", "objectID"));

    return classDefinition;
  }

  private ClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition (
        "ClassWithoutRelatedClassIDColumn", 
        "TableWithoutRelatedClassIDColumn",
        typeof (ClassWithoutRelatedClassIDColumn), 
        DatabaseTest.c_testDomainProviderID);

    classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
        "Distributor", "DistributorID", "objectID"));

    return classDefinition;
  }

  private ClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition (
        "ClassWithoutRelatedClassIDColumnAndDerivation", 
        "TableWithoutRelatedClassIDColumnAndDerivation",
        typeof (ClassWithoutRelatedClassIDColumnAndDerivation), 
        DatabaseTest.c_testDomainProviderID);

    classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition (
        "Company", "CompanyID", "objectID"));

    return classDefinition;
  }

  private ClassDefinition CreateIndustrialSectorDefinition ()
  {
    ClassDefinition industrialSector = new ClassDefinition (
      "IndustrialSector", "IndustrialSector", typeof (IndustrialSector), DatabaseTest.c_testDomainProviderID);
    
    industrialSector.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    return industrialSector;
  }

  private ClassDefinition CreateEmployeeDefinition ()
  {
    ClassDefinition employee = new ClassDefinition (
        "Employee", "Employee", typeof (Employee), DatabaseTest.c_testDomainProviderID);
    
    employee.MyPropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
    employee.MyPropertyDefinitions.Add (new PropertyDefinition ("Supervisor", "SupervisorID", "objectID"));

    return employee;
  }

  private ClassDefinition CreateComputerDefinition ()
  {
    ClassDefinition computer = new ClassDefinition (
        "Computer", "Computer", typeof (Computer), DatabaseTest.c_testDomainProviderID);
    
    computer.MyPropertyDefinitions.Add (new PropertyDefinition ("SerialNumber", "SerialNumber", "string", 20));
    computer.MyPropertyDefinitions.Add (new PropertyDefinition ("Employee", "EmployeeID", "objectID"));

    return computer;
  }

  private static ClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition (
        "ClassWithRelatedClassIDColumnAndNoInheritance", "TableWithRelatedClassIDColumnAndNoInheritance", 
        typeof (ClassWithRelatedClassIDColumnAndNoInheritance), DatabaseTest.c_testDomainProviderID);

    classDefinition.MyPropertyDefinitions.Add (new PropertyDefinition ("ClassWithGuidKey", "TableWithGuidKeyID", "objectID"));

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

    relationDefinitions.Add (CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ());
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

  private RelationDefinition CreateClientToLocationRelationDefinition ()
  {
    ClassDefinition clientClass = _classDefinitions["Client"];
    ClassDefinition locationClass = _classDefinitions["Location"];
    
    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        clientClass, "Locations", false, CardinalityType.Many, typeof (DomainObjectCollection));

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        locationClass, "Client", true);

    RelationDefinition relation = new RelationDefinition ("ClientToLocation", endPoint1, endPoint2);

    clientClass.MyRelationDefinitions.Add (relation);
    locationClass.MyRelationDefinitions.Add (relation);

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
