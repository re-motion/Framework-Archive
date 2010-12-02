// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public class FakeMappingConfiguration
  {
    // types

    // static members and constants

    private static readonly DoubleCheckedLockingContainer<FakeMappingConfiguration> s_current
        = new DoubleCheckedLockingContainer<FakeMappingConfiguration> (() => new FakeMappingConfiguration());

    public static FakeMappingConfiguration Current
    {
      get { return s_current.Value; }
    }

    public static void Reset ()
    {
      s_current.Value = null;
    }

    // member fields

    private ClassDefinitionCollection _classDefinitions;
    private RelationDefinitionCollection _relationDefinitions;
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;

    // construction and disposing

    private FakeMappingConfiguration ()
    {
      _classDefinitions = CreateClassDefinitions();
      _relationDefinitions = CreateRelationDefinitions();
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("DefaultStorageProvider", typeof (UnitTestStorageObjectFactoryStub));

      foreach (ClassDefinition classDefinition in _classDefinitions)
        classDefinition.SetReadOnly();
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
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();

      ReflectionBasedClassDefinition testDomainBase = CreateTestDomainBaseDefinition();
      //classDefinitions.Add (testDomainBase);

      ReflectionBasedClassDefinition storageProviderStubDomainBase = CreateStorageProviderStubDomainBaseDefinition();
      //classDefinitions.Add (storageProviderStubDomainBase);

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

      ReflectionBasedClassDefinition officialDefinition = CreateOfficialDefinition (null);
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

      var targetClassForPersistentMixinDefinition = CreateTargetClassForPersistentMixinDefinition (null);
      classDefinitions.Add (targetClassForPersistentMixinDefinition);
      var derivedTargetClassForPersistentMixinDefinition = CreateDerivedTargetClassForPersistentMixinDefinition (targetClassForPersistentMixinDefinition);
      classDefinitions.Add (derivedTargetClassForPersistentMixinDefinition);
      var derivedDerivedTargetClassForPersistentMixinDefinition = CreateDerivedDerivedTargetClassForPersistentMixinDefinition (derivedTargetClassForPersistentMixinDefinition);
      classDefinitions.Add (derivedDerivedTargetClassForPersistentMixinDefinition);
      var relationTargetForPersistentMixinDefinition = CreateRelationTargetForPersistentMixinDefinition (null);
      classDefinitions.Add (relationTargetForPersistentMixinDefinition);

      var classesByBaseClass = from classDefinition in classDefinitions.Cast<ClassDefinition> ()
                               where classDefinition.BaseClass != null
                               group classDefinition by classDefinition.BaseClass
                                 into grouping
                                 select new { grouping.Key, Values = grouping.Distinct () };

      var classesWithDerivedClasses = new HashSet<ClassDefinition> ();
      foreach (var classesWithBaseClass in classesByBaseClass)
      {
        classesWithBaseClass.Key.SetDerivedClasses (new ClassDefinitionCollection (classesWithBaseClass.Values, true, true));
        classesWithDerivedClasses.Add (classesWithBaseClass.Key);
      }

      foreach (var classDefinition in classDefinitions.Cast<ClassDefinition> ().Where (cd => !classesWithDerivedClasses.Contains (cd)))
        classDefinition.SetDerivedClasses (new ClassDefinitionCollection (true));

      return classDefinitions;
    }

    private ReflectionBasedClassDefinition CreateTestDomainBaseDefinition ()
    {
      ReflectionBasedClassDefinition testDomainBase = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "TestDomainBase", null, _storageProviderDefinition,  typeof (TestDomainBase), true);
      testDomainBase.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return testDomainBase;
    }

    private ReflectionBasedClassDefinition CreateStorageProviderStubDomainBaseDefinition ()
    {
      ReflectionBasedClassDefinition storageProviderStubDomainBase =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "StorageProviderStubDomainBase", null, _storageProviderDefinition, typeof (StorageProviderStubDomainBase), true);
      storageProviderStubDomainBase.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return storageProviderStubDomainBase;
    }

    private ReflectionBasedClassDefinition CreateCompanyDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition company = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Company", "Company", _storageProviderDefinition, typeof (Company), false, baseClass);
      
      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              company, typeof (Company), "Name", "Name", typeof (string), false, 100));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              company, typeof (Company), "IndustrialSector", "IndustrialSectorID", typeof (ObjectID), true));
      company.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return company;
    }

    private ReflectionBasedClassDefinition CreateCustomerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition customer = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer", null, _storageProviderDefinition, typeof (Customer), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              customer, typeof (Customer), "CustomerSince", "CustomerSince", typeof (DateTime?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              customer, typeof (Customer), "Type", "CustomerType", typeof (Customer.CustomerType)));
      customer.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return customer;
    }

    private ReflectionBasedClassDefinition CreatePartnerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition partner = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Partner", null, _storageProviderDefinition, typeof (Partner), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              partner, typeof (Partner), "ContactPerson", "ContactPersonID", typeof (ObjectID), true));
      partner.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return partner;
    }

    private ReflectionBasedClassDefinition CreateSupplierDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition supplier = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Supplier", null, _storageProviderDefinition, typeof (Supplier), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              supplier, typeof (Supplier), "SupplierQuality", "SupplierQuality", typeof (int)));
      supplier.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return supplier;
    }

    private ReflectionBasedClassDefinition CreateDistributorDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition distributor = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Distributor", null, _storageProviderDefinition, typeof (Distributor), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              distributor, typeof (Distributor), "NumberOfShops", "NumberOfShops", typeof (int)));
      distributor.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return distributor;
    }

    private ReflectionBasedClassDefinition CreateOrderDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition order = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "Order", _storageProviderDefinition, typeof (Order), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "OrderNumber", "OrderNo", typeof (int)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "DeliveryDate", "DeliveryDate", typeof (DateTime)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "Customer", "CustomerID", typeof (ObjectID), true));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              order, typeof (Order), "Official", "OfficialID", typeof (ObjectID), true));
      order.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return order;
    }

    private ReflectionBasedClassDefinition CreateOfficialDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition official = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Official", "Official", _storageProviderDefinition, typeof (Official), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              official, typeof (Official), "Name", "Name", typeof (string), false, 100));
      official.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return official;
    }

    private ReflectionBasedClassDefinition CreateSpecialOfficialDefinition (ReflectionBasedClassDefinition officialDefinition)
    {
      var specialOfficial = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "SpecialOfficial", null, _storageProviderDefinition, typeof (SpecialOfficial), false, officialDefinition);
      specialOfficial.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return specialOfficial;
    }

    private ReflectionBasedClassDefinition CreateOrderTicketDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition orderTicket = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrderTicket", "OrderTicket", _storageProviderDefinition, typeof (OrderTicket), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderTicket, typeof (OrderTicket), "FileName", "FileName", typeof (string), false, 255));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderTicket, typeof (OrderTicket), "Order", "OrderID", typeof (ObjectID), true));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderTicket,
              typeof (OrderTicket), "Int32TransactionProperty",
              null,
              typeof (int),
              null,
              null,
              StorageClass.Transaction));
      orderTicket.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return orderTicket;
    }

    private ReflectionBasedClassDefinition CreateOrderItemDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition orderItem = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "OrderItem", "OrderItem", _storageProviderDefinition, typeof (OrderItem), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderItem, typeof (OrderItem), "Order", "OrderID", typeof (ObjectID), true));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderItem, typeof (OrderItem), "Position", "Position", typeof (int)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              orderItem, typeof (OrderItem), "Product", "Product", typeof (string), false, 100));
      orderItem.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return orderItem;
    }

    private ReflectionBasedClassDefinition CreateCeoDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition ceo = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Ceo", "Ceo", _storageProviderDefinition, typeof (Ceo), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              ceo, typeof (Ceo), "Name", "Name", typeof (string), false, 100));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              ceo, typeof (Ceo), "Company", "CompanyID", typeof (ObjectID), true));
      ceo.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return ceo;
    }

    private ReflectionBasedClassDefinition CreatePersonDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition person = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Person", "Person", _storageProviderDefinition, typeof (Person), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              person, typeof (Person), "Name", "Name", typeof (string), false, 100));
      person.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return person;
    }

    private ReflectionBasedClassDefinition CreateClientDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition client = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Client", "Client", _storageProviderDefinition, typeof (Client), false, baseClass);
       
      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              client, typeof (Client), "ParentClient", "ParentClientID", typeof (ObjectID), true));
      client.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return client;
    }

    private ReflectionBasedClassDefinition CreateLocationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition location = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Location", "Location", _storageProviderDefinition, typeof (Location), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              location, typeof (Location), "Client", "ClientID", typeof (ObjectID), true));
      location.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return location;
    }

    private ReflectionBasedClassDefinition CreateFileSystemItemDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition fileSystemItem = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "FileSystemItem", "FileSystemItem", _storageProviderDefinition, typeof (FileSystemItem), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              fileSystemItem,
              typeof (FileSystemItem), "ParentFolder",
              "ParentFolderID",
              typeof (ObjectID),
              true));
      fileSystemItem.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return fileSystemItem;
    }

    private ReflectionBasedClassDefinition CreateFolderDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition folder = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Folder", null, _storageProviderDefinition, typeof (Folder), false, baseClass);
      folder.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return folder;
    }

    private ReflectionBasedClassDefinition CreateFileDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition file = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "File", null, _storageProviderDefinition, typeof (File), false, baseClass);
      file.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return file;
    }

    //TODO: remove Date and NaDate properties
    private ReflectionBasedClassDefinition CreateClassWithAllDataTypesDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classWithAllDataTypes = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithAllDataTypes", "TableWithAllDataTypes", _storageProviderDefinition, typeof (ClassWithAllDataTypes), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "BooleanProperty",
              "Boolean",
              typeof (bool)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "ByteProperty", "Byte", typeof (byte)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "DateProperty", "Date", typeof (DateTime)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "DateTimeProperty",
              "DateTime",
              typeof (DateTime)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "DecimalProperty",
              "Decimal",
              typeof (Decimal)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "DoubleProperty",
              "Double",
              typeof (double)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "EnumProperty",
              "Enum",
              typeof (ClassWithAllDataTypes.EnumType)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "ExtensibleEnumProperty",
              "ExtensibleEnum",
              typeof (Color),
              false));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "FlagsProperty",
              "Flags",
              typeof (ClassWithAllDataTypes.FlagsType)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "GuidProperty", "Guid", typeof (Guid)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int16Property", "Int16", typeof (short)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int32Property", "Int32", typeof (int)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "Int64Property", "Int64", typeof (long)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "SingleProperty", "Single", typeof (float)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "StringProperty",
              "String",
              typeof (string),
              false,
              100));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "StringPropertyWithoutMaxLength",
              "StringWithoutMaxLength",
              typeof (string),
              false));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "BinaryProperty",
              "Binary",
              typeof (byte[]),
              false));

      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaBooleanProperty",
              "NaBoolean",
              typeof (bool?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaByteProperty", "NaByte", typeof (byte?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateProperty",
              "NaDate",
              typeof (DateTime?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateTimeProperty",
              "NaDateTime",
              typeof (DateTime?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDecimalProperty",
              "NaDecimal",
              typeof (Decimal?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDoubleProperty",
              "NaDouble",
              typeof (double?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaEnumProperty",
              "NaEnum",
              typeof (ClassWithAllDataTypes.EnumType?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaFlagsProperty",
              "NaFlags",
              typeof (ClassWithAllDataTypes.FlagsType?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes, typeof (ClassWithAllDataTypes), "NaGuidProperty", "NaGuid", typeof (Guid?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt16Property",
              "NaInt16",
              typeof (short?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt32Property",
              "NaInt32",
              typeof (int?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt64Property",
              "NaInt64",
              typeof (long?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaSingleProperty",
              "NaSingle",
              typeof (float?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "StringWithNullValueProperty",
              "StringWithNullValue",
              typeof (string),
              true,
              100));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "ExtensibleEnumWithNullValueProperty",
              "ExtensibleEnumWithNullValue",
              typeof (Color),
              true));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaBooleanWithNullValueProperty",
              "NaBooleanWithNullValue",
              typeof (bool?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaByteWithNullValueProperty",
              "NaByteWithNullValue",
              typeof (byte?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateWithNullValueProperty",
              "NaDateWithNullValue",
              typeof (DateTime?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDateTimeWithNullValueProperty",
              "NaDateTimeWithNullValue",
              typeof (DateTime?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDecimalWithNullValueProperty",
              "NaDecimalWithNullValue",
              typeof (Decimal?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaDoubleWithNullValueProperty",
              "NaDoubleWithNullValue",
              typeof (double?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaEnumWithNullValueProperty",
              "NaEnumWithNullValue",
              typeof (ClassWithAllDataTypes.EnumType?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaFlagsWithNullValueProperty",
              "NaFlagsWithNullValue",
              typeof (ClassWithAllDataTypes.FlagsType?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaGuidWithNullValueProperty",
              "NaGuidWithNullValue",
              typeof (Guid?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt16WithNullValueProperty",
              "NaInt16WithNullValue",
              typeof (short?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt32WithNullValueProperty",
              "NaInt32WithNullValue",
              typeof (int?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaInt64WithNullValueProperty",
              "NaInt64WithNullValue",
              typeof (long?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NaSingleWithNullValueProperty",
              "NaSingleWithNullValue",
              typeof (float?)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classWithAllDataTypes,
              typeof (ClassWithAllDataTypes), "NullableBinaryProperty",
              "NullableBinary",
              typeof (byte[]),
              true,
              1000000));
      classWithAllDataTypes.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return classWithAllDataTypes;
    }

    private ReflectionBasedClassDefinition CreateClassWithGuidKeyDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithGuidKey",
          "TableWithGuidKey",
          _storageProviderDefinition,
          typeof (ClassWithGuidKey),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithInvalidKeyTypeDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithKeyOfInvalidType",
          "TableWithKeyOfInvalidType",
          _storageProviderDefinition,
          typeof (ClassWithKeyOfInvalidType),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithoutIDColumn",
          "TableWithoutIDColumn",
          _storageProviderDefinition,
          typeof (ClassWithoutIDColumn),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutClassIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithoutClassIDColumn",
          "TableWithoutClassIDColumn",
          _storageProviderDefinition,
          typeof (ClassWithoutClassIDColumn),
          false,
          baseClass);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutTimestampColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithoutTimestampColumn",
              "TableWithoutTimestampColumn",
              _storageProviderDefinition,
              typeof (ClassWithoutTimestampColumn),
              false,
              baseClass);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithValidRelationsDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithValidRelations",
          "TableWithValidRelations",
          _storageProviderDefinition,
          typeof (ClassWithValidRelations),
          false,
          baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithValidRelations), "ClassWithGuidKeyOptional",
              "TableWithGuidKeyOptionalID",
              typeof (ObjectID),
              true));

      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithValidRelations), "ClassWithGuidKeyNonOptional",
              "TableWithGuidKeyNonOptionalID",
              typeof (ObjectID),
              true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithInvalidRelationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithInvalidRelation",
          "TableWithInvalidRelation",
          _storageProviderDefinition,
          typeof (ClassWithInvalidRelation),
          false,
          baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithInvalidRelation), "ClassWithGuidKey",
              "TableWithGuidKeyID",
              typeof (ObjectID),
              true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutRelatedClassIDColumnDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "ClassWithoutRelatedClassIDColumn",
          "TableWithoutRelatedClassIDColumn",
          _storageProviderDefinition,
          typeof (ClassWithoutRelatedClassIDColumn),
          false,
          baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithoutRelatedClassIDColumn), "Distributor",
              "DistributorID",
              typeof (ObjectID),
              true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithoutRelatedClassIDColumnAndDerivationDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithOptionalOneToOneRelationAndOppositeDerivedClass",
              "TableWithOptionalOneToOneRelationAndOppositeDerivedClass",
              _storageProviderDefinition,
              typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass),
              false,
              baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass), "Company",
              "CompanyID",
              typeof (ObjectID),
              true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateClassWithOptionalOneToOneRelationAndOppositeDerivedClassDefinition (
        ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithoutRelatedClassIDColumnAndDerivation",
              "TableWithoutRelatedClassIDColumnAndDerivation",
              _storageProviderDefinition,
              typeof (ClassWithoutRelatedClassIDColumnAndDerivation),
              false,
              baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithoutRelatedClassIDColumnAndDerivation), "Company",
              "CompanyID",
              typeof (ObjectID),
              true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateIndustrialSectorDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition industrialSector = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "IndustrialSector", "IndustrialSector", _storageProviderDefinition, typeof (IndustrialSector), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              industrialSector, typeof (IndustrialSector), "Name", "Name", typeof (string), false, 100));
      industrialSector.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return industrialSector;
    }

    private ReflectionBasedClassDefinition CreateEmployeeDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition employee = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Employee", "Employee", _storageProviderDefinition, typeof (Employee), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              employee, typeof (Employee), "Name", "Name", typeof (string), false, 100));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              employee, typeof (Employee), "Supervisor", "SupervisorID", typeof (ObjectID), true));
      employee.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return employee;
    }

    private ReflectionBasedClassDefinition CreateTargetClassForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition targetClassForPersistentMixin = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "TargetClassForPersistentMixin",
          "MixedDomains_Target",
          _storageProviderDefinition,
          typeof (TargetClassForPersistentMixin),
          false,
          baseClass,
          typeof (MixinAddingPersistentProperties));

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "PersistentProperty",
              "PersistentProperty",
              typeof (int)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "ExtraPersistentProperty",
              "ExtraPersistentProperty",
              typeof (int)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "UnidirectionalRelationProperty",
              "UnidirectionalRelationPropertyID",
              typeof (ObjectID)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "RelationProperty",
              "RelationPropertyID",
              typeof (ObjectID)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (MixinAddingPersistentProperties), "CollectionPropertyNSide",
              "CollectionPropertyNSideID",
              typeof (ObjectID)));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              targetClassForPersistentMixin,
              typeof (BaseForMixinAddingPersistentProperties), "PrivateBaseRelationProperty",
              "PrivateBaseRelationPropertyID",
              typeof (ObjectID)));
      targetClassForPersistentMixin.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return targetClassForPersistentMixin;
    }

    private ReflectionBasedClassDefinition CreateDerivedTargetClassForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition derivedTargetClassForPersistentMixin = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DerivedTargetClassForPersistentMixin",
          null,
          _storageProviderDefinition,
          typeof (DerivedTargetClassForPersistentMixin),
          false,
          baseClass);
      derivedTargetClassForPersistentMixin.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return derivedTargetClassForPersistentMixin;
    }

    private ReflectionBasedClassDefinition CreateDerivedDerivedTargetClassForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition derivedDerivedTargetClassForPersistentMixin = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "DerivedDerivedTargetClassForPersistentMixin",
          null,
          _storageProviderDefinition,
          typeof (DerivedDerivedTargetClassForPersistentMixin),
          false,
          baseClass);
      derivedDerivedTargetClassForPersistentMixin.SetPropertyDefinitions (new PropertyDefinitionCollection());

      return derivedDerivedTargetClassForPersistentMixin;
    }

    private ReflectionBasedClassDefinition CreateComputerDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition computer = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Computer", "Computer", _storageProviderDefinition, typeof (Computer), false, baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer, typeof (Computer), "SerialNumber", "SerialNumber", typeof (string), false, 20));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer, typeof (Computer), "Employee", "EmployeeID", typeof (ObjectID), true));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer,
              typeof (Computer), "Int32TransactionProperty",
              null,
              typeof (int),
              null,
              null,
              StorageClass.Transaction));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer,
              typeof (Computer), "DateTimeTransactionProperty",
              null,
              typeof (DateTime),
              null,
              null,
              StorageClass.Transaction));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              computer,
              typeof (Computer), "EmployeeTransactionProperty",
              null,
              typeof (ObjectID),
              true,
              null,
              StorageClass.Transaction));
      computer.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return computer;
    }

    private ReflectionBasedClassDefinition CreateClassWithRelatedClassIDColumnAndNoInheritanceDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition classDefinition =
          ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
              "ClassWithRelatedClassIDColumnAndNoInheritance",
              "TableWithRelatedClassIDColumnAndNoInheritance",
              _storageProviderDefinition,
              typeof (ClassWithRelatedClassIDColumnAndNoInheritance),
              false,
              baseClass);

      var properties = new List<PropertyDefinition> ();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              classDefinition,
              typeof (ClassWithRelatedClassIDColumnAndNoInheritance), "ClassWithGuidKey",
              "TableWithGuidKeyID",
              typeof (ObjectID),
              true));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return classDefinition;
    }

    private ReflectionBasedClassDefinition CreateRelationTargetForPersistentMixinDefinition (ReflectionBasedClassDefinition baseClass)
    {
      ReflectionBasedClassDefinition relationTargetForPersistentMixinDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "RelationTargetForPersistentMixin",
          "MixedDomains_RelationTarget",
          _storageProviderDefinition,
          typeof (RelationTargetForPersistentMixin),
          false,
          baseClass);

      var properties = new List<PropertyDefinition>();
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              relationTargetForPersistentMixinDefinition,
              typeof (RelationTargetForPersistentMixin), "RelationProperty2",
              "RelationProperty2ID",
              typeof (ObjectID),
              true));
      properties.Add (
          ReflectionBasedPropertyDefinitionFactory.Create (
              relationTargetForPersistentMixinDefinition,
              typeof (RelationTargetForPersistentMixin), "RelationProperty3",
              "RelationProperty3ID",
              typeof (ObjectID),
              true));
      relationTargetForPersistentMixinDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (properties, true));

      return relationTargetForPersistentMixinDefinition;
    }

    #endregion

    #region Methods for creating relation definitions

    private RelationDefinitionCollection CreateRelationDefinitions ()
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
      relationDefinitions.Add (CreateEmployeeToComputerRelationDefinition ());

      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedUnidirectionalRelationDefinition ());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedRelationPropertyRelationDefinition ());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedVirtualRelationPropertyRelationDefinition());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedCollectionProperty1SideCreateTargetClassForPersistentMixinMixedCollectionPropertyRelationDefinition());
      relationDefinitions.Add (CreateTargetClassForPersistentMixinMixedCollectionPropertyNSideRelationDefinition());

      var relationsByClass = from relationDefinition in relationDefinitions.Cast<RelationDefinition>()
                             from endPoint in relationDefinition.EndPointDefinitions
                             where !endPoint.IsAnonymous
                             group relationDefinition by endPoint.ClassDefinition
                               into grouping
                               select new { Key = grouping.Key, Values = grouping.Distinct () };

      var classesWithRelationDefinitions = new HashSet<ClassDefinition>();
      foreach (var classWithRelations in relationsByClass)
      {
        classWithRelations.Key.SetRelationDefinitions (new RelationDefinitionCollection (classWithRelations.Values, true));
        classesWithRelationDefinitions.Add (classWithRelations.Key);
      }

      foreach (var classDefinition in _classDefinitions.Cast<ClassDefinition>().Where(cd=>!classesWithRelationDefinitions.Contains(cd)))
        classDefinition.SetRelationDefinitions (new RelationDefinitionCollection());
      
      return relationDefinitions;
    }

    private RelationDefinition CreateCustomerToOrderRelationDefinition ()
    {
      ClassDefinition customer = _classDefinitions[typeof (Customer)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              customer,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders",
              false,
              CardinalityType.Many,
              typeof (OrderCollection),
              "OrderNumber asc");

      ClassDefinition orderClass = _classDefinitions[typeof (Order)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order"
        + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Customer->"
        +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Customer.Orders", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderTicketRelationDefinition ()
    {
      ClassDefinition orderClass = _classDefinitions[typeof (Order)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              orderClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket", true, CardinalityType.One, typeof (OrderTicket));

      ClassDefinition orderTicketClass = _classDefinitions[typeof (OrderTicket)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderTicketClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order", true);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket"
          + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order->"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderTicket", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateOrderToOrderItemRelationDefinition ()
    {
      ClassDefinition orderClass = _classDefinitions[typeof (Order)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              orderClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems",
              true,
              CardinalityType.Many,
              typeof (ObjectList<OrderItem>));

      ClassDefinition orderItemClass = _classDefinitions[typeof (OrderItem)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderItemClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem"
        + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem.Order->"
        +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderItems", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateOrderToOfficialRelationDefinition ()
    {
      ClassDefinition officialClass = _classDefinitions[typeof (Official)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              officialClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Official.Orders",
              false,
              CardinalityType.Many,
              typeof (ObjectList<Order>));

      ClassDefinition orderClass = _classDefinitions[typeof (Order)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          orderClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Official", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order"
        + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.Official->"
        +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Official.Orders", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateCompanyToCeoRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              companyClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo", true, CardinalityType.One, typeof (Ceo));

      ClassDefinition ceoClass = _classDefinitions[typeof (Ceo)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          ceoClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Ceo.Company", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Ceo"
        + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Ceo.Company->"
        +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.Ceo", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreatePartnerToPersonRelationDefinition ()
    {
      ClassDefinition partnerClass = _classDefinitions[typeof (Partner)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          partnerClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson", true);

      ClassDefinition personClass = _classDefinitions[typeof (Person)];
      VirtualRelationEndPointDefinition endPoint2 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              personClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany",
              false,
              CardinalityType.One,
              typeof (Partner));
      
      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner"
            + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Partner.ContactPerson->"
            +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Person.AssociatedPartnerCompany", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateParentClientToChildClientRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);
      RelationEndPointDefinition endPoint2 =
          new RelationEndPointDefinition (clientClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient", false);
      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client"
            +":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Client.ParentClient", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateClientToLocationRelationDefinition ()
    {
      ClassDefinition clientClass = _classDefinitions[typeof (Client)];
      ClassDefinition locationClass = _classDefinitions[typeof (Location)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (clientClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          locationClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client", true);

      RelationDefinition relation = new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location"
        +":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Location.Client", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateFolderToFileSystemItemRelationDefinition ()
    {
      ClassDefinition fileSystemItemClass = _classDefinitions[typeof (FileSystemItem)];
      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          fileSystemItemClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder", false);

      ClassDefinition folderClass = _classDefinitions[typeof (Folder)];
      VirtualRelationEndPointDefinition endPoint2 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              folderClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems",
              false,
              CardinalityType.Many,
              typeof (ObjectList<FileSystemItem>));

      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem"
            + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.FileSystemItem.ParentFolder->"
            +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Folder.FileSystemItems", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateDistributorToClassWithoutRelatedClassIDColumnRelationDefinition ()
    {
      ClassDefinition distributorClass = _classDefinitions[typeof (Distributor)];

      ClassDefinition classWithoutRelatedClassIDColumnClass = _classDefinitions[typeof (ClassWithoutRelatedClassIDColumn)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              distributorClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn",
              false,
              CardinalityType.One,
              typeof (ClassWithoutRelatedClassIDColumn));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumn.Distributor",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumn"
              + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumn.Distributor->"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Distributor.ClassWithoutRelatedClassIDColumn", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithoutRelatedClassIDColumnAndDerivationRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      ClassDefinition classWithoutRelatedClassIDColumnAndDerivation = _classDefinitions[typeof (ClassWithoutRelatedClassIDColumnAndDerivation)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              companyClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.ClassWithoutRelatedClassIDColumnAndDerivation",
              false,
              CardinalityType.One,
              typeof (ClassWithoutRelatedClassIDColumnAndDerivation));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithoutRelatedClassIDColumnAndDerivation,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumnAndDerivation.Company",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumnAndDerivation:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithoutRelatedClassIDColumnAndDerivation.Company->"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.ClassWithoutRelatedClassIDColumnAndDerivation", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateCompanyToClassWithOptionalOneToOneRelationAndOppositeDerivedClassRelationDefinition ()
    {
      ClassDefinition companyClass = _classDefinitions[typeof (Company)];
      ClassDefinition classWithOptionalOneToOneRelationAndOppositeDerivedClass =
          _classDefinitions[typeof (ClassWithOptionalOneToOneRelationAndOppositeDerivedClass)];

      AnonymousRelationEndPointDefinition endPoint1 = new AnonymousRelationEndPointDefinition (companyClass);

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithOptionalOneToOneRelationAndOppositeDerivedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass"
              +":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithOptionalOneToOneRelationAndOppositeDerivedClass.Company",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsOptional ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsOptional",
              false,
              CardinalityType.One,
              typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions[typeof (ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyOptional", false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations"
              + ":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyOptional->"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsOptional",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithValidRelationsNonOptional ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsNonOptional",
              true,
              CardinalityType.One,
              typeof (ClassWithValidRelations));

      ClassDefinition classWithValidRelations = _classDefinitions[typeof (ClassWithValidRelations)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithValidRelations, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyNonOptional", true);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations:"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithValidRelations.ClassWithGuidKeyNonOptional->"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithValidRelationsNonOptional",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithInvalidRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithInvalidRelation",
              false,
              CardinalityType.One,
              typeof (ClassWithInvalidRelation));

      ClassDefinition classWithInvalidRelation = _classDefinitions[typeof (ClassWithInvalidRelation)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithInvalidRelation, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithInvalidRelation.ClassWithGuidKey", false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithInvalidRelation:"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithInvalidRelation.ClassWithGuidKey->"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithInvalidRelation",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateClassWithGuidKeyToClassWithRelatedClassIDColumnAndNoInheritanceRelation ()
    {
      ClassDefinition classWithGuidKey = _classDefinitions[typeof (ClassWithGuidKey)];
      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              classWithGuidKey,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithRelatedClassIDColumnAndNoInheritance",
              false,
              CardinalityType.One,
              typeof (ClassWithRelatedClassIDColumnAndNoInheritance));

      ClassDefinition classWithRelatedClassIDColumnAndNoInheritance = _classDefinitions[typeof (ClassWithRelatedClassIDColumnAndNoInheritance)];
      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          classWithRelatedClassIDColumnAndNoInheritance,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey",
          false);

      RelationDefinition relation =
          new RelationDefinition (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance:"
              +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithRelatedClassIDColumnAndNoInheritance.ClassWithGuidKey->"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ClassWithGuidKey.ClassWithRelatedClassIDColumnAndNoInheritance",
              endPoint1,
              endPoint2);

      return relation;
    }

    private RelationDefinition CreateIndustrialSectorToCompanyRelationDefinition ()
    {
      ClassDefinition industrialSectorClass = _classDefinitions[typeof (IndustrialSector)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              industrialSectorClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.IndustrialSector.Companies",
              true,
              CardinalityType.Many,
              typeof (ObjectList<Company>));

      ClassDefinition companyClass = _classDefinitions[typeof (Company)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          companyClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.IndustrialSector", false);

      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company"
            +":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Company.IndustrialSector->"
            + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.IndustrialSector.Companies", 
            endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateSupervisorToSubordinateRelationDefinition ()
    {
      ClassDefinition employeeClass = _classDefinitions[typeof (Employee)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              employeeClass,
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Subordinates",
              false,
              CardinalityType.Many,
              typeof (ObjectList<Employee>));

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          employeeClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Supervisor", false);

      RelationDefinition relation =
          new RelationDefinition ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee:"
            +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Supervisor->"
            +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Subordinates", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateEmployeeToComputerRelationDefinition ()
    {
      ClassDefinition employeeClass = _classDefinitions[typeof (Employee)];

      VirtualRelationEndPointDefinition endPoint1 =
          ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
              employeeClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Computer", false, CardinalityType.One, typeof (Computer));

      ClassDefinition computerClass = _classDefinitions[typeof (Computer)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          computerClass, "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Computer.Employee", false);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Computer:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Computer.Employee->"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Employee.Computer", endPoint1, endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedUnidirectionalRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.UnidirectionalRelationProperty",
          false);

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      AnonymousRelationEndPointDefinition endPoint2 = new AnonymousRelationEndPointDefinition (relatedClass);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin:"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.UnidirectionalRelationProperty", 
          endPoint1, 
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedRelationPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.RelationProperty",
          false);

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      var endPoint2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty1",
          false,
          CardinalityType.One,
          typeof (TargetClassForPersistentMixin));

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin:"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.RelationProperty->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty1",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedVirtualRelationPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      var endPoint1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.VirtualRelationProperty",
          false,
          CardinalityType.One,
          typeof (RelationTargetForPersistentMixin));

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty2",
          false);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin"
          +":Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty2->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.VirtualRelationProperty",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedCollectionProperty1SideCreateTargetClassForPersistentMixinMixedCollectionPropertyRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      var endPoint1 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionProperty1Side",
          false,
          CardinalityType.Many,
          typeof (ObjectList<RelationTargetForPersistentMixin>));

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      RelationEndPointDefinition endPoint2 = new RelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty3",
          false);

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin:"
          +"Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty3->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionProperty1Side",
          endPoint1,
          endPoint2);

      return relation;
    }

    private RelationDefinition CreateTargetClassForPersistentMixinMixedCollectionPropertyNSideRelationDefinition ()
    {
      ClassDefinition mixedClass = _classDefinitions[typeof (TargetClassForPersistentMixin)];

      RelationEndPointDefinition endPoint1 = new RelationEndPointDefinition (
          mixedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionPropertyNSide",
          false);

      ClassDefinition relatedClass = _classDefinitions[typeof (RelationTargetForPersistentMixin)];

      var endPoint2 = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          relatedClass,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty4",
          false,
          CardinalityType.Many,
          typeof (ObjectList<TargetClassForPersistentMixin>));

      RelationDefinition relation = new RelationDefinition (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.TargetClassForPersistentMixin:"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.MixinAddingPersistentProperties.CollectionPropertyNSide->"
          + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.MixedMapping.RelationTargetForPersistentMixin.RelationProperty4",
          endPoint1,
          endPoint2);

      return relation;
    }

    #endregion
  }
}