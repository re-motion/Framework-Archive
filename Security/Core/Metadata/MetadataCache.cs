using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using System.Reflection;
using System.Collections.ObjectModel;

namespace Rubicon.Security.Metadata
{

  public class MetadataCache
  {
    // types

    // static members

    // member fields

    private Dictionary<Type, SecurableTypeInfo> _types = new Dictionary<Type, SecurableTypeInfo> ();
    private Dictionary<PropertyInfo, StatePropertyInfo> _stateProperties = new Dictionary<PropertyInfo, StatePropertyInfo> ();

    // construction and disposing

    public MetadataCache ()
    {
    }

    // methods and properties

    public SecurableTypeInfo GetTypeInfo (Type key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_types.ContainsKey (key))
        return _types[key];
      else
        return null;
    }

    public void AddTypeInfo (Type key, SecurableTypeInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _types.Add (key, value);
    }

    public bool ContainsTypeInfo (Type key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _types.ContainsKey (key);
    }

    public StatePropertyInfo GetStatePropertyInfo (PropertyInfo key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      key = NormalizeProperty (key);
      if (_stateProperties.ContainsKey (key))
        return _stateProperties[key];
      else
        return null;
    }

    public void AddStatePropertyInfo (PropertyInfo key, StatePropertyInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _stateProperties.Add (NormalizeProperty (key), value);
    }

    public bool ContainsStatePropertyInfo (PropertyInfo key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _stateProperties.ContainsKey (NormalizeProperty (key));
    }

    private PropertyInfo NormalizeProperty (PropertyInfo property)
    {
      ArgumentUtility.CheckNotNull ("property", property);

      if (property.DeclaringType == property.ReflectedType)
        return property;
      else
        return property.DeclaringType.GetProperty (property.Name);
    }

    public List<SecurableTypeInfo> GetTypeInfos ()
    { 
      return new List<SecurableTypeInfo> (_types.Values);
    }

    public List<StatePropertyInfo> GetStatePropertyInfos ()
    {
      return new List<StatePropertyInfo> (_stateProperties.Values);
    }
  }
}