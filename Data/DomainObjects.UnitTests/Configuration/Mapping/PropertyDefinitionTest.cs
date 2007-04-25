using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : ReflectionBasedMappingTest
  {
    [Test]
    public void InitializeWithValueType ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition ("PropertyName", "ColumnName", typeof (int), null, null, true);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (int.MinValue, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (typeof (int).FullName, actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (int), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
    }

    [Test]
    public void InitializeWithNullableValueType ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition ("PropertyName", "ColumnName", typeof (NaInt32), null, null, true);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (NaInt32.Null, actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (typeof(NaInt32).FullName, actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (NaInt32), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
    }

    [Test]
    public void InitializeWithObjectID ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition ("PropertyName", "ColumnName", typeof (ObjectID), false, null, true);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (TypeInfo.ObjectIDMappingTypeName, actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
    }

    [Test]
    public void InitializeWithEnum ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition ("PropertyName", "ColumnName", typeof (ClassWithAllDataTypes.EnumType), null, null, true);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType).FullName, actual.MappingTypeName);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
    }

    [Test]
    public void StringPropertyWithoutMaxLength ()
    {
      PropertyDefinition definition = new ReflectionBasedPropertyDefinition ("PropertyName", "ColumnName", typeof (string));
      Assert.IsNull (definition.MaxLength);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access property 'StorageSpecificName' for non-persistent property definitions.")]
    public void NonPersistentProperty ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition ("ThePropertyName", "TheColumnName", typeof (int), null, null, false);
      Assert.IsFalse (actual.IsPersistent);
      Dev.Null = actual.StorageSpecificName;
    }

    [Test]
    public void MappingTypeName ()
    {
      PropertyDefinition definition = new ReflectionBasedPropertyDefinition ("test", "test", typeof (int));

      Assert.AreEqual (typeof (int), definition.PropertyType);
      Assert.AreEqual ("System.Int32", definition.MappingTypeName);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "MaxLength parameter cannot be supplied for properties of type 'System.Int32'.\r\n  Property: test")]
    public void IntPropertyWithMaxLength ()
    {
      new ReflectionBasedPropertyDefinition ("test", "test", typeof (int), 10);
    }

    [Test]
    [Ignore("TODO: Implement tests for arg checks")]
    public void CheckValueTypeCtors()
    {
      
    }
  }
}
