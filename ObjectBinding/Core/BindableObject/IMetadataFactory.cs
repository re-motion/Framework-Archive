using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  public interface IMetadataFactory
  {
    IPropertyFinder CreatePropertyFinder (Type concreteType);
    PropertyReflector CreatePropertyReflector (Type concreteType, PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider);
  }
}