using System;
using System.Reflection;

using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;

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
