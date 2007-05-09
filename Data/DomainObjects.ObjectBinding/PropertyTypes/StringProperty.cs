using System;
using System.Reflection;
using Rubicon.ObjectBinding;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class StringProperty : BaseProperty, IBusinessObjectStringProperty
{
  private int? _maxLength;

  public StringProperty (
      IBusinessObjectClass businessObjectClass,
      PropertyInfo propertyInfo, 
      bool isRequired,
      Type itemType, 
      bool isList,
      int? maxLength)
    : base (businessObjectClass, propertyInfo, isRequired, itemType, isList)
  {
    _maxLength = maxLength;
  }

  public int? MaxLength
  {
    get { return _maxLength; }
  }
}
}
