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
public class GuidProperty : NullableProperty, IBusinessObjectStringProperty
{
  public GuidProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType,
      bool isList, 
      bool isNullableType)
    : base (propertyInfo, propertyDefinition, itemType, isList, isNullableType)
  {
  }

  public NaInt32 MaxLength
  {
    get { return 38; }
  }

  protected internal override object FromInternalType(object internalValue)
  {
    if (IsList)
      return internalValue;

    if (IsNullableType)
      return NaGuid.ToBoxedGuid ((NaGuid)internalValue);

    return new Guid (internalValue.ToString ());  
  }

  protected internal override object ToInternalType(object publicValue)
  {
    if (IsList)
      return publicValue;

    if (IsNullableType)
    {
      if (publicValue != null)
        return new NaGuid (new Guid (publicValue.ToString ()));
      else
        return NaGuid.Null;
    }

    return new Guid (publicValue.ToString ());
  }
}
}
