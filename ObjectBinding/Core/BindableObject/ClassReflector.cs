using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject.Properties;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class ClassReflector
  {
    private readonly Type _type;
    private readonly BindableObjectProvider _businessObjectProvider;

    public ClassReflector (Type type, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      _type = type;
      _businessObjectProvider = businessObjectProvider;
    }

    public Type Type
    {
      get { return _type; }
    }

    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    }

    public BindableObjectClass GetMetadata ()
    {
      return _businessObjectProvider.BusinessObjectClassCache.GetOrCreateValue (_type, delegate { return CreateBindableObjectClass(); });
    }

    private BindableObjectClass CreateBindableObjectClass ()
    {
      BindableObjectClass bindableObjectClass;
      // TODO: Use Mixins.TypeUtility.IsAssignableFrom (typeof (IBusinessObjectWithIdentity), _concreteType) instead?
      if (BindableObjectWithIdentityMixin.HasMixin (_type))
        bindableObjectClass = new BindableObjectClassWithIdentity (_type, _businessObjectProvider);
      else
        bindableObjectClass = new BindableObjectClass (_type, _businessObjectProvider);

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
      for (Type currentType = _type; currentType != null; currentType = currentType.BaseType)
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