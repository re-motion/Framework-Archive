using System;
using System.Reflection;

using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class ReferenceProperty : NullableProperty, IBusinessObjectReferenceProperty
{
  public ReferenceProperty (
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList)
      : base (propertyInfo, isRequired, itemType, isList, true)
  {
  }

  public IBusinessObjectClass ReferenceClass
  {
    get { return new DomainObjectClass ((IsList) ? ItemType : PropertyType); }
  }

  public IBusinessObjectWithIdentity[] SearchAvailableObjects (IBusinessObject obj, string searchStatement)
  {
    return new BindableDomainObject[] {};
  }

  public bool SupportsSearchAvailableObjects
  {
    get { return false; }
  }
}
}
