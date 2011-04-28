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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class ReflectionBasedMappingObjectFactoryTest : StandardMappingTest
  {
    private ReflectionBasedMappingObjectFactory _factory;
    private ReflectionBasedNameResolver _mappingNameResolver;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp();
      _mappingNameResolver = new ReflectionBasedNameResolver();
      _factory = new ReflectionBasedMappingObjectFactory (_mappingNameResolver);
    }

    [Test]
    public void CreateClassDefinition ()
    {
      var result = _factory.CreateClassDefinition (typeof (Order), null);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs (typeof (Order)));
      Assert.That (result.BaseClass, Is.Null);
    }

    [Test]
    public void CreateClassDefinition_WithBaseClass ()
    {
      var companyClass = ClassDefinitionFactory.CreateClassDefinition (typeof (Company));
      companyClass.SetPropertyDefinitions (new PropertyDefinitionCollection());
      companyClass.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection());
      var result = _factory.CreateClassDefinition (typeof (Customer), companyClass);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.ClassType, Is.SameAs (typeof (Customer)));
      Assert.That (result.BaseClass, Is.SameAs (companyClass));
    }

    [Test]
    public void CreatePropertyDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyInfo = typeof (Order).GetProperty ("OrderItems");

      var result = _factory.CreatePropertyDefinition (classDefinition, propertyInfo);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.PropertyInfo, Is.SameAs (propertyInfo));
    }

    [Test]
    public void CreateRelationDefinition ()
    {
      var orderClassDefinition = MappingConfiguration.Current.ClassDefinitions["Order"];
      var orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions["OrderItem"];

      var result =
          _factory.CreateRelationDefinition (
              new[] { orderClassDefinition, orderItemClassDefinition }.ToDictionary (cd => cd.ClassType),
              orderItemClassDefinition,
              orderItemClassDefinition.MyRelationEndPointDefinitions[0].PropertyInfo);

      Assert.That (result, Is.Not.Null);
      Assert.That (result.EndPointDefinitions[0], 
        Is.SameAs (orderItemClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order"]));
      Assert.That (result.EndPointDefinitions[1], 
        Is.SameAs (orderClassDefinition.MyRelationEndPointDefinitions["Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems"]));
    }

    [Test]
    public void CreateRelationEndPointDefinition ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyInfo = typeof (Order).GetProperty ("OrderItems");

      var result = _factory.CreateRelationEndPointDefinition (classDefinition, propertyInfo);

      Assert.That (result, Is.TypeOf (typeof (VirtualRelationEndPointDefinition)));
      Assert.That (((VirtualRelationEndPointDefinition) result).PropertyInfo, Is.SameAs (propertyInfo));
    }

    [Test]
    public void CreateClassDefinitionCollection ()
    {
      var result = _factory.CreateClassDefinitionCollection (new[] { typeof (Order), typeof (Company) });

      Assert.That (result.Length, Is.EqualTo (2));
      Assert.That (result.Any (cd=>cd.ClassType == typeof (Order)));
      Assert.That (result.Any (cd=>cd.ClassType == typeof (Company)));
    }
    
    [Test]
    public void CreatePropertyDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (Order), null);
      var propertyInfo1 = typeof (Order).GetProperty ("OrderNumber");
      var propertyInfo2 = typeof (Order).GetProperty ("DeliveryDate");

      var result = _factory.CreatePropertyDefinitionCollection (classDefinition, new[] { propertyInfo1, propertyInfo2 });

      Assert.That (result.Count, Is.EqualTo (2));
      Assert.That (result[0].PropertyInfo, Is.SameAs (propertyInfo1));
      Assert.That (result[1].PropertyInfo, Is.SameAs (propertyInfo2));
    }

    [Test]
    public void CreateRelationDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (OrderItem));
      var propertyDefinition = PropertyDefinitionFactory.Create (
          classDefinition, typeof (OrderItem), "Order", "OrderID", typeof (ObjectID), true);
      var endPoint = new RelationEndPointDefinition (propertyDefinition, false);
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));
      classDefinition.SetRelationEndPointDefinitions (new RelationEndPointDefinitionCollection (new[] { endPoint }, true));

      var result = _factory.CreateRelationDefinitionCollection (new[] { classDefinition }.ToDictionary (cd => cd.ClassType));

      Assert.That (result.Count(), Is.EqualTo (1));
      Assert.That (
          result.First().ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderItem:Remotion.Data.UnitTests.DomainObjects.Core."
              + "Mapping.TestDomain.Integration.OrderItem.Order"));
    }

    [Test]
    public void CreateRelationEndPointDefinitionCollection ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinitionWithoutStorageEntity (typeof (OrderTicket), null);
      var propertyDefinition = PropertyDefinitionFactory.Create (classDefinition, typeof (OrderTicket), "Order", "OrderID", typeof (ObjectID));
      classDefinition.SetPropertyDefinitions (new PropertyDefinitionCollection (new[] { propertyDefinition }, true));

      var result = _factory.CreateRelationEndPointDefinitionCollection (classDefinition);

      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (((RelationEndPointDefinition) result[0]).PropertyName, 
        Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.OrderTicket.Order"));
    }
  }
}