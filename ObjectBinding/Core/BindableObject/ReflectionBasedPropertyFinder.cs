using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class ReflectionBasedPropertyFinder : IPropertyFinder
  {
    private readonly Type _targetType;

    public ReflectionBasedPropertyFinder (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      _targetType = targetType;
    }

    public IEnumerable<PropertyInfo> GetPropertyInfos ()
    {
      PropertyInfoCollection propertyInfos = new PropertyInfoCollection ();
      for (Type currentType = _targetType; currentType != null; currentType = currentType.BaseType)
      {
        foreach (PropertyInfo propertyInfo in currentType.FindMembers (
            MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, PropertyFilter, null))
        {
          if (!propertyInfos.Contains (propertyInfo.Name))
            propertyInfos.Add (propertyInfo);
        }
      }
      return propertyInfos;
    }

    //OPF Mapping
    protected virtual bool PropertyFilter (MemberInfo memberInfo, object filterCriteria)
    {
      ObjectBindingAttribute attribute = AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (memberInfo, true);
      if (attribute != null && !attribute.Visible)
        return false;

      if (((PropertyInfo) memberInfo).GetGetMethod (false) == null)
        return false;

      return true;
    }
  }
}