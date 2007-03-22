using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationReflectorTests
{
  [TestFixture]
  public class RelationProperty: ReflectionBasedMappingTest
  {
    private PropertyReflector _propertyReflector;
    private RelationReflector _relationReflector;
    private ClassDefinition _classWithManySideRelationPropertiesClassDefinition;

    public override void SetUp()
    {
      base.SetUp();
      _propertyReflector = new PropertyReflector();
      _relationReflector = new RelationReflector();
      _classWithManySideRelationPropertiesClassDefinition = new ClassDefinition (
          "ClassWithManySideRelationProperties", "ClassWithManySideRelationProperties", "TestDomain", typeof (ClassWithManySideRelationProperties));
    }

    [Test]
    public void GetRelationDefinition_UnidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToOne");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationReflector.GetRelationEndPointDefinition (propertyInfo, propertyDefinition);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.IsNotNull (relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition.ClassDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
      Assert.IsFalse (relationEndPointDefiniton.IsMandatory);
    }

    [Test]
    public void GetRelationDefinition_UnidirectionalOneToMany()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToMany");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual = _relationReflector.GetRelationEndPointDefinition (propertyInfo, propertyDefinition);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.IsNotNull (relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition.ClassDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
      Assert.IsFalse (relationEndPointDefiniton.IsMandatory);
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }
  }
}