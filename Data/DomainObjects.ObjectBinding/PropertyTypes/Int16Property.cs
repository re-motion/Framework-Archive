using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class Int16Property : NullableProperty, IBusinessObjectInt32Property
{
  public Int16Property (      
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
      return NaInt16.ToBoxedInt16 ((NaInt16)internalValue);

    return base.FromInternalType (short.Parse (internalValue.ToString ()));  
  }

  public override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
    {
      if (publicValue != null)
        return NaInt16.Parse (publicValue.ToString ());
      else
        return NaInt16.Null;
    }

    return short.Parse (base.ToInternalType (publicValue).ToString ());
  }
}
}
