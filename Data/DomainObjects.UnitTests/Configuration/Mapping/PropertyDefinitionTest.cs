using System;
using NUnit.Framework;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;

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
  public void InitializeWithPropertyTypeName ()
  {
    PropertyDefinition actual = new PropertyDefinition ("PropertyName", "ColumnName", "int32", true, NaInt32.Null, true);
    Assert.IsNull (actual.ClassDefinition);
    Assert.AreEqual ("ColumnName", actual.ColumnName);
    Assert.AreEqual (NaInt32.Null, actual.DefaultValue);
    Assert.IsTrue (actual.IsNullable);
    Assert.AreEqual ("int32", actual.MappingTypeName);
    Assert.AreEqual (NaInt32.Null, actual.MaxLength);
    Assert.AreEqual ("PropertyName", actual.PropertyName);
    Assert.AreEqual (typeof (NaInt32), actual.PropertyType);
    Assert.IsTrue (actual.IsPropertyTypeResolved);
  }

  [Test]
  public void InitializeWithUnresolvedPropertyTypeName ()
  {
    PropertyDefinition actual = new PropertyDefinition ("PropertyName", "ColumnName", "int32", true, NaInt32.Null, false);
    Assert.IsNull (actual.ClassDefinition);
    Assert.AreEqual ("ColumnName", actual.ColumnName);
    Assert.IsNull (actual.DefaultValue);
    Assert.IsTrue (actual.IsNullable);
    Assert.AreEqual ("int32", actual.MappingTypeName);
    Assert.AreEqual (NaInt32.Null, actual.MaxLength);
    Assert.AreEqual ("PropertyName", actual.PropertyName);
    Assert.IsNull (actual.PropertyType);
    Assert.IsFalse (actual.IsPropertyTypeResolved);
  }

  [Test]
  public void InitializeWithUnresolvedUnknownPropertyTypeName ()
  {
    PropertyDefinition actual = new PropertyDefinition ("PropertyName", "ColumnName", "UnknownMappingTypeName", true, NaInt32.Null, false);
    Assert.IsNull (actual.ClassDefinition);
    Assert.AreEqual ("ColumnName", actual.ColumnName);
    Assert.IsNull (actual.DefaultValue);
    Assert.IsTrue (actual.IsNullable);
    Assert.AreEqual ("UnknownMappingTypeName", actual.MappingTypeName);
    Assert.AreEqual (NaInt32.Null, actual.MaxLength);
    Assert.AreEqual ("PropertyName", actual.PropertyName);
    Assert.IsNull (actual.PropertyType);
    Assert.IsFalse (actual.IsPropertyTypeResolved);
  }

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
  public void MappingTypeName ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "date");
 
    Assert.AreEqual (typeof (DateTime), definition.PropertyType);
    Assert.AreEqual ("date", definition.MappingTypeName);
  }

  [Test]
  [ExpectedException (typeof (MappingException))]
  public void InvalidMappingTypeName ()
  {
    PropertyDefinition definition = new PropertyDefinition ("test", "test", "InvalidMappingTypeName");
  }
}
}
