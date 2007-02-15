using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaSingleConverterTest
{
  private NaSingleConverter _converter;
  private CultureInfo _cultureBackup;
  private CultureInfo _cultureEnUs;
  private CultureInfo _cultureDeAt;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaSingleConverter();
    _cultureEnUs = new CultureInfo ("en-US");
    _cultureDeAt = new CultureInfo ("de-AT");
    
    _cultureBackup = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
  }

  [TearDown]
  public void TearDown()
  {
    Thread.CurrentThread.CurrentCulture = _cultureBackup;
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
  public void ConvertToStringWithCultureEnUs()
  {
    Type destinationType = typeof (string);
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;

    Assert.AreEqual ("", _converter.ConvertTo (null, null, null, destinationType));
    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureEnUs, NaSingle.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, _cultureEnUs, new NaSingle (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, _cultureEnUs, new NaSingle (-32), destinationType));
    Assert.AreEqual ("4321.123", 
        _converter.ConvertTo (null, _cultureEnUs, new NaSingle (4321.123F), destinationType));
    Assert.AreEqual ("-4321.123", 
        _converter.ConvertTo (null, _cultureEnUs, new NaSingle (-4321.123F), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, _cultureEnUs, NaSingle.Zero, destinationType));
    Assert.AreEqual (NaSingle.MinValue.ToString ("R", _cultureEnUs), 
        _converter.ConvertTo (null, _cultureEnUs, NaSingle.MinValue, destinationType));
    Assert.AreEqual (NaSingle.MaxValue.ToString ("R", _cultureEnUs),
        _converter.ConvertTo (null, _cultureEnUs, NaSingle.MaxValue, destinationType));
  }

  [Test]
  public void ConvertToStringWithCultureDeAt()
  {
    Type destinationType = typeof (string);
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureDeAt, NaSingle.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, _cultureDeAt, new NaSingle (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, _cultureDeAt, new NaSingle (-32), destinationType));
    Assert.AreEqual ("4321,123", 
        _converter.ConvertTo (null, _cultureDeAt, new NaSingle (4321.123F), destinationType));
    Assert.AreEqual ("-4321,123", 
        _converter.ConvertTo (null, _cultureDeAt, new NaSingle (-4321.123F), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, _cultureDeAt, NaSingle.Zero, destinationType));
    Assert.AreEqual (NaSingle.MinValue.ToString ("R", _cultureDeAt), 
        _converter.ConvertTo (null, _cultureDeAt, NaSingle.MinValue, destinationType));
    Assert.AreEqual (NaSingle.MaxValue.ToString ("R", _cultureDeAt),
        _converter.ConvertTo (null, _cultureDeAt, NaSingle.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromStringWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;

    Assert.AreEqual (NaSingle.Null, _converter.ConvertFrom (null, _cultureEnUs, ""));
    Assert.AreEqual (new NaSingle (32), _converter.ConvertFrom (null, _cultureEnUs, "32"));
    Assert.AreEqual (new NaSingle (-32), _converter.ConvertFrom (null, _cultureEnUs, "-32"));
    Assert.AreEqual (new NaSingle (4321.123F), 
        _converter.ConvertFrom (null, _cultureEnUs, "4,321.123"));
    Assert.AreEqual (new NaSingle (-4321.123F), 
        _converter.ConvertFrom (null, _cultureEnUs, "-4,321.123"));
    Assert.AreEqual (NaSingle.Zero, _converter.ConvertFrom (null, _cultureEnUs, "0"));
    Assert.AreEqual (NaSingle.MinValue, 
        _converter.ConvertFrom (null, _cultureEnUs, NaSingle.MinValue.ToString ("R", _cultureEnUs)));
    Assert.AreEqual (NaSingle.MaxValue, 
        _converter.ConvertFrom (null, _cultureEnUs, NaSingle.MaxValue.ToString ("R", _cultureEnUs)));
  }

  [Test]
  public void ConvertFromStringWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual (NaSingle.Null, _converter.ConvertFrom (null, _cultureDeAt, ""));
    Assert.AreEqual (new NaSingle (32), _converter.ConvertFrom (null, _cultureDeAt, "32"));
    Assert.AreEqual (new NaSingle (-32), _converter.ConvertFrom (null, _cultureDeAt, "-32"));
    Assert.AreEqual (new NaSingle (4321.123F), 
        _converter.ConvertFrom (null, _cultureDeAt, "4.321,123"));
    Assert.AreEqual (new NaSingle (-4321.123F), 
        _converter.ConvertFrom (null, _cultureDeAt, "-4.321,123"));
    Assert.AreEqual (NaSingle.Zero, _converter.ConvertFrom (null, _cultureDeAt, "0"));
    Assert.AreEqual (NaSingle.MinValue, 
        _converter.ConvertFrom (null, _cultureDeAt, NaSingle.MinValue.ToString ("R", _cultureDeAt)));
    Assert.AreEqual (NaSingle.MaxValue, 
        _converter.ConvertFrom (null, _cultureDeAt, NaSingle.MaxValue.ToString ("R", _cultureDeAt)));
  }

  [Test]
  [ExpectedException (typeof (FormatException))]
  public void ConvertFromStringEnUsWithCultureDeAt()
  {
    object value = _converter.ConvertFrom (null, _cultureDeAt, "100,001.1");
  }

  [Test]
  [ExpectedException (typeof (FormatException))]
  public void ConvertFromStringDeAtWithCultureEnUs()
  {
    object value = _converter.ConvertFrom (null, _cultureEnUs, "100.001,1");
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

    Assert.AreEqual (32F, _converter.ConvertTo (null, null, new NaSingle (32), destinationType));
    Assert.AreEqual (-32F, _converter.ConvertTo (null, null, new NaSingle (-32), destinationType));
    Assert.AreEqual (321.1234F, _converter.ConvertTo (null, null, new NaSingle (321.1234F), destinationType));
    Assert.AreEqual (-321.1234F, _converter.ConvertTo (null, null, new NaSingle (-321.1234F), destinationType));
    Assert.AreEqual (0F, _converter.ConvertTo (null, null, NaSingle.Zero, destinationType));
    Assert.AreEqual (float.MinValue, _converter.ConvertTo (null, null, NaSingle.MinValue, destinationType));
    Assert.AreEqual (float.MaxValue, _converter.ConvertTo (null, null, NaSingle.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToSingleWithNull()
  {
    _converter.ConvertTo (null, null, NaSingle.Null, typeof (float));
  }

  [Test]
  public void ConvertFromSingle()
  {
    Assert.AreEqual (new NaSingle (32), _converter.ConvertFrom (null, null, 32F));
    Assert.AreEqual (new NaSingle (-32), _converter.ConvertFrom (null, null, -32F));
    Assert.AreEqual (new NaSingle (321.1234F), _converter.ConvertFrom (null, null, 321.1234F));
    Assert.AreEqual (new NaSingle (-321.1234F), _converter.ConvertFrom (null, null, -321.1234F));
    Assert.AreEqual (NaSingle.Zero, _converter.ConvertFrom (null, null, 0F));
    Assert.AreEqual (NaSingle.MinValue, _converter.ConvertFrom (null, null, float.MinValue));
    Assert.AreEqual (NaSingle.MaxValue, _converter.ConvertFrom (null, null, float.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaSingle.Null, _converter.ConvertFrom (null, null, null));
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
    _converter.ConvertTo (null, null, NaSingle.Null, typeof (DBNull));
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaSingle.Null, _converter.ConvertFrom (null, null, DBNull.Value));
  }
}            

}
