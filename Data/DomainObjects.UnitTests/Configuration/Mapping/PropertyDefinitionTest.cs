using System;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
[TestFixture]
public class PropertyDefinitionTest
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PropertyDefinitionTest ()
  {
  }

  // methods and properties

  [Test]
  [ExpectedException (typeof (MappingException), 
      "Property 'PropertyName' of type 'System.String' must have MaxLength defined.")]     
  public void StringPropertyWithoutMaxLength ()
  {
    PropertyDefinition definition = new PropertyDefinition ("PropertyName", "ColumnName", "string");
  }

  [Test]
  [ExpectedException (typeof (MappingException),
      "MaxLength parameter cannot be supplied with value of type 'System.Int32'.")]
  public void IntPropertyWithMaxLength ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "int32", new NaInt32 (10));
  }

  [Test]
  public void MappingType ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "date");
 
    Assert.AreEqual (typeof (DateTime), definition.PropertyType);
    Assert.AreEqual ("date", definition.MappingType);
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void InvalidMappingType ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "InvalidMappingType");
  }
}
}
