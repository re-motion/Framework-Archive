using System;
using System.ComponentModel;
using System.Collections;
using NUnit.Framework;
using Rubicon.Utilities;
using Rubicon.NullableValueTypes;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class TypeConversionServicesTest
{
  private TypeConversionServicesMock _services;
  private Type _naInt32 = typeof (NaInt32);
  private Type _int32 = typeof (int);
  private Type _string = typeof (string);
  private Type _naDouble = typeof (NaDouble);
  private Type _double = typeof (double);

  [SetUp]
  public void SetUp()
  {
    _services = new TypeConversionServicesMock();
    _services.ClearCache();
  }

  [Test]
  public void CanConvertFromInt32ToNaInt32()
  {
    Assert.IsTrue (_services.CanConvert (_int32, _naInt32));
  }

  [Test]
  public void CanConvertFromNaInt32ToInt32()
  {
    Assert.IsTrue (_services.CanConvert (_naInt32, _int32));
  }

  [Test]
  public void CanConvertFromNaInt32ToString()
  {
    Assert.IsTrue (_services.CanConvert (_naInt32, _string));
  }

  [Test]
  public void CanConvertFromStringToNaInt32()
  {
    Assert.IsTrue (_services.CanConvert (_string, _naInt32));
  }

  [Test]
  [Ignore ("BasicType not implemented.")]
  public void CanConvertFromInt32ToString()
  {
    Assert.IsTrue (_services.CanConvert (_string, _int32));
  }

  [Test]
  [Ignore ("BasicType not implemented.")]
  public void CanConvertFromStringToInt32()
  {
    Assert.IsTrue (_services.CanConvert (_int32, _string));
  }

  [Test]
  public void GetTypeConverterFromInt32ToNaInt32 ()
  {
    TypeConverter converter = _services.GetTypeConverter (_int32, _naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromNaInt32ToInt32 ()
  {
    TypeConverter converter = _services.GetTypeConverter (_naInt32, _int32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromNaInt32ToString ()
  {
    TypeConverter converter = _services.GetTypeConverter (_naInt32, _string);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromStringToNaInt32 ()
  {
    TypeConverter converter = _services.GetTypeConverter (_string, _naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  [Ignore ("BasicType not implemented.")]
  public void GetTypeConverterFromInt32ToString ()
  {
    TypeConverter converter = _services.GetTypeConverter (_int32, _string);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  [Ignore ("BasicType not implemented.")]
  public void GetTypeConverterFromStringToInt32 ()
  {
    TypeConverter converter = _services.GetTypeConverter (_string, _int32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromNaDoubleToString ()
  {
    TypeConverter converter = _services.GetTypeConverter (_naDouble, _string);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDoubleConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromStringToNaDouble ()
  {
    TypeConverter converter = _services.GetTypeConverter (_string, _naDouble);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDoubleConverter), converter.GetType());
  }

  [Test]
  [Ignore ("BasicType not implemented.")]
  public void GetTypeConverterFromDoubleToString ()
  {
    TypeConverter converter = _services.GetTypeConverter (_double, _string);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDoubleConverter), converter.GetType());
  }

  [Test]
  [Ignore ("BasicType not implemented.")]
  public void GetTypeConverterFromStringToDouble ()
  {
    TypeConverter converter = _services.GetTypeConverter (_string, _double);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDoubleConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForNaInt32 ()
  {
    TypeConverter converter = _services.GetTypeConverter (_naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForInt32 ()
  {
    TypeConverter converter = _services.GetTypeConverter (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterByAttributeForNaInt32()
  {
    TypeConverter converter = _services.GetTypeConverterByAttribute (_naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterByAttributeForInt32()
  {
    TypeConverter converter = _services.GetTypeConverterByAttribute (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetCachedTypeConverterByAttributeForNaInt32()
  {
    TypeConverter converterFirstRun = _services.GetCachedTypeConverterByAttribute (_naInt32);
    TypeConverter converterSecondRun = _services.GetCachedTypeConverterByAttribute (_naInt32);
    Assert.IsNotNull (converterFirstRun, "TypeConverter from first run is null.");
    Assert.IsNotNull (converterSecondRun, "TypeConverter from second run is null.");
    Assert.AreSame (converterFirstRun, converterSecondRun);
  }

  [Test]
  public void GetCachedTypeConverterByAttributeForInt32()
  {
    TypeConverter converter = _services.GetCachedTypeConverterByAttribute (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetCachedTypeConverter()
  {
    NaInt32Converter converter = new NaInt32Converter();
    _services.AddTypeConverterToCache (_naInt32, converter);
    Assert.AreSame (converter, _services.GetCachedTypeConverter (_naInt32));
  }

  [Test]
  public void HasCachedTypeConverter()
  {
    NaInt32Converter converter = new NaInt32Converter();
    _services.AddTypeConverterToCache (_naInt32, converter);
    Assert.IsTrue (_services.HasCachedTypeConverter (_naInt32));
  }
}

}
