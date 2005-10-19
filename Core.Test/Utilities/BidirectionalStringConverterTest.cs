using System;
using System.ComponentModel;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class BidirectionalStringConverterTest
{
  private BidirectionalStringConverter _converter;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new BidirectionalStringConverter();
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
