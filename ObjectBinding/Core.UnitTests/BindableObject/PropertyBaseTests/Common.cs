using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Development.UnitTesting;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.PropertyBaseTests
{
  [TestFixture]
  public class Common : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _bindableObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void Initialize ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyBase propertyBase = new StubPropertyBase (new PropertyBase.Parameters (_bindableObjectProvider, propertyInfo, null, true));

      Assert.That (propertyBase.PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (propertyBase.PropertyType, Is.SameAs (propertyInfo.PropertyType));
      Assert.That (propertyBase.IsRequired, Is.True);
      Assert.That (propertyBase.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
      Assert.That (((IBusinessObjectProperty) propertyBase).BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetListInfo ()
    {
      IListInfo expected = new ListInfo (typeof (SimpleReferenceType));
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Array"), expected, false));

      Assert.That (property.IsList, Is.True);
      Assert.That (property.ListInfo, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access ListInfo for non-list properties.\r\nProperty: Scalar")]
    public void GetListInfo_WithNonListProperty ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"), null, false));

      Assert.That (property.IsList, Is.False);
      Dev.Null = property.ListInfo;
    }


    [Test]
    public void ConvertFromNativePropertyType ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"), null, false));
      SimpleReferenceType expected = new SimpleReferenceType();

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertToNativePropertyType_Scalar ()
    {
      PropertyBase property = new StubPropertyBase (
          new PropertyBase.Parameters (
              _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar"), null, false));
      SimpleReferenceType expected = new SimpleReferenceType();

      Assert.That (property.ConvertToNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetDisplayName ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void IsAccessible ()
    {
    }

    [Test]
    [Ignore ("TODO: test")]
    public void IsRequired ()
    {
    }
  }
}