using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections;
using System.Xml.Serialization;

using Rubicon.Utilities;
using Rubicon.ObjectBinding;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes
{
public class EnumerationProperty : DomainObjectProperty, IBusinessObjectEnumerationProperty
{
  private const string c_disabledPrefix = "Disabled_";

  public EnumerationProperty (
      PropertyInfo propertyInfo, 
      PropertyDefinition propertyDefinition, 
      Type itemType, 
      bool isList)
      : base (propertyInfo, propertyDefinition, itemType, isList)
  {
  }

  public IEnumerationValueInfo[] GetEnabledValues()
  {
    return GetValues (false);
  }

  public IEnumerationValueInfo[] GetAllValues()
  {
    return GetValues (true);
  }

  private IEnumerationValueInfo[] GetValues (bool includeDisabledValues)
  {
    Debug.Assert (PropertyInfo.PropertyType.IsEnum, "type.IsEnum");
    FieldInfo[] fields = PropertyInfo.PropertyType.GetFields (BindingFlags.Static | BindingFlags.Public);
    ArrayList valueInfos = new ArrayList (fields.Length);

    foreach (FieldInfo field in fields)
    {
      bool isEnabled = ! field.Name.StartsWith (c_disabledPrefix);

      if ((!includeDisabledValues && isEnabled) || includeDisabledValues)
      {
        valueInfos.Add (
            new EnumerationValueInfo (field.GetValue (null), field.Name, field.Name, isEnabled));
      }
    }

    return (IEnumerationValueInfo[]) valueInfos.ToArray (typeof (IEnumerationValueInfo));
  }

  /// <param name="value">
  ///   An enum value that belongs to the enum identified by <see cref="DomainObjectProperty.PropertyType"/>.
  /// </param>
  public IEnumerationValueInfo GetValueInfoByValue (object value)
  {
    if (value == null)
    {
      return null;
    }
    else
    {
      string valueString = value.ToString();

      //  Test if enum value is correct type, throws an exception if not
      Enum.Parse (PropertyType, valueString, false);
      bool isEnabled = ! valueString.StartsWith (c_disabledPrefix);

      return new EnumerationValueInfo (value, value.ToString(), value.ToString(), isEnabled);
    }
  }

  public IEnumerationValueInfo GetValueInfoByIdentifier (string identifier)
  {
    object value = Enum.Parse (PropertyType, identifier, false);
    string valueString = value.ToString();
    bool isEnabled = ! valueString.StartsWith (c_disabledPrefix);

    return new EnumerationValueInfo (value, value.ToString(), value.ToString(), isEnabled);
  }

  public override bool IsRequired
  {
    get { return true; }
  }
}
}
