using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class GuidProperty : NullableProperty, IBusinessObjectStringProperty
{
  public GuidProperty (
      PropertyInfo propertyInfo,
      bool isRequired,
      Type itemType,
      bool isList, 
      bool isNullableType)
    : base (propertyInfo, isRequired, itemType, isList, isNullableType)
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

    Guid guidValue = new Guid (internalValue.ToString ());

    if (guidValue == Guid.Empty)
      return null;

    return guidValue;  
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

    return new Guid (base.ToInternalType (publicValue).ToString ());
  }
}
}
