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
  private NaInt32 _maxLength;

  public StringProperty (
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList,
      NaInt32 maxLength)
    : base (propertyInfo, isRequired, itemType, isList)
  {
    _maxLength = maxLength;
  }

  public NaInt32 MaxLength
  {
    get { return _maxLength; }
  }

  protected internal override object ToInternalType (object publicValue)
  {
    return publicValue;
  }
}
}
