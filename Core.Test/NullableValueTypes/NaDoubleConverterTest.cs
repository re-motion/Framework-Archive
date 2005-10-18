using System;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaDoubleConverterTest
{
  private NaDoubleConverter _converter;
  private CultureInfo _culture;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaDoubleConverter();
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

    Assert.AreEqual ("", _converter.ConvertTo (NaDouble.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (new NaDouble (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (new NaDouble (-32), destinationType));
    Assert.AreEqual ("654321.123456789", _converter.ConvertTo (new NaDouble (654321.123456789), destinationType));
    Assert.AreEqual ("-654321.123456789", _converter.ConvertTo (new NaDouble (-654321.123456789), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (NaDouble.Zero, destinationType));
    Assert.AreEqual (NaDouble.MinValue.ToString(), _converter.ConvertTo (NaDouble.MinValue, destinationType));
    Assert.AreEqual (NaDouble.MaxValue.ToString(), _converter.ConvertTo (NaDouble.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaDouble.Null, _converter.ConvertFrom (NaDouble.Null.ToString()));
    Assert.AreEqual (new NaDouble (32), _converter.ConvertFrom (new NaDouble (32).ToString()));
    Assert.AreEqual (new NaDouble (-32), _converter.ConvertFrom (new NaDouble (-32).ToString()));
    Assert.AreEqual (new NaDouble (654321.123456789), _converter.ConvertFrom (new NaDouble (654321.123456789).ToString()));
    Assert.AreEqual (new NaDouble (-654321.123456789), _converter.ConvertFrom (new NaDouble (-654321.123456789).ToString()));
    Assert.AreEqual (NaDouble.Zero, _converter.ConvertFrom (NaDouble.Zero.ToString()));
  }

  [Test]
  [Ignore ("Precision Problem in Double.ToString().")]
  public void ConvertFromStringMaxAndMinValues()
  {
    Assert.AreEqual (NaDouble.MinValue, _converter.ConvertFrom (NaDouble.MinValue.ToString()));
    Assert.AreEqual (NaDouble.MaxValue, _converter.ConvertFrom (NaDouble.MaxValue.ToString()));
  }

  [Test]
  public void CanConvertToDouble()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (double)));
  }

  [Test]
  public void CanConvertFromDouble()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (double)));
  }

  [Test]
  public void ConvertToDouble()
  {
    Type destinationType = typeof (double);

    Assert.AreEqual ((double) 32, _converter.ConvertTo (new NaDouble (32), destinationType));
    Assert.AreEqual ((double) -32, _converter.ConvertTo (new NaDouble (-32), destinationType));
    Assert.AreEqual ((double) 654321.123456789, _converter.ConvertTo (new NaDouble (654321.123456789), destinationType));
    Assert.AreEqual ((double) -654321.123456789, _converter.ConvertTo (new NaDouble (-654321.123456789), destinationType));
    Assert.AreEqual ((double) 0, _converter.ConvertTo (NaDouble.Zero, destinationType));
    Assert.AreEqual (double.MinValue, _converter.ConvertTo (NaDouble.MinValue, destinationType));
    Assert.AreEqual (double.MaxValue, _converter.ConvertTo (NaDouble.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToDoubleWithNull()
  {
    Type destinationType = typeof (double);

    _converter.ConvertTo (NaDouble.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDouble()
  {
    Assert.AreEqual (new NaDouble (32), _converter.ConvertFrom ((double) 32));
    Assert.AreEqual (new NaDouble (-32), _converter.ConvertFrom ((double) -32));
    Assert.AreEqual (new NaDouble (654321.123456789), _converter.ConvertFrom ((double) 654321.123456789));
    Assert.AreEqual (new NaDouble (-654321.123456789), _converter.ConvertFrom ((double) -654321.123456789));
    Assert.AreEqual (NaDouble.Zero, _converter.ConvertFrom ((double) 0));
    Assert.AreEqual (NaDouble.MinValue, _converter.ConvertFrom (double.MinValue));
    Assert.AreEqual (NaDouble.MaxValue, _converter.ConvertFrom (double.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaDouble.Null, _converter.ConvertFrom (null));
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
    _converter.ConvertTo (NaDouble.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaDouble.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
