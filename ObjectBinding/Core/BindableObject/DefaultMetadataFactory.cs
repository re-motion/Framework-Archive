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

    public virtual IPropertyFinder CreatePropertyFinder (Type targetType)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);

      return new ReflectionBasedPropertyFinder (targetType);
    }

    public virtual PropertyReflector CreatePropertyReflector (Type targetType, PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("targetType", targetType);
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      return new PropertyReflector (propertyInfo, businessObjectProvider);
    }
  }
}