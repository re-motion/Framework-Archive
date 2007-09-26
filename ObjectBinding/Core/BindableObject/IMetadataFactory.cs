using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  public interface IMetadataFactory
  {
    IPropertyFinder CreatePropertyFinder (Type targetType);
    PropertyReflector CreatePropertyReflector (PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider);
  }
}