using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.NullableValueTypes;

namespace Remotion.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaDecimalConverterTest
{
  private NaDecimalConverter _converter;
  private CultureInfo _cultureBackup;
  private CultureInfo _cultureEnUs;
  private CultureInfo _cultureDeAt;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaDecimalConverter();
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
    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureEnUs, NaDecimal.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, _cultureEnUs, new NaDecimal (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, _cultureEnUs, new NaDecimal (-32), destinationType));
    Assert.AreEqual ("987654321.123456789", 
        _converter.ConvertTo (null, _cultureEnUs, new NaDecimal (987654321.123456789M), destinationType));
    Assert.AreEqual ("-987654321.123456789", 
        _converter.ConvertTo (null, _cultureEnUs, new NaDecimal (-987654321.123456789M), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, _cultureEnUs, NaDecimal.Zero, destinationType));
    Assert.AreEqual (NaDecimal.MinValue.ToString (_cultureEnUs), 
        _converter.ConvertTo (null, _cultureEnUs, NaDecimal.MinValue, destinationType));
    Assert.AreEqual (NaDecimal.MaxValue.ToString (_cultureEnUs), 
        _converter.ConvertTo (null, _cultureEnUs, NaDecimal.MaxValue, destinationType));
  }

  [Test]
  public void ConvertToStringWithCultureDeAt()
  {
    Type destinationType = typeof (string);
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureDeAt, NaDecimal.Null, destinationType));
    Assert.AreEqual ("32", _converter.ConvertTo (null, _cultureDeAt, new NaDecimal (32), destinationType));
    Assert.AreEqual ("-32", _converter.ConvertTo (null, _cultureDeAt, new NaDecimal (-32), destinationType));
    Assert.AreEqual ("987654321,123456789", 
        _converter.ConvertTo (null, _cultureDeAt, new NaDecimal (987654321.123456789M), destinationType));
    Assert.AreEqual ("-987654321,123456789", 
        _converter.ConvertTo (null, _cultureDeAt, new NaDecimal (-987654321.123456789M), destinationType));
    Assert.AreEqual ("0", _converter.ConvertTo (null, _cultureDeAt, NaDecimal.Zero, destinationType));
    Assert.AreEqual (NaDecimal.MinValue.ToString (_cultureDeAt), 
        _converter.ConvertTo (null, _cultureDeAt, NaDecimal.MinValue, destinationType));
    Assert.AreEqual (NaDecimal.MaxValue.ToString (_cultureDeAt), 
        _converter.ConvertTo (null, _cultureDeAt, NaDecimal.MaxValue, destinationType));
  }

  [Test]
  public void ConvertFromStringWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;

    Assert.AreEqual (NaDecimal.Null, _converter.ConvertFrom (null, _cultureEnUs, ""));
    Assert.AreEqual (new NaDecimal (32), _converter.ConvertFrom (null, _cultureEnUs, "32"));
    Assert.AreEqual (new NaDecimal (-32), _converter.ConvertFrom (null, _cultureEnUs, "-32"));
    Assert.AreEqual (new NaDecimal (987654321.123456789M), 
        _converter.ConvertFrom (null, _cultureEnUs, "987,654,321.123456789"));
    Assert.AreEqual (new NaDecimal (-987654321.123456789M), 
        _converter.ConvertFrom (null, _cultureEnUs, "-987,654,321.123456789"));
    Assert.AreEqual (NaDecimal.Zero, _converter.ConvertFrom (null, _cultureEnUs, "0"));
    Assert.AreEqual (NaDecimal.MinValue, 
        _converter.ConvertFrom (null, _cultureEnUs, NaDecimal.MinValue.ToString (_cultureEnUs)));
    Assert.AreEqual (NaDecimal.MaxValue, 
        _converter.ConvertFrom (null, _cultureEnUs, NaDecimal.MaxValue.ToString (_cultureEnUs)));
  }

  [Test]
  public void ConvertFromStringWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual (NaDecimal.Null, _converter.ConvertFrom (null, _cultureDeAt, ""));
    Assert.AreEqual (new NaDecimal (32), _converter.ConvertFrom (null, _cultureDeAt, "32"));
    Assert.AreEqual (new NaDecimal (-32), _converter.ConvertFrom (null, _cultureDeAt, "-32"));
    Assert.AreEqual (new NaDecimal (987654321.123456789M), 
        _converter.ConvertFrom (null, _cultureDeAt, "987.654.321,123456789"));
    Assert.AreEqual (new NaDecimal (-987654321.123456789M), 
        _converter.ConvertFrom (null, _cultureDeAt, "-987.654.321,123456789"));
    Assert.AreEqual (NaDecimal.Zero, _converter.ConvertFrom (null, _cultureDeAt, "0"));
    Assert.AreEqual (NaDecimal.MinValue, 
        _converter.ConvertFrom (null, _cultureDeAt, NaDecimal.MinValue.ToString (_cultureDeAt)));
    Assert.AreEqual (NaDecimal.MaxValue, 
        _converter.ConvertFrom (null, _cultureDeAt, NaDecimal.MaxValue.ToString (_cultureDeAt)));
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

    Assert.AreEqual (32M, _converter.ConvertTo (null, null, new NaDecimal (32), destinationType));
    Assert.AreEqual (-32M, _converter.ConvertTo (null, null, new NaDecimal (-32), destinationType));
    Assert.AreEqual (987654321.123456789M, _converter.ConvertTo (null, null, new NaDecimal (987654321.123456789M), destinationType));
    Assert.AreEqual (-987654321.123456789M, _converter.ConvertTo (null, null, new NaDecimal (-987654321.123456789M), destinationType));
    Assert.AreEqual (0M, _converter.ConvertTo (null, null, NaDecimal.Zero, destinationType));
    Assert.AreEqual (decimal.MinValue, _converter.ConvertTo (null, null, NaDecimal.MinValue, destinationType));
    Assert.AreEqual (decimal.MaxValue, _converter.ConvertTo (null, null, NaDecimal.MaxValue, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToDecimalWithNull()
  {
    _converter.ConvertTo (null, null, NaDecimal.Null, typeof (decimal));
  }

  [Test]
  public void ConvertFromDecimal()
  {
    Assert.AreEqual (new NaDecimal (32), _converter.ConvertFrom (null, null, 32M));
    Assert.AreEqual (new NaDecimal (-32), _converter.ConvertFrom (null, null, -32M));
    Assert.AreEqual (new NaDecimal (987654321.123456789M), _converter.ConvertFrom (null, null, 987654321.123456789M));
    Assert.AreEqual (new NaDecimal (-987654321.123456789M), _converter.ConvertFrom (null, null, -987654321.123456789M));
    Assert.AreEqual (NaDecimal.Zero, _converter.ConvertFrom (null, null, 0M));
    Assert.AreEqual (NaDecimal.MinValue, _converter.ConvertFrom (null, null, decimal.MinValue));
    Assert.AreEqual (NaDecimal.MaxValue, _converter.ConvertFrom (null, null, decimal.MaxValue));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaDecimal.Null, _converter.ConvertFrom (null, null, null));
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
    _converter.ConvertTo (null, null, NaDecimal.Null, typeof (DBNull));
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaDecimal.Null, _converter.ConvertFrom (null, null, DBNull.Value));
  }
}            

}
