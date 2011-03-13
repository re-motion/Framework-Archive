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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class OneSideRelationProperty : MappingReflectionTestBase
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithVirtualRelationEndPoints));
    }

    [Test]
    public void GetMetadata_ForOptional()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("NoAttribute");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
         "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NoAttribute",
         actual.PropertyName);
      Assert.IsFalse (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_ForMandatory()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("NotNullable");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.NotNullable",
          actual.PropertyName);
      Assert.IsTrue (actual.IsMandatory);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToOne",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithRealRelationEndPoints), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.One, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration.ReflectionBasedMappingSample.ClassWithVirtualRelationEndPoints.BidirectionalOneToMany",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithRealRelationEndPoints>), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.Many, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      Assert.AreEqual ("NoAttribute", relationEndPointDefinition.SortExpressionText);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithVirtualRelationEndPoints).GetProperty ("BidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo, Configuration.NameResolver);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, UnitTestDomainStorageProviderDefinition, type, false);
    }
  }
}
