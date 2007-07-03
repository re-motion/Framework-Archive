using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.UnitTests.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.BindableObject
{
  [TestFixture]
  public class NumericPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    [SetUp]
    public void SetUp ()
    {
      _businessObjectProvider = new BindableObjectProvider();
    }

    [Test]
    public void Initialize_ByteProperty ()
    {
      IBusinessObjectNumericProperty property = new ByteProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Byte"), null, false));

      Assert.That (property.Type, Is.SameAs (typeof (Byte)));
      Assert.That (property.AllowNegative, Is.False);
    }

    [Test]
    public void Initialize_Int16Property ()
    {
      IBusinessObjectNumericProperty property = new Int16Property (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int16"), null, false));

      Assert.That (property.Type, Is.SameAs (typeof (Int16)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_Int32Property ()
    {
      IBusinessObjectNumericProperty property = new Int32Property (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int32"), null, false));

      Assert.That (property.Type, Is.SameAs (typeof (Int32)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_Int64Property ()
    {
      IBusinessObjectNumericProperty property = new Int64Property (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int64"), null, false));

      Assert.That (property.Type, Is.SameAs (typeof (Int64)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_SingleProperty ()
    {
      IBusinessObjectNumericProperty property = new SingleProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Single"), null, false));

      Assert.That (property.Type, Is.SameAs (typeof (Single)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_DoubleProperty ()
    {
      IBusinessObjectNumericProperty property = new DoubleProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Double"), null, false));

      Assert.That (property.Type, Is.SameAs (typeof (Double)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_DecimalProperty ()
    {
      IBusinessObjectNumericProperty property = new DecimalProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Decimal"), null, false));

      Assert.That (property.Type, Is.SameAs (typeof (Decimal)));
      Assert.That (property.AllowNegative, Is.True);
    }
  }
}