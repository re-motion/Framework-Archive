using System;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class StringProperty : BaseProperty, IBusinessObjectStringProperty
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

  public override object ToInternalType (object publicValue)
  {
    return publicValue;
  }
}
}
