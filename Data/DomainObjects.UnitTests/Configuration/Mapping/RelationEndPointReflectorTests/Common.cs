using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Factories;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain.ReflectionBasedMappingSample;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests
{
  [TestFixture]
  public class Common: StandardMappingTest
  {
    private ClassDefinitionCollection _classDefinitions;

    public override void SetUp()
    {
      base.SetUp();

      _classDefinitions = new ClassDefinitionCollection();
    }

    [Test]
    public void CreateRelationEndPointReflector()
    {
      PropertyInfo propertyInfo = typeof (ClassWithOneSideRelationProperties).GetProperty ("NoAttribute");
      Assert.IsInstanceOfType (typeof (RdbmsRelationEndPointReflector), RelationEndPointReflector.CreateRelationEndPointReflector (propertyInfo));
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The Rubicon.Data.DomainObjects.MandatoryAttribute may be only applied to properties assignable to types "
        + "Rubicon.Data.DomainObjects.DomainObject or Rubicon.Data.DomainObjects.DomainObjectCollection.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests.Common, "
        + "property: Int32Property")]
    public void GetMetadata_WithAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("Int32Property", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The Rubicon.Data.DomainObjects.StringPropertyAttribute may be only applied to properties of type System.String.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests.Common, "
        + "property: PropertyWithStringAttribute")]
    public void GetMetadata_WithStringAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("PropertyWithStringAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The Rubicon.Data.DomainObjects.BinaryPropertyAttribute may be only applied to properties of type System.Byte[].\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.RelationEndPointReflectorTests.Common, "
        + "property: PropertyWithBinaryAttribute")]
    public void GetMetadata_WithBinaryAttributeAppliedToInvalidProperty()
    {
      PropertyInfo propertyInfo = GetType().GetProperty ("PropertyWithBinaryAttribute", BindingFlags.Instance | BindingFlags.NonPublic);
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    [Test]
    [Ignore("Not sure if this test is required, since a relation with 2 virtual end points is already covered by the RelationDefinition ctor.")]
    [ExpectedException (typeof (MappingException),
        ExpectedMessage = "The Rubicon.Data.DomainObjects.DBBidirectionalRelationAttribute requires that one side contains the foreign key for relation.\r\n"
        + "Declaring type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide, "
        + "property: NoContainsKeyLeftSide")]
    public void GetMetadata_WithNeitherSideContainingTheKey ()
    {
      Type type = TestDomainFactory.ConfigurationMappingTestDomainErrors.GetType (
          "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.ClassWithInvalidBidirectionalRelationLeftSide", true, false);
      ClassDefinition classDefinition = new ReflectionBasedClassDefinition (
          "ClassWithInvalidBidirectionalRelationLeftSide",
          "ClassWithInvalidBidirectionalRelationLeftSide",
          "TestDomain", type, false);
      _classDefinitions.Add (classDefinition);
      PropertyInfo propertyInfo = type.GetProperty ("NoContainsKeyLeftSide");
      RdbmsRelationEndPointReflector relationEndPointReflector = new RdbmsRelationEndPointReflector (propertyInfo);

      relationEndPointReflector.GetMetadata (_classDefinitions);
    }

    [Mandatory]
    private int Int32Property
    {
      get { throw new NotImplementedException(); }
    }

    [StringProperty]
    private ClassWithManySideRelationProperties PropertyWithStringAttribute
    {
      get { throw new NotImplementedException(); }
    }

    [BinaryProperty]
    private ClassWithManySideRelationProperties PropertyWithBinaryAttribute
    {
      get { throw new NotImplementedException(); }
    }
  }
}