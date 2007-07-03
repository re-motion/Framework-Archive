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
    public void GetMetadata_WithByte ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Byte");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (ByteProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Byte"));
    }

    [Test]
    public void GetMetadata_WithDate ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Date");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (DateProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Date"));
    }

    [Test]
    public void GetMetadata_WithDateTime ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("DateTime");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (DateTimeProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("DateTime"));
    }

    [Test]
    public void GetMetadata_WithDecimal ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Decimal");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (DecimalProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Decimal"));
    }

    [Test]
    public void GetMetadata_WithDouble ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Double");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (DoubleProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Double"));
    }

    [Test]
    public void GetMetadata_WithEnum ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Enum");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (EnumerationProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Enum"));
    }

    [Test]
    public void GetMetadata_WithInt16 ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Int16");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (Int16Property)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Int16"));
    }

    [Test]
    public void GetMetadata_WithInt32 ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Int32");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (Int32Property)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Int32"));
    }

    [Test]
    public void GetMetadata_WithInt64 ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Int64");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (Int64Property)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Int64"));
    }

    [Test]
    public void GetMetadata_WithSingle ()
    {
      IBusinessObjectProperty businessObjectProperty = GetMetadataFromPropertyReflector ("Single");

      Assert.That (businessObjectProperty, Is.InstanceOfType (typeof (SingleProperty)));
      Assert.That (businessObjectProperty.Identifier, Is.EqualTo ("Single"));
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