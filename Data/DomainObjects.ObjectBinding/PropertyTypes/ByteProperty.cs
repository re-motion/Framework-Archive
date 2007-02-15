using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class ByteProperty : NullableProperty, IBusinessObjectInt32Property
{
  public ByteProperty (      
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
    get { return false; }
  }

  public override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaByte.ToBoxedByte ((NaByte)internalValue);

    return base.FromInternalType (int.Parse (internalValue.ToString ()));  
  }

  public override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
    {
      if (publicValue != null)
        return NaByte.Parse (publicValue.ToString ());
      else
        return NaByte.Null;
    }

    return byte.Parse (base.ToInternalType (publicValue).ToString ());
  }
}
}
