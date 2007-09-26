using System;
using System.Reflection;

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
      return new ReflectionBasedPropertyFinder (targetType);
    }

    public virtual PropertyReflector CreatePropertyReflector (PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      return new PropertyReflector (propertyInfo, businessObjectProvider);
    }
  }
}