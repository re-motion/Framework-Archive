using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class StringProperty : BaseProperty, IBusinessObjectStringProperty
{
  private int? _maxLength;

  public StringProperty (
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList,
      int? maxLength)
    : base (propertyInfo, isRequired, itemType, isList)
  {
    _maxLength = maxLength;
  }

  public int? MaxLength
  {
    get { return _maxLength; }
  }

  public override object ToInternalType (object publicValue)
  {
    return publicValue;
  }
}
}
