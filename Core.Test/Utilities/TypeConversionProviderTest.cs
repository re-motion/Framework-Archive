using System;
using System.ComponentModel;
using NUnit.Framework;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

[TestFixture]
public class TypeConversionProviderTest
{
  public enum Int32Enum: int
  {
    ValueA = 0,
    ValueB = 1
  }

  private StubTypeConversionProvider _provider;
  private Type _int32 = typeof (int);
  private Type _nullableInt32 = typeof (int?);
  private Type _naInt32 = typeof (NaInt32);
  private Type _string = typeof (string);
  private Type _naDouble = typeof (NaDouble);
  private Type _object = typeof (object);
  private Type _guid = typeof (Guid);
  private Type _int32Enum = typeof (Int32Enum);
  private Type _nullableInt32Enum = typeof (Int32Enum?);

  [SetUp]
  public void SetUp()
  {
    _provider = new StubTypeConversionProvider();

    StubTypeConversionProvider.ClearCache();
  }

  [Test]
  public void Create()
  { 
    Assert.IsNotNull (TypeConversionProvider.Create());
  }

  [Test]
  public void TestCurrent()
  { 
    Assert.IsNotNull (TypeConversionProvider.Current);
    TypeConversionProvider provider = TypeConversionProvider.Create();
    TypeConversionProvider.SetCurrent (provider);
    Assert.IsNotNull (TypeConversionProvider.Current);
    Assert.AreSame (provider, TypeConversionProvider.Current);
  }

  [Test]
  public void CanConvertFromInt32ToInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _int32));
  }

  [Test]
  public void CanConvertFromNaInt32ToNaInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_naInt32, _naInt32));
  }

  [Test]
  public void CanConvertFromNullableInt32ToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _nullableInt32));
  }

  [Test]
  public void CanConvertFromInt32ToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _nullableInt32));
  }

  [Test]
  public void CanConvertFromInt32ToNaInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _naInt32));
  }

  [Test]
  public void CanConvertFromNaInt32ToInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_naInt32, _int32));
  }

  [Test]
  public void CanConvertFromNullableInt32ToInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _int32));
  }

  [Test]
  public void CanConvertFromNaInt32ToString()
  {
    Assert.IsTrue (_provider.CanConvert (_naInt32, _string));
  }

  [Test]
  public void CanConvertFromStringToNaInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _naInt32));
  }

  [Test]
  public void CanConvertFromInt32ToString()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _int32));
  }

  [Test]
  public void CanConvertFromStringToInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _string));
  }

  [Test]
  public void CanConvertFromNullableInt32ToString ()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _nullableInt32));
  }

  [Test]
  public void CanConvertFromStringToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _string));
  }

  [Test]
  public void CanConvertFromObjectToNaInt32()
  {
    Assert.IsFalse (_provider.CanConvert (_object, _naInt32));
  }

  [Test]
  public void CanConvertFromGuidToString()
  {
    Assert.IsTrue (_provider.CanConvert (_guid, _string));
  }

  [Test]
  public void CanConvertFromStringToGuid()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _guid));
  }


  [Test]
  public void CanConvertFromInt32EnumToInt32Enum()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _int32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToInt32()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _int32));
  }

  [Test]
  public void CanConvertFromInt32ToInt32Enum()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _int32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToString()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _string));
  }

  [Test]
  public void CanConvertFromStringToInt32Enum()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _int32Enum));
  }


  [Test]
  public void CanConvertFromNullableInt32EnumToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32Enum, _nullableInt32Enum));
  }

  [Test]
  public void CanConvertFromNullableInt32EnumToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32Enum, _nullableInt32));
  }

  [Test]
  public void CanConvertFromNullableInt32ToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32, _nullableInt32Enum));
  }

  [Test]
  public void CanConvertFromInt32EnumToNullableInt32 ()
  {
    Assert.IsTrue (_provider.CanConvert (_int32Enum, _nullableInt32));
  }

  [Test]
  public void CanConvertFromInt32ToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_int32, _nullableInt32Enum));
  }

  [Test]
  public void CanConvertFromNullableInt32EnumToString ()
  {
    Assert.IsTrue (_provider.CanConvert (_nullableInt32Enum, _string));
  }

  [Test]
  public void CanConvertFromStringToNullableInt32Enum ()
  {
    Assert.IsTrue (_provider.CanConvert (_string, _nullableInt32Enum));
  }


  [Test]
  public void ConvertFromInt32ToInt32()
  {
    Assert.AreEqual (1, _provider.Convert (_int32, _int32, 1));
  }

  [Test]
  public void ConvertFromNaInt32ToNaInt32()
  {
    Assert.AreEqual (new NaInt32 (1), _provider.Convert (_naInt32, _naInt32, new NaInt32 (1)));
  }

  [Test]
  public void ConvertFromNaInt32ToInt32()
  {
    Assert.AreEqual (1, _provider.Convert (_naInt32, _int32, new NaInt32 (1)));
  }

  [Test]
  public void ConvertFromInt32ToNaInt32()
  {
    Assert.AreEqual (new NaInt32 (1), _provider.Convert (_int32, _naInt32, 1));
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromObjectToNaInt32()
  {
    _provider.Convert (_object, _naInt32, new object());
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromNaInt32ToObject()
  {
    _provider.Convert (_naInt32, _object, new NaInt32 (1));
  }

  [Test]
  public void ConvertFromNullableInt32ToInt32 ()
  {
    Assert.AreEqual (1, _provider.Convert (_nullableInt32, _int32, 1));
  }

  [Test]
  public void ConvertFromInt32ToNullableInt32 ()
  {
    Assert.AreEqual (1, _provider.Convert (_int32, _nullableInt32, 1));
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromObjectToNullableInt32 ()
  {
    _provider.Convert (_object, _nullableInt32, new object ());
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromNullableInt32ToObject ()
  {
    _provider.Convert (_naInt32, _object, 1);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt32ToObject()
  {
    _provider.Convert (_int32, _object, 1);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromObjectToInt32()
  {
    _provider.Convert (_object, _naInt32, new object());
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt64ToInt32 ()
  {
    _provider.Convert (_object, _int32, 1L);
  }

  [Test]
  public void ConvertFromNaInt32ToString()
  {
    Assert.AreEqual ("1", _provider.Convert (_naInt32, _string, new NaInt32 (1)));
  }

  [Test]
  public void ConvertFromStringToNaInt32()
  {
    Assert.AreEqual (new NaInt32 (1), _provider.Convert (_string, _naInt32, "1"));
  }

  [Test]
  public void ConvertFromInt32ToNaInt32WithNull()
  {
    Assert.AreEqual (NaInt32.Null, _provider.Convert (_int32, _naInt32, null));
  }

  [Test]
  public void ConvertFromInt32ToNaInt32WithDBNull()
  {
    Assert.AreEqual (NaInt32.Null, _provider.Convert (_int32, _naInt32, DBNull.Value));
  }

  [Test]
  public void ConvertFromStringToNaInt32WithEmpty()
  {
    Assert.AreEqual (NaInt32.Null, _provider.Convert (_string, _naInt32, string.Empty));
  }

  [Test]
  public void ConvertFromNullableInt32ToString ()
  {
    Assert.AreEqual ("1", _provider.Convert (_nullableInt32, _string, 1));
  }

  [Test]
  public void ConvertFromStringToNullableInt32 ()
  {
    Assert.AreEqual (1, _provider.Convert (_string, _nullableInt32, "1"));
  }

  [Test]
  public void ConvertFromInt32ToNullableInt32WithNull ()
  {
    Assert.AreEqual (null, _provider.Convert (_int32, _nullableInt32, null));
  }

  [Test]
  [Ignore ("TODO: Proper behavior needs to be defined when underlying types match but the value does not match the source type.")]
  public void ConvertFromInt32ToNullableInt32WithDBNull ()
  {
    Assert.AreEqual (null, _provider.Convert (_int32, _nullableInt32, DBNull.Value));
  }

  [Test]
  public void ConvertFromStringToNullableInt32WithEmpty ()
  {
    Assert.AreEqual (null, _provider.Convert (_string, _nullableInt32, string.Empty));
  }

  [Test]
  public void ConvertFromInt32ToString()
  {
    Assert.AreEqual ("1", _provider.Convert (_int32, _string, 1));
  }

  [Test]
  public void ConvertFromStringToInt32()
  {
    Assert.AreEqual (1, _provider.Convert (_string, _int32, "1"));
  }

  [Test]
  public void ConvertFromInt32ToStringWithNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_int32, _string, null));
  }

  [Test]
  public void ConvertFromInt32ToStringWithDBNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_int32, _string, DBNull.Value));
  }

  [Test]
  [ExpectedException (typeof (ParseException))]
  public void ConvertFromStringToInt32WithEmpty()
  {
    _provider.Convert (_string, _int32, string.Empty);
  }


  [Test]
  public void ConvertFromInt32EnumToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_int32Enum, _int32Enum, Int32Enum.ValueA));
  }

  [Test]
  public void ConvertFromInt32EnumToInt32()
  {
    Assert.AreEqual (0, _provider.Convert (_int32Enum, _int32, Int32Enum.ValueA));
    Assert.AreEqual (1, _provider.Convert (_int32Enum, _int32, Int32Enum.ValueB));
  }

  [Test]
  public void ConvertFromInt32ToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_int32, _int32Enum, 0));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_int32, _int32Enum, 1));
  }

  [Test]
  public void ConvertFromInt32EnumToString()
  {
    Assert.AreEqual ("ValueA", _provider.Convert (_int32Enum, _string, Int32Enum.ValueA));
    Assert.AreEqual ("ValueB", _provider.Convert (_int32Enum, _string, Int32Enum.ValueB));
  }

  [Test]
  public void ConvertFromStringToInt32Enum()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_string, _int32Enum, "ValueA"));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_string, _int32Enum, "ValueB"));
  }

  [Test]
  public void ConvertFromInt32EnumToStringWithNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_int32Enum, _string, null));
  }

  [Test]
  [ExpectedException (typeof (InvalidCastException))]
  public void ConvertFromInt32EnumToStringWithDBNull()
  {
    _provider.Convert (_int32Enum, _string, DBNull.Value);
  }

  [Test]
  [ExpectedException (typeof (ParseException), ExpectedMessage = " is not a valid value for Int32Enum.")]
  public void ConvertFromStringToInt32EnumWithEmpty ()
  {
    _provider.Convert (_string, _int32Enum, string.Empty);
  }

  [Test]
  [ExpectedException (typeof (NotSupportedException))]
  public void ConvertFromInt32ToInt32EnumWithNull()
  {
    _provider.Convert (_int32, _int32Enum, null);
  }



  [Test]
  public void ConvertFromNullableInt32EnumToNullableInt32Enum ()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_nullableInt32Enum, _nullableInt32Enum, Int32Enum.ValueA));
  }

  [Test]
  public void ConvertFromNullableInt32EnumToNullableInt32 ()
  {
    Assert.AreEqual (0, _provider.Convert (_nullableInt32Enum, _nullableInt32, Int32Enum.ValueA));
    Assert.AreEqual (1, _provider.Convert (_nullableInt32Enum, _nullableInt32, Int32Enum.ValueB));
    Assert.IsNull (_provider.Convert (_nullableInt32Enum, _nullableInt32, null));
  }

  [Test]
  public void ConvertFromNullableInt32ToNullableInt32Enum ()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_nullableInt32, _nullableInt32Enum, 0));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_nullableInt32, _nullableInt32Enum, 1));
    Assert.IsNull (_provider.Convert (_nullableInt32, _nullableInt32Enum, null));
  }

  [Test]
  public void ConvertFromNullableInt32EnumToString ()
  {
    Assert.AreEqual ("ValueA", _provider.Convert (_nullableInt32Enum, _string, Int32Enum.ValueA));
    Assert.AreEqual ("ValueB", _provider.Convert (_nullableInt32Enum, _string, Int32Enum.ValueB));
    Assert.AreEqual (string.Empty, _provider.Convert (_nullableInt32Enum, _string, null));
  }

  [Test]
  public void ConvertFromStringToNullableInt32Enum ()
  {
    Assert.AreEqual (Int32Enum.ValueA, _provider.Convert (_string, _nullableInt32Enum, "ValueA"));
    Assert.AreEqual (Int32Enum.ValueB, _provider.Convert (_string, _nullableInt32Enum, "ValueB"));
    Assert.IsNull (_provider.Convert (_string, _nullableInt32Enum, null));
    Assert.IsNull (_provider.Convert (_string, _nullableInt32Enum, string.Empty));
  }

  [Test]
  [ExpectedException (typeof (InvalidCastException))]
  public void ConvertFromNullableInt32EnumToStringWithDBNull ()
  {
    _provider.Convert (_nullableInt32Enum, _string, DBNull.Value);
  }


  [Test]
  public void ConvertFromStringToString()
  {
    string value = "Hello World!";
    Assert.AreEqual (value, _provider.Convert (_string, _string, value));
  }

  [Test]
  public void ConvertFromStringToStringWithNull()
  {
    Assert.AreEqual (string.Empty, _provider.Convert (_string, _string, null));
  }

  [Test]
  public void GetTypeConverterFromInt32ToNaInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_int32, _naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromNaInt32ToInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_naInt32, _int32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromNaInt32ToString ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_naInt32, _string);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromStringToNaInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_string, _naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (BidirectionalStringConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromNaDoubleToString ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_naDouble, _string);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDoubleConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromStringToNaDouble ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_string, _naDouble);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (BidirectionalStringConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterFromObjectToString ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_object, _string);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterFromStringToObject ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_string, _object);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaByte ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaByte));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaByteConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForByte ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (byte));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaInt16 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaInt16));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt16Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForInt16 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (short));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNullableInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (_nullableInt32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaInt64 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaInt64));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt64Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForInt64 ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (long));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaSingle ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaSingle));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaSingleConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForSingle ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (float));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaDouble ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaDouble));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDoubleConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForDouble ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (double));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaDateTime ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaDateTime));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDateTimeConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForDateTime ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (DateTime));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaBoolean ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaBoolean));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaBooleanConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForBoolean ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (bool));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForNaGuid ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaGuid));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaGuidConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForNaDecimal ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (NaDecimal));
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaDecimalConverter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterForDecimal ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (decimal));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterForGuid ()
  {
    TypeConverter converter = _provider.GetTypeConverter (typeof (Guid));
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterByAttributeForNaInt32()
  {
    TypeConverter converter = _provider.GetTypeConverterByAttribute (_naInt32);
    Assert.IsNotNull (converter, "TypeConverter is null.");
    Assert.AreEqual (typeof (NaInt32Converter), converter.GetType());
  }

  [Test]
  public void GetTypeConverterByAttributeForInt32()
  {
    TypeConverter converter = _provider.GetTypeConverterByAttribute (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetTypeConverterByAttributeForNullableInt32 ()
  {
    TypeConverter converter = _provider.GetTypeConverterByAttribute (_nullableInt32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetBasicTypeConverterForNaInt32()
  {
    TypeConverter converterFirstRun = _provider.GetBasicTypeConverter (_naInt32);
    TypeConverter converterSecondRun = _provider.GetBasicTypeConverter (_naInt32);
    Assert.IsNotNull (converterFirstRun, "TypeConverter from first run is null.");
    Assert.IsNotNull (converterSecondRun, "TypeConverter from second run is null.");
    Assert.AreSame (converterFirstRun, converterSecondRun);
  }

  [Test]
  public void GetBasicTypeConverterForInt32()
  {
    TypeConverter converter = _provider.GetBasicTypeConverter (_int32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetBasicTypeConverterForNullableInt32 ()
  {
    TypeConverter converter = _provider.GetBasicTypeConverter (_nullableInt32);
    Assert.IsNull (converter, "TypeConverter is not null.");
  }

  [Test]
  public void GetBasicTypeConverterForInt32Enum()
  {
    TypeConverter converterFirstRun = _provider.GetBasicTypeConverter (_int32Enum);
    TypeConverter converterSecondRun = _provider.GetBasicTypeConverter (_int32Enum);
    Assert.IsNotNull (converterFirstRun, "TypeConverter from first run is null.");
    Assert.IsNotNull (converterSecondRun, "TypeConverter from second run is null.");
    Assert.AreSame (converterFirstRun, converterSecondRun);
    Assert.AreEqual (typeof (AdvancedEnumConverter), converterFirstRun.GetType());
  }

  [Test]
  public void GetTypeConverterFromCache()
  {
    NaInt32Converter converter = new NaInt32Converter();
    _provider.AddTypeConverterToCache (_naInt32, converter);
    Assert.AreSame (converter, _provider.GetTypeConverterFromCache (_naInt32));
  }

  [Test]
  public void HasTypeInCache()
  {
    NaInt32Converter converter = new NaInt32Converter();
    _provider.AddTypeConverterToCache (_naInt32, converter);
    Assert.IsTrue (_provider.HasTypeInCache (_naInt32));
  }

  [Test]
  public void AddTypeConverter()
  {
    NaGuidConverter converter = new NaGuidConverter();
    Assert.IsNull (_provider.GetTypeConverter (_guid));
    _provider.AddTypeConverter (_guid, converter);
    Assert.AreSame (converter, _provider.GetTypeConverter (_guid));
  }

  [Test]
  public void RemoveTypeConverter()
  {
    NaGuidConverter converter = new NaGuidConverter();
    _provider.AddTypeConverter (_guid, converter);
    Assert.AreSame (converter, _provider.GetTypeConverter (_guid));
    _provider.RemoveTypeConverter (_guid);
    Assert.IsNull (_provider.GetTypeConverter (_guid));
  }
}

}
