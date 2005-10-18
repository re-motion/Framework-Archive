using System;
using System.ComponentModel;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaInt64ConverterTest
{
  private NaInt64Converter _converter;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaInt64Converter();
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

    Assert.AreEqual ("", _converter.ConvertTo (NaInt64.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (new NaInt64 (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (new NaInt64 (-32), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (NaInt64.Zero, destinationType));
    Assert.AreEqual (NaInt64.MinValue.ToString(), _converter.ConvertTo (NaInt64.MinValue, destinationType));
    Assert.AreEqual (NaInt64.MaxValue.ToString(), _converter.ConvertTo (NaInt64.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaInt64.Null, _converter.ConvertFrom (NaInt64.Null.ToString()));
    Assert.AreEqual (new NaInt64 (32), _converter.ConvertFrom (new NaInt64 (32).ToString()));
    Assert.AreEqual (new NaInt64 (-32), _converter.ConvertFrom (new NaInt64 (-32).ToString()));
    Assert.AreEqual (NaInt64.Zero, _converter.ConvertFrom (NaInt64.Zero.ToString()));
    Assert.AreEqual (NaInt64.MinValue, _converter.ConvertFrom (NaInt64.MinValue.ToString()));
    Assert.AreEqual (NaInt64.MaxValue, _converter.ConvertFrom (NaInt64.MaxValue.ToString()));
  }

  [Test]
  public void CanConvertToInt64()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (long)));
  }

  [Test]
  public void CanConvertFromInt64()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (long)));
  }

  [Test]
  public void ConvertToInt64()
  {
    Type destinationType = typeof (long);

    Assert.AreEqual ((long) 32, _converter.ConvertTo (new NaInt64 (32), destinationType));
    Assert.AreEqual ((long) -32, _converter.ConvertTo (new NaInt64 (-32), destinationType));
    Assert.AreEqual ((long) 0, _converter.ConvertTo (NaInt64.Zero, destinationType));
    Assert.AreEqual (long.MinValue, _converter.ConvertTo (NaInt64.MinValue, destinationType));
    Assert.AreEqual (long.MaxValue, _converter.ConvertTo (NaInt64.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToInt64WithNull()
  {
    Type destinationType = typeof (long);

    _converter.ConvertTo (NaInt64.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromInt64()
  {
    Assert.AreEqual (new NaInt64 (32), _converter.ConvertFrom ((long) 32));
    Assert.AreEqual (new NaInt64 (-32), _converter.ConvertFrom ((long) -32));
    Assert.AreEqual (NaInt64.Zero, _converter.ConvertFrom ((long) 0));
    Assert.AreEqual (NaInt64.MinValue, _converter.ConvertFrom (long.MinValue));
    Assert.AreEqual (NaInt64.MaxValue, _converter.ConvertFrom (long.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaInt64.Null, _converter.ConvertFrom (null));
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
    _converter.ConvertTo (NaInt64.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaInt64.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
