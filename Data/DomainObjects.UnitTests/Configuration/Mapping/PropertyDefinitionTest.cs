using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : ReflectionBasedMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _classDefinition = new ReflectionBasedClassDefinition ("Order", "OrderTable", c_testDomainProviderID, typeof (Order), false);
    }

    [Test]
    public void InitializeWithValueType ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (int), null, null, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (int.MinValue, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (int), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableValueType ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (int?), null, null, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (int?), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithObjectID ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (ObjectID), false, null, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ObjectID), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsTrue (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithEnum ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (ClassWithAllDataTypes.EnumType), null, null, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableEnum ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (ClassWithAllDataTypes.EnumType?), null, null, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (ClassWithAllDataTypes.EnumType?), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableStringAndMaxLength ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (string), true, 100, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNotNullableStringWithoutMaxLength ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (string), false, null, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (string), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNullableArrayAndMaxLength ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (byte[]), true, 100, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.AreEqual (100, actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void InitializeWithNotNullableArrayWithoutMaxLength ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "PropertyName", "ColumnName", typeof (byte[]), false, null, true);
      Assert.AreSame (_classDefinition, actual.ClassDefinition);
      Assert.AreEqual ("ColumnName", actual.StorageSpecificName);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsNull (actual.MaxLength);
      Assert.AreEqual ("PropertyName", actual.PropertyName);
      Assert.AreEqual (typeof (byte[]), actual.PropertyType);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.IsTrue (actual.IsPersistent);
      Assert.IsFalse (actual.IsObjectID);
    }

    [Test]
    public void GetToString ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "ThePropertyName", "TheColumnName", typeof (int), null, null, false);

      Assert.That (actual.ToString (), Is.EqualTo (typeof (ReflectionBasedPropertyDefinition).FullName + ": ThePropertyName"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access property 'StorageSpecificName' for non-persistent property definitions.")]
    public void NonPersistentProperty ()
    {
      PropertyDefinition actual = new ReflectionBasedPropertyDefinition (_classDefinition, "ThePropertyName", "TheColumnName", typeof (int), null, null, false);
      Assert.IsFalse (actual.IsPersistent);
      Dev.Null = actual.StorageSpecificName;
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "MaxLength parameter can only be supplied for strings and byte arrays but the property is of type 'System.Int32'.\r\n  Property: test")]
    public void IntPropertyWithMaxLength ()
    {
      new ReflectionBasedPropertyDefinition (_classDefinition, "test", "test", typeof (int), 10);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
       "IsNullable parameter can only be supplied for reference types but the property is of type 'System.Int32'.\r\n  Property: test")]
    public void CheckValueTypeCtors ()
    {
      new ReflectionBasedPropertyDefinition (_classDefinition, "test", "test", typeof (int), false);
    }
  }
}
