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
public class StringProperty : DomainObjectProperty, IBusinessObjectStringProperty
{
  public StringProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList)
    : base (propertyInfo, propertyDefinition, itemType, isList)
  {
  }

  public NaInt32 MaxLength
  {
    get 
    { 
      return (PropertyDefinition != null) ? PropertyDefinition.MaxLength : NaInt32.Null; 
    }
  }

  protected internal override object ToInternalType(object publicValue)
  {
    return publicValue;
  }

}
}
