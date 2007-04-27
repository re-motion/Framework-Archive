using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class DoubleProperty : NullableProperty, IBusinessObjectDoubleProperty
{
  public DoubleProperty (
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
      return NaDouble.ToBoxedDouble ((NaDouble)internalValue);

    return base.FromInternalType (internalValue);
  }

  public override object ToInternalType (object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNaNullableType)
      return NaDouble.FromBoxedDouble (publicValue);

    return base.ToInternalType (publicValue);
  }
}
}
