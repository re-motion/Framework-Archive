using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject.Properties;

namespace Rubicon.ObjectBinding.BindableObject
{
  public interface IMetadataFactory
  {
    IPropertyFinder CreatePropertyFinder (Type concreteType);
    PropertyReflector CreatePropertyReflector (Type concreteType, IPropertyInformation propertyInfo, BindableObjectProvider businessObjectProvider);
  }
}