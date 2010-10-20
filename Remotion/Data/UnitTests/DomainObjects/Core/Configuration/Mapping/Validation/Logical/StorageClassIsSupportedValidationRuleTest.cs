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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Logical
{
  [TestFixture]
  public class StorageClassIsSupportedValidationRuleTest : ValidationRuleTestBase
  {
    private StorageClassIsSupportedValidationRule _validationRule;
    private Type _type;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new StorageClassIsSupportedValidationRule();

      _type = typeof (DerivedValidationDomainObjectClass);
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (_type.Name, _type.Name, "SPID", _type, false);
    }

    [Test]
    public void PropertyWithoutStorageClassAttribute ()
    {
      var propertyInfo = _type.GetProperty ("Property");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.None);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassPersistent ()
    {
      var propertyInfo = _type.GetProperty ("PropertyWithStorageClassPersistent");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Persistent);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassTransaction ()
    {
      var propertyInfo = _type.GetProperty ("PropertyWithStorageClassTransaction");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.Transaction);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void PropertyWithStorageClassNone ()
    {
      var propertyInfo = _type.GetProperty ("PropertyWithStorageClassNone");
      var propertyDefinition = new TestablePropertyDefinition (_classDefinition, propertyInfo, 20, StorageClass.None);
      _classDefinition.MyPropertyDefinitions.Add (propertyDefinition);
      _classDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_classDefinition);

      var expectedMessage = "Only StorageClass.Persistent and StorageClass.Transaction is supported.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

  }
}