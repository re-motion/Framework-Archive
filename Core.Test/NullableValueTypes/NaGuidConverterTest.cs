using System;
using System.ComponentModel;
using NUnit.Framework;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.NullableValueTypes
{

[TestFixture]
public class NaGuidConverterTest
{
  private NaGuidConverter _converter;
  private Guid _guid;
  private NaGuid _naGuid;
  private string _guidString;
  
  [SetUp]
  public void SetUp()
  {
    _converter = new NaGuidConverter();
    _guid = Guid.NewGuid();
    _naGuid = new NaGuid (_guid);
    _guidString = _guid.ToString();
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

    Assert.AreEqual ("", _converter.ConvertTo (NaGuid.Null, destinationType));
    Assert.AreEqual (Guid.Empty.ToString(), _converter.ConvertTo (NaGuid.Empty, destinationType));
    Assert.AreEqual (_guidString, _converter.ConvertTo (_naGuid, destinationType));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (NaGuid.Null, _converter.ConvertFrom (""));
    Assert.AreEqual (NaGuid.Empty, _converter.ConvertFrom (Guid.Empty.ToString()));
    Assert.AreEqual (_naGuid, _converter.ConvertFrom (_guidString));
  }

  [Test]
  public void CanConvertToGuid()
  {
    Assert.IsTrue (_converter.CanConvertTo (typeof (Guid)));
  }

  [Test]
  public void CanConvertFromGuid()
  {
    Assert.IsTrue (_converter.CanConvertFrom (typeof (Guid)));
  }

  [Test]
  public void ConvertToGuid()
  {
    Type destinationType = typeof (Guid);

    Assert.AreEqual (Guid.Empty, _converter.ConvertTo (NaGuid.Empty, destinationType));
    Assert.AreEqual (_guid, _converter.ConvertTo (_naGuid, destinationType));
  }

  [Test]
  [ExpectedException (typeof (NaNullValueException))]
  public void ConvertToGuidWithNull()
  {
    Type destinationType = typeof (Guid);

    _converter.ConvertTo (NaGuid.Null, destinationType);
    Assert.Fail();
  }

  [Test]
  public void ConvertFromGuid()
  {
    Assert.AreEqual (NaGuid.Empty, _converter.ConvertFrom (Guid.Empty));
    Assert.AreEqual (_naGuid, _converter.ConvertFrom (_guid));
  }

  [Test]
  public void ConvertFromNull()
  {
    Assert.AreEqual (NaGuid.Null, _converter.ConvertFrom (null));
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
    _converter.ConvertTo (NaGuid.Null, typeof (DBNull));
    Assert.Fail();
  }

  [Test]
  public void ConvertFromDBNull()
  {
    Assert.AreEqual (NaGuid.Null, _converter.ConvertFrom (DBNull.Value));
  }
}            

}
