using System;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaSingleConverterTest
{
  private NaSingleConverter _converter;
  private CultureInfo _culture;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaSingleConverter();
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

    Assert.AreEqual ("", _converter.ConvertTo (NaSingle.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (new NaSingle (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (new NaSingle (-32), destinationType));
    Assert.AreEqual ("321.1234", _converter.ConvertTo (new NaSingle (321.1234F), destinationType));
    Assert.AreEqual ("-321.1234", _converter.ConvertTo (new NaSingle (-321.1234F), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (NaSingle.Zero, destinationType));
    Assert.AreEqual (NaSingle.MinValue.ToString(), _converter.ConvertTo (NaSingle.MinValue, destinationType));
    Assert.AreEqual (NaSingle.MaxValue.ToString(), _converter.ConvertTo (NaSingle.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaSingle.Null, _converter.ConvertFrom (NaSingle.Null.ToString()));
    Assert.AreEqual (new NaSingle (32), _converter.ConvertFrom (new NaSingle (32).ToString()));
    Assert.AreEqual (new NaSingle (-32), _converter.ConvertFrom (new NaSingle (-32).ToString()));
    Assert.AreEqual (new NaSingle (321.1234F), _converter.ConvertFrom (new NaSingle (321.1234F).ToString()));
    Assert.AreEqual (new NaSingle (-321.1234F), _converter.ConvertFrom (new NaSingle (-321.1234F).ToString()));
    Assert.AreEqual (NaSingle.Zero, _converter.ConvertFrom (NaSingle.Zero.ToString()));
   }

  [Test]
  [Ignore ("Precision Problem in Single.ToString().")]
  public void ConvertFromStringMaxAndMinValues()
  {
    Assert.AreEqual (NaSingle.MinValue, _converter.ConvertFrom (NaSingle.MinValue.ToString()));
    Assert.AreEqual (NaSingle.MaxValue, _converter.ConvertFrom (NaSingle.MaxValue.ToString()));
  }

  [Test]
  public void CanConvertToSingle()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (float)));
  }

  [Test]
  public void CanConvertFromSingle()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (float)));
  }

  [Test]
  public void ConvertToSingle()
  {
    Type destinationType = typeof (float);

    Assert.AreEqual ((float) 32, _converter.ConvertTo (new NaSingle (32), destinationType));
    Assert.AreEqual ((float) -32, _converter.ConvertTo (new NaSingle (-32), destinationType));
    Assert.AreEqual ((float) 321.1234F, _converter.ConvertTo (new NaSingle (321.1234F), destinationType));
    Assert.AreEqual ((float) -321.1234F, _converter.ConvertTo (new NaSingle (-321.1234F), destinationType));
    Assert.AreEqual ((float) 0, _converter.ConvertTo (NaSingle.Zero, destinationType));
    Assert.AreEqual (float.MinValue, _converter.ConvertTo (NaSingle.MinValue, destinationType));
    Assert.AreEqual (float.MaxValue, _converter.ConvertTo (NaSingle.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToSingleWithNull()
  {
    Type destinationType = typeof (float);

    _converter.ConvertTo (NaSingle.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromSingle()
  {
    Assert.AreEqual (new NaSingle (32), _converter.ConvertFrom ((float) 32));
    Assert.AreEqual (new NaSingle (-32), _converter.ConvertFrom ((float) -32));
    Assert.AreEqual (new NaSingle (321.1234F), _converter.ConvertFrom ((float) 321.1234F));
    Assert.AreEqual (new NaSingle (-321.1234F), _converter.ConvertFrom ((float) -321.1234F));
    Assert.AreEqual (NaSingle.Zero, _converter.ConvertFrom ((float) 0));
    Assert.AreEqual (NaSingle.MinValue, _converter.ConvertFrom (float.MinValue));
    Assert.AreEqual (NaSingle.MaxValue, _converter.ConvertFrom (float.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaSingle.Null, _converter.ConvertFrom (null));
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
    _converter.ConvertTo (NaSingle.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaSingle.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
