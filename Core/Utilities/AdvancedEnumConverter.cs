using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace Rubicon.Utilities
{

/// <summary> Specialization of <see cref="TypeConverter"/> for conversions from and to <see cref="Enum"/> types. </summary>
public class AdvancedEnumConverter: EnumConverter
{
  private Type _underlyingType;

  public AdvancedEnumConverter (Type enumType) : base (enumType)
  {
    _underlyingType = Enum.GetUnderlyingType (EnumType);
  }

  /// <summary> Test: Can convert from <paramref name="sourceType"/> to <see cref="String"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="sourceType"> 
  ///   The <see cref="Type"/> of the value to be converted into an <see cref="Enum"/> type.
  /// </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
  {
    if (sourceType == _underlyingType)
      return true;
    return base.CanConvertFrom (context, sourceType);
  }

  /// <summary> Test: Can convert from <see cref="String"/> to <paramref name="destinationType"/>? </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="destinationType"> The <see cref="Type"/> to convert an <see cref="Enum"/> value to. </param>
  /// <returns> <see langword="true"/> if the conversion is supported. </returns>
  public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
  {
    if (destinationType == _underlyingType)
      return true;
    return base.CanConvertFrom (context, destinationType);
  }

  /// <summary> Converts <paramref name="value"/> into an <see cref="Enum"/> value. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The source value. Must not be <see langword="null"/> or empty. </param>
  /// <returns> An <see cref="Enum"/> value.  </returns>
  /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
  public override object ConvertFrom (ITypeDescriptorContext context, CultureInfo culture, object value)
  {
    if (!(value is string) && value != null && _underlyingType == value.GetType())
      return Enum.ToObject (EnumType, value);
    return base.ConvertFrom (context, culture, value);
  }

  /// <summary> Convertes an <see cref="Enum"/> value into the <paramref name="destinationType"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="value"> The <see cref="Enum"/> value to be converted. </param>
  /// <param name="destinationType"> The destination <see cref="Type"/>. Must not be <see langword="null"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted value. </returns>
  /// <exception cref="NotSupportedException"> The conversion could not be performed. </exception>
  public override object ConvertTo (
      ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
  {
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    if (value is Enum && destinationType == _underlyingType)
      return Convert.ChangeType (value, destinationType, culture);
    return base.ConvertTo (context, culture, value, destinationType);
  }
}

}
