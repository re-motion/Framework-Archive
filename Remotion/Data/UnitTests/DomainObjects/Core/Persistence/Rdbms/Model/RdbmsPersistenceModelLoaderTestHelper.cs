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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model.RdbmsPersistenceModelLoaderTestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.Model
{
  // Test Domain:
  //
  //                 BaseBase
  //                     |
  //                   Base
  //                 /      \
  //            Table1       Table2
  //                         /    \
  //                   Derived1  Derived2
  //                                |
  //                          DerivedDerived
  //                                |
  //                       DerivedDerivedDerived
  //
  // All Base classes are persisted as UnionViewDefinitions, all Tables as TableDefinitions, all Derived as FilterViewDefinitions.
  
  public class RdbmsPersistenceModelLoaderTestHelper
  {
    private readonly ClassDefinition _baseBaseClassDefinition;
    private readonly ClassDefinition _baseClassDefinition;
    private readonly ClassDefinition _tableClassDefinition1;
    private readonly ClassDefinition _tableClassDefinition2;
    private readonly ClassDefinition _derivedClassDefinition1;
    private readonly ClassDefinition _derivedClassDefinition2;
    private readonly ClassDefinition _derivedDerivedClassDefinition;
    private readonly ClassDefinition _derivedDerivedDerivedClassDefinition;
    private readonly PropertyDefinition _baseBasePropertyDefinition;
    private readonly PropertyDefinition _basePropertyDefinition;
    private readonly PropertyDefinition _tablePropertyDefinition1;
    private readonly PropertyDefinition _tablePropertyDefinition2;
    private readonly PropertyDefinition _derivedPropertyDefinition1;
    private readonly PropertyDefinition _derivedPropertyDefinition2;
    private readonly PropertyDefinition _derivedDerivedPropertyDefinition;

    public RdbmsPersistenceModelLoaderTestHelper ()
    {
      _baseBaseClassDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (BaseBaseClass), null);
      _baseClassDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (BaseClass), _baseBaseClassDefinition);
      _tableClassDefinition1 = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (Table1Class), _baseClassDefinition);
      _tableClassDefinition2 = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (Table2Class), _baseClassDefinition);
      _derivedClassDefinition1 = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (Derived1Class), _tableClassDefinition2);
      _derivedClassDefinition2 = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (Derived2Class), _tableClassDefinition2);
      _derivedDerivedClassDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (DerivedDerivedClass), _derivedClassDefinition2);
      _derivedDerivedDerivedClassDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (
          typeof (DerivedDerivedDerivedClass), _derivedDerivedClassDefinition);

      _baseBaseClassDefinition.SetDerivedClasses (new[] { _baseClassDefinition });
      _baseClassDefinition.SetDerivedClasses (new[] { _tableClassDefinition1, _tableClassDefinition2 });
      _tableClassDefinition2.SetDerivedClasses (new[] { _derivedClassDefinition1, _derivedClassDefinition2 });
      _derivedClassDefinition2.SetDerivedClasses (new[] { _derivedDerivedClassDefinition });
      _tableClassDefinition1.SetDerivedClasses (new ClassDefinition[0]);
      _derivedClassDefinition1.SetDerivedClasses (new ClassDefinition[0]);
      _derivedDerivedClassDefinition.SetDerivedClasses (new[] { _derivedDerivedDerivedClassDefinition });
      _derivedDerivedDerivedClassDefinition.SetDerivedClasses (new ClassDefinition[0]);

      _baseBaseClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      _baseClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _tableClassDefinition1.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _tableClassDefinition2.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedClassDefinition1.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedClassDefinition2.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedDerivedClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      _derivedDerivedDerivedClassDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection ());
      
      _baseBasePropertyDefinition = CreateAndAddPropertyDefinition (
          _baseBaseClassDefinition, "BaseBaseProperty", typeof (BaseBaseClass).GetProperty ("BaseBaseProperty"));
      _basePropertyDefinition = CreateAndAddPropertyDefinition (_baseClassDefinition, "BaseProperty", typeof (BaseClass).GetProperty ("BaseProperty"));
      _tablePropertyDefinition1 = CreateAndAddPropertyDefinition (
          _tableClassDefinition1, "TableProperty1", typeof (Table1Class).GetProperty ("TableProperty1"));
      _tablePropertyDefinition2 = CreateAndAddPropertyDefinition (
          _tableClassDefinition2, "TableProperty2", typeof (Table2Class).GetProperty ("TableProperty2"));
      _derivedPropertyDefinition1 = CreateAndAddPropertyDefinition (
          _derivedClassDefinition1, "DerivedProperty1", typeof (Derived1Class).GetProperty ("DerivedProperty1"));
      _derivedPropertyDefinition2 = CreateAndAddPropertyDefinition (
          _derivedClassDefinition2, "DerivedProperty2", typeof (Derived2Class).GetProperty ("DerivedProperty2"));
      _derivedDerivedPropertyDefinition = CreateAndAddPropertyDefinition (
          _derivedDerivedClassDefinition, "DerivedDerivedProperty", typeof (DerivedDerivedClass).GetProperty ("DerivedDerivedProperty"));
      _derivedDerivedDerivedClassDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection());
    }

    public ClassDefinition BaseBaseClassDefinition
    {
      get { return _baseBaseClassDefinition; }
    }

    public ClassDefinition BaseClassDefinition
    {
      get { return _baseClassDefinition; }
    }

    public ClassDefinition TableClassDefinition1
    {
      get { return _tableClassDefinition1; }
    }

    public ClassDefinition TableClassDefinition2
    {
      get { return _tableClassDefinition2; }
    }

    public ClassDefinition DerivedClassDefinition1
    {
      get { return _derivedClassDefinition1; }
    }

    public ClassDefinition DerivedClassDefinition2
    {
      get { return _derivedClassDefinition2; }
    }

    public ClassDefinition DerivedDerivedClassDefinition
    {
      get { return _derivedDerivedClassDefinition; }
    }

    public ClassDefinition DerivedDerivedDerivedClassDefinition
    {
      get { return _derivedDerivedDerivedClassDefinition; }
    }

    public PropertyDefinition BaseBasePropertyDefinition
    {
      get { return _baseBasePropertyDefinition; }
    }

    public PropertyDefinition BasePropertyDefinition
    {
      get { return _basePropertyDefinition; }
    }

    public PropertyDefinition TablePropertyDefinition1
    {
      get { return _tablePropertyDefinition1; }
    }

    public PropertyDefinition TablePropertyDefinition2
    {
      get { return _tablePropertyDefinition2; }
    }

    public PropertyDefinition DerivedPropertyDefinition1
    {
      get { return _derivedPropertyDefinition1; }
    }

    public PropertyDefinition DerivedPropertyDefinition2
    {
      get { return _derivedPropertyDefinition2; }
    }

    public PropertyDefinition DerivedDerivedPropertyDefinition
    {
      get { return _derivedDerivedPropertyDefinition; }
    }

    private PropertyDefinition CreateAndAddPropertyDefinition (
        ClassDefinition classDefinition, string propertyName, PropertyInfo propertyInfo)
    {
      var propertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition,
          propertyName,
          typeof (string),
          true,
          null,
          StorageClass.Persistent,
          propertyInfo,
          null);

      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      return propertyDefinition;
    }
  }
}