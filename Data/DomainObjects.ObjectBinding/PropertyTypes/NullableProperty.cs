using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class NullableProperty : DomainObjectProperty
{
  bool _isNullableType;

  public NullableProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList, 
      bool isNullableType)
    : base (propertyInfo, propertyDefinition, itemType, isList)
  {
    _isNullableType = isNullableType;
  }

  protected bool IsNullableType
  {
    get { return _isNullableType; }
  }

  public override bool IsRequired
  {
    get { return ! _isNullableType; }
  }

  protected internal override object FromInternalType(object internalValue)
  {
    if (internalValue == null)
      return internalValue;
    else
      return base.FromInternalType (internalValue);
  }

  protected internal override object ToInternalType(object publicValue)
  {
    if (!IsNullableType && publicValue == null)
      throw new InvalidNullAssignmentException (ItemType);

    return publicValue;
  }

}
}
