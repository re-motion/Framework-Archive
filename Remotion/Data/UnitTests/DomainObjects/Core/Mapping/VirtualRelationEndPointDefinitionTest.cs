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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class VirtualRelationEndPointDefinitionTest : MappingReflectionTestBase
  {
    private ClassDefinition _customerClassDefinition;
    private VirtualRelationEndPointDefinition _customerOrdersEndPoint;

    private ClassDefinition _orderClassDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _customerClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Customer",
          "Customer",
          "TestDomain",
          typeof (Customer),
          false);
      _customerOrdersEndPoint = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _customerClassDefinition,
          "Orders",
          false,
          CardinalityType.Many,
          typeof (OrderCollection),
          "OrderNumber desc");

      _orderClassDefinition = ClassDefinitionFactory.CreateOrderDefinition ();
    }

    [Test]
    public void InitializeWithPropertyType ()
    {
      var endPoint = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition(
          _orderClassDefinition,
          "VirtualEndPoint",
          true,
          CardinalityType.One,
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem, Remotion.Data.UnitTests",
          null);

      Assert.IsTrue (endPoint.IsPropertyTypeResolved);
      Assert.AreSame (typeof (OrderItem), endPoint.PropertyType);
      Assert.AreEqual (typeof (OrderItem).AssemblyQualifiedName, endPoint.PropertyTypeName);
    }

    [Test]
    public void InitializeWithSortExpression ()
    {
      var endPointDefinition = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _customerClassDefinition,
          "Orders",
          false,
          CardinalityType.Many,
          typeof (OrderCollection),
          "OrderNumber desc");

      Assert.AreEqual ("OrderNumber desc", endPointDefinition.SortExpressionText);
    }

    [Test]
    public void IsAnonymous ()
    {
      Assert.IsFalse (_customerOrdersEndPoint.IsAnonymous);
    }

    [Test]
    public void RelationDefinition_Null ()
    {
      Assert.IsNull (_customerOrdersEndPoint.RelationDefinition);
    }

    [Test]
    public void RelationDefinition_NonNull ()
    {
      _customerOrdersEndPoint.SetRelationDefinition (new RelationDefinition ("Test", _customerOrdersEndPoint, _customerOrdersEndPoint));
      Assert.IsNotNull (_customerOrdersEndPoint.RelationDefinition);
    }

    [Test]
    public void GetSortExpression_Null ()
    {
      var endPoint = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _orderClassDefinition,
          "OrderItems",
          false,
          CardinalityType.Many,
          typeof (ObjectList<OrderItem>),
          null);
      Assert.That (endPoint.SortExpressionText, Is.Null);

      Assert.That (endPoint.GetSortExpression(), Is.Null);
    }

    [Test]
    public void GetSortExpression_NonNull ()
    {
      var endPoint = CreateFullVirtualEndPoint ("Product asc");
      
      Assert.That (endPoint.GetSortExpression(), Is.Not.Null);
      Assert.That (endPoint.GetSortExpression().ToString(), Is.EqualTo ("Product ASC"));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.Order.OrderNumber: SortExpression 'Product asc asc' cannot be "
        + "parsed: Expected one or two parts (a property name and an optional identifier), found 3 parts instead.")]
    public void GetSortExpression_Error ()
    {
      var endPoint = CreateFullVirtualEndPoint ("Product asc asc");
      Dev.Null = endPoint.GetSortExpression();
    }

    private ReflectionBasedVirtualRelationEndPointDefinition CreateFullVirtualEndPoint (string sortExpressionString)
    {
      var endPoint = ReflectionBasedVirtualRelationEndPointDefinitionFactory.CreateReflectionBasedVirtualRelationEndPointDefinition (
          _orderClassDefinition,
          "OrderItems",
          false,
          CardinalityType.Many,
          typeof (ObjectList<OrderItem>),
          sortExpressionString);
      var orderItemClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (OrderItem));
      var oppositeProperty = ReflectionBasedPropertyDefinitionFactory.Create (orderItemClassDefinition, "Order", typeof (ObjectID));
      orderItemClassDefinition.MyPropertyDefinitions.Add (oppositeProperty);
      var productProperty = ReflectionBasedPropertyDefinitionFactory.Create (orderItemClassDefinition, "Product", typeof (string));
      orderItemClassDefinition.MyPropertyDefinitions.Add (productProperty);
      var oppositeEndPoint = new RelationEndPointDefinition (orderItemClassDefinition, "Order", false);
      var relationDefinition = new RelationDefinition ("test", endPoint, oppositeEndPoint);
      orderItemClassDefinition.SetReadOnly ();
      Assert.That (endPoint.RelationDefinition, Is.SameAs (relationDefinition));
      return endPoint;
    }
  }
}
