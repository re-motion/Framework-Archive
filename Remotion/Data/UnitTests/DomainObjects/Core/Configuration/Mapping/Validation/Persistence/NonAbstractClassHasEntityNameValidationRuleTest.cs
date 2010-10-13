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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation;
using Remotion.Data.DomainObjects.Mapping.Configuration.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.TestDomain.Validation.Persistence.NonAbstractClassHasEntityNameValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping.Validation.Persistence
{
  [TestFixture]
  public class NonAbstractClassHasEntityNameValidationRuleTest
  {
    private NonAbstractClassHasEntityNameValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new NonAbstractClassHasEntityNameValidationRule();
    }

    [Test]
    public void ClassTypeResolved_NoEntityName_Abstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameClass",
          null,
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (NonAbstractClassHasEntityNameClass),
          true);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_EntityName_NotAbstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameClass",
          "EntityName",
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (NonAbstractClassHasEntityNameClass),
          false);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_EntityName_Abstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameClass",
          "EntityName",
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (NonAbstractClassHasEntityNameClass),
          false);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void ClassTypeResolved_NoEntityName_NotAbstract ()
    {
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "NonAbstractClassHasEntityNameClass",
          null,
          "NonAbstractClassHasEntityNameStorageProviderID",
          typeof (NonAbstractClassHasEntityNameClass),
          false);

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage = string.Format (
          "Neither class 'NonAbstractClassHasEntityNameClass' nor its base classes specify an entity name. Make "
          + "class '{0}' abstract or apply a DBTable attribute to it or one of its base classes.",
          typeof (NonAbstractClassHasEntityNameClass).AssemblyQualifiedName);
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    private void AssertMappingValidationResult (MappingValidationResult validationResult, bool expectedIsValid, string expectedMessage)
    {
      Assert.That (validationResult.IsValid, Is.EqualTo (expectedIsValid));
      Assert.That (validationResult.Message, Is.EqualTo (expectedMessage));
    }
  }
}