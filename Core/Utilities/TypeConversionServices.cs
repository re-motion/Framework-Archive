using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using Rubicon.NullableValueTypes;

namespace Rubicon.Utilities
{

public class TypeConversionServices
{
  private static Hashtable s_typeConverters = new Hashtable();

  public TypeConversionServices()
	{
	}

  /// <summary> 
  ///   Gets the <see cref="TypeConverter"/> that is able destinationType convert a value of the <paramref name="sourceType"/> 
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
  ///   <see cref="Type"/> into an instance of the <paramref name="destinationType"/> <see cref="Type"/>.
  /// </summary>
  /// <param name="type"> 
  ///   The <see cref="Type"/> destinationType get the <see cref="TypeConverter"/> for. Must not be <see langword="null"/>.
  /// </param>
  /// <returns>
  ///   A <see cref="TypeConverter"/> or <see langword="null"/> of no <see cref="TypeConverter"/> can be found.
  /// </returns>
  public virtual TypeConverter GetTypeConverter (Type type)
  {
    return GetCachedTypeConverterByAttribute (type);
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

    if (   typeof (IConvertible).IsAssignableFrom (sourceType) && destinationType == typeof (string)
        || sourceType == typeof (string) && typeof (IConvertible).IsAssignableFrom (destinationType))
    {
      return true;
    }

    return false;
  }

  public object Convert (Type sourceType, Type destinationType, object value)
  {
    return Convert (null, null, sourceType, destinationType, value);
  }

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
   
    if (typeof (IConvertible).IsAssignableFrom (sourceType) && destinationType == typeof (string))
    {
      if (value == null)
        value = DBNull.Value;
      return System.Convert.ChangeType (value, destinationType, culture);
    }
    else if (sourceType == typeof (string) && typeof (IConvertible).IsAssignableFrom (destinationType))
    {
      return System.Convert.ChangeType (value, destinationType, culture);
    }

    throw new NotSupportedException (string.Format (
        "Cannot convert value '{0}' to type '{1}'.", value, destinationType));
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

  protected TypeConverter GetCachedTypeConverterByAttribute (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    TypeConverter converter = GetCachedTypeConverter (type);
    if (converter == null && ! HasCachedTypeConverter (type))
    {
      converter = GetTypeConverterByAttribute (type);
      if (converter != null)
        AddTypeConverterToCache (type, converter);
    }
    return converter;
  }

  protected void AddTypeConverterToCache (Type type, TypeConverter converter)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    ArgumentUtility.CheckNotNull ("converter", converter);

    lock (s_typeConverters.SyncRoot)
    {
      s_typeConverters[type] = converter;
    }
  }

  protected TypeConverter GetCachedTypeConverter (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    return (TypeConverter) s_typeConverters[type];
  }

  protected bool HasCachedTypeConverter (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);
    return s_typeConverters.ContainsKey (type);
  }
}

}
