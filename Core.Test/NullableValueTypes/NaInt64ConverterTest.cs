using System;
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

    Assert.AreEqual ("", _converter.ConvertTo (null, null,null, destinationType));
    Assert.AreEqual ("", _converter.ConvertTo (null, null, NaInt64.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, null, new NaInt64 (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, null, new NaInt64 (-32), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, null, NaInt64.Zero, destinationType));
    Assert.AreEqual (NaInt64.MinValue.ToString(), _converter.ConvertTo (null, null, NaInt64.MinValue, destinationType));
    Assert.AreEqual (NaInt64.MaxValue.ToString(), _converter.ConvertTo (null, null, NaInt64.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaInt64.Null, _converter.ConvertFrom (null, null, ""));
    Assert.AreEqual (new NaInt64 (32), _converter.ConvertFrom (null, null, "32"));
    Assert.AreEqual (new NaInt64 (-32), _converter.ConvertFrom (null, null, "-32"));
    Assert.AreEqual (NaInt64.Zero, _converter.ConvertFrom (null, null, "0"));
    Assert.AreEqual (NaInt64.MinValue, _converter.ConvertFrom (null, null, NaInt64.MinValue.ToString()));
    Assert.AreEqual (NaInt64.MaxValue, _converter.ConvertFrom (null, null, NaInt64.MaxValue.ToString()));
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

    Assert.AreEqual (32L, _converter.ConvertTo (null, null, new NaInt64 (32), destinationType));
    Assert.AreEqual (-32L, _converter.ConvertTo (null, null, new NaInt64 (-32), destinationType));
    Assert.AreEqual (0L, _converter.ConvertTo (null, null, NaInt64.Zero, destinationType));
    Assert.AreEqual (long.MinValue, _converter.ConvertTo (null, null, NaInt64.MinValue, destinationType));
    Assert.AreEqual (long.MaxValue, _converter.ConvertTo (null, null, NaInt64.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToInt64WithNull()
  {
    _converter.ConvertTo (null, null, NaInt64.Null, typeof (long));
  }

  [Test]
  public void ConvertFromInt64()
  {
    Assert.AreEqual (new NaInt64 (32), _converter.ConvertFrom (null, null, 32L));
    Assert.AreEqual (new NaInt64 (-32), _converter.ConvertFrom (null, null, -32L));
    Assert.AreEqual (NaInt64.Zero, _converter.ConvertFrom (null, null, 0L));
    Assert.AreEqual (NaInt64.MinValue, _converter.ConvertFrom (null, null, long.MinValue));
    Assert.AreEqual (NaInt64.MaxValue, _converter.ConvertFrom (null, null, long.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaInt64.Null, _converter.ConvertFrom (null, null, null));
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
    _converter.ConvertTo (null, null, NaInt64.Null, typeof (DBNull));
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaInt64.Null, _converter.ConvertFrom (null, null, DBNull.Value));
  }
}            

}
