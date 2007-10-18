using System;
using System.Reflection;
using Rubicon.ObjectBinding.BindableObject.Properties;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface IBindableObjectGlobalizationService : IBusinessObjectService
  {
    string GetEnumerationValueDisplayName (Enum value);
    string GetBooleanValueDisplayName (bool value);
    string GetPropertyDisplayName (IPropertyInformation info);
  }
}