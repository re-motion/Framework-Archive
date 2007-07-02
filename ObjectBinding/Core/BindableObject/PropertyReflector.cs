using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  public class PropertyReflector
  {
    private readonly PropertyInfo _propertyInfo;
    private readonly BindableObjectProvider _businessObjectProvider;

    public PropertyReflector (PropertyInfo propertyInfo, BindableObjectProvider businessObjectProvider)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      ArgumentUtility.CheckNotNull ("businessObjectProvider", businessObjectProvider);

      _propertyInfo = propertyInfo;
      _businessObjectProvider = businessObjectProvider;
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    } 

    public BindableObjectProvider BusinessObjectProvider
    {
      get { return _businessObjectProvider; }
    } 

    public PropertyBase GetMetadata ()
    {
      Type nativeType = GetNativeType();

      if (nativeType == typeof (Boolean))
        return new BooleanProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (Byte))
        return new ByteProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (Decimal))
        return new DecimalProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (Double))
        return new DoubleProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType.IsEnum && nativeType.IsValueType)
        return new EnumerationProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (Int16))
        return new Int16Property (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (Int32))
        return new Int32Property (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (Int64))
        return new Int64Property (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (Single))
        return new SingleProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
      else if (nativeType == typeof (String))
        return new StringProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired (), GetMaxLength ());
      else
        return new NotSupportedProperty (_businessObjectProvider, _propertyInfo, GetListInfo (), GetIsRequired ());
    }

    private Type GetNativeType ()
    {
      return Nullable.GetUnderlyingType (GetItemType()) ?? GetItemType();
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
        throw new Exception ("ItemTypeAttribute is required for properties of type IList.");

      return itemTypeAttribute.ItemType;
    }

    private IListInfo GetListInfo ()
    {
      bool isList =
          _propertyInfo.PropertyType.IsArray
          || ReflectionUtility.CanAscribe (_propertyInfo.PropertyType, typeof (IList<>))
          || typeof (IList).IsAssignableFrom (_propertyInfo.PropertyType);

      if (isList)
        return new ListInfo (GetItemType());

      return null;
    }

    private bool GetIsRequired ()
    {
      if (_propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType (_propertyInfo.PropertyType) == null)
        return true;
      return false;
    }

    private int? GetMaxLength ()
    {
      return null;
    }
  }
}