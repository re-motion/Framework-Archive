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
public class CharProperty : DomainObjectProperty, IBusinessObjectStringProperty
{
  public CharProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList)
    : base (propertyInfo, propertyDefinition, itemType, isList)
  {
  }

  public NaInt32 MaxLength
  {
    get { return 1; }
  }

  protected internal override object ToInternalType(object publicValue)
  {
    return char.Parse (publicValue.ToString ());
  }
}
}
