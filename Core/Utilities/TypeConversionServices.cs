using System;
using System.Collections;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

namespace Rubicon.Utilities
{

public class TypeConverterFactory
{
  private static Hashtable s_typeConverters = new Hashtable();
  private static TypeConverterFactory s_current = new TypeConverterFactory();

  /// <summary> Gets the current <see cref="TypeConverterFactory"/>. </summary>
  /// <value> An instance of the <see cref="TypeConverterFactory"/> type. </value>
  public static TypeConverterFactory Current
  {
    get
    {
      if (s_current == null)
      {
        lock (typeof (TypeConverterFactory))
        {
          if (s_current == null)
            s_current = new TypeConverterFactory();
        }
      }
      return s_current;
    }
  }

  /// <summary> Sets the current <see cref="TypeConverterFactory"/>. </summary>
  /// <param name="factory"> A <see cref="TypeConverterFacotry"/>. Must not be <see langword="null"/>. </param>
  public static void SetCurrent (TypeConverterFactory factory)
  {
    ArgumentUtility.CheckNotNull ("factory", factory);
    lock (typeof (TypeConverterFactory))
    {
      s_current = factory;
    }
  }

	public TypeConverterFactory()
	{
	}

  /// <summary> 
  ///   Gets the <see cref="TypeConverter"/> that is able to convert a value of the <paramref name"from"/> 
  ///   <see cref="Type"/> into an instance of the <paramref name="to"/> <see cref="Type"/>.
  /// </summary>
  /// <param name="from"> The source <see cref="Type"/> of the value. Must not be <see langword="null"/>. </param>
  /// <param name="to"> The destination <see cref="Type"/> of the value. Must not be <see langword="null"/>. </param>
  /// <returns> 
  ///   A <see cref="TypeConverter"/> or <see langword="null"/> of no matching <see cref="TypeConverter"/> can be found.
  /// </returns>
  public TypeConverter GetTypeConverter (Type from, Type to)
  {
    ArgumentUtility.CheckNotNull ("from", from);
    ArgumentUtility.CheckNotNull ("to", to);

    TypeConverter fromConverter = GetTypeConverter (from);
    if (fromConverter != null && fromConverter.CanConvertTo (to))
      return fromConverter;

    TypeConverter toConverter = GetTypeConverter (to);
    if (toConverter != null && toConverter.CanConvertFrom (from))
      return toConverter;
    
    return null;
  }

  /// <summary> 
  ///   Gets the <see cref="TypeConverter"/> that is associated with the specified <paramref name"type"/>.
  ///   <see cref="Type"/> into an instance of the <paramref name="to"/> <see cref="Type"/>.
  /// </summary>
  /// <param name="type"> 
  ///   The <see cref="Type"/> to get the <see cref="TypeConverter"/> for. Must not be <see langword="null"/>.
  /// </param>
  /// <returns>
  ///   A <see cref="TypeConverter"/> or <see langword="null"/> of no <see cref="TypeConverter"/> can be found.
  /// </returns>
  public TypeConverter GetTypeConverter (Type type)
  {
    return GetCachedTypeConverterByAttribute (type);
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
    if (converter == null)
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

}

}
