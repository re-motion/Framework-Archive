using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class Int16Property : NullableProperty, IBusinessObjectInt32Property
{
  public Int16Property (      
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
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaInt16.ToBoxedInt16 ((NaInt16)internalValue);

    return short.Parse (internalValue.ToString ());  
  }

  protected internal override object ToInternalType (object publicValue)
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

    return short.Parse (publicValue.ToString ());
  }
}
}
