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
      BindableObjectClass bindableObjectClass = new BindableObjectClass (_type, _businessObjectProvider);
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
      return _type.GetProperties (BindingFlags.Instance | BindingFlags.Public);
    }
  }
}