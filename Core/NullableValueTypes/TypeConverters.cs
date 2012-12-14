using System;
using System.ComponentModel;
using System.Globalization;

namespace Rubicon.NullableValueTypes
{

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaBoolean"/>.
/// </summary>
/// <remarks> Conversion is only supported for values of type <see cref="string"/>. </remarks>
public class NaBooleanConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaBoolean"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaBoolean"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    return sourceType == typeof (string);
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaBoolean"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    An <see cref="NaBoolean"/> or <see cref="NaBoolean.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return NaBoolean.Parse ((string)value);
    return NaBoolean.Null;
  }

  /// <summary>
  ///   Convertes an <see cref="NaBoolean"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaBoolean"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaBoolean)
      return ((NaBoolean) value).ToString ("~");
    return base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary> The list of possible values for the converion into <see cref="NaBoolean"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns>
  ///   A <see cref="TypeConverter.StandardValuesCollection"/> that holds a standard set of 
  ///   valid values, or <see langword="null"/> if the data type does not support a standard set
  ///   of values.
  /// </returns>
  public override TypeConverter.StandardValuesCollection GetStandardValues (ITypeDescriptorContext context)
  {
    return new TypeConverter.StandardValuesCollection (new object[] { NaBoolean.True, NaBoolean.False, NaBoolean.Null });
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="true"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return true;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="true"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return true;
  }
}

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaInt32"/>.
/// </summary>
/// <remarks> 
///   Conversion is supported for values of types <see cref="string"/>.
/// </remarks>
public class NaInt32Converter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaInt32"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaInt32"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaInt32"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    An <see cref="NaInt32"/> or <see cref="NaInt32.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return NaInt32.Parse ((string) value);
    return NaInt32.Null;
  }

  /// <summary>
  ///   Convertes an <see cref="NaInt32"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaInt32"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaInt32)
      return ((NaInt32) value).ToString ("~");
    return base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaByte"/>.
/// </summary>
/// <remarks> 
///   Conversion is supported for values of types <see cref="string"/>.
/// </remarks>
public class NaByteConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaByte"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaByte"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaByte"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="NaByte"/> or <see cref="NaByte.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return NaByte.Parse ((string) value);
    
    return NaByte.Null;
  }

  /// <summary>
  ///   Convertes a <see cref="NaByte"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaByte"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaByte)
      return ((NaByte) value).ToString ("~");
    return 
      base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaDecimal"/>.
/// </summary>
/// <remarks> 
///   Conversion is supported for values of types <see cref="string"/>.
/// </remarks>
public class NaDecimalConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaDecimal"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaDecimal"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaDecimal"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="NaDecimal"/> or <see cref="NaDecimal.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return NaDecimal.Parse ((string) value);
    
    return NaDecimal.Null;
  }

  /// <summary>
  ///   Convertes a <see cref="NaDecimal"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaDecimal"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaDecimal)
      return ((NaDecimal) value).ToString ("~");
    return 
      base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaInt16"/>.
/// </summary>
/// <remarks> 
///   Conversion is supported for values of types <see cref="string"/>.
/// </remarks>
public class NaInt16Converter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaInt16"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaInt16"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaInt16"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="NaInt16"/> or <see cref="NaInt16.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return NaInt16.Parse ((string) value);
    
    return NaInt16.Null;
  }

  /// <summary>
  ///   Convertes a <see cref="NaInt16"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaInt16"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaInt16)
      return ((NaInt16) value).ToString ("~");
    return 
      base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaInt64"/>.
/// </summary>
/// <remarks> 
///   Conversion is supported for values of types <see cref="string"/>.
/// </remarks>
public class NaInt64Converter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaInt64"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaInt64"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaInt64"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="NaInt64"/> or <see cref="NaInt64.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return NaInt64.Parse ((string) value);
    
    return NaInt64.Null;
  }

  /// <summary>
  ///   Convertes a <see cref="NaInt64"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaInt64"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaInt64)
      return ((NaInt64) value).ToString ("~");
    return 
      base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaSingle"/>.
/// </summary>
/// <remarks> 
///   Conversion is supported for values of types <see cref="string"/>.
/// </remarks>
public class NaSingleConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaSingle"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaSingle"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaSingle"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="NaSingle"/> or <see cref="NaSingle.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return NaSingle.Parse ((string) value);
    
    return NaSingle.Null;
  }

  /// <summary>
  ///   Convertes a <see cref="NaSingle"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaSingle"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaSingle)
      return ((NaSingle) value).ToString ("~");
    return 
      base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

/// <summary>
///   Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaGuid"/>.
/// </summary>
/// <remarks> 
///   Conversion is supported for values of types <see cref="string"/>.
/// </remarks>
public class NaGuidConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaGuid"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaGuid"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaGuid"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns>
  ///    A <see cref="NaGuid"/> or <see cref="NaGuid.Null"/> if the conversion failed. 
  /// </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
  {
    if (value is string)
      return new NaGuid (new Guid ((string) value));
    
    return NaGuid.Null;
  }

  /// <summary>
  ///   Convertes a <see cref="NaGuid"/> into the <paramref name="destinationType"/>.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaGuid"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  public override object ConvertTo (ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
  {
    if (value is NaGuid)
      return ((NaGuid) value).ToString ("~");
    return 
      base.ConvertTo (context, culture, value, destinationType);
  }

  /// <summary>
  ///   Returns whether the collection of standard values returned by 
  ///   <see cref="TypeConverter.GetStandardValues "/> is an exclusive list.
  /// </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesExclusive (ITypeDescriptorContext context)
  {
    return false;
  }

  /// <summary>
  ///   Returns whether this object supports a standard set of values that can be picked from a 
  ///   list.
  /// </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}
}
