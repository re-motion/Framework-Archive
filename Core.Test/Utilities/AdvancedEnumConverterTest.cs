using System;
using NUnit.Framework;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class AdvancedEnumConverterTest
{
  private AdvancedEnumConverter _int32EnumConverter;
  private AdvancedEnumConverter _int16EnumConverter;

  public enum Int32Enum: int
  {
    ValueA = 0,
    ValueB = 1
  }

  public enum Int16Enum: short
  {
    ValueA = 0,
    ValueB = 1
  }
  
  [SetUp]
  public void SetUp()
  {
    _int32EnumConverter = new AdvancedEnumConverter (typeof (Int32Enum));
    _int16EnumConverter = new AdvancedEnumConverter (typeof (Int16Enum));
  }

  [Test]
  public void CanConvertFromString()
  {
    Assert.IsTrue (_int32EnumConverter.CanConvertFrom (typeof (string)));
  }

  [Test]
  public void CanConvertToString()
  {
    Assert.IsTrue (_int32EnumConverter.CanConvertTo (typeof (string)));
  }

  [Test]
  public void CanConvertFromNumeric()
  {
    Assert.IsTrue (_int32EnumConverter.CanConvertFrom (typeof (int)));
    Assert.IsTrue (_int16EnumConverter.CanConvertFrom (typeof (short)));

    Assert.IsFalse (_int32EnumConverter.CanConvertFrom (typeof (short)));
    Assert.IsFalse (_int16EnumConverter.CanConvertFrom (typeof (int)));
  }

  [Test]
  public void CanConvertToNumeric()
  {
    Assert.IsTrue (_int32EnumConverter.CanConvertTo (typeof (int)));
    Assert.IsTrue (_int16EnumConverter.CanConvertTo (typeof (short)));
    
    Assert.IsFalse (_int32EnumConverter.CanConvertTo (typeof (short)));
    Assert.IsFalse (_int16EnumConverter.CanConvertTo (typeof (int)));
  }

  [Test]
  public void ConvertFromString()
  {
    Assert.AreEqual (Int32Enum.ValueA, _int32EnumConverter.ConvertFrom ("ValueA"));
    Assert.AreEqual (Int32Enum.ValueB, _int32EnumConverter.ConvertFrom ("ValueB"));
  }

  [Test]
  public void ConvertToString()
  {
    Type destinationType = typeof (string);

    Assert.AreEqual ("ValueA", _int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType));
    Assert.AreEqual ("ValueB", _int32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType));
  }

  [Test]
  public void ConvertFromInt32()
  {
    Assert.AreEqual (Int32Enum.ValueA, _int32EnumConverter.ConvertFrom (0));
    Assert.AreEqual (Int32Enum.ValueB, _int32EnumConverter.ConvertFrom (1));
  }

  [Test]
  public void ConvertToInt32()
  {
    Type destinationType = typeof (int);

    Assert.AreEqual (0, _int32EnumConverter.ConvertTo (Int32Enum.ValueA, destinationType));
    Assert.AreEqual (1, _int32EnumConverter.ConvertTo (Int32Enum.ValueB, destinationType));
  }


  [Test]
  public void ConvertFromInt16()
  {
    Assert.AreEqual (Int16Enum.ValueA, _int16EnumConverter.ConvertFrom ((short) 0));
    Assert.AreEqual (Int16Enum.ValueB, _int16EnumConverter.ConvertFrom ((short) 1));
  }

  [Test]
  public void ConvertToInt16()
  {
    Type destinationType = typeof (short);

    Assert.AreEqual ((short) 0, _int16EnumConverter.ConvertTo (Int16Enum.ValueA, destinationType));
    Assert.AreEqual ((short) 1, _int16EnumConverter.ConvertTo (Int16Enum.ValueB, destinationType));
  }
}            

}
