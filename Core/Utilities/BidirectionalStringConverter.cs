using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Rubicon.Utilities
{

/// <summary> Specialization of <see cref="TypeConverter"/> for conversions from and to <see cref="String"/>. </summary>
public class BidirectionalStringConverter: TypeConverter
{
  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="String"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into a <see cref="String"/>.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == null)
      return false;
    if (sourceType.IsArray)
      return false;
    return true;  
  }

  /// <summary> Test: Can convert from <see cref="String"/> to <paramref name="destinationType"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="destinationType"> The <see cref="Type"/>  to convert a <see cref="String"/> value to. </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == null)
      return false;
    if (destinationType.IsArray)
      return false;
    return    StringUtility.GetParseMethodWithFormatProvider (destinationType) != null
           || StringUtility.GetParseMethod (destinationType) != null;
  }

  /// <summary> Converts <paramref name="value"/> into a <see cref="String"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value">  The source value. </param>
  /// <returns> A <see cref="String"/> or <see cref="NaSingle.Null"/> if the conversion failed.  </returns>
  /// <exception cref="NotSupportedException">
  ///   The passed <paramref name="value"/> is of an unsupported <see cref="Type"/>. 
  /// </exception>
  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (value == null)
      return string.Empty;
    if (! value.GetType().IsArray)
    {
      IFormattable formattable = value as IFormattable;
      if (formattable != null)
        return formattable.ToString (null, culture);
      return value.ToString();
    }
    throw new NotSupportedException (string.Format ("Cannot convert from '{0}' to String.", value.GetType()));
  }

  /// <summary> Convertes a <see cref="String"/> into the <paramref name="destinationType"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="String"/> to be converted. Must not be <see langword="null"/>. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. Must not be <see langword="null"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  /// <exception cref="NotSupportedException"> 
  ///   The passed <paramref name="value"/> is of an unsupported <see cref="Type"/>. 
  /// </exception>
  public override object ConvertTo (
      ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
  {
    ArgumentUtility.CheckNotNull ("value", value);
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    if (value is string && CanConvertTo (context, destinationType))
      return StringUtility.Parse (destinationType, (string) value, culture);
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
