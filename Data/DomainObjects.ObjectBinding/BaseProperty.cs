using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Globalization;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

using Rubicon.ObjectBinding;

using Rubicon.Data.DomainObjects.Mapping;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
public class DomainObjectProperty: IBusinessObjectProperty
{
  public static DomainObjectProperty Create (
      PropertyInfo propertyInfo, 
      ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    bool isList = false;
    Type itemType = null;

    if (propertyInfo.PropertyType == typeof (DomainObjectCollection)
        || propertyInfo.PropertyType.IsSubclassOf (typeof (DomainObjectCollection)))
    {
      isList = true;

      IRelationEndPointDefinition oppositeRelationEndPointDefinition = 
          classDefinition.GetOppositeEndPointDefinition (propertyInfo.Name);

      itemType = oppositeRelationEndPointDefinition.ClassDefinition.ClassType;
    }
    else
    {
      isList = propertyInfo.PropertyType.IsArray;
      itemType = isList ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;
    }

    bool isNullableType = false;
    if (NaTypeUtility.IsNaNullableType (itemType))
    {
      isNullableType = true;
      itemType = NaTypeUtility.GetBasicType (itemType);
    }

    PropertyDefinition propertyDefinition = 
        classDefinition.GetPropertyDefinition (propertyInfo.Name);

    if (itemType == typeof (string))
    {
      return new StringProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
    if (itemType == typeof (Guid))
    {
      return new GuidProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
    else if (itemType == typeof (char))
    {
      return new CharProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
    else if (itemType == typeof (byte))
    {
      return new ByteProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
    else if (itemType == typeof (int))
    {
      return new Int32Property (propertyInfo, propertyDefinition, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (Int16))
    {
      return new Int16Property (propertyInfo, propertyDefinition, itemType, isList);
    }
    else if (itemType == typeof (Single))
    {
      return new SingleProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
    else if (itemType == typeof (double))
    {
      return new DoubleProperty (propertyInfo, propertyDefinition, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (decimal))
    {
      return new DecimalProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
    else if (itemType == typeof (bool))
    {
      return new BooleanProperty (propertyInfo, propertyDefinition, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (DateTime))
    {
      if (propertyDefinition != null && propertyDefinition.MappingType == "date")
        return new DateProperty (propertyInfo, propertyDefinition, itemType, isList, isNullableType);
      else
        return new DateTimeProperty (propertyInfo, propertyDefinition, itemType, isList, isNullableType);
    }
    else if (itemType.IsEnum)
    {
      return new EnumerationProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
    else if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (itemType))
    {
      return new ReferenceProperty (propertyInfo, propertyDefinition, 
        classDefinition.GetRelationEndPointDefinition (propertyInfo.Name), itemType, isList);
    }
    else
    {
      return new DomainObjectProperty (propertyInfo, propertyDefinition, itemType, isList);
    }
  }

  PropertyInfo _propertyInfo;
  PropertyDefinition _propertyDefinition;
  Type _itemType;
  bool _isList;

  protected DomainObjectProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList)
  {
    _propertyInfo = propertyInfo;
    _propertyDefinition = propertyDefinition;
    _itemType = itemType;
    _isList = isList;
  }

  public bool IsList
  {
    get { return _isList; }
  }

  public IList CreateList (int count)
  {
    if (! IsList)
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
      {
        displayName = MultiLingualResourcesAttribute.GetResourceText (
            _propertyInfo.DeclaringType, "property:" + _propertyInfo.Name);
      }

      if (displayName == string.Empty)
        displayName = _propertyInfo.Name;

      return displayName; 
    }
  }

  public virtual bool IsRequired
  {
    get { return (_propertyDefinition != null) ? !_propertyDefinition.IsNullable : true;  }
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
    return ! _propertyInfo.CanWrite;
  }

  public PropertyInfo PropertyInfo
  {
    get { return _propertyInfo; }
  }

  public PropertyDefinition PropertyDefinition
  {
    get { return _propertyDefinition; }
  }

  internal protected virtual object FromInternalType (object internalValue)
  {
    return internalValue;
  }

  internal protected virtual object ToInternalType (object publicValue)
  {
    return publicValue;
  }
  
  public IBusinessObjectProvider BusinessObjectProvider
  {
    get { return DomainObjectProvider.Instance; }
  }
}
}
