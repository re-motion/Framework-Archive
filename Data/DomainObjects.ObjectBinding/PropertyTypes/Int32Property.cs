using System;
using System.Reflection;

using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

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

  protected internal override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaInt32.ToBoxedInt32 ((NaInt32)internalValue);
    
    return base.FromInternalType (internalValue);
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
      return NaInt32.FromBoxedInt32 (publicValue);

    return base.ToInternalType (publicValue);
  }
}
}
