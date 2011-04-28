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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.Validation.Logical;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Logical
{
  [TestFixture]
  public class SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRuleTest : ValidationRuleTestBase
  {
    private SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule _validationRule;
    private ClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new SortExpressionIsSupportedForCardianlityOfRelationPropertyValidationRule();
      _classDefinition = FakeMappingConfiguration.Current.TypeDefinitions[typeof (Order)];
    }

    [Test]
    public void NoVirtualRelationEndPointDefinition ()
    {
      var endPointDefinition = new AnonymousRelationEndPointDefinition (_classDefinition);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);

      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void CardinalityIsMany ()
    {
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
          _classDefinition, "Property", false, CardinalityType.Many, typeof (DomainObjectCollection), null);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void CardinalityIsOne_And_EndPointDefinitionHasNoSortExpression ()
    {
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
        _classDefinition, "Property", false, CardinalityType.One, typeof (DerivedValidationDomainObjectClass), null);
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void CardinalityOne_And_EndPointDefinitionsHaveSortExpression ()
    {
      var endPointDefinition = VirtualRelationEndPointDefinitionFactory.CreateVirtualRelationEndPointDefinition (
        _classDefinition, "Property", false, CardinalityType.One, typeof (DerivedValidationDomainObjectClass), null);
      PrivateInvoke.SetNonPublicField (endPointDefinition, "_sortExpressionText", "SortExpression1");
      var relationDefinition = new RelationDefinition ("Test", endPointDefinition, endPointDefinition);
      
      var validationResult = _validationRule.Validate (relationDefinition);

      var expectedMessage = "Property 'OrderNumber' of class 'Order' must not specify a SortExpression, because cardinality is equal to 'one'.\r\n\r\n"
        + "Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order\r\nProperty: OrderNumber";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
    
  }
}