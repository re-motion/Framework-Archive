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
public class ByteProperty : NullableProperty, IBusinessObjectInt32Property
{
  public ByteProperty (      
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
    get { return false; }
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaByte.ToBoxedByte ((NaByte)internalValue);

    return int.Parse (internalValue.ToString ());  
  }

  protected internal override object ToInternalType (object publicValue)
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

    return byte.Parse (publicValue.ToString ());
  }
}
}
