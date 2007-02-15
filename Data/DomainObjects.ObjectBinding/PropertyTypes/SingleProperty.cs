using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class SingleProperty : NullableProperty, IBusinessObjectDoubleProperty
{
  public SingleProperty (
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList, 
      bool isNullableType)
      : base (propertyInfo, isRequired, itemType, isList, isNullableType)
  {
  }

  public bool AllowNegative
  {
    get { return true; }
  }

  public override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaSingle.ToBoxedSingle ((NaSingle)internalValue);

    return base.FromInternalType (float.Parse (internalValue.ToString ()));  
  }

  public override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
    {
      if (publicValue != null)
        return NaSingle.Parse (publicValue.ToString ());
      else
        return NaSingle.Null;
    }

    return float.Parse (base.ToInternalType (publicValue).ToString ());
  }
}
}
