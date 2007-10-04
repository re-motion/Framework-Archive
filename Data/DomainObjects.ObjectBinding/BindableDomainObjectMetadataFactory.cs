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

    public IPropertyFinder CreatePropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      return new BindableDomainObjectPropertyFinder (concreteType);
    }

    public PropertyReflector CreatePropertyReflector (Type concreteType, PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new BindableDomainObjectPropertyReflector (concreteType, propertyInfo, businessObjectProvider);
    }
  }
}