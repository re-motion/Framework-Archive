using System;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Remotion.NullableValueTypes;

namespace Remotion.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaDateTimeConverterTest
{
  private NaDateTimeConverter _converter;
  private CultureInfo _cultureBackup;
  private CultureInfo _cultureEnUs;
  private CultureInfo _cultureDeAt;

  private DateTime _dateTime;
  private NaDateTime _naDateTime;
  private string _dateTimeStringEnUs;
  private string _dateTimeStringDeAt;

  private DateTime _date;
  private NaDateTime _naDate;
  private string _dateStringEnUs;
  private string _dateStringDeAt;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaDateTimeConverter();
    
    _cultureEnUs = new CultureInfo ("en-US");
    _cultureDeAt = new CultureInfo ("de-AT");
    
    _cultureBackup = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;

    _dateTime = new DateTime (2005, 12, 24, 13, 30, 30, 0);
    _naDateTime = new DateTime (2005, 12, 24, 13, 30, 30, 0);
    _dateTimeStringEnUs = "12/24/2005 1:30:30 PM";
    _dateTimeStringDeAt = "24.12.2005 13:30:30";
    
    _date = new DateTime (2005, 12, 24, 0, 0, 0, 0);
    _naDate = new DateTime (2005, 12, 24, 0, 0, 0, 0);
    _dateStringEnUs = "12/24/2005 12:00:00 AM";
    _dateStringDeAt = "24.12.2005 00:00:00";
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
    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureEnUs, NaDateTime.Null, destinationType));
    Assert.AreEqual (_dateTimeStringEnUs, _converter.ConvertTo (null, _cultureEnUs, _naDateTime, destinationType));
    Assert.AreEqual (_dateStringEnUs, _converter.ConvertTo (null, _cultureEnUs, _naDate, destinationType));
  }

  [Test]
  public void ConvertToStringWithCultureDeAt()
  {
    Type destinationType = typeof (string);
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual ("", _converter.ConvertTo (null, _cultureDeAt, NaDateTime.Null, destinationType));
    Assert.AreEqual (_dateTimeStringDeAt, _converter.ConvertTo (null, _cultureDeAt, _naDateTime, destinationType));
    Assert.AreEqual (_dateStringDeAt, _converter.ConvertTo (null, _cultureDeAt, _naDate, destinationType));
  }

  [Test]
  public void ConvertFromStringWithCultureEnUs()
  {
    Thread.CurrentThread.CurrentCulture = _cultureDeAt;

    Assert.AreEqual (NaDateTime.Null, _converter.ConvertFrom (null, _cultureEnUs, ""));
    Assert.AreEqual (_naDateTime, _converter.ConvertFrom (null, _cultureEnUs, _dateTimeStringEnUs));
    Assert.AreEqual (_naDate, _converter.ConvertFrom (null, _cultureEnUs, _dateStringEnUs));
  }

  [Test]
  public void ConvertFromStringWithCultureDeAt()
  {
    Thread.CurrentThread.CurrentCulture = _cultureEnUs;

    Assert.AreEqual (NaDateTime.Null, _converter.ConvertFrom (null, _cultureDeAt, ""));
    Assert.AreEqual (_naDateTime, _converter.ConvertFrom (null, _cultureDeAt, _dateTimeStringDeAt));
    Assert.AreEqual (_naDate, _converter.ConvertFrom (null, _cultureDeAt, _dateStringDeAt));
  }

  [Test]
  [ExpectedException (typeof (FormatException))]
  public void ConvertFromStringEnUsWithCultureDeAt()
  {
    _converter.ConvertFrom (null, _cultureDeAt, _dateTimeStringEnUs);
  }

  [Test]
  [ExpectedException (typeof (FormatException))]
  public void ConvertFromStringDeAtWithCultureEnUs()
  {
    _converter.ConvertFrom (null, _cultureEnUs, _dateTimeStringDeAt);
  }

  [Test]
  public void CanConvertToDateTime()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (DateTime)));
  }

  [Test]
  public void CanConvertFromDateTime()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (DateTime)));
  }

  [Test]
  public void ConvertToDateTime()
  {
    Type destinationType = typeof (DateTime);

    Assert.AreEqual (_dateTime, _converter.ConvertTo (null, null, _naDateTime, destinationType));
    Assert.AreEqual (_date, _converter.ConvertTo (null, null, _naDate, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToDateTimeWithNull()
  {
    _converter.ConvertTo (NaDateTime.Null, typeof (DateTime));
  }

  [Test]
  public void ConvertFromDateTime()
  {
    Assert.AreEqual (_naDateTime, _converter.ConvertFrom (null, null, _dateTime));
    Assert.AreEqual (_naDate, _converter.ConvertFrom (null, null, _date));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaDateTime.Null, _converter.ConvertFrom (null));
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
    _converter.ConvertTo (NaDateTime.Null, typeof (DBNull));
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaDateTime.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
