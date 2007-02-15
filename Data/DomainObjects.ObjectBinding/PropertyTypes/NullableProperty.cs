using System;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class NullableProperty : BaseProperty
{
  bool _isNullableType;

  public NullableProperty (
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList, 
      bool isNullableType)
    : base (propertyInfo, isRequired, itemType, isList)
  {
    _isNullableType = isNullableType;
  }

  protected bool IsNullableType
  {
    get { return _isNullableType; }
  }

  public override bool IsRequired
  {
    get { return base.IsRequired; }
  }

  public override object FromInternalType (object internalValue)
  {
    if (internalValue == null)
      return null;

    return base.FromInternalType (internalValue);
  }

  public override object ToInternalType (object publicValue)
  {
    if (!IsNullableType && publicValue == null)
      throw new InvalidNullAssignmentException (ItemType);

    return publicValue;
  }
}
}
