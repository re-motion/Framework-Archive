using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class GuidProperty : NullableProperty, IBusinessObjectStringProperty
{
  public GuidProperty (
      PropertyInfo propertyInfo,
      bool isRequired,
      Type itemType,
      bool isList, 
      bool isNullableType)
    : base (propertyInfo, isRequired, itemType, isList, isNullableType)
  {
  }

  public int? MaxLength
  {
    get { return 38; }
  }

  public override object FromInternalType(object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNaNullableType)
      return NaGuid.ToBoxedGuid ((NaGuid)internalValue);

    if (IsNullableType && internalValue == null)
      return null;

    Guid guidValue = new Guid (internalValue.ToString ());

    if (guidValue == Guid.Empty)
      return null;

    return guidValue;  
  }

  public override object ToInternalType(object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNaNullableType)
    {
      if (publicValue != null)
        return new NaGuid (new Guid (publicValue.ToString ()));
      else
        return NaGuid.Null;
    }

    if (IsNullableType && publicValue == null)
      return null;

    return new Guid (base.ToInternalType (publicValue).ToString ());
  }
}
}
