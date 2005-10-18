using System;
using System.ComponentModel;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaBooleanConverterTest
{
  private NaBooleanConverter _converter;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaBooleanConverter();
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

    Assert.AreEqual ("", _converter.ConvertTo (NaBoolean.Null, destinationType));
    Assert.AreEqual ("True", _converter.ConvertTo (NaBoolean.True, destinationType));
    Assert.AreEqual ("False", _converter.ConvertTo (NaBoolean.False, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaBoolean.Null, _converter.ConvertFrom (""));
    Assert.AreEqual (NaBoolean.True, _converter.ConvertFrom ("True"));
    Assert.AreEqual (NaBoolean.False, _converter.ConvertFrom ("False"));
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
  public void ConvertToBoolean()
  {
    Type destinationType = typeof (bool);

    Assert.AreEqual (true, _converter.ConvertTo (NaBoolean.True, destinationType));
    Assert.AreEqual (false, _converter.ConvertTo (NaBoolean.False, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToBooleanWithNull()
  {
    Type destinationType = typeof (bool);

    _converter.ConvertTo (NaBoolean.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromBoolean()
  {
    Assert.AreEqual (NaBoolean.True, _converter.ConvertFrom (true));
    Assert.AreEqual (NaBoolean.False, _converter.ConvertFrom (false));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaBoolean.Null, _converter.ConvertFrom (null));
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
    _converter.ConvertTo (NaBoolean.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaBoolean.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
