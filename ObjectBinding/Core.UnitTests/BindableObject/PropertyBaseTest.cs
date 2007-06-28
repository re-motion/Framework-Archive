using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Development.UnitTesting;
using Rubicon.ObjectBinding.BindableObject;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class PropertyBaseTest : TestBase
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
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "Object");
      PropertyBase propertyBase = new StubPropertyBase (_bindableObjectProvider, propertyInfo, null, true);

      Assert.That (propertyBase.PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (propertyBase.PropertyType, Is.SameAs (propertyInfo.PropertyType));
      Assert.That (propertyBase.IsRequired, Is.True);
      Assert.That (propertyBase.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetListInfo ()
    {
      IListInfo expected = new ListInfo (typeof (UnboundClass));
      PropertyBase propertyBase =
          new StubPropertyBase (_bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "ObjectArray"), expected, false);

      Assert.That (propertyBase.IsList, Is.True);
      Assert.That (propertyBase.ListInfo, Is.SameAs (expected));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "Cannot access ListInfo for non-list properties.\r\nProperty: Object")]
    public void GetListInfo_WithNonListProperty ()
    {
      PropertyBase propertyBase =
          new StubPropertyBase (_bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Object"), null, false);

      Assert.That (propertyBase.IsList, Is.False);
      Dev.Null = propertyBase.ListInfo;
    }


    [Test]
    public void ConvertFromNativePropertyType_Scalar ()
    {
      PropertyBase property = new StubPropertyBase (_bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Object"), null, false);
      string expected = "A String";

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertFromNativePropertyType_Array ()
    {
      PropertyBase property = new StubPropertyBase (
          _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "ObjectArray"), new ListInfo (typeof (UnboundClass)), false);
      string[] expected = new string[] {"A", "String[]"};

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertFromNativePropertyType_ArrayList ()
    {
      PropertyBase property = new StubPropertyBase (
          _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "ObjectArrayList"), new ListInfo (typeof (UnboundClass)), false);
      ArrayList expected = new ArrayList (new string[] {"A", "String", "ArrayList"});

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertFromNativePropertyType_ListOfT ()
    {
      PropertyBase property = new StubPropertyBase (
          _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "ListOfObject"), new ListInfo (typeof (UnboundClass)), false);
      List<string> expected = new List<string> (new string[] {"A", "List<String>"});

      Assert.That (property.ConvertFromNativePropertyType (expected), Is.SameAs (expected));
    }


    [Test]
    public void ConvertToNativePropertyType_Scalar ()
    {
      PropertyBase property = new StubPropertyBase (_bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Object"), null, false);
      UnboundClass expected = new UnboundClass();

      Assert.That (property.ConvertToNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertToNativePropertyType_Array ()
    {
      PropertyBase property = new StubPropertyBase (
          _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "ObjectArray"), new ListInfo (typeof (UnboundClass)), false);
      UnboundClass[] expected = new UnboundClass[] {new UnboundClass(), new UnboundClass()};

      Assert.That (property.ConvertToNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertToNativePropertyType_ArrayList ()
    {
      PropertyBase property = new StubPropertyBase (
          _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "ObjectArrayList"), new ListInfo (typeof (UnboundClass)), false);
      ArrayList expected = new ArrayList (new UnboundClass[] {new UnboundClass(), new UnboundClass()});

      Assert.That (property.ConvertToNativePropertyType (expected), Is.SameAs (expected));
    }

    [Test]
    public void ConvertToNativePropertyType_ListOfT ()
    {
      PropertyBase property = new StubPropertyBase (
          _bindableObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "ListOfObject"), new ListInfo (typeof (UnboundClass)), false);
      List<UnboundClass> expected = new List<UnboundClass> (new UnboundClass[] {new UnboundClass(), new UnboundClass()});

      Assert.That (property.ConvertToNativePropertyType (expected), Is.SameAs (expected));
    }
  }
}