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
  public enum Int32Enum: int
  {
    ValueA = 0,
    ValueB = 1
  }

  private TypeConversionServicesMock _services;
  private Type _naInt32 = typeof (NaInt32);
  private Type _int32 = typeof (int);
  private Type _string = typeof (string);
  private Type _naDouble = typeof (NaDouble);
  private Type _object = typeof (object);
  private Type _guid = typeof (Guid);
  private Type _int32Enum = typeof (Int32Enum);

  [SetUp]
  public void SetUp()
  {
    _services = new TypeConversionServicesMock();

    TypeConversionServicesMock.ClearCache();
  }

  [Test]
  public void CanConvertFromInt32ToInt32()
  {
    Assert.IsTrue (_services.CanConvert (_int32, _int32));
  }

  [Test]
  public void CanConvertFromNaInt32ToNaInt32()
  {
    Assert.IsTrue (_services.CanConvert (_naInt32, _naInt32));
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
  public void CanConvertFromInt32ToString()
  {
    Assert.IsTrue (_services.CanConvert (_string, _int32));
  }

  [Test]
  public void CanConvertFromStringToInt32()
  {
    Assert.IsTrue (_services.CanConvert (_int32, _string));
  }

  [Test]
  public void CanConvertFromObjectToNaInt32()
  {
    Assert.IsFalse (_services.CanConvert (_object, _naInt32));
  }

  [Test]
  public void CanConvertFromGuidToString()
  {
    Assert.IsTrue (_services.CanConvert (_guid, _string));
  }

  [Test]
  public void CanConvertFromStringToGuid()
  {
    Assert.IsTrue (_services.CanConvert (_string, _guid));
  }

  [Test]
  public void CanConvertFromInt32EnumToInt32Enum()
  {
    Assert.IsTrue (_services.CanConvert (_int32Enum, _int32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToInt32()
  {
    Assert.IsTrue (_services.CanConvert (_int32Enum, _int32));
  }

  [Test]
  public void CanConvertFromInt32ToInt32Enum()
  {
    Assert.IsTrue (_services.CanConvert (_int32, _int32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToString()
  {
    Assert.IsTrue (_services.CanConvert (_int32Enum, _string));
  }

  [Test]
  public void CanConvertFromStringToInt32Enum()
  {
    Assert.IsTrue (_services.CanConvert (_string, _int32Enum));
  }


  [Test]
  public void ConvertFromInt32ToInt32()
  {
    Assert.AreEqual (1, _services.Convert (_int32, _int32, 1));
  }

  [Test]
  public void ConvertFromNaInt32ToNaInt32()
  {
    Assert.AreEqual (new NaInt32 (1), _services.Convert (_naInt32, _naInt32, new NaInt32 (1)));
  }

  [Test]
  public void ConvertFromNaInt32ToInt32()
  {
    Assert.AreEqual (1, _services.Convert (_naInt32, _int32, new NaInt32 (1)));
  }

  [Test]
  public void ConvertFromInt32ToNaInt32()
  {
    Assert.AreEqual (new NaInt32 (1), _services.Convert (_int32, _naInt32, 1));
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromObjectToNaInt32()
  {
    _services.Convert (_object, _naInt32, new object());
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromNaInt32ToObject()
  {
    _services.Convert (_naInt32, _object, new NaInt32 (1));
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt32ToObject()
  {
    _services.Convert (_int32, _object, 1);
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromObjectToInt32()
  {
    _services.Convert (_object, _naInt32, new object());
    Assert.Fail();
  }

  [Test]
  public void ConvertFromNaInt32ToString()
  {
    Assert.AreEqual ("1", _services.Convert (_naInt32, _string, new NaInt32 (1)));
  }

  [Test]
  public void ConvertFromStringToNaInt32()
  {
    Assert.AreEqual (new NaInt32 (1), _services.Convert (_string, _naInt32, "1"));
  }

  [Test]
  public void ConvertFromInt32ToNaInt32WithNull()
  {
    Assert.AreEqual (NaInt32.Null, _services.Convert (_int32, _naInt32, null));
  }

  [Test]
  public void ConvertFromInt32ToNaInt32WithDBNull()
  {
    Assert.AreEqual (NaInt32.Null, _services.Convert (_int32, _naInt32, DBNull.Value));
  }

  [Test]
  public void ConvertFromStringToNaInt32WithEmpty()
  {
    Assert.AreEqual (NaInt32.Null, _services.Convert (_string, _naInt32, ""));
  }

  [Test]
  public void ConvertFromInt32ToString()
  {
    Assert.AreEqual ("1", _services.Convert (_int32, _string, 1));
  }

  [Test]
  public void ConvertFromStringToInt32()
  {
    Assert.AreEqual (1, _services.Convert (_string, _int32, "1"));
  }

  [Test]
  public void ConvertFromInt32ToStringWithNull()
  {
    Assert.AreEqual ("", _services.Convert (_int32, _string, null));
  }

  [Test]
  public void ConvertFromInt32ToStringWithDBNull()
  {
    Assert.AreEqual ("", _services.Convert (_int32, _string, DBNull.Value));
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ConvertFromStringToInt32WithEmpty()
  {
    _services.Convert (_string, _int32, "");
    Assert.Fail();
  }


  [Test]
  public void ConvertFromInt32EnumToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _services.Convert (_int32Enum, _int32Enum, Int32Enum.ValueA));
  }

  [Test]
  public void ConvertFromInt32EnumToInt32()
  {
    Assert.AreEqual (0, _services.Convert (_int32Enum, _int32, Int32Enum.ValueA));
    Assert.AreEqual (1, _services.Convert (_int32Enum, _int32, Int32Enum.ValueB));
  }

  [Test]
  public void ConvertFromInt32ToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _services.Convert (_int32, _int32Enum, 0));
    Assert.AreEqual (Int32Enum.ValueB, _services.Convert (_int32, _int32Enum, 1));
  }

  [Test]
  public void ConvertFromInt32EnumToString()
  {
    Assert.AreEqual ("ValueA", _services.Convert (_int32Enum, _string, Int32Enum.ValueA));
    Assert.AreEqual ("ValueB", _services.Convert (_int32Enum, _string, Int32Enum.ValueB));
  }

  [Test]
  public void ConvertFromStringToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _services.Convert (_string, _int32Enum, "ValueA"));
    Assert.AreEqual (Int32Enum.ValueB, _services.Convert (_string, _int32Enum, "ValueB"));
  }
  [Test]
  public void ConvertFromInt32EnumToStringWithNull()
  {
    Assert.AreEqual ("", _services.Convert (_int32Enum, _string, null));
  }

  [Test]
  [ExpectedException (typeof (InvalidCastException))]
  public void ConvertFromInt32EnumToStringWithDBNull()
  {
    _services.Convert (_int32Enum, _string, DBNull.Value);
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (ArgumentException))]
  public void ConvertFromStringToInt32EnumWithEmpty()
  {
    _services.Convert (_string, _int32Enum, "");
    Assert.Fail();
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt32ToInt32EnumWithNull()
  {
    _services.Convert (_int32, _int32Enum, null);
    Assert.Fail();
  }


  [Test]
  public void ConvertFromGuidToString()
  {
    Guid guid = Guid.NewGuid();
    Assert.AreEqual (guid.ToString(), _services.Convert (_guid, _string, guid));
  }

  [Test]
  public void ConvertFromStringToGuid()
  {
    Guid guid = Guid.NewGuid();
    Assert.AreEqual (guid, _services.Convert (_string, _guid, guid.ToString()));
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
    Assert.AreEqual (typeof (BidirectionalStringConverter), converter.GetType());
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
    Assert.AreEqual (typeof (BidirectionalStringConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromObjectToString ()
  {
    TypeConverter converter = _services.GetTypeConverter (_object, _string);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterFromStringToObject ()
  {
    TypeConverter converter = _services.GetTypeConverter (_string, _object);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaByte ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaByte));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaByteConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForByte ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (byte));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaInt16 ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaInt16));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt16Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForInt16 ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (short));
    Assert.IsNull (converter, "TypeConverter is not null.");
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
  public void GetTypeConverterForNaInt64 ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaInt64));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt64Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForInt64 ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (long));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaSingle ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaSingle));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaSingleConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForSingle ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (float));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaDouble ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaDouble));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDoubleConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForDouble ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (double));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaDateTime ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaDateTime));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDateTimeConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForDateTime ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (DateTime));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaBoolean ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaBoolean));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaBooleanConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForBoolean ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (bool));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaGuid ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaGuid));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaGuidConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForNaDecimal ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (NaDecimal));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDecimalConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForDecimal ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (decimal));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForGuid ()
  {
    TypeConverter converter = _services.GetTypeConverter (typeof (Guid));
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
  public void GetBasicTypeConverterForNaInt32()
  {
    TypeConverter converterFirstRun = _services.GetBasicTypeConverter (_naInt32);
    TypeConverter converterSecondRun = _services.GetBasicTypeConverter (_naInt32);
    Assert.IsNotNull (converterFirstRun, "TypeConverter from first run is null.");
    Assert.IsNotNull (converterSecondRun, "TypeConverter from second run is null.");
    Assert.AreSame (converterFirstRun, converterSecondRun);
  }

  [Test]
  public void GetBasicTypeConverterForInt32()
  {
    TypeConverter converter = _services.GetBasicTypeConverter (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetBasicTypeConverterForInt32Enum()
  {
    TypeConverter converterFirstRun = _services.GetBasicTypeConverter (_int32Enum);
    TypeConverter converterSecondRun = _services.GetBasicTypeConverter (_int32Enum);
    Assert.IsNotNull (converterFirstRun, "TypeConverter from first run is null.");
    Assert.IsNotNull (converterSecondRun, "TypeConverter from second run is null.");
    Assert.AreSame (converterFirstRun, converterSecondRun);
    Assert.AreEqual (typeof (AdvancedEnumConverter), converterFirstRun.GetType());
  }

  [Test]
  public void GetTypeConverterFromCache()
  {
    NaInt32Converter converter = new NaInt32Converter();
    _services.AddTypeConverterToCache (_naInt32, converter);
    Assert.AreSame (converter, _services.GetTypeConverterFromCache (_naInt32));
  }

  [Test]
  public void HasTypeInCache()
  {
    NaInt32Converter converter = new NaInt32Converter();
    _services.AddTypeConverterToCache (_naInt32, converter);
    Assert.IsTrue (_services.HasTypeInCache (_naInt32));
  }

  [Test]
  public void AddTypeConverter()
  {
    NaGuidConverter converter = new NaGuidConverter();
    Assert.IsNull (_services.GetTypeConverter (_guid));
    _services.AddTypeConverter (_guid, converter);
    Assert.AreSame (converter, _services.GetTypeConverter (_guid));
  }

  [Test]
  public void RemoveTypeConverter()
  {
    NaGuidConverter converter = new NaGuidConverter();
    _services.AddTypeConverter (_guid, converter);
    Assert.AreSame (converter, _services.GetTypeConverter (_guid));
    _services.RemoveTypeConverter (_guid);
    Assert.IsNull (_services.GetTypeConverter (_guid));
  }
}

}
