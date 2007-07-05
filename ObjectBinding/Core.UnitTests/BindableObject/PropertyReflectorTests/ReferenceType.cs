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
  public class ReferenceType : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void GetMetadata_WithScalar ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Scalar");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Scalar"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsList, Is.False);
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }

    [Test]
    public void GetMetadata_WithArray ()
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithReferenceType<SimpleReferenceType>), "Array");
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      IBusinessObjectProperty businessObjectProperty = propertyReflector.GetMetadata();

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (PropertyBase)));
      Assert.That (((PropertyBase) businessObjectProperty).PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Array"));
      Assert.That (businessObjectProperty.PropertyType, Is.SameAs (typeof (SimpleReferenceType[])));
      Assert.That (businessObjectProperty.IsList, Is.True);
      Assert.That (businessObjectProperty.ListInfo, Is.Not.Null);
      Assert.That (businessObjectProperty.ListInfo.ItemType, Is.SameAs (typeof (SimpleReferenceType)));
      Assert.That (businessObjectProperty.IsRequired, Is.False);
    }
  }
}