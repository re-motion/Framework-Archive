using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding
{
  public class PropertyReflector
  {
   private readonly PropertyInfo _propertyInfo;

    public PropertyReflector (PropertyInfo propertyInfo)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    public PropertyBase GetMetadata ()
    {
      Type itemType = GetItemType();
      if (itemType == typeof (string))
        return CreateStringProperty ();

      return new NotSupportedProperty (_propertyInfo, itemType, GetIsList (), GetIsRequired ());
    }

    private StringProperty CreateStringProperty ()
    {
      return new StringProperty (_propertyInfo, typeof (string), GetIsList (), GetIsRequired (), GetMaxLength ());
    }

    private Type GetItemType ()
    {
      if (_propertyInfo.PropertyType.IsArray)
        return _propertyInfo.PropertyType.GetElementType();
      if (ReflectionUtility.CanAscribe (_propertyInfo.PropertyType, typeof (IList<>)))
        return ReflectionUtility.GetAscribedGenericArguments (_propertyInfo.PropertyType, typeof (IList<>))[0];
      if (typeof (IList).IsAssignableFrom (_propertyInfo.PropertyType))
        return GetItemTypeFromAttribute();
      return _propertyInfo.PropertyType;
    }

    private Type GetItemTypeFromAttribute ()
    {
      ItemTypeAttribute itemTypeAttribute = AttributeUtility.GetCustomAttribute<ItemTypeAttribute> (_propertyInfo, true);
      if (itemTypeAttribute == null)
        throw new Exception("ItemTypeAttribute is required for properties of type IList.");

      return itemTypeAttribute.ItemType;
    }

    private bool GetIsList ()
    {
      return _propertyInfo.PropertyType.IsArray
        || ReflectionUtility.CanAscribe (_propertyInfo.PropertyType, typeof (IList<>))
        || typeof (IList).IsAssignableFrom (_propertyInfo.PropertyType);
    }

    private bool GetIsRequired ()
    {
      return false;
    }

    private int? GetMaxLength ()
    {
      return null;
    }
  }
}