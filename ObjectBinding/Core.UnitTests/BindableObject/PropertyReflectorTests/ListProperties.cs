using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class ListProperties : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithListProperties), "Array");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Array"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }

    [Test]
    public void GetMetadata_WithListOfT ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithListProperties), "ListOfT");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ListOfT"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (List<SimpleReferenceType>)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }

    [Test]
    public void GetMetadata_WithArrayList ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithListProperties), "ArrayList");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (NotSupportedProperty)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ArrayList"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ArrayList)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
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