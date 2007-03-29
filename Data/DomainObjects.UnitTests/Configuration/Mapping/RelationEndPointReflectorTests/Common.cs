using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common: ReflectionBasedMappingTest
  {
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinitions = new ClassDefinitionCollection();
    }

    [Test]
    public void CreateRelationEndPointReflector ()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NoAttribute");
      Assert.IsInstanceOfType (typeof (RdbmsRelationEndPointReflector), RelationEndPointReflector.CreateRelationEndPointReflector (propertyInfo));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.MandatoryAttribute may be only applied to properties assignable to types "
        + "Rubicon.Data.DomainObjects.DomainObject or Rubicon.Data.DomainObjects.DomainObjectCollection.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests.Common, "
        + "property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.StringAttribute may be only applied to properties of type System.String.\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests.Common, "
        + "property: PropertyWithStringAttribute")]
    public void GetMetadata_WithStringAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("PropertyWithStringAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.BinaryAttribute may be only applied to properties of type System.Byte[].\r\n  "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests.Common, "
        + "property: PropertyWithBinaryAttribute")]
    public void GetMetadata_WithBinaryAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("PropertyWithBinaryAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
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