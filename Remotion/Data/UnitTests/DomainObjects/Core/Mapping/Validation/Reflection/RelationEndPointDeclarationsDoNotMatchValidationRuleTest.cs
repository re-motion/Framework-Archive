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
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.RelationEndPointDeclarationsDoNotMatchValidationRule;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class RelationEndPointDeclarationsDoNotMatchValidationRuleTest : ValidationRuleTestBase
  {
    private RelationEndPointDeclarationsDoNotMatchValidationRule _validationRule;
    private ReflectionBasedClassDefinition _classDefinition1;
    private ReflectionBasedClassDefinition _classDefinition2;
    
    [SetUp]
    public void SetUp ()
    {
      _validationRule = new RelationEndPointDeclarationsDoNotMatchValidationRule();
      _classDefinition1 = new ReflectionBasedClassDefinition (
          "ID",
          "EntityName",
          "SPID",
          typeof (RelationEndPointPropertyClass1),
          false,
          null,
          new PersistentMixinFinderMock (typeof (RelationEndPointPropertyClass1), new Type[0]));
      _classDefinition2 = new ReflectionBasedClassDefinition (
          "ID",
          "EntityName",
          "SPID",
          typeof (RelationEndPointPropertyClass2),
          false,
          null,
          new PersistentMixinFinderMock (typeof (RelationEndPointPropertyClass2), new Type[0]));
    }

    [Test]
    public void ValidRelation ()
    {
      var endPointDefinition1 = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition1,
          "RelationProperty2",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass1).GetProperty ("RelationProperty2"));
      var endPointDefinition2 = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition2,
          "RelationProperty2",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass2).GetProperty ("RelationProperty2"));

      var relationDefinition = new RelationDefinition ("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate (relationDefinition);
      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void OppositeRelationPropertyHasNoBidirectionalRelationAttributeDefined ()
    {
      var endPointDefinition1 = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition1,
          "RelationProperty1",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass1).GetProperty ("RelationProperty1"));
      var endPointDefinition2 = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition2,
          "RelationPopertyWithoutBidirectionalRelationAttribute",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass2).GetProperty ("RelationPopertyWithoutBidirectionalRelationAttribute"));

      var relationDefinition = new RelationDefinition ("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage = "Opposite relation property 'RelationPopertyWithoutBidirectionalRelationAttribute' declared on type "
                            + "'RelationEndPointPropertyClass2' does not define a matching 'DBBidirectionalRelationAttribute'.\r\n\r\n"
                            +"Declaration type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
                            +"RelationEndPointDeclarationsDoNotMatchValidationRule.RelationEndPointPropertyClass1'";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }

    [Test]
    public void OppositeRelationPropertyNameDoesNotMatch ()
    {
      var endPointDefinition1 = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition1,
          "RelationProperty3",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass1).GetProperty ("RelationProperty3"));
      var endPointDefinition2 = new ReflectionBasedVirtualRelationEndPointDefinition (
          _classDefinition2,
          "RelationPopertyWithNonMatchingPropertyName",
          false,
          CardinalityType.One,
          typeof (string),
          null,
          typeof (RelationEndPointPropertyClass2).GetProperty ("RelationPopertyWithNonMatchingPropertyName"));

      var relationDefinition = new RelationDefinition ("Test", endPointDefinition1, endPointDefinition2);

      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage = "Opposite relation property 'RelationPopertyWithNonMatchingPropertyName' declared on type 'RelationEndPointPropertyClass2' "
        +"defines a 'DBBidirectionalRelationAttribute' whose opposite property does not match.\r\n\r\n"
        +"Declaration type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"RelationEndPointDeclarationsDoNotMatchValidationRule.RelationEndPointPropertyClass1'";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  }
}