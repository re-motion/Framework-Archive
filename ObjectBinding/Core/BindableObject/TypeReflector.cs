using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class TypeReflector
  {
    private readonly Type _type;

    public TypeReflector (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _type = type;
    }

    public PropertyBase[] GetProperties ()
    {
      List<PropertyBase> properties = new List<PropertyBase>();
      foreach (PropertyInfo propertyInfo in GetPropertyInfos())
      {
        PropertyReflector propertyReflector = new PropertyReflector (propertyInfo);
        properties.Add (propertyReflector.GetMetadata());
      }

      return properties.ToArray();
    }

    private PropertyInfo[] GetPropertyInfos ()
    {
      return _type.GetProperties (BindingFlags.Instance | BindingFlags.Public);
    }
  }
}