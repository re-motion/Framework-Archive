using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

using Rubicon.ObjectBinding;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class DomainObjectByteProperty: DomainObjectProperty, IBusinessObjectInt32Property
{
  public DomainObjectByteProperty (      
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList)
      : base (propertyInfo, propertyDefinition, itemType, isList)
  {
  }

  public bool AllowNegative
  {
    get { return false; }
  }

  protected internal override object FromInternalType (object internalValue)
  {
    if (! IsList)
      return int.Parse (internalValue.ToString ());
    else
      return internalValue;
  }

  protected internal override object ToInternalType (object publicValue)
  {
    if (! IsList)
      return byte.Parse (publicValue.ToString ());
    else
      return publicValue;
  }
}
}
