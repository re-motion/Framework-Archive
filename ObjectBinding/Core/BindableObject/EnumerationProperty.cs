using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.ObjectBinding.BindableObject
{
  //TODO: doc
  public class EnumerationProperty : PropertyBase, IBusinessObjectEnumerationProperty
  {
    private readonly Enum _undefinedValue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="businessObjectProvider"></param>
    /// <param name="propertyInfo"></param>
    /// <param name="listInfo"></param>
    /// <param name="isRequired"></param>
    /// <exception cref="InvalidOperationException">
    /// The enum type has an UndefinedEnumValueAttribute with a value that does not match the enum's type.
    /// <para>- or -</para>
    /// <para>The property is nullable and the property's type defines an UndefinedEnumValueAttribute</para>
    /// </exception>
    public EnumerationProperty (BindableObjectProvider businessObjectProvider, PropertyInfo propertyInfo, IListInfo listInfo, bool isRequired)
        : base (businessObjectProvider, propertyInfo, listInfo, isRequired)
    {
      _undefinedValue = GetUndefinedValue();
    }

    // methods and properties
    /// <summary> Returns a list of all the enumeration's values. </summary>
    /// <returns> 
    ///   A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the values defined in the enumeration. 
    /// </returns>
    public IEnumerationValueInfo[] GetAllValues ()
    {
      List<IEnumerationValueInfo> valueInfos = new List<IEnumerationValueInfo>();
      foreach (Enum value in Enum.GetValues (UnderlyingType))
      {
        IEnumerationValueInfo enumerationValueInfo = GetValueInfoByValue (value, null);
        if (enumerationValueInfo != null)
          valueInfos.Add (enumerationValueInfo);
      }
      return valueInfos.ToArray();
    }

    /// <summary> Returns a list of the enumeration's values that can be used in the current context. </summary>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine the enabled enum values. </param>
    /// <returns> A list of <see cref="IEnumerationValueInfo"/> objects encapsulating the enabled values in the enumeration. </returns>
    /// <remarks> CLS type enums do not inherently support the disabling of its values. </remarks>
    public IEnumerationValueInfo[] GetEnabledValues (IBusinessObject businessObject)
    {
      return GetAllValues();
    }

    /// <overloads> Returns a specific enumeration value. </overloads>
    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="value"> The enumeration value to return the <see cref="IEnumerationValueInfo"/> for. </param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="value"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByValue (object value, IBusinessObject businessObject)
    {
      if (value == null)
        return null;

      Enum enumValue = (Enum) value;
      if (IsUndefinedValue (enumValue))
        return null;

      return new EnumerationValueInfo (value, GetIdenfier (enumValue), GetDisplayName (enumValue), IsEnabled (enumValue, businessObject));
    }

    /// <summary> Returns a specific enumeration value. </summary>
    /// <param name="identifier"> The string identifying the  enumeration value to return the <see cref="IEnumerationValueInfo"/> for. </param>
    /// <param name="businessObject"> The <see cref="IBusinessObject"/> used to determine whether the enum value is enabled. </param>
    /// <returns> The <see cref="IEnumerationValueInfo"/> object for the provided <paramref name="identifier"/>. </returns>
    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier, IBusinessObject businessObject)
    {
      return GetValueInfoByValue (StringUtility.Parse (UnderlyingType, identifier, null), businessObject);
    }

    public override object ConvertFromNativePropertyType (object nativeValue)
    {
      if (nativeValue != null)
      {
        if (IsUndefinedValue ((Enum) nativeValue))
          return null;
      }

      return base.ConvertFromNativePropertyType (nativeValue);
    }

    public override object ConvertToNativePropertyType (object publicValue)
    {
      if (publicValue == null && _undefinedValue != null)
        return base.ConvertToNativePropertyType (_undefinedValue);

      return base.ConvertToNativePropertyType (publicValue);
    }

    private string GetIdenfier (Enum value)
    {
      return value.ToString();
    }

    private string GetDisplayName (Enum value)
    {
      return EnumDescription.GetDescription (value) ?? value.ToString();
    }

    private bool IsEnabled (Enum value, IBusinessObject businessObject)
    {
      return Enum.IsDefined (UnderlyingType, value);
    }

    private bool IsUndefinedValue (Enum value)
    {
      return value.Equals (_undefinedValue);
    }

    private Enum GetUndefinedValue ()
    {
      UndefinedEnumValueAttribute undefinedEnumValueAttribute =
          AttributeUtility.GetCustomAttribute<UndefinedEnumValueAttribute> (UnderlyingType, false);

      if (undefinedEnumValueAttribute == null)
        return null;

      if (!UnderlyingType.IsInstanceOfType (undefinedEnumValueAttribute.Value))
      {
        throw new InvalidOperationException (
            string.Format (
                "The enum type '{0}' defines a '{1}' with an enum value that belongs to a different enum type.",
                UnderlyingType,
                typeof (UndefinedEnumValueAttribute)));
      }

      if (IsNullable)
      {
        throw new InvalidOperationException (
            string.Format (
                "The property '{0}' defined on type '{1}' must not be nullable since the property's type already defines a '{2}'.",
                Identifier,
                PropertyInfo.DeclaringType,
                typeof (UndefinedEnumValueAttribute)));
      }

      return undefinedEnumValueAttribute.Value;
    }
  }
}