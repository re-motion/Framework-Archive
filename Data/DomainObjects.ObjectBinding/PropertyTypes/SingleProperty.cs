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
public class SingleProperty : DomainObjectProperty, IBusinessObjectDoubleProperty
{
  public SingleProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList)
      : base (propertyInfo, propertyDefinition, itemType, isList)
  {
  }

  public bool AllowNegative
  {
    get { return true; }
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (!IsList)
      return Single.Parse (internalValue.ToString ());
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (!IsList)
      return Single.Parse (publicValue.ToString ());
    else
      return publicValue;
  }
}
}
