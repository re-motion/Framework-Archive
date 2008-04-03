using System;
using System.Reflection;
using Remotion.ObjectBinding;

namespace Remotion.Data.DomainObjects.ObjectBinding.PropertyTypes
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
