using System;
using System.Reflection;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public interface IBindableObjectGlobalizationService : IBusinessObjectService
  {
    string GetEnumerationValueDisplayName (Enum value);
    string GetBooleanValueDisplayName (bool value);
    string GetPropertyDisplayName (PropertyInfo info);
  }
}