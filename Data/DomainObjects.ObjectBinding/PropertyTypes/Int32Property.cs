using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class Int32Property : NullableProperty, IBusinessObjectInt32Property
{
  public Int32Property (      
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

    if (IsNaNullableType)
      return NaInt32.ToBoxedInt32 ((NaInt32)internalValue);
    
    return base.FromInternalType (internalValue);
  }

  public override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNaNullableType)
      return NaInt32.FromBoxedInt32 (publicValue);

    return base.ToInternalType (publicValue);
  }
}
}
