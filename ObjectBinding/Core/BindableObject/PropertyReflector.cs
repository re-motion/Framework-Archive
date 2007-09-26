using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Rubicon.Mixins;
using Rubicon.ObjectBinding.BindableObject.Properties;
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
      Type underlyingType = GetUnderlyingType();
      PropertyBase.Parameters parameters = CreateParameters(underlyingType);

      if (underlyingType == typeof (Boolean))
        return new BooleanProperty (parameters);
      else if (underlyingType == typeof (Byte))
        return new ByteProperty (parameters);
      else if (underlyingType == typeof (DateTime) && AttributeUtility.IsDefined<DatePropertyAttribute> (_propertyInfo, true))
        return new DateProperty (parameters);
      else if (underlyingType == typeof (DateTime))
        return new DateTimeProperty (parameters);
      else if (underlyingType == typeof (Decimal))
        return new DecimalProperty (parameters);
      else if (underlyingType == typeof (Double))
        return new DoubleProperty (parameters);
      else if (underlyingType.IsEnum)
        return new EnumerationProperty (parameters);
      else if (underlyingType == typeof (Guid))
        return new GuidProperty (parameters);
      else if (underlyingType == typeof (Int16))
        return new Int16Property (parameters);
      else if (underlyingType == typeof (Int32))
        return new Int32Property (parameters);
      else if (underlyingType == typeof (Int64))
        return new Int64Property (parameters);
      else if (typeof (IBusinessObject).IsAssignableFrom (GetConcreteType(underlyingType)))
        return new ReferenceProperty (parameters, GetConcreteType (underlyingType));
      else if (underlyingType == typeof (Single))
        return new SingleProperty (parameters);
      else if (underlyingType == typeof (String))
        return new StringProperty (parameters, GetMaxLength ());
      else
        return new NotSupportedProperty (parameters);
    }

    private PropertyBase.Parameters CreateParameters (Type underlyingType)
    {
      return new PropertyBase.Parameters (_businessObjectProvider, _propertyInfo, underlyingType, GetListInfo (),
          GetIsRequired (), GetIsReadOnly(), GetIsNullable());
    }

    protected virtual Type GetConcreteType (Type type)
    {
      if (MixinConfiguration.ActiveContext.ContainsClassContext (type))
        return TypeFactory.GetConcreteType(type);
      return type;
    }

    protected virtual Type GetUnderlyingType ()
    {
      return Nullable.GetUnderlyingType (GetItemType()) ?? GetItemType();
    }

    protected virtual Type GetItemType ()
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

    protected virtual ListInfo GetListInfo ()
    {
      if (IsListProperty ())
        return new ListInfo (_propertyInfo.PropertyType, GetItemType ());

      return null;
    }

    //OPF Mapping
    protected virtual bool GetIsRequired ()
    {
      if (_propertyInfo.PropertyType.IsEnum && AttributeUtility.IsDefined<UndefinedEnumValueAttribute> (_propertyInfo.PropertyType, false))
        return false;
      if (_propertyInfo.PropertyType.IsValueType && Nullable.GetUnderlyingType (_propertyInfo.PropertyType) == null)
        return true;
      return false;
    }

    protected virtual bool GetIsReadOnly ()
    {
      ObjectBindingAttribute attribute = AttributeUtility.GetCustomAttribute<ObjectBindingAttribute>(_propertyInfo, true);
      if (attribute != null && attribute.ReadOnly)
        return true;

      if (ReflectionUtility.CanAscribe (_propertyInfo.PropertyType, typeof (ReadOnlyCollection<>)))
        return true;

      if (_propertyInfo.GetSetMethod (false) != null)
        return false;

      if (IsListProperty() && !_propertyInfo.PropertyType.IsArray)
        return false;

      return true;
    }

    //OPF Mapping
    protected virtual int? GetMaxLength ()
    {
      return null;
    }

    protected virtual bool IsListProperty ()
    {
      return typeof(IList).IsAssignableFrom (_propertyInfo.PropertyType);
    }

    protected virtual bool GetIsNullable ()
    {
      return Nullable.GetUnderlyingType (IsListProperty() ? GetListInfo().ItemType : PropertyInfo.PropertyType) != null;
    }
  }
}