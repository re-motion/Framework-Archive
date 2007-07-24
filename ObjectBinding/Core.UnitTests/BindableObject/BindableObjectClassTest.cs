using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class BindableObjectClassTest : TestBase
  {
    private BindableObjectProvider _bindableObjectProvider;

    public override void SetUp ()
    {
      base.SetUp();

      _bindableObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void Initialize ()
    {
      BindableObjectClass bindableObjectClass = new BindableObjectClass (typeof (SimpleBusinessObjectClass), _bindableObjectProvider);

      Assert.That (bindableObjectClass.Type, Is.SameAs (typeof (SimpleBusinessObjectClass)));
      Assert.That (
          bindableObjectClass.Identifier,
          Is.EqualTo ("Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.SimpleBusinessObjectClass, Rubicon.ObjectBinding.UnitTests"));
      Assert.That (bindableObjectClass.RequiresWriteBack, Is.False);
      Assert.That (bindableObjectClass.BusinessObjectProvider, Is.SameAs (_bindableObjectProvider));
    }

    [Test]
    public void Initialize_WithGeneric ()
    {
      BindableObjectClass bindableObjectClass = new BindableObjectClass (typeof (ClassWithReferenceType<SimpleReferenceType>), _bindableObjectProvider);

      Assert.That (bindableObjectClass.Type, Is.SameAs (typeof (ClassWithReferenceType<SimpleReferenceType>)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        ExpectedMessage =
        "Type 'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.SimpleReferenceType' does not implement the "
        + "'Rubicon.ObjectBinding.IBusinessObject' interface via the 'Rubicon.ObjectBinding.BindableObject.BindableObjectMixin'.\r\n"
        + "Parameter name: type")]
    public void Initialize_WithTypeNotUsingBindableObjectMixin ()
    {
      new BindableObjectClass (typeof (SimpleReferenceType), _bindableObjectProvider);
    }

    [Test]
    public void GetPropertyDefinition ()
    {
      PropertyReflector propertyReflector =
          new PropertyReflector (GetPropertyInfo (typeof (SimpleBusinessObjectClass), "String"), _bindableObjectProvider);
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      CheckPropertyBase (propertyReflector.GetMetadata(), bindableObjectClass.GetPropertyDefinition ("String"));
    }

    [Test]
    [ExpectedException (typeof (KeyNotFoundException),
        ExpectedMessage = 
        "The property 'Invalid' was not found on business object class "
        + "'Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain.ClassWithAllDataTypes, Rubicon.ObjectBinding.UnitTests'.")]
    public void GetPropertyDefinition_WithInvalidPropertyName ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata();

      bindableObjectClass.GetPropertyDefinition ("Invalid");
    }

    [Test]
    public void HasPropertyDefinition ()
    {
      ClassReflector classReflector = new ClassReflector (typeof (ClassWithAllDataTypes), _bindableObjectProvider);
      BindableObjectClass bindableObjectClass = classReflector.GetMetadata ();

      Assert.That (bindableObjectClass.HasPropertyDefinition ("String"), Is.True);
      Assert.That(bindableObjectClass.HasPropertyDefinition ("Invalid"), Is.False);
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      Type type = typeof (ClassWithReferenceType<SimpleReferenceType>);
      PropertyBase[] expectedProperties = new PropertyBase[]
          {
              CreateProperty (type, "Scalar"),
              CreateProperty (type, "ReadOnlyScalar"),
              CreateProperty (type, "ReadOnlyAttributeScalar"),
              CreateProperty (type, "Array")
          };

      ClassReflector classReflector = new ClassReflector (type, _bindableObjectProvider);
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