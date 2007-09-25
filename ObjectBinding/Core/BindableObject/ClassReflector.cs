using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class ClassReflector
  {
    private readonly Type _targetType;
    private readonly Type _concreteType;
    private readonly BindableObjectProvider _businessObjectProvider;

    public ClassReflector (Type targetType, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      _targetType = targetType;
      _concreteType = Mixins.TypeUtility.GetConcreteType (_targetType);
      _businessObjectProvider = businessObjectProvider;
    }

    public Type TargetType
    {
      get { return _targetType; }
    }

    public Type ConcreteType
    {
      get { return _concreteType; }
    }

    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    public BindableObjectClass GetMetadata ()
    {
      return _businessObjectProvider.BusinessObjectClassCache.GetOrCreateValue (_targetType, delegate { return CreateBindableObjectClass(); });
    }

    private BindableObjectClass CreateBindableObjectClass ()
    {
      BindableObjectClass bindableObjectClass;
      if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (ConcreteType))
        bindableObjectClass = new BindableObjectClassWithIdentity (_concreteType, _businessObjectProvider);
      else
        bindableObjectClass = new BindableObjectClass (_concreteType, _businessObjectProvider);

      bindableObjectClass.SetProperties (GetProperties());

      return bindableObjectClass;
    }

    private List<PropertyBase> GetProperties ()
    {
      List<PropertyBase> properties = new List<PropertyBase>();
      foreach (PropertyInfo propertyInfo in GetPropertyInfos())
      {
        PropertyReflector propertyReflector = new PropertyReflector (propertyInfo, _businessObjectProvider);
        properties.Add (propertyReflector.GetMetadata());
      }

      return properties;
    }

    private PropertyInfo[] GetPropertyInfos ()
    {
      PropertyInfoCollection propertyInfos = new PropertyInfoCollection();
      for (Type currentType = _targetType; currentType != null; currentType = currentType.BaseType)
      {
        foreach (PropertyInfo propertyInfo in currentType.FindMembers (
            MemberTypes.Property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly, PropertyFilter, null))
        {
          if (!propertyInfos.Contains (propertyInfo.Name))
            propertyInfos.Add (propertyInfo);
        }
      }
      return propertyInfos.ToArray();
    }

    //OPF Mapping
    private bool PropertyFilter (MemberInfo memberInfo, object filterCriteria)
    {
      ObjectBindingAttribute attribute = AttributeUtility.GetCustomAttribute<ObjectBindingAttribute> (memberInfo, true);
      if (attribute != null && !attribute.Visible)
        return false;

      if (((PropertyInfo)memberInfo).GetGetMethod (false) == null)
        return false;

      return true;
    }
  }
}