using System;
using System.Reflection;
using System.Collections;

using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class BaseProperty : IBusinessObjectProperty
{
  PropertyInfo _propertyInfo;
  bool _isRequired;
  Type _itemType;
  bool _isList;

  internal BaseProperty (PropertyInfo propertyInfo, bool isRequired, Type itemType, bool isList)
  {
    _propertyInfo = propertyInfo;
    _isRequired = isRequired;
    _itemType = itemType;
    _isList = isList;
  }

  public bool IsList
  {
    get { return _isList; }
  }

  public IList CreateList (int count)
  {
    if (!IsList)
      throw new InvalidOperationException ("Cannot create lists for non-list properties.");

    return Array.CreateInstance (_itemType, count);
  }

  public Type ItemType
  {
    get { return _itemType; }
  }

  public virtual Type PropertyType
  {
    get { return _propertyInfo.PropertyType;  }
  }

  public string Identifier
  {
    get { return _propertyInfo.Name; }
  }

  public string DisplayName
  {
    get 
    { 
      string displayName = string.Empty;

      if (MultiLingualResourcesAttribute.ExistsResource (_propertyInfo.DeclaringType))
        displayName = MultiLingualResourcesAttribute.GetResourceText (_propertyInfo.DeclaringType, "property:" + _propertyInfo.Name);

      if (displayName == string.Empty)
        displayName = _propertyInfo.Name;

      return displayName; 
    }
  }

  public virtual bool IsRequired
  {
    get { return _isRequired;  }
  }
  
  public bool IsAccessible (IBusinessObjectClass objectClass, IBusinessObject obj)
  {
    return true;
  }

  public bool IsAccessible (IBusinessObject obj)
  {
    return true;
  }

  public bool IsReadOnly (IBusinessObject obj)
  {
    return !_propertyInfo.CanWrite;
  }

  public PropertyInfo PropertyInfo
  {
    get { return _propertyInfo; }
  }

  internal protected virtual object FromInternalType (object internalValue)
  {
    if (!IsList)
    {
      FieldInfo minValueFieldInfo = _itemType.GetField ("MinValue");

      if (minValueFieldInfo != null)
      {
        if (minValueFieldInfo.GetValue (_itemType).ToString () == internalValue.ToString ())
          return null;
      }
    }

    return internalValue;
  }

  internal protected virtual object ToInternalType (object publicValue)
  {
    if (_itemType.IsValueType && publicValue == null)
      throw new InvalidNullAssignmentException (_itemType);

    return publicValue;
  }
  
  public IBusinessObjectProvider BusinessObjectProvider
  {
    get { return DomainObjectProvider.Instance; }
  }
}
}
