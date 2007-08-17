using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class ValueType : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void GetMetadata_WithScalar ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "Scalar");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithNullableScalar ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "NullableScalar");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("NullableScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType?)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithUndefinedEnum ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithUndefinedEnumValue), "Scalar");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (EnumerationProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (EnumWithUndefinedValue)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithReadOnlyScalar ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "ReadOnlyScalar");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyNonPublicSetterScalar ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "ReadOnlyNonPublicSetterScalar");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyNonPublicSetterScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithReadOnlyAttributeScalar ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "ReadOnlyAttributeScalar");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ReadOnlyAttributeScalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.True);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.True);
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "Array");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Array"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleValueType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }

    [Test]
    public void GetMetadata_WithNullableArray ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithValueType<SimpleValueType>), "NullableArray");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("NullableArray"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleValueType?[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleValueType?)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (businessObjectProperty.IsReadOnly (null), Is.False);
    }
  }
}