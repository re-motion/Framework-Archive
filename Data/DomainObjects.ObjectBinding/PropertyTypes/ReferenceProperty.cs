using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class ReferenceProperty : DomainObjectProperty, IBusinessObjectReferenceProperty
{
  private IRelationEndPointDefinition _relationEndPointDefinition;

  public ReferenceProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      IRelationEndPointDefinition relationEndPointDefinition,
      Type itemType, 
      bool isList)
      : base (propertyInfo, propertyDefinition, itemType, isList)
  {
    _relationEndPointDefinition = relationEndPointDefinition;
  }

  public IBusinessObjectClass ReferenceClass
  {
    get
    {
      return new DomainObjectClass ((IsList) ? ItemType : PropertyType);
    }
  }

  public IBusinessObjectWithIdentity[] SearchAvailableObjects(IBusinessObject obj, string searchStatement)
  {
    return new BindableDomainObject[] {};
  }

  public bool SupportsSearchAvailableObjects
  {
    get
    {
      return false;
    }
  }

  public override bool IsRequired
  {
    get
    {
      bool isRequired = base.IsRequired;

      if (_relationEndPointDefinition != null)
        isRequired = _relationEndPointDefinition.IsMandatory;

      return isRequired;
    }
  }

}
}
