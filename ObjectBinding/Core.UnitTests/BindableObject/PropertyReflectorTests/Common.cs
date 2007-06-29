using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject.PropertyReflectorTests
{
  [TestFixture]
  public class Common : TestBase
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
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), "String");

      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      Assert.That (propertyReflector.PropertyInfo, Is.SameAs (propertyInfo));
      Assert.That (propertyReflector.BusinessObjectProvider, Is.SameAs (_businessObjectProvider));
    }

    [Test]
    public void GetMetadata_WithBoolean ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Boolean");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (BooleanProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Boolean"));
    }

    [Test]
    public void GetMetadata_WithEnum ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Enum");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (EnumerationProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Enum"));
    }

    [Test]
    public void GetMetadata_WithString ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("String");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (StringProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("String"));
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

    private IBusinessObjectProperty GetMetadataFromPropertyReflector (string propertyName)
    {
      PropertyInfo propertyInfo = GetPropertyInfo (typeof (ClassWithAllDataTypes), propertyName);
      PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);

      return propertyReflector.GetMetadata ();
    }
  }
}