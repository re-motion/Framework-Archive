using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
  public class BindableDomainObjectMetadataFactory : IMetadataFactory
  {
    public static readonly BindableDomainObjectMetadataFactory Instance = new BindableDomainObjectMetadataFactory ();

    protected BindableDomainObjectMetadataFactory ()
    {
    }

    public IPropertyFinder CreatePropertyFinder (Type targetType)
    {
      return new BindableDomainObjectPropertyFinder (targetType);
    }

    public PropertyReflector CreatePropertyReflector (PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      return DefaultMetadataFactory.Instance.CreatePropertyReflector (propertyInfo, businessObjectProvider);
    }
  }
}