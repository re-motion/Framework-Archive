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

    private Dictionary<Type, SecurableClassInfo> _types = new Dictionary<Type, SecurableClassInfo> ();
    private Dictionary<PropertyInfo, StatePropertyInfo> _stateProperties = new Dictionary<PropertyInfo, StatePropertyInfo> ();
    private Dictionary<Enum, EnumValueInfo> _enumValues = new Dictionary<Enum, EnumValueInfo> ();
    private Dictionary<Enum, EnumValueInfo> _accessTypes = new Dictionary<Enum, EnumValueInfo> ();

    // construction and disposing

    public MetadataCache ()
    {
    }

    // methods and properties

    public SecurableClassInfo GetTypeInfo (Type key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_types.ContainsKey (key))
        return _types[key];
      else
        return null;
    }

    public void AddTypeInfo (Type key, SecurableClassInfo value)
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

    public EnumValueInfo GetEnumValueInfo (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_enumValues.ContainsKey (key))
        return _enumValues[key];
      else
        return null;
    }

    public void AddEnumValueInfo (Enum key, EnumValueInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _enumValues.Add (key, value);
    }

    public bool ContainsEnumValueInfo (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _enumValues.ContainsKey (key);
    }

    public EnumValueInfo GetAccessType (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      if (_accessTypes.ContainsKey (key))
        return _accessTypes[key];
      else
        return null;
    }

    public void AddAccessType (Enum key, EnumValueInfo value)
    {
      ArgumentUtility.CheckNotNull ("key", key);
      ArgumentUtility.CheckNotNull ("value", value);

      _accessTypes.Add (key, value);
    }

    public bool ContainsAccessType (Enum key)
    {
      ArgumentUtility.CheckNotNull ("key", key);

      return _accessTypes.ContainsKey (key);
    }

    public List<SecurableClassInfo> GetTypeInfos ()
    { 
      return new List<SecurableClassInfo> (_types.Values);
    }

    public List<StatePropertyInfo> GetStatePropertyInfos ()
    {
      return new List<StatePropertyInfo> (_stateProperties.Values);
    }

    public List<EnumValueInfo> GetAccessTypes ()
    {
      return new List<EnumValueInfo> (_accessTypes.Values);
    }
  }
}