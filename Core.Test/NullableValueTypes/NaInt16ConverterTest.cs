using System;
using System.ComponentModel;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaInt16ConverterTest
{
  private NaInt16Converter _converter;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaInt16Converter();
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

    Assert.AreEqual ("", _converter.ConvertTo (NaInt16.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (new NaInt16 (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (new NaInt16 (-32), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (NaInt16.Zero, destinationType));
    Assert.AreEqual (NaInt16.MinValue.ToString(), _converter.ConvertTo (NaInt16.MinValue, destinationType));
    Assert.AreEqual (NaInt16.MaxValue.ToString(), _converter.ConvertTo (NaInt16.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaInt16.Null, _converter.ConvertFrom (NaInt16.Null.ToString()));
    Assert.AreEqual (new NaInt16 (32), _converter.ConvertFrom (new NaInt16 (32).ToString()));
    Assert.AreEqual (new NaInt16 (-32), _converter.ConvertFrom (new NaInt16 (-32).ToString()));
    Assert.AreEqual (NaInt16.Zero, _converter.ConvertFrom (NaInt16.Zero.ToString()));
    Assert.AreEqual (NaInt16.MinValue, _converter.ConvertFrom (NaInt16.MinValue.ToString()));
    Assert.AreEqual (NaInt16.MaxValue, _converter.ConvertFrom (NaInt16.MaxValue.ToString()));
  }

  [Test]
  public void CanConvertToInt16()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (short)));
  }

  [Test]
  public void CanConvertFromInt16()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (short)));
  }

  [Test]
  public void ConvertToInt16()
  {
    Type destinationType = typeof (short);

    Assert.AreEqual ((short) 32, _converter.ConvertTo (new NaInt16 (32), destinationType));
    Assert.AreEqual ((short) -32, _converter.ConvertTo (new NaInt16 (-32), destinationType));
    Assert.AreEqual ((short) 0, _converter.ConvertTo (NaInt16.Zero, destinationType));
    Assert.AreEqual (short.MinValue, _converter.ConvertTo (NaInt16.MinValue, destinationType));
    Assert.AreEqual (short.MaxValue, _converter.ConvertTo (NaInt16.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToInt16WithNull()
  {
    Type destinationType = typeof (short);

    _converter.ConvertTo (NaInt16.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromInt16()
  {
    Assert.AreEqual (new NaInt16 (32), _converter.ConvertFrom ((short) 32));
    Assert.AreEqual (new NaInt16 (-32), _converter.ConvertFrom ((short) -32));
    Assert.AreEqual (NaInt16.Zero, _converter.ConvertFrom ((short) 0));
    Assert.AreEqual (NaInt16.MinValue, _converter.ConvertFrom (short.MinValue));
    Assert.AreEqual (NaInt16.MaxValue, _converter.ConvertFrom (short.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaInt16.Null, _converter.ConvertFrom (null));
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
    _converter.ConvertTo (NaInt16.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaInt16.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
