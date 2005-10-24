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
  private CultureInfo _cultureBackup;
  private CultureInfo _cultureEnUs;
  private CultureInfo _cultureDeAt;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaDoubleConverter();
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
    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureEnUs, NaDouble.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, _cultureEnUs, new NaDouble (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, _cultureEnUs, new NaDouble (-32), destinationType));
    Assert.AreEqual ("654321.123456789", 
        _converter.ConvertTo (null, _cultureEnUs, new NaDouble (654321.123456789), destinationType));
    Assert.AreEqual ("-654321.123456789", 
        _converter.ConvertTo (null, _cultureEnUs, new NaDouble (-654321.123456789), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, _cultureEnUs, NaDouble.Zero, destinationType));
    Assert.AreEqual (NaDouble.MinValue.ToString ("R", _cultureEnUs), 
        _converter.ConvertTo (null, _cultureEnUs, NaDouble.MinValue, destinationType));
    Assert.AreEqual (NaDouble.MaxValue.ToString ("R", _cultureEnUs), 
        _converter.ConvertTo (null, _cultureEnUs, NaDouble.MaxValue, destinationType));
  }

  [Test]
  public void ConvertToStringWithCultureDeAt()
  {
    Type destinationType = typeof (string);
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureDeAt, NaDouble.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, _cultureDeAt, new NaDouble (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, _cultureDeAt, new NaDouble (-32), destinationType));
    Assert.AreEqual ("654321,123456789", 
        _converter.ConvertTo (null, _cultureDeAt, new NaDouble (654321.123456789), destinationType));
    Assert.AreEqual ("-654321,123456789", 
        _converter.ConvertTo (null, _cultureDeAt, new NaDouble (-654321.123456789), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, _cultureDeAt, NaDouble.Zero, destinationType));
    Assert.AreEqual (NaDouble.MinValue.ToString ("R", _cultureDeAt), 
        _converter.ConvertTo (null, _cultureDeAt, NaDouble.MinValue, destinationType));
    Assert.AreEqual (NaDouble.MaxValue.ToString ("R", _cultureDeAt), 
        _converter.ConvertTo (null, _cultureDeAt, NaDouble.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromStringWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;

    Assert.AreEqual (NaDouble.Null, _converter.ConvertFrom (null, _cultureEnUs, ""));
    Assert.AreEqual (new NaDouble (32), _converter.ConvertFrom (null, _cultureEnUs, "32"));
    Assert.AreEqual (new NaDouble (-32), _converter.ConvertFrom (null, _cultureEnUs, "-32"));
    Assert.AreEqual (new NaDouble (654321.123456789), 
        _converter.ConvertFrom (null, _cultureEnUs, "654,321.123456789"));
    Assert.AreEqual (new NaDouble (-654321.123456789), 
        _converter.ConvertFrom (null, _cultureEnUs, "-654,321.123456789"));
    Assert.AreEqual (NaDouble.Zero, _converter.ConvertFrom (null, _cultureEnUs, "0"));
    Assert.AreEqual (NaDouble.MinValue, 
        _converter.ConvertFrom (null, _cultureEnUs, NaDouble.MinValue.ToString ("R", _cultureEnUs)));
    Assert.AreEqual (NaDouble.MaxValue, 
        _converter.ConvertFrom (null, _cultureEnUs, NaDouble.MaxValue.ToString ("R", _cultureEnUs)));
  }

  [Test]
  public void ConvertFromStringWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual (NaDouble.Null, _converter.ConvertFrom (null, _cultureDeAt, ""));
    Assert.AreEqual (new NaDouble (32), _converter.ConvertFrom (null, _cultureDeAt, "32"));
    Assert.AreEqual (new NaDouble (-32), _converter.ConvertFrom (null, _cultureDeAt, "-32"));
    Assert.AreEqual (new NaDouble (654321.123456789), 
        _converter.ConvertFrom (null, _cultureDeAt, "654.321,123456789"));
    Assert.AreEqual (new NaDouble (-654321.123456789), 
        _converter.ConvertFrom (null, _cultureDeAt, "-654.321,123456789"));
    Assert.AreEqual (NaDouble.Zero, _converter.ConvertFrom (null, _cultureDeAt, "0"));
    Assert.AreEqual (NaDouble.MinValue, 
        _converter.ConvertFrom (null, _cultureDeAt, NaDouble.MinValue.ToString ("R", _cultureDeAt)));
    Assert.AreEqual (NaDouble.MaxValue, 
        _converter.ConvertFrom (null, _cultureDeAt, NaDouble.MaxValue.ToString ("R", _cultureDeAt)));
  }

  [Test]
  [ExpectedException (typeof (FormatException))]
  public void ConvertFromStringEnUsWithCultureDeAt()
  {
    object value = _converter.ConvertFrom (null, _cultureDeAt, "100,001.1");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (FormatException))]
  public void ConvertFromStringDeAtWithCultureEnUs()
  {
    object value = _converter.ConvertFrom (null, _cultureEnUs, "100.001,1");
    Assert.Fail();
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

    Assert.AreEqual (32d, _converter.ConvertTo (null, null, new NaDouble (32), destinationType));
    Assert.AreEqual (-32d, _converter.ConvertTo (null, null, new NaDouble (-32), destinationType));
    Assert.AreEqual (654321.123456789, _converter.ConvertTo (null, null, new NaDouble (654321.123456789), destinationType));
    Assert.AreEqual (-654321.123456789, _converter.ConvertTo (null, null, new NaDouble (-654321.123456789), destinationType));
    Assert.AreEqual (0d, _converter.ConvertTo (null, null, NaDouble.Zero, destinationType));
    Assert.AreEqual (double.MinValue, _converter.ConvertTo (null, null, NaDouble.MinValue, destinationType));
    Assert.AreEqual (double.MaxValue, _converter.ConvertTo (null, null, NaDouble.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToDoubleWithNull()
  {
    Type destinationType = typeof (double);

    _converter.ConvertTo (null, null, NaDouble.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDouble()
  {
    Assert.AreEqual (new NaDouble (32), _converter.ConvertFrom (null, null, 32d));
    Assert.AreEqual (new NaDouble (-32), _converter.ConvertFrom (null, null, -32d));
    Assert.AreEqual (new NaDouble (654321.123456789), _converter.ConvertFrom (null, null, 654321.123456789));
    Assert.AreEqual (new NaDouble (-654321.123456789), _converter.ConvertFrom (null, null, -654321.123456789));
    Assert.AreEqual (NaDouble.Zero, _converter.ConvertFrom (null, null, 0d));
    Assert.AreEqual (NaDouble.MinValue, _converter.ConvertFrom (null, null, double.MinValue));
    Assert.AreEqual (NaDouble.MaxValue, _converter.ConvertFrom (null, null, double.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaDouble.Null, _converter.ConvertFrom (null, null, null));
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
    _converter.ConvertTo (null, null, NaDouble.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaDouble.Null, _converter.ConvertFrom (null, null, DBNull.Value));
  }
}            

}
