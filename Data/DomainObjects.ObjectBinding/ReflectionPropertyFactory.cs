using System;
using System.Reflection;

using Rubicon.Utilities;
using Rubicon.NullableValueTypes;

using Rubicon.ObjectBinding;
using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;

namespace Rubicon.Data.DomainObjects.ObjectBinding
{
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
      return new StringProperty (propertyInfo, isRequired, itemType, isList, GetMaxStringLength (propertyInfo));
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
    object[] itemTypeAttributes = propertyInfo.GetCustomAttributes (typeof (ItemTypeAttribute), true);

    if (itemTypeAttributes.Length > 0)
    {
      itemType = ((ItemTypeAttribute) itemTypeAttributes[0]).ItemType;
      if (itemType != typeof (BindableDomainObject) && !itemType.IsSubclassOf (typeof (BindableDomainObject)))
      {
        throw new InvalidOperationException ("The ItemType defined for a property of type "
          + "DomainObjectCollection or subclass of must be a DomainObject or a subclass of.");
      }
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
    object[] isRequiredAttributes = propertyInfo.GetCustomAttributes (typeof (IsRequiredAttribute), true);

    if (isRequiredAttributes.Length > 0)
      return ((IsRequiredAttribute) isRequiredAttributes[0]).IsRequired;
      
    if (NaTypeUtility.IsNaNullableType (propertyInfo.PropertyType) || propertyInfo.PropertyType == typeof (string))
      return false;

    return true;
  }

  protected virtual NaInt32 GetMaxStringLength (PropertyInfo propertyInfo)
  {
    return NaInt32.Null;
  }

  protected virtual bool IsDateType (PropertyInfo propertyInfo)
  {
    object[] dateTypeAttributes = propertyInfo.GetCustomAttributes (typeof (DateTypeAttribute), true);

    if (dateTypeAttributes.Length > 0)
      return ((DateTypeAttribute) dateTypeAttributes[0]).DateType == DateTypeEnum.Date;

    return false;
  }

  protected virtual bool IsRelationMandatory (PropertyInfo propertyInfo)
  {
    return IsPropertyRequired (propertyInfo);
  }
}
}
