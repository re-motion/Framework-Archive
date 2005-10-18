using System;
using System.ComponentModel;
using System.Globalization;
using Rubicon.Utilities;

namespace Rubicon.NullableValueTypes
{

/// <summary> Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaInt32"/>. </summary>
/// <remarks> 
///   Conversion from and to <see cref="NaInt32"/> values is supported for the values of the following types:
///   <see cref="string"/> and <see cref="int"/>. It is also possible to convert from <see cref="DBNull"/> and
///   <see langword="null"/>.
/// </remarks>
public class NaInt32Converter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaInt32"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> The <see cref="Type"/> of the value to be converted into an <see cref="NaInt32"/>. </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    if (sourceType == typeof (int))
      return true;
    if (sourceType == typeof (DBNull))
      return true;
    return false;  
  }

  /// <summary> Test: Can convert from <see cref="NaInt32"/> to <paramref name="destinationType"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="destinationType"> The <see cref="Type"/>  to convert an <see cref="NaInt32"/> value to. </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == typeof (string))
      return true;
    if (destinationType == typeof (int))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaInt32"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns> An <see cref="NaInt32"/> or <see cref="NaInt32.Null"/> if the conversion failed.  </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (value is string)
      return NaInt32.Parse ((string) value);
    if (value is int)
      return new NaInt32 ((int) value);
    if (value == DBNull.Value)
      return NaInt32.Null;
    if (value == null)
      return NaInt32.Null;
    return NaInt32.Null;
  }

  /// <summary> Convertes an <see cref="NaInt32"/> into the <paramref name="destinationType"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaInt32"/> to be converted. Must not be <see langword="null"/>. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. Must not be <see langword="null"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  /// <exception cref="NaNullValueException"> The passed <paramref name="value"/> is <see langword="null"/>. </exception>
  /// <exception cref="NotSupportedException"> 
  ///   The passed <paramref name="value"/> is of an unsupported <see cref="Type"/>. 
  /// </exception>
  public override object ConvertTo (
      ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    if (value is NaInt32)
    {
      if (destinationType == typeof (string))
        return ((NaInt32) value).ToString ("~");
      if (destinationType == typeof (int))
        return ((NaInt32) value).Value;
    }
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

  /// <summary> Returns whether this object supports a standard set of values that can be picked from a list. </summary>
  /// <param name="context"> An ITypeDescriptorContext that provides a format context. </param>
  /// <returns> <see langword="false"/>. </returns>
  public override bool GetStandardValuesSupported (ITypeDescriptorContext context)
  {
    return false;
  }
}

}
