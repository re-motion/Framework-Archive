using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.ObjectBinding.UnitTests
{
  [TestFixture]
  public class PropertyReflectorTest : TestBase
  {
    [Test]
    public void GetMetadata_WithString ()
    {
      Type type = typeof (ClassWithAllDataTypes);

      PropertyReflector propertyReflector = new PropertyReflector (GetPropertyInfo (type, "String"));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (StringProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("String"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.ItemType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (((IBusinessObjectStringProperty) businessObjectProperty).MaxLength, Is.Null);
    }

    [Test]
    public void GetMetadata_WithStringArray ()
    {
      Type type = typeof (ClassWithAllDataTypes);

      PropertyReflector propertyReflector = new PropertyReflector (GetPropertyInfo (type, "StringArray"));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (StringProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("StringArray"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (string[])));
      Assert.That (businessObjectProperty.ItemType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (((IBusinessObjectStringProperty) businessObjectProperty).MaxLength, Is.Null);
    }

    [Test]
    public void GetMetadata_WithListOfString ()
    {
      Type type = typeof (ClassWithAllDataTypes);

      PropertyReflector propertyReflector = new PropertyReflector (GetPropertyInfo (type, "ListOfString"));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (StringProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("ListOfString"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (List<string>)));
      Assert.That (businessObjectProperty.ItemType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (((IBusinessObjectStringProperty) businessObjectProperty).MaxLength, Is.Null);
    }

    [Test]
    public void GetMetadata_WithStringArrayList ()
    {
      Type type = typeof (ClassWithAllDataTypes);

      PropertyReflector propertyReflector = new PropertyReflector (GetPropertyInfo (type, "StringArrayList"));

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (StringProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("StringArrayList"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (ArrayList)));
      Assert.That (businessObjectProperty.ItemType, Is.SameAs (typeof (string)));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
      Assert.That (((IBusinessObjectStringProperty) businessObjectProperty).MaxLength, Is.Null);
    }

    [Test]
    [Ignore ("TODO: test")]
    public void GetMetadata_WithRequiredStringAttribute ()
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