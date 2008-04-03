using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.Factories;
using Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;
using Remotion.Mixins.Context;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class BaseOneSideRelationProperty: StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinition = CreateReflectionBasedClassDefinition (typeof (ClassWithOneSideRelationProperties));
    }

    [Test]
    public void GetMetadata_BidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToOne",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ClassWithManySideRelationProperties), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.One, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
    }

    [Test]
    public void GetMetadata_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo);

      IRelationEndPointDefinition actual = relationEndPointReflector.GetMetadata ();

      Assert.IsInstanceOfType (typeof (VirtualRelationEndPointDefinition), actual);
      VirtualRelationEndPointDefinition relationEndPointDefinition = (VirtualRelationEndPointDefinition) actual;
      Assert.AreSame (_classDefinition, relationEndPointDefinition.ClassDefinition);
      Assert.AreEqual (
          "Remotion.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample.ClassWithOneSideRelationPropertiesNotInMapping.BaseBidirectionalOneToMany",
          relationEndPointDefinition.PropertyName);
      Assert.AreSame (typeof (ObjectList<ClassWithManySideRelationProperties>), relationEndPointDefinition.PropertyType);
      Assert.AreEqual (CardinalityType.Many, relationEndPointDefinition.Cardinality);
      Assert.IsNull (relationEndPointDefinition.RelationDefinition);
      Assert.AreEqual ("The Sort Expression", relationEndPointDefinition.SortExpression);
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToOne ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToOne");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint ());
    }

    [Test]
    public void IsVirtualEndRelationEndpoint_BidirectionalOneToMany ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("BaseBidirectionalOneToMany");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (_classDefinition, propertyInfo);

      Assert.IsTrue (relationEndPointReflector.IsVirtualEndRelationEndpoint());
    }

    private ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, new List<Type> ());
    }
  }
}