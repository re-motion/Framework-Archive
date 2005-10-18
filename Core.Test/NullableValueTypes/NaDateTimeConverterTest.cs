using System;
using System.ComponentModel;
using System.Threading;
using System.Globalization;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaDateTimeConverterTest
{
  private NaDateTimeConverter _converter;
  private DateTime _dateTime;
  private NaDateTime _naDateTime;
  private string _dateTimeString;
  private DateTime _date;
  private NaDateTime _naDate;
  private string _dateString;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaDateTimeConverter();
    _dateTime = new DateTime (2005, 12, 24, 13, 30, 30, 0);
    _naDateTime = new NaDateTime (_dateTime);
    _dateTimeString = _dateTime.ToString();
    _date = DateTime.Now.Date;
    _naDate = new NaDateTime (_date);
    _dateString = _date.ToString();
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

    Assert.AreEqual ("", _converter.ConvertTo (NaDateTime.Null, destinationType));
    Assert.AreEqual (_dateTimeString, _converter.ConvertTo (_naDateTime, destinationType));
    Assert.AreEqual (_dateString, _converter.ConvertTo (_naDate, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaDateTime.Null, _converter.ConvertFrom (""));
    Assert.AreEqual (_naDateTime, _converter.ConvertFrom (_dateTimeString));
    Assert.AreEqual (_naDate, _converter.ConvertFrom (_dateString));
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

    Assert.AreEqual (_dateTime, _converter.ConvertTo (_naDateTime, destinationType));
    Assert.AreEqual (_date, _converter.ConvertTo (_naDate, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToDateTimeWithNull()
  {
    Type destinationType = typeof (DateTime);

    _converter.ConvertTo (NaDateTime.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDateTime()
  {
    Assert.AreEqual (_naDateTime, _converter.ConvertFrom (_dateTime));
    Assert.AreEqual (_naDate, _converter.ConvertFrom (_date));
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
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaDateTime.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
