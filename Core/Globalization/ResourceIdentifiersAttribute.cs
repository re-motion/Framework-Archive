using System;
using Rubicon.Utilities;

namespace Rubicon.Globalization
{

[AttributeUsage (AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public class ResourceIdentifiersAttribute: Attribute
{
  public static string GetResourceIdentifier (Enum enumValue)
  {
    ArgumentUtility.CheckNotNull ("enumValue", enumValue);
    Type type = enumValue.GetType();
    if (type.DeclaringType != null) // if the enum is a nested type, suppress enum name
      type = type.DeclaringType;
    return type.FullName + "." + enumValue.ToString();

//    string typePath = type.FullName.Substring (0, type.FullName.Length - type.Name.Length);
//    if (typePath.EndsWith ("+"))
//      return typePath.Substring (0, typePath.Length - 1) + "." + enumValue.ToString(); // nested enum type: exclude enum type name
//    else
//      return type.FullName + "." + enumValue.ToString();
  }

  /// <summary> Initializes a new instance. </summary>
  public ResourceIdentifiersAttribute ()
  {
  }
}

}
