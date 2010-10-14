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
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Logical;
using
    Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Logical.
        PropertyNamesAreUniqueWithinInheritanceTreeValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Logical
{
  [TestFixture]
  public class PropertyNamesAreUniqueWithinInheritanceTreeValidationRuleTest : ValidationRuleTestBase
  {
    private PropertyNamesAreUniqueWithinInheritanceTreeValidationRule _validationRule;
    private ReflectionBasedClassDefinition _baseOfBaseClassDefinition;
    private ReflectionBasedClassDefinition _derivedBaseClassDefinition;
    private ReflectionBasedClassDefinition _derivedClassDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new PropertyNamesAreUniqueWithinInheritanceTreeValidationRule();
      _baseOfBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseOfBaseDomainObject",
          "BaseEntityName",
          "SPID",
          typeof (PropertyNamesAreUniqueWithinInheritanceTreeBaseOfBaseDomainObject),
          false);
      _derivedBaseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject",
          null,
          "SPID",
          typeof (PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject),
          false,
          _baseOfBaseClassDefinition,
          new PersistentMixinFinderMock (typeof (PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject), new Type[0]));
      _derivedClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject",
          null,
          "SPID",
          typeof (PropertyNamesAreUniqueWithinInheritanceTreeDomainObject),
          false,
          _derivedBaseClassDefinition,
          new PersistentMixinFinderMock (typeof (PropertyNamesAreUniqueWithinInheritanceTreeDomainObject), new Type[0]));
    }

    [Test]
    public void HasNoBaseClass ()
    {
      var validationResult = _validationRule.Validate (_baseOfBaseClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClass_And_HasNoPropertyDefintions ()
    {
      _baseOfBaseClassDefinition.SetReadOnly();
      _derivedBaseClassDefinition.SetReadOnly ();
      
      var validationResult = _validationRule.Validate (_derivedBaseClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClass_And_HasPropertyDefintions ()
    {
      _derivedBaseClassDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClassDefinition, "FirstName", "FirstName"));
      _derivedBaseClassDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClassDefinition, "LastName", "LastName"));
      
      _baseOfBaseClassDefinition.SetReadOnly ();
      _derivedBaseClassDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_derivedBaseClassDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void HasBaseClass_And_HasSamePropertyDefintionsInBaseClass ()
    {
      _derivedBaseClassDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedBaseClassDefinition, "Name", "Name"));
      _baseOfBaseClassDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_baseOfBaseClassDefinition, "Name", "Name"));

      _baseOfBaseClassDefinition.SetReadOnly ();
      _derivedBaseClassDefinition.SetReadOnly();

      var validationResult = _validationRule.Validate (_derivedBaseClassDefinition);

      var expectedMessage = "Class 'PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject' must not define property 'Name', "
        +"because base class 'PropertyNamesAreUniqueWithinInheritanceTreeBaseOfBaseDomainObject' already defines a property with the same name.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void HasBaseClass_And_HasSamePropertyDefintionsInBaseOfBaseClass ()
    {
      _derivedClassDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_derivedClassDefinition, "Name", "Name"));
      _baseOfBaseClassDefinition.MyPropertyDefinitions.Add (
          ReflectionBasedPropertyDefinitionFactory.CreateForFakePropertyInfo (_baseOfBaseClassDefinition, "Name", "Name"));

      _baseOfBaseClassDefinition.SetReadOnly ();
      _derivedBaseClassDefinition.SetReadOnly();
      _derivedClassDefinition.SetReadOnly ();

      var validationResult = _validationRule.Validate (_derivedClassDefinition);

      var expectedMessage = "Class 'PropertyNamesAreUniqueWithinInheritanceTreeBaseDomainObject' must not define property 'Name', "
        + "because base class 'PropertyNamesAreUniqueWithinInheritanceTreeBaseOfBaseDomainObject' already defines a property with the same name.";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}