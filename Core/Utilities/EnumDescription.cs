using System;
using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

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

public sealed class EnumDescription
{
  /// <summary>
  /// IDictionary&lt;Type, IDictionary&lt;System.Enum, string&gt;&gt;
  /// </summary>
  private static IDictionary s_typeDescriptions = new HybridDictionary ();

  public static string GetDescription (System.Enum value)
  {
    Type enumType = value.GetType();
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
    string description = (string) descriptions[value];
    if (description != null)
      return description;

    return value.ToString();
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
