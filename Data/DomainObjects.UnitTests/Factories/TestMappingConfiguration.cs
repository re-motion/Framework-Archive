using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
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
    _relationDefinitions = CreateRelationDefintions ();
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
    classDefinitions.Add (CreateOfficialDefinition ());
    classDefinitions.Add (CreateCeoDefinition ());
    classDefinitions.Add (CreatePersonDefinition ());
    classDefinitions.Add (CreateClassWithoutRelatedClassIDColumnAndDerivation ());
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

    return classDefinitions;
  }

  private ClassDefinition CreateCompanyDefinition ()
  {
    ClassDefinition company = new ClassDefinition (
        "Company", "Company", typeof (Company), DatabaseTest.c_testDomainProviderID);

    company.PropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
    company.PropertyDefinitions.Add (new PropertyDefinition (
        "IndustrialSector", "IndustrialSectorID", "objectID"));
    
    return company;
  }

  private ClassDefinition CreateCustomerDefinition (ClassDefinition baseClass)
  {
    ClassDefinition customer = new ClassDefinition (
        "Customer", "Company", typeof (Customer), DatabaseTest.c_testDomainProviderID, baseClass);
    
    customer.PropertyDefinitions.Add (new PropertyDefinition ("CustomerSince", "CustomerSince", "dateTime", true));

    customer.PropertyDefinitions.Add (
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

    partner.PropertyDefinitions.Add (new PropertyDefinition ("ContactPerson", "ContactPersonID", "objectID"));
    
    return partner;
  }

  private ClassDefinition CreateSupplierDefinition (ClassDefinition baseClass)
  {
    ClassDefinition supplier = new ClassDefinition (
        "Supplier", "Company", typeof (Supplier), DatabaseTest.c_testDomainProviderID, baseClass);

    supplier.PropertyDefinitions.Add (new PropertyDefinition ("SupplierQuality", "SupplierQuality", "int32"));
    
    return supplier;
  }

  private ClassDefinition CreateDistributorDefinition (ClassDefinition baseClass)
  {
    ClassDefinition distributor = new ClassDefinition (
        "Distributor", "Company", typeof (Distributor), DatabaseTest.c_testDomainProviderID, baseClass);

    distributor.PropertyDefinitions.Add (new PropertyDefinition ("NumberOfShops", "NumberOfShops", "int32"));
    
    return distributor;
  }

  private ClassDefinition CreateOrderDefinition ()
  {
    ClassDefinition order = new ClassDefinition (
        "Order", "Order", typeof (Order), DatabaseTest.c_testDomainProviderID);
    
    order.PropertyDefinitions.Add (new PropertyDefinition ("OrderNumber", "OrderNo", "int32"));
    order.PropertyDefinitions.Add (new PropertyDefinition ("DeliveryDate", "DeliveryDate", "dateTime"));
    order.PropertyDefinitions.Add (new PropertyDefinition ("Customer", "CustomerID", "objectID"));
    order.PropertyDefinitions.Add (new PropertyDefinition ("Official", "OfficialID", "objectID"));

    return order;
  }

  private ClassDefinition CreateOfficialDefinition ()
  {
    ClassDefinition official = new ClassDefinition (
        "Official", "Official", typeof (Official), DatabaseTest.c_unitTestStorageProviderStubID);
    
    official.PropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    return official;
  }

  private ClassDefinition CreateOrderTicketDefinition ()
  {
    ClassDefinition orderTicket = new ClassDefinition (
        "OrderTicket", "OrderTicket", typeof (OrderTicket), DatabaseTest.c_testDomainProviderID);
    
    orderTicket.PropertyDefinitions.Add (new PropertyDefinition ("FileName", "FileName", "string", 255));
    orderTicket.PropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID"));

    return orderTicket;
  }

  private ClassDefinition CreateOrderItemDefinition ()
  {
    ClassDefinition orderItem = new ClassDefinition (
        "OrderItem", "OrderItem", typeof (OrderItem), DatabaseTest.c_testDomainProviderID);
    
    orderItem.PropertyDefinitions.Add (new PropertyDefinition ("Order", "OrderID", "objectID"));
    orderItem.PropertyDefinitions.Add (new PropertyDefinition ("Position", "Position", "int32"));
    orderItem.PropertyDefinitions.Add (new PropertyDefinition ("Product", "Product", "string", 100));

    return orderItem;
  }

  private ClassDefinition CreateCeoDefinition ()
  {
    ClassDefinition order = new ClassDefinition (
        "Ceo", "Ceo", typeof (Ceo), DatabaseTest.c_testDomainProviderID);
    
    order.PropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
    order.PropertyDefinitions.Add (new PropertyDefinition ("Company", "CompanyID", "objectID"));

    return order;
  }

  private ClassDefinition CreatePersonDefinition ()
  {
    ClassDefinition order = new ClassDefinition (
        "Person", "Person", typeof (Person), DatabaseTest.c_testDomainProviderID);
    
    order.PropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    return order;
  }

  private ClassDefinition CreateClassWithAllDataTypesDefinition ()
  {
    ClassDefinition classWithAllDataTypes = new ClassDefinition (
        "ClassWithAllDataTypes", "TableWithAllDataTypes", typeof (ClassWithAllDataTypes), DatabaseTest.c_testDomainProviderID);
    
    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "BooleanProperty", "Boolean", "boolean"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition ("ByteProperty", "Byte", "byte"));
    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition ("CharProperty", "Char", "char"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "DateProperty", "Date", "date"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "DateTimeProperty", "DateTime", "dateTime"));
    
    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "DecimalProperty", "Decimal", "decimal"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "DoubleProperty", "Double", "double"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "EnumProperty", 
        "Enum", 
        "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes+EnumType, Rubicon.Data.DomainObjects.UnitTests"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition ("GuidProperty", "Guid", "guid"));
    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition ("Int16Property", "Int16", "int16"));
    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition ("Int32Property", "Int32", "int32"));
    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition ("Int64Property", "Int64", "int64"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "SingleProperty", "Single", "single"));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "StringProperty", "String", "string", 100));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaBooleanProperty", "NaBoolean", "boolean", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaDateProperty", "NaDate", "date", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaDateTimeProperty", "NaDateTime", "dateTime", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaDoubleProperty", "NaDouble", "double", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaInt32Property", "NaInt32", "int32", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "StringWithNullValueProperty", "StringWithNullValue", "string", true, 100));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaBooleanWithNullValueProperty", "NaBooleanWithNullValue", "boolean", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaDateWithNullValueProperty", "NaDateWithNullValue", "date", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaDateTimeWithNullValueProperty", "NaDateTimeWithNullValue", "dateTime", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaDoubleWithNullValueProperty", "NaDoubleWithNullValue", "double", true));

    classWithAllDataTypes.PropertyDefinitions.Add (new PropertyDefinition (
        "NaInt32WithNullValueProperty", "NaInt32WithNullValue", "int32", true));

    return classWithAllDataTypes;
  }

  private static ClassDefinition CreateClassWithGuidKeyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithGuidKey", "TableWithGuidKey",
        typeof (ClassWithGuidKey), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithInvalidKeyTypeDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithKeyOfInvalidType", "TableWithKeyOfInvalidType",
        typeof (ClassWithKeyOfInvalidType), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithoutIDPropertyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutIDProperty", "TableWithoutIDColumn",
        typeof (ClassWithoutIDProperty), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithoutClassIDPropertyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutClassIDProperty", "TableWithoutClassIDColumn",
        typeof (ClassWithoutClassIDProperty), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithoutTimestampPropertyDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithoutTimestampProperty", "TableWithoutTimestampColumn",
        typeof (ClassWithoutTimestampProperty), DatabaseTest.c_testDomainProviderID);

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithValidRelationsDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithValidRelations", "TableWithValidRelations",
        typeof (ClassWithValidRelations), DatabaseTest.c_testDomainProviderID);

    classDefinition.PropertyDefinitions.Add (new PropertyDefinition (
        "ClassWithGuidKeyOptional", "TableWithGuidKeyOptionalID", "objectID"));

    classDefinition.PropertyDefinitions.Add (new PropertyDefinition (
        "ClassWithGuidKeyNonOptional", "TableWithGuidKeyNonOptionalID", "objectID"));

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithInvalidRelationDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition ("ClassWithInvalidRelation", "TableWithInvalidRelation",
        typeof (ClassWithInvalidRelation), DatabaseTest.c_testDomainProviderID);

    classDefinition.PropertyDefinitions.Add (new PropertyDefinition (
        "ClassWithGuidKey", "TableWithGuidKeyID", "objectID"));

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition ()
  {
    ClassDefinition classDefinition = new ClassDefinition (
        "ClassWithoutRelatedClassIDColumn", 
        "TableWithoutRelatedClassIDColumn",
        typeof (ClassWithoutRelatedClassIDColumn), 
        DatabaseTest.c_testDomainProviderID);

    classDefinition.PropertyDefinitions.Add (new PropertyDefinition (
        "Partner", "PartnerID", "objectID"));

    return classDefinition;
  }

  private static ClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivation ()
  {
    ClassDefinition classDefinition = new ClassDefinition (
        "ClassWithoutRelatedClassIDColumnAndDerivation", 
        "TableWithoutRelatedClassIDColumnAndDerivation",
        typeof (ClassWithoutRelatedClassIDColumnAndDerivation), 
        DatabaseTest.c_testDomainProviderID);

    classDefinition.PropertyDefinitions.Add (new PropertyDefinition (
        "Company", "CompanyID", "objectID"));

    return classDefinition;
  }

  private ClassDefinition CreateIndustrialSectorDefinition ()
  {
    ClassDefinition industrialSector = new ClassDefinition (
      "IndustrialSector", "IndustrialSector", typeof (IndustrialSector), DatabaseTest.c_testDomainProviderID);
    
    industrialSector.PropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));

    return industrialSector;
  }

  private ClassDefinition CreateEmployeeDefinition ()
  {
    ClassDefinition employee = new ClassDefinition (
        "Employee", "Employee", typeof (Employee), DatabaseTest.c_testDomainProviderID);
    
    employee.PropertyDefinitions.Add (new PropertyDefinition ("Name", "Name", "string", 100));
    employee.PropertyDefinitions.Add (new PropertyDefinition ("Supervisor", "SupervisorID", "objectID"));

    return employee;
  }

  private ClassDefinition CreateComputerDefinition ()
  {
    ClassDefinition computer = new ClassDefinition (
        "Computer", "Computer", typeof (Computer), DatabaseTest.c_testDomainProviderID);
    
    computer.PropertyDefinitions.Add (new PropertyDefinition ("SerialNumber", "SerialNumber", "string", 20));
    computer.PropertyDefinitions.Add (new PropertyDefinition ("Employee", "EmployeeID", "objectID"));

    return computer;
  }
  
  #endregion

  #region Methods for creating relation definitions

  private RelationDefinitionCollection CreateRelationDefintions ()
  {
    RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection ();
    
    relationDefinitions.Add (CreateCustomerToOrderRelationDefinition ());
    relationDefinitions.Add (CreateOrderToOrderItemRelationDefinition ());    
    relationDefinitions.Add (CreateOrderToOrderTicketRelationDefinition ());    
    relationDefinitions.Add (CreateOrderToOfficialRelationDefinition ());    
    relationDefinitions.Add (CreateCompanyToCeoRelationDefinition ());    
    relationDefinitions.Add (CreatePartnerToPersonRelationDefinition ());    
    relationDefinitions.Add (CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ());
    relationDefinitions.Add (CreatePartnerToClassWithoutRelatedClassIDColumnRelationDefinition ());
    relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsOptional ());
    relationDefinitions.Add (CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ());
    relationDefinitions.Add (CreateClassWithGuidKeyToClassWithInvalidRelation ());
    relationDefinitions.Add (CreateIndustrialSectorToCompanyRelationDefinition ());
    relationDefinitions.Add (CreateSupervisorToSubordinateRelationDefinition ());
    relationDefinitions.Add (CreateEmployeeToComputerRelationDefinition ());

    return relationDefinitions;
  }

  private RelationDefinition CreateCustomerToOrderRelationDefinition ()
  {  
    ClassDefinition customer = _classDefinitions.GetByClassID ("Customer");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        customer, "Orders", false, CardinalityType.Many, typeof (OrderCollection));

    ClassDefinition orderClass = _classDefinitions.GetByClassID ("Order");
    
    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        orderClass, "Customer", true);

    RelationDefinition relation = new RelationDefinition ("CustomerToOrder", endPoint1, endPoint2);

    customer.RelationDefinitions.Add (relation);
    orderClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateOrderToOrderTicketRelationDefinition ()
  {
    ClassDefinition orderClass = _classDefinitions.GetByClassID ("Order");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        orderClass, "OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

    ClassDefinition orderTicketClass = _classDefinitions.GetByClassID ("OrderTicket");
    
    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        orderTicketClass, "Order", true);

    RelationDefinition relation = new RelationDefinition ("OrderToOrderTicket", endPoint1, endPoint2);

    orderClass.RelationDefinitions.Add (relation);
    orderTicketClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateOrderToOrderItemRelationDefinition ()
  {
    ClassDefinition orderClass = _classDefinitions.GetByClassID ("Order");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        orderClass, "OrderItems", true, CardinalityType.Many, typeof (DomainObjectCollection));

    ClassDefinition orderItemClass = _classDefinitions.GetByClassID ("OrderItem");
    
    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        orderItemClass, "Order", true);

    RelationDefinition relation = new RelationDefinition ("OrderToOrderItem", endPoint1, endPoint2);

    orderClass.RelationDefinitions.Add (relation);
    orderItemClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateOrderToOfficialRelationDefinition ()
  {
    ClassDefinition officialClass = _classDefinitions.GetByClassID ("Official");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        officialClass, "Orders", false, CardinalityType.Many, typeof (DomainObjectCollection));

    ClassDefinition orderClass = _classDefinitions.GetByClassID ("Order");
    
    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        orderClass, "Official", true);

    RelationDefinition relation = new RelationDefinition ("OfficialToOrder", endPoint1, endPoint2);

    officialClass.RelationDefinitions.Add (relation);
    orderClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateCompanyToCeoRelationDefinition ()
  {
    ClassDefinition companyClass = _classDefinitions.GetByClassID ("Company");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        companyClass, "Ceo", true, CardinalityType.One, typeof (Ceo));

    ClassDefinition ceoClass = _classDefinitions.GetByClassID ("Ceo");
    
    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        ceoClass, "Company", true);

    RelationDefinition relation = new RelationDefinition ("CompanyToCeo", endPoint1, endPoint2);

    companyClass.RelationDefinitions.Add (relation);
    ceoClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreatePartnerToPersonRelationDefinition ()
  {
    ClassDefinition personClass = _classDefinitions.GetByClassID ("Person");

    ClassDefinition partnerClass = _classDefinitions.GetByClassID ("Partner");
    
    RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
        partnerClass, "ContactPerson", true);

    VirtualRelationEndPointDefinition endPoint2 = new VirtualRelationEndPointDefinition (
        personClass, "AssociatedPartnerCompany", false, CardinalityType.One, typeof (Partner));

    RelationDefinition relation = new RelationDefinition ("PartnerToPerson", endPoint1, endPoint2);

    personClass.RelationDefinitions.Add (relation);
    partnerClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreatePartnerToClassWithoutRelatedClassIDColumnRelationDefinition ()
  {
    ClassDefinition partnerClass = _classDefinitions.GetByClassID ("Partner");
  
    ClassDefinition classWithoutRelatedClassIDColumnClass = _classDefinitions.GetByClassID (
        "ClassWithoutRelatedClassIDColumn");
    
    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        partnerClass, 
        "ClassWithoutRelatedClassIDColumn", 
        false, 
        CardinalityType.One, 
        typeof (ClassWithoutRelatedClassIDColumn));

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        classWithoutRelatedClassIDColumnClass, "Partner", false);

    RelationDefinition relation = new RelationDefinition ("PartnerToClassWithoutRelatedClassIDColumn", endPoint1, endPoint2);

    partnerClass.RelationDefinitions.Add (relation);
    classWithoutRelatedClassIDColumnClass.RelationDefinitions.Add (relation);

    return relation;
  }
  
  private RelationDefinition CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ()
  {
    ClassDefinition companyClass = _classDefinitions.GetByClassID ("Company");
  
    ClassDefinition classWithoutRelatedClassIDColumnAndDerivation = _classDefinitions.GetByClassID (
        "ClassWithoutRelatedClassIDColumnAndDerivation");
    
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

    companyClass.RelationDefinitions.Add (relation);
    classWithoutRelatedClassIDColumnAndDerivation.RelationDefinitions.Add (relation);

    return relation;
  }
  
  private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsOptional ()
  {
    ClassDefinition classWithGuidKey = _classDefinitions.GetByClassID ("ClassWithGuidKey");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        classWithGuidKey, "ClassWithValidRelationsOptional", false, CardinalityType.One, typeof (ClassWithValidRelations));

    ClassDefinition classWithValidRelations = _classDefinitions.GetByClassID ("ClassWithValidRelations");

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        classWithValidRelations, "ClassWithGuidKeyOptional", false);
    
    RelationDefinition relation = new RelationDefinition ("ClassWithGuidKeyToClassWithValidRelationsOptional",
        endPoint1, endPoint2);

    classWithGuidKey.RelationDefinitions.Add (relation);
    classWithValidRelations.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ()
  {
    ClassDefinition classWithGuidKey = _classDefinitions.GetByClassID ("ClassWithGuidKey");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        classWithGuidKey, 
        "ClassWithValidRelationsNonOptional", 
        true, 
        CardinalityType.One, 
        typeof (ClassWithValidRelations));

    ClassDefinition classWithValidRelations = _classDefinitions.GetByClassID ("ClassWithValidRelations");

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        classWithValidRelations, "ClassWithGuidKeyNonOptional", true);

    RelationDefinition relation = new RelationDefinition ("ClassWithGuidKeyToClassWithValidRelationsNonOptional",
        endPoint1, endPoint2);

    classWithGuidKey.RelationDefinitions.Add (relation);
    classWithValidRelations.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateClassWithGuidKeyToClassWithInvalidRelation ()
  {
    ClassDefinition classWithGuidKey = _classDefinitions.GetByClassID ("ClassWithGuidKey");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        classWithGuidKey, "ClassWithInvalidRelation", false, CardinalityType.One, typeof (ClassWithInvalidRelation));

    ClassDefinition classWithInvalidRelation = _classDefinitions.GetByClassID ("ClassWithInvalidRelation");

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        classWithInvalidRelation, "ClassWithGuidKey", false);

    RelationDefinition relation = new RelationDefinition ("ClassWithGuidKeyToClassWithInvalidRelation",
        endPoint1, endPoint2);

    classWithGuidKey.RelationDefinitions.Add (relation);
    classWithInvalidRelation.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateIndustrialSectorToCompanyRelationDefinition ()
  {
    ClassDefinition industrialSectorClass = _classDefinitions.GetByClassID ("IndustrialSector");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        industrialSectorClass, "Companies", true, CardinalityType.Many, typeof (DomainObjectCollection));

    ClassDefinition companyClass = _classDefinitions.GetByClassID ("Company");

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        companyClass, "IndustrialSector", false);

    RelationDefinition relation = new RelationDefinition ("IndustrialSectorToCompany", endPoint1, endPoint2);

    industrialSectorClass.RelationDefinitions.Add (relation);
    companyClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateSupervisorToSubordinateRelationDefinition ()
  {
    ClassDefinition employeeClass = _classDefinitions.GetByClassID ("Employee");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        employeeClass, "Subordinates", false, CardinalityType.Many, typeof (DomainObjectCollection));

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        employeeClass, "Supervisor", false);

    RelationDefinition relation = new RelationDefinition ("SupervisorToSubordinate", endPoint1, endPoint2);

    employeeClass.RelationDefinitions.Add (relation);

    return relation;
  }

  private RelationDefinition CreateEmployeeToComputerRelationDefinition ()
  {
    ClassDefinition employeeClass = _classDefinitions.GetByClassID ("Employee");

    VirtualRelationEndPointDefinition endPoint1 = new VirtualRelationEndPointDefinition (
        employeeClass, "Computer", false, CardinalityType.One, typeof (Computer));

    ClassDefinition computerClass = _classDefinitions.GetByClassID ("Computer");

    RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
        computerClass, "Employee", false);

    RelationDefinition relation = new RelationDefinition ("EmployeeToComputer", endPoint1, endPoint2);

    employeeClass.RelationDefinitions.Add (relation);
    computerClass.RelationDefinitions.Add (relation);

    return relation;
  }

  #endregion
}
}
