using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
// TODO Doc: 
public class ReflectionPropertyFactory
{
	public ReflectionPropertyFactory()
	{
	}

  public BaseProperty CreateProperty (PropertyInfo propertyInfo)
  {
    ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

    Type itemType = GetItemType (propertyInfo);

    bool isNullableType = NaTypeUtility.IsNaNullableType (itemType);
    if (isNullableType)
      itemType = NaTypeUtility.GetBasicType (itemType);

    return CreateProperty (propertyInfo, itemType, isNullableType);
  }

  protected virtual BaseProperty CreateProperty (PropertyInfo propertyInfo, Type itemType, bool isNullableType)
  {
    bool isRequired = IsPropertyRequired (propertyInfo);
    bool isList = IsList (propertyInfo);

    if (itemType == typeof (string))
    {
      return new StringProperty (propertyInfo, isRequired, itemType, isList, NaInt32.FromBoxedInt32 (GetMaxStringLength (propertyInfo)));
    }
    else if (itemType == typeof (Guid))
    {
      return new GuidProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (byte))
    {
      return new ByteProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (int))
    {
      return new Int32Property (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (Int16))
    {
      return new Int16Property (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (Single))
    {
      return new SingleProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (double))
    {
      return new DoubleProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (decimal))
    {
      return new DecimalProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (bool))
    {
      return new BooleanProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (DateTime))
    {
      if (IsDateType (propertyInfo))
        return new DateProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
      else
        return new DateTimeProperty (propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType.IsEnum)
    {
      return new EnumerationProperty (propertyInfo, isRequired, itemType, isList);
    }
    else if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (itemType))
    {
      return new ReferenceProperty (propertyInfo, IsRelationMandatory (propertyInfo), itemType, isList);
    }
    else
    {
      return new BaseProperty (propertyInfo, isRequired, itemType, isList);
    }
  }

  protected virtual Type GetItemType (PropertyInfo propertyInfo)
  {
    if (IsDomainObjectCollection (propertyInfo))
      return GetItemTypeForDomainObjectCollection (propertyInfo);

    return IsList (propertyInfo) ? propertyInfo.PropertyType.GetElementType() : propertyInfo.PropertyType;
  }

  protected virtual Type GetItemTypeForDomainObjectCollection (PropertyInfo propertyInfo)
  {
    Type itemType = typeof (BindableDomainObject);
    ItemTypeAttribute itemTypeAttribute = AttributeUtility.GetCustomAttribute<ItemTypeAttribute> (propertyInfo, true);
    if (itemTypeAttribute != null)
    {
      itemType = itemTypeAttribute.ItemType;
      if (itemType != typeof (BindableDomainObject) && !itemType.IsSubclassOf (typeof (BindableDomainObject)))
      {
        throw new InvalidOperationException (string.Format ("The ItemType defined for the collection property '{0}' "
            + "must be 'Rubicon.Data.DomainObjects.ObjectBinding.BindableDomainObject' or a subclass of it.", propertyInfo.Name));
      }
    }
    else if (propertyInfo.PropertyType.IsGenericType)
    {
      itemType = propertyInfo.PropertyType.GetGenericArguments ()[0];
    }

    return itemType;
  }

  protected virtual bool IsDomainObjectCollection (PropertyInfo propertyInfo)
  {
    return propertyInfo.PropertyType == typeof (DomainObjectCollection) || propertyInfo.PropertyType.IsSubclassOf (typeof (DomainObjectCollection));
  }

  protected virtual bool IsList (PropertyInfo propertyInfo)
  {
    return IsDomainObjectCollection (propertyInfo) || propertyInfo.PropertyType.IsArray;
  }

  protected virtual bool IsPropertyRequired (PropertyInfo propertyInfo)
  {
    IsRequiredAttribute isRequiredAttribute = AttributeUtility.GetCustomAttribute<IsRequiredAttribute> (propertyInfo, true);
    if (isRequiredAttribute != null)
      return isRequiredAttribute.IsRequired;
      
    if (NaTypeUtility.IsNaNullableType (propertyInfo.PropertyType) || propertyInfo.PropertyType == typeof (string))
      return false;

    if (!propertyInfo.PropertyType.IsValueType)
      return false;

    return true;
  }

  protected virtual int? GetMaxStringLength (PropertyInfo propertyInfo)
  {
    return null;
  }

  protected virtual bool IsDateType (PropertyInfo propertyInfo)
  {
    DateTypeAttribute dateTypeAttribute = AttributeUtility.GetCustomAttribute<DateTypeAttribute> (propertyInfo, true);
    if (dateTypeAttribute != null)
      return dateTypeAttribute.DateType == DateTypeEnum.Date;

    return false;
  }

  protected virtual bool IsRelationMandatory (PropertyInfo propertyInfo)
  {
    return IsPropertyRequired (propertyInfo);
  }
}
}
