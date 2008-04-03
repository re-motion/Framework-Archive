using System;
using NUnit.Framework;
using Remotion.NullableValueTypes;

namespace Remotion.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaInt32ConverterTest
{
  private NaInt32Converter _converter;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaInt32Converter();
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

    Assert.AreEqual ("", _converter.ConvertTo (null, null, null, destinationType));
    Assert.AreEqual ("", _converter.ConvertTo (null, null, NaInt32.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, null, new NaInt32 (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, null, new NaInt32 (-32), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, null, NaInt32.Zero, destinationType));
    Assert.AreEqual (NaInt32.MinValue.ToString(), _converter.ConvertTo (null, null, NaInt32.MinValue, destinationType));
    Assert.AreEqual (NaInt32.MaxValue.ToString(), _converter.ConvertTo (null, null, NaInt32.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaInt32.Null, _converter.ConvertFrom (null, null, ""));
    Assert.AreEqual (new NaInt32 (32), _converter.ConvertFrom (null, null, "32"));
    Assert.AreEqual (new NaInt32 (-32), _converter.ConvertFrom (null, null, "-32"));
    Assert.AreEqual (NaInt32.Zero, _converter.ConvertFrom (null, null, "0"));
    Assert.AreEqual (NaInt32.MinValue, _converter.ConvertFrom (null, null, NaInt32.MinValue.ToString()));
    Assert.AreEqual (NaInt32.MaxValue, _converter.ConvertFrom (null, null, NaInt32.MaxValue.ToString()));
  }

  [Test]
  public void CanConvertToInt32()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (int)));
  }

  [Test]
  public void CanConvertFromInt32()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (int)));
  }

  [Test]
  public void ConvertToInt32()
  {
    Type destinationType = typeof (int);

    Assert.AreEqual ((int) 32, _converter.ConvertTo (null, null, new NaInt32 (32), destinationType));
    Assert.AreEqual ((int) -32, _converter.ConvertTo (null, null, new NaInt32 (-32), destinationType));
    Assert.AreEqual ((int) 0, _converter.ConvertTo (null, null, NaInt32.Zero, destinationType));
    Assert.AreEqual (int.MinValue, _converter.ConvertTo (null, null, NaInt32.MinValue, destinationType));
    Assert.AreEqual (int.MaxValue, _converter.ConvertTo (null, null, NaInt32.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToInt32WithNull()
  {
    _converter.ConvertTo (null, null, NaInt32.Null, typeof (int));
  }

  [Test]
  public void ConvertFromInt32()
  {
    Assert.AreEqual (new NaInt32 (32), _converter.ConvertFrom (null, null, (int) 32));
    Assert.AreEqual (new NaInt32 (-32), _converter.ConvertFrom (null, null, (int) -32));
    Assert.AreEqual (NaInt32.Zero, _converter.ConvertFrom (null, null, (int) 0));
    Assert.AreEqual (NaInt32.MinValue, _converter.ConvertFrom (null, null, int.MinValue));
    Assert.AreEqual (NaInt32.MaxValue, _converter.ConvertFrom (null, null, int.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaInt32.Null, _converter.ConvertFrom (null, null, null));
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
    _converter.ConvertTo (null, null, NaInt32.Null, typeof (DBNull));
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaInt32.Null, _converter.ConvertFrom (null, null, DBNull.Value));
  }
}            

}
