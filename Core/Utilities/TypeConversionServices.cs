using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Rubicon.NullableValueTypes;

namespace Rubicon.Utilities
{

/// <summary> 
///   Provides functionality to get the <see cref="TypeConverter"/> for a <see cref="Type"/> and to convert a value
///   from a source <see cref="Type"/> into a destination <see cref="Type"/>.
/// </summary>
/// <remarks>
///   Conversion is possible under the following conditions:
///   <list type="bullet">
///     <item>
///       A type has a <see cref="TypeConverter"/> applied through the <see cref="TypeConverterAttribute"/> that
///       supports the conversion. 
///     </item>
///     <item>
///       For <see cref="Enum"/> types into the <see cref="String"/> value or the underlying numeric 
///       <see cref="Type"/>.
///     </item>
///     <item>
///       For types without a <see cref="TypeConverter"/>, the <b>TypeConversionServices</b> try to use the 
///       <see cref="BidirectionalStringConverter"/>. See the documentation of the string converter for details on the
///       supported types.
///     </item>
///   </list>
/// </remarks>
public class TypeConversionServices
{
  private static Hashtable s_typeConverters = new Hashtable();

  private Hashtable _additionalTypeConverters = new Hashtable();
  private BidirectionalStringConverter _stringConverter = new BidirectionalStringConverter();

  public TypeConversionServices()
	{
	}

  /// <summary> 
  ///   Gets the <see cref="TypeConverter"/> that is able to convert an instance of the <paramref name="sourceType"/> 
  ///   <see cref="Type"/> into an instance of the <paramref name="destinationType"/> <see cref="Type"/>.
  /// </summary>
  /// <param name="sourceType"> 
  ///   The source <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
  /// </param>
  /// <param name="destinationType"> 
  ///   The destination <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
  /// </param>
  /// <returns> 
  ///   A <see cref="TypeConverter"/> or <see langword="null"/> of no matching <see cref="TypeConverter"/> can be found.
  /// </returns>
  /// <remarks> 
  ///   You can identify whether you must use the <see cref="TypeConverter.ConvertTo"/> or the 
  ///   <see cref="TypeConverter.ConvertFrom"/> method by testing the returned <see cref="TypeConverter"/>'s
  ///   <see cref="TypeConverter.CanConvertTo"/> and <see cref="TypeConverter.CanConvertFrom"/> methods.
  /// </remarks>
  public virtual TypeConverter GetTypeConverter (Type sourceType, Type destinationType)
  {
    ArgumentUtility.CheckNotNull ("sourceType", sourceType);
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    TypeConverter sourceTypeConverter = GetTypeConverter (sourceType);
    if (sourceTypeConverter != null && sourceTypeConverter.CanConvertTo (destinationType))
      return sourceTypeConverter;

    TypeConverter destinationTypeConverter = GetTypeConverter (destinationType);
    if (destinationTypeConverter != null && destinationTypeConverter.CanConvertFrom (sourceType))
      return destinationTypeConverter;
    
    return null;
  }

  /// <summary> 
  ///   Gets the <see cref="TypeConverter"/> that is associated with the specified <paramref name="type"/>.
  /// </summary>
  /// <param name="type"> 
  ///   The <see cref="Type"/> to get the <see cref="TypeConverter"/> for. Must not be <see langword="null"/>.
  /// </param>
  /// <returns>
  ///   A <see cref="TypeConverter"/> or <see langword="null"/> of no <see cref="TypeConverter"/> can be found.
  /// </returns>
  public virtual TypeConverter GetTypeConverter (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    TypeConverter converter = GetAdditionalTypeConverter (type);
    if (converter != null)
      return converter;

    converter = GetBasicTypeConverter (type);
    if (converter != null)
      return converter;
      
    if (type == typeof (string))
      return _stringConverter;

    return null;
  }

  /// <summary> 
  ///   Registers the <paramref name="converter"/> for the <paramref name="type"/>, overriding the default settings. 
  /// </summary>
  /// <param name="type"> 
  ///   The <see cref="Type"/> for which the <paramref name="converter"/> should be used. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <param name="converter"> The <see cref="TypeConverter"/> to register. Must not be <see langword="null"/>. </param>
  public void AddTypeConverter (Type type, TypeConverter converter)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    ArgumentUtility.CheckNotNull ("converter", converter);
    _additionalTypeConverters[type] = converter;
  }

  /// <summary>
  ///   Unregisters a special <see cref="TypeConverter"/> previously registered by using <see cref="AddTypeConverter"/>.
  /// </summary>
  /// <param name="type">
  ///   The <see cref="Type"/> whose special <see cref="TypeConverter"/> should be removed. 
  ///   Must not be <see langword="null"/>.
  /// </param>
  /// <remarks> If no <see cref="TypeConverter"/> has been registered, the method has no effect. </remarks>
  public void RemoveTypeConverter (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    _additionalTypeConverters.Remove (type);
  }

  /// <summary> 
  ///   Test whether the <see cref="TypeConversionServices"/> object can convert an object of <see cref="Type"/> 
  ///   <paramref name="sourceType"/> into an object of <see cref="Type"/> <paramref name="destinationType"/>
  ///   by using the <see cref="Convert"/> method.
  /// </summary>
  /// <param name="sourceType"> 
  ///   The source <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
  /// </param>
  /// <param name="destinationType"> 
  ///   The destination <see cref="Type"/> of the value. Must not be <see langword="null"/>. 
  /// </param>
  /// <returns> <see langword="true"/> if a conversion is possible. </returns>
  public virtual bool CanConvert (Type sourceType, Type destinationType)
  {
    ArgumentUtility.CheckNotNull ("sourceType", sourceType);
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    if (sourceType == destinationType)
      return true;

    TypeConverter converter = GetTypeConverter (sourceType, destinationType);
    if (converter != null)
      return true;

    return false;
  }

  /// <summary> Convertes the <paramref name="value"/> into the <paramref name="destinationType"/>. </summary>
  /// <param name="sourceType"> 
  ///   The source <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
  /// </param>
  /// <param name="destinationType"> 
  ///   The destination <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
  /// </param>
  /// <param name="value"> The <see cref="NaInt16"/> to be converted. Must not be <see langword="null"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted <paramref name="value"/>. </returns>
  public object Convert (Type sourceType, Type destinationType, object value)
  {
    return Convert (null, null, sourceType, destinationType, value);
  }

  /// <summary> Convertes the <paramref name="value"/> into the <paramref name="destinationType"/>. </summary>
  /// <param name="context"> An <see cref="ITypeDescriptorContext"/> that provides a format context. </param>
  /// <param name="culture"> The <see cref="CultureInfo"/> to use as the current culture. </param>
  /// <param name="sourceType"> 
  ///   The source <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
  /// </param>
  /// <param name="destinationType"> 
  ///   The destination <see cref="Type"/> of the <paramref name="value"/>. Must not be <see langword="null"/>. 
  /// </param>
  /// <param name="value"> The <see cref="NaInt16"/> to be converted. Must not be <see langword="null"/>. </param>
  /// <returns> An <see cref="Object"/> that represents the converted <paramref name="value"/>. </returns>
  public virtual object Convert (
      ITypeDescriptorContext context, CultureInfo culture, Type sourceType, Type destinationType, object value)
  {
    ArgumentUtility.CheckNotNull ("sourceType", sourceType);
    ArgumentUtility.CheckNotNull ("destinationType", destinationType);

    if (sourceType == destinationType)
      return value;
    
    TypeConverter sourceTypeConverter = GetTypeConverter (sourceType);
    if (sourceTypeConverter != null && sourceTypeConverter.CanConvertTo (destinationType))
      return sourceTypeConverter.ConvertTo (context, culture, value, destinationType);

    TypeConverter destinationTypeConverter = GetTypeConverter (destinationType);
    if (destinationTypeConverter != null && destinationTypeConverter.CanConvertFrom (sourceType))
      return destinationTypeConverter.ConvertFrom (context, culture, value);

    throw new NotSupportedException (string.Format (
        "Cannot convert value '{0}' to type '{1}'.", value, destinationType));
  }

  protected TypeConverter GetBasicTypeConverter (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    TypeConverter converter = GetTypeConverterFromCache (type);
    if (converter == null && ! HasTypeInCache (type))
    {
      converter = GetTypeConverterByAttribute (type);
      if (converter == null && type.IsEnum)
        converter = new AdvancedEnumConverter (type);
      AddTypeConverterToCache (type, converter);
    }
    return converter;
  }

  protected TypeConverter GetTypeConverterByAttribute (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    TypeConverterAttribute[] typeConverters = 
        (TypeConverterAttribute[]) type.GetCustomAttributes (typeof (TypeConverterAttribute), true);
    if (typeConverters.Length == 1) 
    {
      Type typeConverterType = Type.GetType (typeConverters[0].ConverterTypeName, true, false);
      return (TypeConverter) Activator.CreateInstance (typeConverterType);
    }
    return null;
  }

  protected void AddTypeConverterToCache (Type type, TypeConverter converter)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    lock (s_typeConverters.SyncRoot)
    {
      s_typeConverters[type] = converter;
    }
  }

  protected TypeConverter GetTypeConverterFromCache (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    return (TypeConverter) s_typeConverters[type];
  }

  protected bool HasTypeInCache (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    return s_typeConverters.ContainsKey (type);
  }

  protected TypeConverter GetAdditionalTypeConverter (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    return (TypeConverter) _additionalTypeConverters[type];
  }
}

}
