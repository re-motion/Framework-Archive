using System;
using System.Reflection;
using System.Collections.Specialized;
using System.Xml;
using System.IO;
using Rubicon.Utilities;

namespace Rubicon.Globalization
{

[AttributeUsage (AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
public class ResourceIdentifierAttribute: Attribute
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

  private string _defaultResourceFileName;

  public ResourceIdentifierAttribute ()
    : this (null)
  {
  }

  public ResourceIdentifierAttribute (string defaultResourceFileName)
  {
    _defaultResourceFileName = defaultResourceFileName;
  }

  public string DefaultResourceFileName
  {
    get { return _defaultResourceFileName; }
  }
}

public class ResourceSummaryUtility // move to .exe utility
{
  public static NameValueCollection GetResourceIdentifiers (string assemblyFilename)
  {
    Assembly assembly = Assembly.LoadFile (assemblyFilename);

    XmlDocument commentsDocument = null;
    string commentsFilename = Path.Combine (
        Path.GetDirectoryName (assemblyFilename), 
        Path.GetFileNameWithoutExtension (assemblyFilename)) + ".xml";

    if (File.Exists (commentsFilename))
    {
      commentsDocument = new XmlDocument();
      commentsDocument.Load (commentsFilename);
    }

    NameValueCollection resourceIdentifiers = new NameValueCollection ();
    foreach (Type type in assembly.GetTypes ()) // gibt der auch die nested types zurück?
    {
      if (HasResourceIdentifiersAttribute (type))
      {
        FieldInfo[] fields = type.GetFields (BindingFlags.Public | BindingFlags.Static);
        foreach (FieldInfo field in fields)
        {
          Enum enumValue = (Enum)field.GetValue(null);
          string name = ResourceIdentifierAttribute.GetResourceIdentifier (enumValue);
          string comment = null;
          if (commentsDocument != null)
          {
            string commentID = "T:" + type.FullName.Replace ("+", ".") + "." + enumValue.ToString();
            string commentPath = "/doc/members/member[@name='" + commentID + "']/summary";
            XmlNode summaryElement = commentsDocument.SelectSingleNode (commentPath);
            if (summaryElement != null)
              comment = summaryElement.InnerText;
          }
          resourceIdentifiers.Add (name, comment);
        }
      }
    }

    return resourceIdentifiers;
  }

  private static bool HasResourceIdentifiersAttribute (Type type)
  {
    object[] attributes = type.GetCustomAttributes (typeof (ResourceIdentifierAttribute), false); // typeof -> string literal
    foreach (object attribute in attributes)
    {
      if (attribute.GetType().FullName == typeof (ResourceIdentifierAttribute).FullName)
        return true;
    }
    return false;
  }
}

}
