using System;
using System.ComponentModel;
using System.Globalization;
using Remotion.Utilities;

namespace Remotion.NullableValueTypes
{

/// <summary> Specialization of <see cref="TypeConverter"/> for values of the type <see cref="NaDateTime"/>. </summary>
/// <remarks> 
///   <para>
///     Conversion from and to <see cref="NaDateTime"/> values is supported for the values of the following types:
///     <see cref="string"/> and <see cref="DateTime"/>. It is also possible to convert from <see cref="DBNull"/> and
///     <see langword="null"/>.
///   </para><para> 
///     Conversions to <see cref="String"/> are done using "~" as format string. 
///   </para>
/// </remarks>
public class NaDateTimeConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="NaDateTime"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="NaDateTime"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == typeof (string))
      return true;
    if (sourceType == typeof (DateTime))
      return true;
    if (sourceType == typeof (DBNull))
      return true;
    return false;  
  }

  /// <summary> Test: Can convert from <see cref="NaDateTime"/> to <paramref name="destinationType"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="destinationType"> The <see cref="Type"/>  to convert an <see cref="NaDateTime"/> value to. </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == typeof (string))
      return true;
    if (destinationType == typeof (DateTime))
      return true;
    return false;  
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="NaDateTime"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns> An <see cref="NaDateTime"/> or <see cref="NaDateTime.Null"/> if the conversion failed.  </returns>
  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (value is string)
      return NaDateTime.Parse ((string) value, culture);
    if (value is DateTime)
      return new NaDateTime ((DateTime) value);
    if (value == DBNull.Value)
      return NaDateTime.Null;
    if (value == null)
      return NaDateTime.Null;
    return NaDateTime.Null;
  }

  /// <summary> Convertes an <see cref="NaDateTime"/> into the <paramref name="destinationType"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="NaDateTime"/> to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. Must not be <see langword="null"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  /// <exception cref="NaNullValueException"> The passed <paramref name="value"/> is <see langword="null"/>. </exception>
  /// <exception cref="NotSupportedException"> 
  ///   The passed <paramref name="value"/> is of an unsupported <see cref="Type"/>. 
  /// </exception>
  /// <remarks> Conversions to <see cref="String"/> are done using "~" as format string. </remarks>
  public override object ConvertTo (
      ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
  {
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    if (value == null)
      value = NaDateTime.Null;

    if (value is NaDateTime)
    {
      if (destinationType == typeof (string))
        return ((NaDateTime) value).ToString ("~", culture);
      if (destinationType == typeof (DateTime))
        return ((NaDateTime) value).Value;
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
