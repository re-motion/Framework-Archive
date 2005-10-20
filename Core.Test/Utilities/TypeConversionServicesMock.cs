using System;
using System.Collections;
using System.ComponentModel;

using Rubicon.Utilities;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Core.UnitTests.Utilities
{

/// <summary> Exposes non-public members of the <see cref="TypeConversionServices"/> type. </summary>
public class TypeConversionServicesMock: TypeConversionServices
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

  public new bool HasTypeConverterInCache (Type type)
  {
    return base.HasTypeConverterInCache (type);
  }

  public static void ClearCache()
  {
    Hashtable cache = (Hashtable) PrivateInvoke.GetNonPublicStaticField (typeof (TypeConversionServices), "s_typeConverters");
    cache.Clear();
  }
}

}
