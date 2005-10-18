using System;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaDecimalConverterTest
{
  private NaDecimalConverter _converter;
  private CultureInfo _culture;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaDecimalConverter();
    _culture = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");
  }

  [TearDown]
  public void TearDown()
  {
    Thread.CurrentThread.CurrentCulture = _culture;
  }

  [Test]
  public void CanConvertToString()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (string)));
  }

  [Test]
  public void CanConvertFromString()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (string)));
  }

  [Test]
  public void ConvertToString()
  {
    Type destinationType = typeof (string);

    Assert.AreEqual ("", _converter.ConvertTo (NaDecimal.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (new NaDecimal (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (new NaDecimal (-32), destinationType));
    Assert.AreEqual ("987654321.123456789", _converter.ConvertTo (new NaDecimal (987654321.123456789M), destinationType));
    Assert.AreEqual ("-987654321.123456789", _converter.ConvertTo (new NaDecimal (-987654321.123456789M), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (NaDecimal.Zero, destinationType));
    Assert.AreEqual (NaDecimal.MinValue.ToString(), _converter.ConvertTo (NaDecimal.MinValue, destinationType));
    Assert.AreEqual (NaDecimal.MaxValue.ToString(), _converter.ConvertTo (NaDecimal.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaDecimal.Null, _converter.ConvertFrom (""));
    Assert.AreEqual (new NaDecimal (32), _converter.ConvertFrom ("32"));
    Assert.AreEqual (new NaDecimal (-32), _converter.ConvertFrom ("-32"));
    Assert.AreEqual (new NaDecimal (987654321.123456789M), _converter.ConvertFrom ("987654321.123456789"));
    Assert.AreEqual (new NaDecimal (-987654321.123456789M), _converter.ConvertFrom ("-987654321.123456789"));
    Assert.AreEqual (NaDecimal.Zero, _converter.ConvertFrom ("0"));
    Assert.AreEqual (NaDecimal.MinValue, _converter.ConvertFrom (NaDecimal.MinValue.ToString()));
    Assert.AreEqual (NaDecimal.MaxValue, _converter.ConvertFrom (NaDecimal.MaxValue.ToString()));
  }

  [Test]
  public void CanConvertToDecimal()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (decimal)));
  }

  [Test]
  public void CanConvertFromDecimal()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (decimal)));
  }

  [Test]
  public void ConvertToDecimal()
  {
    Type destinationType = typeof (decimal);

    Assert.AreEqual (32M, _converter.ConvertTo (new NaDecimal (32), destinationType));
    Assert.AreEqual (-32M, _converter.ConvertTo (new NaDecimal (-32), destinationType));
    Assert.AreEqual (987654321.123456789M, _converter.ConvertTo (new NaDecimal (987654321.123456789M), destinationType));
    Assert.AreEqual (-987654321.123456789M, _converter.ConvertTo (new NaDecimal (-987654321.123456789M), destinationType));
    Assert.AreEqual (0M, _converter.ConvertTo (NaDecimal.Zero, destinationType));
    Assert.AreEqual (decimal.MinValue, _converter.ConvertTo (NaDecimal.MinValue, destinationType));
    Assert.AreEqual (decimal.MaxValue, _converter.ConvertTo (NaDecimal.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToDecimalWithNull()
  {
    Type destinationType = typeof (decimal);

    _converter.ConvertTo (NaDecimal.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDecimal()
  {
    Assert.AreEqual (new NaDecimal (32), _converter.ConvertFrom (32M));
    Assert.AreEqual (new NaDecimal (-32), _converter.ConvertFrom (-32M));
    Assert.AreEqual (new NaDecimal (987654321.123456789M), _converter.ConvertFrom (987654321.123456789M));
    Assert.AreEqual (new NaDecimal (-987654321.123456789M), _converter.ConvertFrom (-987654321.123456789M));
    Assert.AreEqual (NaDecimal.Zero, _converter.ConvertFrom (0M));
    Assert.AreEqual (NaDecimal.MinValue, _converter.ConvertFrom (decimal.MinValue));
    Assert.AreEqual (NaDecimal.MaxValue, _converter.ConvertFrom (decimal.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaDecimal.Null, _converter.ConvertFrom (null));
  }

  [Test]
  public void CanConvertToDBNull()
  {
    Assert.IsFalse (_converter.CanConvertTo (typeof (DBNull)));
  }

  [Test]
  public void CanConvertFromDBNull()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (DBNull)));
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertToDBNull()
  {
    _converter.ConvertTo (NaDecimal.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaDecimal.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
