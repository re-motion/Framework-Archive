using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;
using System.Resources;
using System.Globalization;

namespace Rubicon
{

/// <summary>
/// Use this attribute to specify a clear text representation of a certain enumeration value.
/// </summary>
[AttributeUsage (AttributeTargets.Field, AllowMultiple = false)]
public class EnumDescriptionAttribute: Attribute
{
  private string _description;

  public EnumDescriptionAttribute (string description)
	{
    _description = description;
	}

  public string Description
  {
    get { return _description; }
  }
}

[AttributeUsage (AttributeTargets.Enum, AllowMultiple = false)]
public class EnumDescriptionResourceAttribute: Attribute
{
  private string _baseName;

  public EnumDescriptionResourceAttribute (string baseName)
  {
    _baseName = baseName;
  }

  public string BaseName
  {
    get { return _baseName; }
  }
}

public struct EnumValue
{
  public readonly Enum Value;
  public readonly string Description;

  public EnumValue (Enum value, string description)
  {
    Value = value;
    Description = description;
  }
}

/// <summary>
/// Use this class to get the clear text representations of enumeration values.
/// </summary>
public sealed class EnumDescription
{
  /// <summary> IDictionary&lt;Type, IDictionary&lt;System.Enum, string&gt;&gt; </summary>
  /// <remarks> This is for enums with the EnumDescriptionAttribute on values. </remarks>
  private static IDictionary s_typeDescriptions;

  /// <summary> IDictionary&lt;string resourceKey, ResourceManager&gt; </summary>
  /// <remarks> This is for enums with the EnumDescriptionResourceAttribute. </remarks>
  private static IDictionary s_enumResourceManagers;

  static EnumDescription ()
  {
    lock (typeof (EnumDescription))
    {
      s_typeDescriptions = new HybridDictionary ();
      s_enumResourceManagers = new HybridDictionary ();
    }
  }


  public static EnumValue[] GetAllValues (Type enumType)
  {
    return GetAllValues (enumType, null);
  }
  
  public static EnumValue[] GetAllValues (Type enumType, CultureInfo culture)
  {
    EnumDescriptionResourceAttribute[] resourceAttributes = (EnumDescriptionResourceAttribute[]) 
          enumType.GetCustomAttributes (typeof (EnumDescriptionResourceAttribute), false);
    if (resourceAttributes.Length > 0)
    {
      FieldInfo[] fields = enumType.GetFields (BindingFlags.Static | BindingFlags.Public);
      EnumValue[] values = new EnumValue[fields.Length];
      ResourceManager rm = GetResourceManager (resourceAttributes[0].BaseName, enumType.Assembly);
      
      for (int i = 0; i < fields.Length; ++i)
      {
        Enum value = (Enum) fields[i].GetValue(null);
        string description = rm.GetString (enumType.FullName + "." + value.ToString(), culture);
        values[i] = new EnumValue (value, description);
      }
      return values;
    }
    else
    {
      IDictionary descriptions = GetOrCreateDescriptions (enumType);
      EnumValue[] values = new EnumValue[descriptions.Count];
      int i = 0;
      foreach (Enum value in descriptions.Keys)
      {
        values[i] = new EnumValue (value, (string) descriptions[value]);
        ++i;
      }
      return values;
    }
  }

  private static ResourceManager GetResourceManager (string baseName, Assembly assembly)
  {
    string resourceKey = baseName + " in " + assembly.FullName;

    ResourceManager rm = (ResourceManager) s_enumResourceManagers[resourceKey];
    if (rm == null)
    {
      lock (typeof (EnumDescription))
      {
        rm = (ResourceManager) s_enumResourceManagers[resourceKey];
        if (rm == null)
        {
          rm = new ResourceManager (baseName, assembly, null);
          s_enumResourceManagers[resourceKey] = rm;
        }
      }
    }
    return rm;
  }

  public static string GetDescription (System.Enum value)
  {
    return GetDescription (value, null);
  }

  public static string GetDescription (System.Enum value, CultureInfo culture)
  {
    Type enumType = value.GetType();
    EnumDescriptionResourceAttribute[] resourceAttributes = (EnumDescriptionResourceAttribute[]) 
          enumType.GetCustomAttributes (typeof (EnumDescriptionResourceAttribute), false);
    if (resourceAttributes.Length > 0)
    {
      ResourceManager rm = GetResourceManager (resourceAttributes[0].BaseName, enumType.Assembly);
      return rm.GetString (enumType.FullName + "." + value.ToString(), culture);
    }
    else
    {
      IDictionary descriptions = GetOrCreateDescriptions (enumType);
      string description = (string) descriptions[value];
      if (description != null)
        return description;

      return value.ToString();
    }
  }

  private static IDictionary GetOrCreateDescriptions (Type enumType)
  {
    IDictionary descriptions = (IDictionary) s_typeDescriptions[enumType];
    if (descriptions == null)
    {
      lock (s_typeDescriptions)
      {
        descriptions = (IDictionary) s_typeDescriptions[enumType];
        if (descriptions == null)
        {
          descriptions = CreateDesciptionsDictionary (enumType);
          s_typeDescriptions.Add (enumType, descriptions);
        }
      }
    }
    return descriptions;
  }

  /// <returns>IDictionary&lt;System.Enum, string&gt;</returns>
  private static IDictionary CreateDesciptionsDictionary (Type enumType)
  {
    FieldInfo[] fields = enumType.GetFields (BindingFlags.Static | BindingFlags.Public);
    IDictionary dictionary = new HybridDictionary (fields.Length);
    foreach (FieldInfo field in fields)
    {
      EnumDescriptionAttribute[] descriptionAttributes = (EnumDescriptionAttribute[]) field.GetCustomAttributes (typeof (EnumDescriptionAttribute), false);
      if (descriptionAttributes.Length > 0)
      {
        System.Enum value = (System.Enum) field.GetValue (null);
        dictionary.Add (value, descriptionAttributes[0].Description);
      }
    }
    return dictionary;
  }

  private EnumDescription()
  {
  }
}

}
