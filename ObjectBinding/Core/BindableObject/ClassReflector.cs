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
    private readonly IMetadataFactory _metadataFactory;

    public ClassReflector (Type targetType, BindableObjectProvider businessObjectProvider, IMetadataFactory metadataFactory)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);
      ArgumentUtility.CheckNotNull ("metadataFactory", metadataFactory);

      _targetType = targetType;
      _concreteType = Mixins.TypeUtility.GetConcreteType (_targetType);
      _businessObjectProvider = businessObjectProvider;
      _metadataFactory = metadataFactory;
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
      IPropertyFinder propertyFinder = _metadataFactory.CreatePropertyFinder (_targetType);
      
      List <PropertyBase> properties = new List<PropertyBase> ();
      foreach (PropertyInfo propertyInfo in propertyFinder.GetPropertyInfos ())
      {
        PropertyReflector propertyReflector = _metadataFactory.CreatePropertyReflector (propertyInfo, _businessObjectProvider);
        properties.Add (propertyReflector.GetMetadata());
      }

      return properties;
    }
  }
}