using System;
using System.Reflection;

using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class DecimalProperty : NullableProperty, IBusinessObjectDoubleProperty
{
  public DecimalProperty (
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

  protected internal override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaDecimal.ToBoxedDecimal ((NaDecimal)internalValue);

    return base.FromInternalType (decimal.Parse (internalValue.ToString ()));  
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
    {
      if (publicValue != null)
        return NaDecimal.Parse (publicValue.ToString ());
      else
        return NaDecimal.Null;
    }

    return decimal.Parse (base.ToInternalType (publicValue).ToString ());
  }
}
}
