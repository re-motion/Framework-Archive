using System;
using System.Collections;
using System.ComponentModel;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Core.UnitTests.Utilities
{

/// <summary> Exposes non-public members of the <see cref="TypeConversionProvider"/> type. </summary>
public class TypeConversionServicesMock: TypeConversionProvider
{
  public new TypeConverter GetTypeConverterByAttribute (Type type)
  {
    return base.GetTypeConverterByAttribute (type);
  }

  public new TypeConverter GetBasicTypeConverter (Type type)
  {
    return base.GetBasicTypeConverter (type);
  }

  public new void AddTypeConverterToCache (Type key, TypeConverter converter)
  {
    base.AddTypeConverterToCache (key, converter);
  }

  public new TypeConverter GetTypeConverterFromCache (Type key)
  {
    return base.GetTypeConverterFromCache (key);
  }

  public new bool HasTypeInCache (Type type)
  {
    return base.HasTypeInCache (type);
  }

  public static void ClearCache()
  {
    Hashtable cache = (Hashtable) PrivateInvoke.GetNonPublicStaticField (typeof (TypeConversionProvider), "s_typeConverters");
    cache.Clear();
  }
}

}
