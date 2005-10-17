using System;
using System.ComponentModel;

namespace Rubicon.NullableValueTypes
{

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

}
