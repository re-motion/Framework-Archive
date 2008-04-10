using System;
using NUnit.Framework;
using Remotion.NullableValueTypes;

namespace Remotion.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaByteConverterTest
{
  private NaByteConverter _converter;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaByteConverter();
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
    Assert.AreEqual ("", _converter.ConvertTo (null, null, NaByte.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, null, new NaByte (32), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, null, NaByte.Zero, destinationType));
    Assert.AreEqual (NaByte.MinValue.ToString(), _converter.ConvertTo (null, null, NaByte.MinValue, destinationType));
    Assert.AreEqual (NaByte.MaxValue.ToString(), _converter.ConvertTo (null, null, NaByte.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaByte.Null, _converter.ConvertFrom (null, null, ""));
    Assert.AreEqual (new NaByte (32), _converter.ConvertFrom (null, null, new NaByte (32).ToString()));
    Assert.AreEqual (NaByte.Zero, _converter.ConvertFrom (null, null, NaByte.Zero.ToString()));
    Assert.AreEqual (NaByte.MinValue, _converter.ConvertFrom (null, null, NaByte.MinValue.ToString()));
    Assert.AreEqual (NaByte.MaxValue, _converter.ConvertFrom (null, null, NaByte.MaxValue.ToString()));
  }

  [Test]
  public void CanConvertToByte()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (byte)));
  }

  [Test]
  public void CanConvertFromByte()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (byte)));
  }

  [Test]
  public void ConvertToByte()
  {
    Type destinationType = typeof (byte);

    Assert.AreEqual ((byte) 32, _converter.ConvertTo (null, null, new NaByte (32), destinationType));
    Assert.AreEqual ((byte) 0, _converter.ConvertTo (null, null, NaByte.Zero, destinationType));
    Assert.AreEqual (byte.MinValue, _converter.ConvertTo (null, null, NaByte.MinValue, destinationType));
    Assert.AreEqual (byte.MaxValue, _converter.ConvertTo (null, null, NaByte.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToByteWithNull()
  {
    _converter.ConvertTo (null, null, NaByte.Null,  typeof (byte));
  }

  [Test]
  public void ConvertFromByte()
  {
    Assert.AreEqual (new NaByte (32), _converter.ConvertFrom (null, null, (byte) 32));
    Assert.AreEqual (NaByte.Zero, _converter.ConvertFrom (null, null, (byte) 0));
    Assert.AreEqual (NaByte.MinValue, _converter.ConvertFrom (null, null, byte.MinValue));
    Assert.AreEqual (NaByte.MaxValue, _converter.ConvertFrom (null, null, byte.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaByte.Null, _converter.ConvertFrom (null, null, null));
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
    _converter.ConvertTo (null, null, NaByte.Null, typeof (DBNull));
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaByte.Null, _converter.ConvertFrom (null, null, DBNull.Value));
  }
}            

}
