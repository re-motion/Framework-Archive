using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;
using Rubicon.ObjectBinding;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
  //TODO Doc: The disabled prefix feature is not documented anywhere. Should it be? => See comment on c_disabledPrefix.
  public class EnumerationProperty : BaseProperty, IBusinessObjectEnumerationProperty
  {
    // TODO: Remove this feature or provide an attribute for disabled enum values!
    private const string c_disabledPrefix = "Disabled_";

    public EnumerationProperty (
        IBusinessObjectClass businessObjectClass, 
        PropertyInfo propertyInfo,
        bool isRequired,
        Type itemType,
        bool isList)
      : base (businessObjectClass, propertyInfo, isRequired, itemType, isList)
    {
    }

    public virtual IEnumerationValueInfo[] GetEnabledValues ()
    {
      return GetValues (false);
    }

    public virtual IEnumerationValueInfo[] GetAllValues ()
    {
      return GetValues (true);
    }

    private IEnumerationValueInfo[] GetValues (bool includeDisabledValues)
    {
      Debug.Assert (PropertyInfo.PropertyType.IsEnum, "type.IsEnum");
      FieldInfo[] fields = PropertyInfo.PropertyType.GetFields (BindingFlags.Static | BindingFlags.Public);
      ArrayList valueInfos = new ArrayList (fields.Length);

      object undefinedValue = GetUndefinedValue ();
      foreach (FieldInfo field in fields)
      {
        Enum fieldValue = (Enum) field.GetValue (null);

        if ((undefinedValue != null) && fieldValue.Equals (undefinedValue))
          continue;

        bool isEnabled = !field.Name.StartsWith (c_disabledPrefix);
        if (isEnabled || includeDisabledValues)
        {
          string multiLingualDisplayName = EnumDescription.GetDescription (fieldValue);
          valueInfos.Add (new EnumerationValueInfo (field.GetValue (null), field.Name, multiLingualDisplayName, isEnabled));
        }
      }

      return (IEnumerationValueInfo[]) valueInfos.ToArray (typeof (IEnumerationValueInfo));
    }

    protected virtual object GetUndefinedValue ()
    {
      UndefinedEnumValueAttribute undefinedEnumValueAttribute = AttributeUtility.GetCustomAttribute<UndefinedEnumValueAttribute> (UnderlyingType, true);
      if (undefinedEnumValueAttribute != null)
        return undefinedEnumValueAttribute.Value;

      return null;
    }

    public override object FromInternalType (IBusinessObject bindableObject, object internalValue)
    {
      ArgumentUtility.CheckNotNullAndType<Enum> ("internalValue", internalValue);
      ArgumentUtility.CheckValidEnumValue ("internalValue", (Enum) internalValue);

      object undefinedValue = GetUndefinedValue ();
      if ((undefinedValue != null) && internalValue.Equals (undefinedValue))
        return null;

      return base.FromInternalType (bindableObject, internalValue);
    }

    public override object ToInternalType (object publicValue)
    {
      object undefinedValue = GetUndefinedValue ();
      if ((undefinedValue != null) && publicValue == null)
        return undefinedValue;

      return base.ToInternalType (publicValue);
    }

    /// <param name="value">
    ///   An enum value that belongs to the enum identified by <see cref="BaseProperty.PropertyType"/>.
    /// </param>
    public IEnumerationValueInfo GetValueInfoByValue (object value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      string valueString = value.ToString ();

      //  Test if enum value is correct type, throws an exception if not
      Enum.Parse (PropertyType, valueString, false);
      bool isEnabled = !valueString.StartsWith (c_disabledPrefix);

      string multiLingualEnumName = EnumDescription.GetDescription ((System.Enum) value);
      string enumDisplayName = (multiLingualEnumName != null) ? multiLingualEnumName : valueString;

      return new EnumerationValueInfo (value, value.ToString (), enumDisplayName, isEnabled);
    }

    public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("identifier", identifier);

      object value = Enum.Parse (PropertyType, identifier, false);
      return GetValueInfoByValue (value);
    }

    // TODO: Remove this property. Check if we can do so.
    public override bool IsRequired
    {
      get { return true; }
    }
  }
}
