using System;
using System.ComponentModel;
using Rubicon.NullableValueTypes;

namespace Rubicon.Utilities
{

public class TypeConverterFactory
{
	public TypeConverterFactory()
	{
	}

  public TypeConverter GetTypeConverter (Type from, Type to)
  {
    ArgumentUtility.CheckNotNull ("from", from);
    ArgumentUtility.CheckNotNull ("to", to);

    return new NaInt32Converter();
  }

  public TypeConverter GetTypeConverter (Type type)
  {
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
}

}
