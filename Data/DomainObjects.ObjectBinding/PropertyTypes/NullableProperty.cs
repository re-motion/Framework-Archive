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
public class DomainObjectNullableProperty: DomainObjectProperty
{
  bool _isNullableType;

  public DomainObjectNullableProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList, 
      bool isNullableType)
    : base (propertyInfo, propertyDefinition, itemType, isList)
  {
    _isNullableType = isNullableType;
  }

  protected bool IsNullableType
  {
    get { return _isNullableType; }
  }

  public override bool IsRequired
  {
    get { return ! _isNullableType; }
  }
}
}
