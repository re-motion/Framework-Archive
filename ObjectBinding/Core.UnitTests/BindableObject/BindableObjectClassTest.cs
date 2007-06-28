using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectClassTest : TestBase
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
      ClassReflector classReflector = new ClassReflector (typeof (SimpleClass), _bindableObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      Assert.That (bindableObjectClass.Type, Is.SameAs (typeof (SimpleClass)));
      Assert.That (bindableObjectClass.Identifier, Is.EqualTo ("SimpleClass"));
      Assert.That (bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      PropertyReflector propertyReflector = new PropertyReflector (GetPropertyInfo (typeof (SimpleClass), "String"), _bindableObjectProvider);
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      CheckPropertyBase (propertyReflector.GetMetadata(), bindableObjectClass.GetPropertyDefinition ("String"));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException),
        ExpectedMessage = "The property 'Invalid' was not found on business object class 'ClassWithAllDataTypes'.")]
    public void GetPropertyDefinition_WithInvalidPropertyName ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      bindableObjectClass.GetPropertyDefinition ("Invalid");
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      PropertyBase[] expectedProperties = new PropertyBase[]
          {
              CreateProperty (typeof (ClassWithAllDataTypes), "String"),
              CreateProperty (typeof (ClassWithAllDataTypes), "Object"),
              CreateProperty (typeof (ClassWithAllDataTypes), "ObjectArray"),
              CreateProperty (typeof (ClassWithAllDataTypes), "ListOfObject"),
              CreateProperty (typeof (ClassWithAllDataTypes), "ObjectArrayList")
          };

      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();
      IBusinessObjectProperty[] actualProperties = bindableObjectClass.GetPropertyDefinitions();

      Assert.That (actualProperties.Length, Is.EqualTo (expectedProperties.Length));
      foreach (PropertyBase expectedProperty in expectedProperties)
      {
        bool isFound = false;
        foreach (IBusinessObjectProperty actualProperty in actualProperties)
        {
          if (actualProperty.Identifier == expectedProperty.Identifier)
          {
            Assert.That (isFound, Is.False, "Multiple properties '{0}' found", expectedProperty.Identifier);
            CheckPropertyBase (expectedProperty, actualProperty);
            isFound = true;
          }
        }
        Assert.That (isFound, Is.True, "Property '{0}' was not found", expectedProperty.Identifier);
      }
    }

    private void CheckPropertyBase (IBusinessObjectProperty expectedProperty, IBusinessObjectProperty actualProperty)
    {
      ArgumentUtility.CheckNotNull ("expectedProperty", expectedProperty);

      Assert.That (actualProperty, Is.Not.Null);
      Assert.That (actualProperty.GetType(), Is.SameAs (expectedProperty.GetType()), "BusinessObjectPropertyType");
      Assert.That (expectedProperty.PropertyType, Is.EqualTo (actualProperty.PropertyType), "PropertyType");
      Assert.That (expectedProperty.IsList, Is.EqualTo (actualProperty.IsList), "IsList");
      if (expectedProperty.IsList)
        Assert.That (expectedProperty.ListInfo.ItemType, Is.EqualTo (actualProperty.ListInfo.ItemType), "ListInfo.ItemType");
      Assert.That (expectedProperty.IsRequired, Is.EqualTo (actualProperty.IsRequired), "IsRequired");

      if (typeof (IBusinessObjectStringProperty).IsAssignableFrom (actualProperty.GetType()))
        CheckStringProperty ((IBusinessObjectStringProperty) actualProperty, expectedProperty);
    }

    private void CheckStringProperty (IBusinessObjectStringProperty expectedProperty, IBusinessObjectProperty actualProperty)
    {
      Assert.That (
          expectedProperty.MaxLength,
          Is.EqualTo (((IBusinessObjectStringProperty) actualProperty).MaxLength),
          "MaxLength");
    }

    private PropertyBase CreateProperty (Type type, string propertyName)
    {
      PropertyReflector propertyReflector = new PropertyReflector (GetPropertyInfo (type, propertyName), _bindableObjectProvider);
      return propertyReflector.GetMetadata();
    }
  }
}