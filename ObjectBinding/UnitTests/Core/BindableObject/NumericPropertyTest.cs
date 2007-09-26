using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.ObjectBinding.UnitTests.Core.BindableObject.TestDomain;

namespace Rubicon.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class NumericPropertyTest : TestBase
  {
    private BindableObjectProvider _businessObjectProvider;

    public override void SetUp ()
    {
      base.SetUp ();

      _businessObjectProvider = new BindableObjectProvider ();
    }

    [Test]
    public void Initialize_ByteProperty ()
    {
      IBusinessObjectNumericProperty property = new ByteProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Byte"), typeof (byte), null, false, false, false));

      Assert.That (property.Type, Is.SameAs (typeof (Byte)));
      Assert.That (property.AllowNegative, Is.False);
    }

    [Test]
    public void Initialize_Int16Property ()
    {
      IBusinessObjectNumericProperty property = new Int16Property (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int16"), typeof (Int16), null, false, false, false));

      Assert.That (property.Type, Is.SameAs (typeof (Int16)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_Int32Property ()
    {
      IBusinessObjectNumericProperty property = new Int32Property (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int32"), typeof (Int32), null, false, false, false));

      Assert.That (property.Type, Is.SameAs (typeof (Int32)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_Int64Property ()
    {
      IBusinessObjectNumericProperty property = new Int64Property (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Int64"), typeof (Int64), null, false, false, false));

      Assert.That (property.Type, Is.SameAs (typeof (Int64)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_SingleProperty ()
    {
      IBusinessObjectNumericProperty property = new SingleProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Single"), typeof (Single), null, false, false, false));

      Assert.That (property.Type, Is.SameAs (typeof (Single)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_DoubleProperty ()
    {
      IBusinessObjectNumericProperty property = new DoubleProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Double"), typeof (Double), null, false, false, false));

      Assert.That (property.Type, Is.SameAs (typeof (Double)));
      Assert.That (property.AllowNegative, Is.True);
    }

    [Test]
    public void Initialize_DecimalProperty ()
    {
      IBusinessObjectNumericProperty property = new DecimalProperty (
          new PropertyBase.Parameters (_businessObjectProvider, GetPropertyInfo (typeof (ClassWithAllDataTypes), "Decimal"), typeof (Decimal), null, false, false, false));

      Assert.That (property.Type, Is.SameAs (typeof (Decimal)));
      Assert.That (property.AllowNegative, Is.True);
    }
  }
}