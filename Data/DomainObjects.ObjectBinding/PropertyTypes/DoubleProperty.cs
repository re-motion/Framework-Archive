using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class DoubleProperty : NullableProperty, IBusinessObjectDoubleProperty
{
  public DoubleProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList, 
      bool isNullableType)
      : base (propertyInfo, propertyDefinition, itemType, isList, isNullableType)
  {
  }

  public bool AllowNegative
  {
    get { return true; }
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (!IsList && IsNullableType)
      return NaDouble.ToBoxedDouble ((NaDouble)internalValue);
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (!IsList && IsNullableType)
      return NaDouble.FromBoxedDouble (publicValue);
    else
      return publicValue;
  }
}
}
