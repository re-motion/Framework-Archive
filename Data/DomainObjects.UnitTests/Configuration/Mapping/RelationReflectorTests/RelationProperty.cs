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
    public void GetMetadata_ForOptional()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("NoAttribute");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual =
          _relationReflector.GetRelationEndPointDefinition (propertyInfo, _classWithManySideRelationPropertiesClassDefinition);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.IsFalse (relationEndPointDefiniton.IsMandatory);
    }

    [Test]
    public void GetMetadata_ForMandatory()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("NotNullable");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual =
          _relationReflector.GetRelationEndPointDefinition (propertyInfo, _classWithManySideRelationPropertiesClassDefinition);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.IsTrue (relationEndPointDefiniton.IsMandatory);
    }

    [Test]
    public void GetRelationDefinition_UnidirectionalOneToOne()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToOne");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual =
          _relationReflector.GetRelationEndPointDefinition (propertyInfo, _classWithManySideRelationPropertiesClassDefinition);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    public void GetRelationDefinition_UnidirectionalOneToMany()
    {
      PropertyInfo propertyInfo = typeof (ClassWithManySideRelationProperties).GetProperty ("UnidirectionalOneToMany");
      PropertyDefinition propertyDefinition = _propertyReflector.GetMetadata (propertyInfo);
      _classWithManySideRelationPropertiesClassDefinition.MyPropertyDefinitions.Add (propertyDefinition);

      IRelationEndPointDefinition actual =
          _relationReflector.GetRelationEndPointDefinition (propertyInfo, _classWithManySideRelationPropertiesClassDefinition);

      Assert.IsInstanceOfType (typeof (RelationEndPointDefinition), actual);
      RelationEndPointDefinition relationEndPointDefiniton = (RelationEndPointDefinition) actual;
      Assert.AreSame (_classWithManySideRelationPropertiesClassDefinition, relationEndPointDefiniton.ClassDefinition);
      Assert.AreSame (propertyDefinition, relationEndPointDefiniton.PropertyDefinition);
      Assert.IsNull (relationEndPointDefiniton.RelationDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.MandatoryAttribute may be only applied to properties assignable to types "
        + "Rubicon.Data.DomainObjects.DomainObject or Rubicon.Data.DomainObjects.DomainObjectCollection.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationReflectorTests.RelationProperty, property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);

      _propertyReflector.GetMetadata (propertyInfo);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.StringAttribute may be only applied to properties of type System.String.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationReflectorTests.RelationProperty, "
        + "property: PropertyWithStringAttribute")]
    public void GetMetadata_WithStringAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("PropertyWithStringAttribute", BindingFlags.Instance | BindingFlags.NonPublic);

      _relationReflector.GetRelationEndPointDefinition (propertyInfo, _classWithManySideRelationPropertiesClassDefinition);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.BinaryAttribute may be only applied to properties of type System.Byte[].\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationReflectorTests.RelationProperty, "
        + "property: PropertyWithBinaryAttribute")]
    public void GetMetadata_WithBinaryAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("PropertyWithBinaryAttribute", BindingFlags.Instance | BindingFlags.NonPublic);

      _relationReflector.GetRelationEndPointDefinition (propertyInfo, _classWithManySideRelationPropertiesClassDefinition);
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }

    [String]
    private ClassWithManySideRelationProperties PropertyWithStringAttribute
    {
      get { throw new NotImplementedException(); }
    }

    [Binary]
    private ClassWithManySideRelationProperties PropertyWithBinaryAttribute
    {
      get { throw new NotImplementedException(); }
    }
  }
}