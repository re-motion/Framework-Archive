using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class PropertyReflectorTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void Initialize ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "Object");

      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      Assert.That (propertyReflector.PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (propertyReflector.BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    public void GetMetadata_WithObject ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "Object");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Object"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (UnboundClass)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "ObjectArray");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ObjectArray"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (UnboundClass[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (UnboundClass)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }

    [Test]
    public void GetMetadata_WithListOfT ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "ListOfObject");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ListOfObject"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (List<UnboundClass>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (UnboundClass)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }

    [Test]
    public void GetMetadata_WithArrayList ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "ObjectArrayList");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ObjectArrayList"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ArrayList)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (UnboundClass)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }

    [Test]
    public void GetMetadata_WithString ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "String");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata ();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (StringProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("String"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (((StringProperty)businessObjectProperty).MaxLength, Is.Null);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithRequiredStringAttribute ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithMaxLengthStringAttribute ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithMissingItemTypeAttributeOnIList ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithItemTypeAttributeOnOverride ()
    {
    }
  }
}