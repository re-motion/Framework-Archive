using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests
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
      BindableObjectClass bindableObjectClass = new BindableObjectClass (typeof (SimpleClass), _bindableObjectProvider);

      Assert.That (bindableObjectClass.Type, Is.SameAs (typeof (SimpleClass)));
      Assert.That (bindableObjectClass.Identifier, Is.EqualTo ("SimpleClass"));
      Assert.That (bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      PropertyReflector propertyReflector = new PropertyReflector (GetPropertyInfo (typeof (SimpleClass), "String"));
      BindableObjectClass bindableObjectClass = new BindableObjectClass (typeof (ClassWithAllDataTypes), _bindableObjectProvider);

      CheckPropertyBase (propertyReflector.GetMetadata(), bindableObjectClass.GetPropertyDefinition ("String"));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException), 
        ExpectedMessage = "The property 'Invalid' was not found on business object class 'ClassWithAllDataTypes'.")]
    public void GetPropertyDefinition_WithInvalidPropertyName ()
    {
      BindableObjectClass bindableObjectClass = new BindableObjectClass (typeof (ClassWithAllDataTypes), _bindableObjectProvider);

      bindableObjectClass.GetPropertyDefinition ("Invalid");
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      TypeReflector typeReflector = new TypeReflector (typeof (ClassWithAllDataTypes));
      PropertyBase[] expectedProperties = typeReflector.GetProperties();
      
      BindableObjectClass bindableObjectClass = new BindableObjectClass (typeof (ClassWithAllDataTypes), _bindableObjectProvider);
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
      Assert.That (expectedProperty.ItemType, Is.EqualTo (actualProperty.ItemType), "ItemType");
      Assert.That (expectedProperty.IsList, Is.EqualTo (actualProperty.IsList), "IsList");
      Assert.That (expectedProperty.IsRequired, Is.EqualTo (actualProperty.IsRequired), "IsRequired");

      if (typeof (IBusinessObjectStringProperty).IsAssignableFrom (actualProperty.GetType ()))
        CheckStringProperty ((IBusinessObjectStringProperty) actualProperty, expectedProperty);
    }

    private void CheckStringProperty (IBusinessObjectStringProperty expectedProperty, IBusinessObjectProperty actualProperty)
    {
      Assert.That (
          expectedProperty.MaxLength,
          Is.EqualTo (((IBusinessObjectStringProperty) actualProperty).MaxLength),
          "MaxLength");
    }
  }
}