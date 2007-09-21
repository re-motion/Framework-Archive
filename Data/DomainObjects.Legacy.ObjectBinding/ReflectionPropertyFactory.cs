using System;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.NullableValueTypes;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;
using StringProperty=Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes.StringProperty;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
// TODO Doc: 
public class ReflectionPropertyFactory
{
  private IBusinessObjectClass _businessObjectClass;

  public ReflectionPropertyFactory (IBusinessObjectClass businessObjectClass)
	{
    ArgumentUtility.CheckNotNull ("businessObjectClass", businessObjectClass);

    _businessObjectClass = businessObjectClass;
	}

  protected IBusinessObjectClass BusinessObjectClass
  {
    get { return _businessObjectClass; }
  }

  public BaseProperty CreateProperty (PropertyInfo propertyInfo)
  {
    ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);

    Type itemType = GetItemType (propertyInfo);

    bool isNullableType = false;
    if (Nullable.GetUnderlyingType (itemType) != null)
    {
      itemType = Nullable.GetUnderlyingType (itemType);
      isNullableType = true;
    }
    else if (NaTypeUtility.IsNaNullableType (itemType))
    {
      itemType = NaTypeUtility.GetBasicType (itemType);
      isNullableType = true;
    }

    return CreateProperty (propertyInfo, itemType, isNullableType);
  }

  protected virtual BaseProperty CreateProperty (PropertyInfo propertyInfo, Type itemType, bool isNullableType)
  {
    bool isRequired = IsPropertyRequired (propertyInfo);
    bool isList = IsList (propertyInfo);

    if (itemType == typeof (string))
    {
      return new StringProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, GetMaxStringLength (propertyInfo));
    }
    else if (itemType == typeof (Guid))
    {
      return new GuidProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (byte))
    {
      return new ByteProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (int))
    {
      return new Int32Property (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (Int16))
    {
      return new Int16Property (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (Single))
    {
      return new SingleProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (double))
    {
      return new DoubleProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (decimal))
    {
      return new DecimalProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (bool))
    {
      return new BooleanProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType == typeof (DateTime))
    {
      if (IsDateType (propertyInfo))
        return new DateProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
      else
        return new DateTimeProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList, isNullableType);
    }
    else if (itemType.IsEnum)
    {
      return new EnumerationProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList);
    }
    else if (typeof (IBusinessObjectWithIdentity).IsAssignableFrom (itemType))
    {
      return new ReferenceProperty (_businessObjectClass, propertyInfo, IsRelationMandatory (propertyInfo), itemType, isList);
    }
    else
    {
      return new BaseProperty (_businessObjectClass, propertyInfo, isRequired, itemType, isList);
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

    if (Nullable.GetUnderlyingType (propertyInfo.PropertyType) != null 
        || NaTypeUtility.IsNaNullableType (propertyInfo.PropertyType)
        || propertyInfo.PropertyType == typeof (string))
    {
      return false;
    }

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
