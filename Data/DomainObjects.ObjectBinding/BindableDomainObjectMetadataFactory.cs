using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.Mapping;

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
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      return new BindableDomainObjectPropertyFinder (targetType);
    }

    public PropertyReflector CreatePropertyReflector (Type targetType, PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new BindableDomainObjectPropertyReflector (targetType, propertyInfo, businessObjectProvider);
    }
  }
}