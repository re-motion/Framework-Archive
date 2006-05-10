using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data;

namespace Rubicon.Security.Metadata
{

  public class TypeReflector
  {
    // types

    // static members

    // member fields

    StatePropertyReflector _statePropertyReflector = new StatePropertyReflector ();

    // construction and disposing

    public TypeReflector ()
    {
    }

    // methods and properties

    public SecurableTypeInfo GetMetadata (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableType));
      if (type.IsValueType)
        throw new ArgumentException ("Value types are not supported.", "type");

      SecurableTypeInfo info = new SecurableTypeInfo();
      info.Name = type.FullName;
      PermanentGuidAttribute guidAttribute = (PermanentGuidAttribute) Attribute.GetCustomAttribute (type, typeof (PermanentGuidAttribute), true);
      if (guidAttribute != null)
        info.ID = guidAttribute.Value;
      info.Properties = GetProperties (type);

      return info;
    }

    protected virtual List<StatePropertyInfo> GetProperties (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (ISecurableType));

      MemberInfo[] propertyInfos = type.FindMembers (
          MemberTypes.Property, 
          BindingFlags.Instance | BindingFlags.Public, 
          FindStatePropertiesFilter, 
          null);

      List<StatePropertyInfo> statePropertyInfos = new List<StatePropertyInfo> ();
      foreach (PropertyInfo propertyInfo in propertyInfos)
        statePropertyInfos.Add (_statePropertyReflector.GetMetadata (propertyInfo));

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