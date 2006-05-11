using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data;

namespace Rubicon.Security.Metadata
{

  public class TypeReflector : ITypeReflector
  {
    // types

    // static members

    // member fields

    private IStatePropertyReflector _statePropertyReflector;
    
    // construction and disposing

    public TypeReflector () : this (new StatePropertyReflector ())
    {
    }

    public TypeReflector (IStatePropertyReflector statePropertyReflector)
    {
      ArgumentUtility.CheckNotNull ("statePropertyReflector", statePropertyReflector);
      _statePropertyReflector = statePropertyReflector;
    }

    // methods and properties

    public IStatePropertyReflector StatePropertyReflector
    {
      get { return _statePropertyReflector; }
    }

    public SecurableTypeInfo GetMetadata (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableType));
      if (type.IsValueType)
        throw new ArgumentException ("Value types are not supported.", "type");
      ArgumentUtility.CheckNotNull ("cache", cache);

      SecurableTypeInfo info = cache.GetTypeInfo (type);
      if (info == null)
      {
        info = new SecurableTypeInfo ();
        info.Name = type.FullName;
        PermanentGuidAttribute guidAttribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (type, typeof (PermanentGuidAttribute), true);
        if (guidAttribute != null)
          info.ID = guidAttribute.Value;
        info.Properties = GetProperties (type, cache);

        cache.AddTypeInfo (type, info);

        if (typeof (ISecurableType).IsAssignableFrom (type.BaseType))
          GetMetadata (type.BaseType, cache);
      }

      return info;
    }

    protected virtual List<StatePropertyInfo> GetProperties (Type type, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableType));
      ArgumentUtility.CheckNotNull ("cache", cache);

      MemberInfo[] propertyInfos = type.FindMembers (
          MemberTypes.Property, 
          BindingFlags.Instance | BindingFlags.Public, 
          FindStatePropertiesFilter, 
          null);

      List<StatePropertyInfo> statePropertyInfos = new List<StatePropertyInfo> ();
      foreach (PropertyInfo propertyInfo in propertyInfos)
        statePropertyInfos.Add (_statePropertyReflector.GetMetadata (propertyInfo, cache));

      return statePropertyInfos;
    }

    protected bool FindStatePropertiesFilter (MemberInfo member, object filterCriteria)
    {
      ArgumentUtility.CheckNotNullAndType ("member", member, typeof (PropertyInfo));

      PropertyInfo property = (PropertyInfo) member;
      return property.PropertyType.IsEnum && Attribute.IsDefined (property.PropertyType, typeof (SecurityStateAttribute), false);
    }
 }
}