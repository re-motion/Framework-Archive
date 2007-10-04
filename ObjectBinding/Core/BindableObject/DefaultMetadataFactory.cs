using System;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class DefaultMetadataFactory : IMetadataFactory
  {
    public static readonly DefaultMetadataFactory Instance = new DefaultMetadataFactory ();

    protected DefaultMetadataFactory ()
    {
    }

    public virtual IPropertyFinder CreatePropertyFinder (Type concreteType)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);

      return new ReflectionBasedPropertyFinder (concreteType);
    }

    public virtual PropertyReflector CreatePropertyReflector (Type concreteType, PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("concreteType", concreteType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new PropertyReflector (propertyInfo, businessObjectProvider);
    }
  }
}