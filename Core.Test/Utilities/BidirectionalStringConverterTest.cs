using System;
using System.ComponentModel;
using System.Globalization;
using System.Threading;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class BidirectionalStringConverterTest
{
  private BidirectionalStringConverter _converter;
  private CultureInfo _culture;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new BidirectionalStringConverter();
    _culture = Thread.CurrentThread.CurrentCulture;
    Thread.CurrentThread.CurrentCulture = new CultureInfo ("en-US");
  }

  [TearDown]
  public void TearDown()
  {
    Thread.CurrentThread.CurrentCulture = _culture;
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
  public void CanConvertToBoolean()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (bool)));
  }

  [Test]
  public void CanConvertFromBoolean()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (bool)));
  }

  [Test]
  public void CanConvertToInt32Array()
  {
    Assert.IsFalse (_converter.CanConvertTo (typeof (int[])));
  }

  [Test]
  public void CanConvertFromInt32Array()
  {
    Assert.IsFalse (_converter.CanConvertFrom (typeof (int[])));
  }

  [Test]
  public void CanConvertToObject()
  {
    Assert.IsFalse (_converter.CanConvertTo (typeof (object)));
  }

  [Test]
  public void CanConvertFromObject()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (object)));
  }


  [Test]
  public void ConvertToByte()
  {
    Type destinationType = typeof (short);

    Assert.AreEqual ((short) 0, _converter.ConvertTo ("0", destinationType));
    Assert.AreEqual ((short) 1, _converter.ConvertTo ("1", destinationType));
  }

  [Test]
  public void ConvertFromByte()
  {
    Assert.AreEqual ("0", _converter.ConvertFrom ((byte) 0));
    Assert.AreEqual ("1", _converter.ConvertFrom ((byte) 1));
  }

  [Test]
  public void ConvertToInt16()
  {
    Type destinationType = typeof (byte);

    Assert.AreEqual ((byte) 0, _converter.ConvertTo ("0", destinationType));
    Assert.AreEqual ((byte) 1, _converter.ConvertTo ("1", destinationType));
  }

  [Test]
  public void ConvertFromInt16()
  {
    Assert.AreEqual ("0", _converter.ConvertFrom ((short) 0));
    Assert.AreEqual ("1", _converter.ConvertFrom ((short) 1));
  }

  [Test]
  public void ConvertToInt32()
  {
    Type destinationType = typeof (int);

    Assert.AreEqual (0, _converter.ConvertTo ("0", destinationType));
    Assert.AreEqual (1, _converter.ConvertTo ("1", destinationType));
  }

  [Test]
  public void ConvertFromInt32()
  {
    Assert.AreEqual ("0", _converter.ConvertFrom (0));
    Assert.AreEqual ("1", _converter.ConvertFrom (1));
  }

  [Test]
  public void ConvertToInt64()
  {
    Type destinationType = typeof (long);

    Assert.AreEqual (0L, _converter.ConvertTo ("0", destinationType));
    Assert.AreEqual (1L, _converter.ConvertTo ("1", destinationType));
  }

  [Test]
  public void ConvertFromInt64()
  {
    Assert.AreEqual ("0", _converter.ConvertFrom (0L));
    Assert.AreEqual ("1", _converter.ConvertFrom (1L));
  }

  [Test]
  public void ConvertToSingle()
  {
    Type destinationType = typeof (float);

    Assert.AreEqual (0.5F, _converter.ConvertTo ("0.5", destinationType));
    Assert.AreEqual (1.5F, _converter.ConvertTo ("1.5", destinationType));
  }

  [Test]
  public void ConvertFromSingle()
  {
    Assert.AreEqual ("0.5", _converter.ConvertFrom (0.5F));
    Assert.AreEqual ("1.5", _converter.ConvertFrom (1.5F));
  }

  [Test]
  public void ConvertToDouble()
  {
    Type destinationType = typeof (double);

    Assert.AreEqual (0.5, _converter.ConvertTo ("0.5", destinationType));
    Assert.AreEqual (1.5, _converter.ConvertTo ("1.5", destinationType));
  }

  [Test]
  public void ConvertFromDouble()
  {
    Assert.AreEqual ("0.5", _converter.ConvertFrom (0.5));
    Assert.AreEqual ("1.5", _converter.ConvertFrom (1.5));
  }

  [Test]
  public void ConvertToDateTime()
  {
    Type destinationType = typeof (DateTime);
    DateTime dateTime = new DateTime (2005, 12, 24, 13, 30, 30, 0);
    string dateTimeString = dateTime.ToString();

    Assert.AreEqual (dateTime, _converter.ConvertTo (dateTimeString, destinationType));
  }

  [Test]
  public void ConvertFromDateTime()
  {
    DateTime dateTime = new DateTime (2005, 12, 24, 13, 30, 30, 0);
    string dateTimeString = dateTime.ToString();

    Assert.AreEqual (dateTimeString, _converter.ConvertFrom (dateTime));
  }

  [Test]
  public void ConvertToBoolean()
  {
    Type destinationType = typeof (bool);

    Assert.AreEqual (true, _converter.ConvertTo ("True", destinationType));
    Assert.AreEqual (false, _converter.ConvertTo ("False", destinationType));
  }

  [Test]
  public void ConvertFromBoolean()
  {
    Assert.AreEqual ("True", _converter.ConvertFrom (true));
    Assert.AreEqual ("False", _converter.ConvertFrom (false));
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertToInt32Array()
  {
    Type destinationType = typeof (int[]);
    _converter.ConvertTo ("0, 1", destinationType);
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt32Array()
  {
    _converter.ConvertFrom (new int[] {0, 1});
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (ArgumentNullException))]
  public void ConvertToInt32WithNull()
  {
    Type destinationType = typeof (int);

    _converter.ConvertTo (null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual ("", _converter.ConvertFrom (null));
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
    _converter.ConvertTo ("", typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual ("", _converter.ConvertFrom (DBNull.Value));
  }
}            

}
