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
public class GuidProperty : DomainObjectProperty, IBusinessObjectStringProperty
{
  public GuidProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType,
      bool isList)
    : base (propertyInfo, propertyDefinition, itemType, isList)
  {
  }

  public NaInt32 MaxLength
  {
    get { return 38; }
  }

  protected internal override object FromInternalType(object internalValue)
  {
    if (!IsList)
      return new Guid (internalValue.ToString ());
    else
      return internalValue;
  }

  protected internal override object ToInternalType(object publicValue)
  {
    if (!IsList)
      return new Guid (publicValue.ToString ());
    else
      return publicValue;
  }
}
}
