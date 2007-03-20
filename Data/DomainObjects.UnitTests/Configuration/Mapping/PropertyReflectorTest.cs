using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [TestFixture]
  public class PropertyReflectorTest: StandardMappingTest
  {
    [Test]
    public void GetMetadata_WithBasicType()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("BooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("BooleanProperty", actual.ColumnName);
      Assert.AreEqual (false, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual ("boolean", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (bool), actual.PropertyType);
    }

    [Test]
    public void GetMetadata_WithNullableBasicType()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("NaBooleanProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("NaBooleanProperty", actual.ColumnName);
      Assert.AreEqual (NaBoolean.Null, actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual ("boolean", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NaBooleanProperty", actual.PropertyName);
      Assert.AreSame (typeof (NaBoolean), actual.PropertyType);
    }

    [Test]
    public void GetMetadata_WithStringProperty()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("StringProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("StringProperty", actual.ColumnName);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.AreEqual (new NaInt32 (100), actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringProperty", actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
    }

    [Test]
    public void GetMetadata_WithStringPropertyWithoutMaxLength()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("StringPropertyWithoutMaxLength");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("StringPropertyWithoutMaxLength", actual.ColumnName);
      Assert.AreEqual (string.Empty, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringPropertyWithoutMaxLength", actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
    }

    [Test]
    public void GetMetadata_WithStringWithNullValueProperty()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("StringWithNullValueProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("StringWithNullValueProperty", actual.ColumnName);
      Assert.IsNull (actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual ("string", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.StringWithNullValueProperty", actual.PropertyName);
      Assert.AreSame (typeof (string), actual.PropertyType);
    }

    [Test]
    public void GetMetadata_WithEnumProperty()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("EnumProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("EnumProperty", actual.ColumnName);
      Assert.AreEqual (ClassWithAllDataTypes.EnumType.Value0, actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual (
          "Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes+EnumType, Rubicon.Data.DomainObjects.UnitTests",
          actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.EnumProperty", actual.PropertyName);
      Assert.AreSame (typeof (ClassWithAllDataTypes.EnumType), actual.PropertyType);
    }

    [Test]
    public void GetMetadata_WithBinaryProperty()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("BinaryProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("BinaryProperty", actual.ColumnName);
      Assert.AreEqual (new byte[0], actual.DefaultValue);
      Assert.IsFalse (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual ("binary", actual.MappingTypeName);
      Assert.AreEqual (NaInt32.Null, actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.BinaryProperty", actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
    }

    [Test]
    public void GetMetadata_WithNullableBinaryPropertyWithoutMaxLength()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = typeof (ClassWithAllDataTypes).GetProperty ("NullableBinaryProperty");

      PropertyDefinition actual = propertyReflector.GetMetadata (propertyInfo);

      Assert.IsNotNull (actual);
      Assert.IsNull (actual.ClassDefinition);
      Assert.AreEqual ("NullableBinaryProperty", actual.ColumnName);
      Assert.AreEqual (null, actual.DefaultValue);
      Assert.IsTrue (actual.IsNullable);
      Assert.IsTrue (actual.IsPropertyTypeResolved);
      Assert.AreEqual ("binary", actual.MappingTypeName);
      Assert.AreEqual (new NaInt32 (1000000), actual.MaxLength);
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.NullableBinaryProperty", actual.PropertyName);
      Assert.AreSame (typeof (byte[]), actual.PropertyType);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
        "The Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping.StringPropertyAttribute may be only applied to properties of type System.String. "
        + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTest, property: Int32PropertyWithStringPropertyAttribute")]
    public void GetMetadata_WithStringPropertyAttributeNotAppliedToStringProperty()
    {
      PropertyReflector propertyReflector = new PropertyReflector();
      PropertyInfo propertyInfo = 
          typeof (PropertyReflectorTest).GetProperty ("Int32PropertyWithStringPropertyAttribute", BindingFlags.Instance | BindingFlags.NonPublic);

      propertyReflector.GetMetadata (propertyInfo);
    }

    [Test]
    [ExpectedException (typeof (MappingException),
       "The Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping.BinaryPropertyAttribute may be only applied to properties of type System.Byte[]. "
       + "Type: Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyReflectorTest, property: Int32PropertyWithBinaryPropertyAttribute")]
    public void GetMetadata_WithBinaryPropertyAttributeNotAppliedToStringProperty ()
    {
      PropertyReflector propertyReflector = new PropertyReflector ();
      PropertyInfo propertyInfo =
          typeof (PropertyReflectorTest).GetProperty ("Int32PropertyWithBinaryPropertyAttribute", BindingFlags.Instance | BindingFlags.NonPublic);

      propertyReflector.GetMetadata (propertyInfo);
    }

    [StringProperty]
    private int Int32PropertyWithStringPropertyAttribute
    {
      get { throw new NotImplementedException(); }
    }

    [BinaryProperty]
    private int Int32PropertyWithBinaryPropertyAttribute
    {
      get { throw new NotImplementedException (); }
    }

    [Test]
    public void GetPropertyName ()
    {
      Assert.AreEqual ("System.DateTime.Year", PropertyReflector.GetPropertyName (typeof (DateTime).GetProperty ("Year")));
      Assert.AreEqual ("Rubicon.Data.DomainObjects.UnitTests.TestDomain.ClassWithAllDataTypes.DateTimeProperty",
          PropertyReflector.GetPropertyName (typeof (ClassWithAllDataTypes).GetProperty ("DateTimeProperty")));
    }
  }
}
