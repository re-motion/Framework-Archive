using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

namespace Rubicon.Data.DomainObjects.Configuration.Loader
{
public sealed class LoaderUtility
{
  // types

  // static members and constants

  private static readonly NameValueCollection s_typeMapping;

  public static string GetExecutingAssemblyPath ()
  {
    AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName ();

    Uri codeBaseUri = new Uri (assemblyName.CodeBase);
    return Path.GetDirectoryName (codeBaseUri.LocalPath);
  }

  public static Type MapType (string shortHandTypeName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("shortHandTypeName", shortHandTypeName);

    if (Array.IndexOf (s_typeMapping.AllKeys, shortHandTypeName) >= 0)
      return Type.GetType (s_typeMapping[shortHandTypeName], true);
    else
      return Type.GetType (shortHandTypeName, true);
  }

  public static Type GetTypeFromNode (XmlNode node)
  {
    ArgumentUtility.CheckNotNull ("node", node);

    return MapType (node.InnerText.Trim ());    
  }

  public static Type GetTypeFromNode (XmlNode node, string xPath, XmlNamespaceManager namespaceManager)
  {
    ArgumentUtility.CheckNotNull ("node", node);
    ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);

    return GetTypeFromNode (node.SelectSingleNode (xPath, namespaceManager));
  }

  public static Type GetTypeFromOptionalNode (XmlNode selectionNode, string xPath, XmlNamespaceManager namespaceManager)
  {
    ArgumentUtility.CheckNotNull ("selectionNode", selectionNode);
    ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);
    
    XmlNode typeNode = selectionNode.SelectSingleNode (xPath, namespaceManager);

    if (typeNode != null)
      return GetTypeFromNode (typeNode);
    else
      return null;
  }

  public static string GetXmlFileName (string appSettingKey, string defaultFileName)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("appSettingKey", appSettingKey);
    ArgumentUtility.CheckNotNullOrEmpty ("defaultFileName", defaultFileName);

    string fileName = ConfigurationSettings.AppSettings[appSettingKey];

    if (File.Exists (fileName))
      return fileName;

    return Path.Combine (GetExecutingAssemblyPath (), defaultFileName);
  }

  // member fields

  // construction and disposing

  static LoaderUtility ()
  {
    s_typeMapping = new NameValueCollection ();
    s_typeMapping.Add ("boolean", "System.Boolean, Mscorlib");
    s_typeMapping.Add ("byte", "System.Byte, Mscorlib");
    s_typeMapping.Add ("dateTime", "System.DateTime, Mscorlib");
    s_typeMapping.Add ("decimal", "System.Decimal, Mscorlib");
    s_typeMapping.Add ("double", "System.Double, Mscorlib");
    s_typeMapping.Add ("guid", "System.Guid, Mscorlib");
    s_typeMapping.Add ("int16", "System.Int16, Mscorlib");
    s_typeMapping.Add ("int32", "System.Int32, Mscorlib");
    s_typeMapping.Add ("int64", "System.Int64, Mscorlib");
    s_typeMapping.Add ("single", "System.Single, Mscorlib");
    s_typeMapping.Add ("string", "System.String, Mscorlib");
    s_typeMapping.Add ("char", "System.Char, Mscorlib");
    s_typeMapping.Add ("objectID", "Rubicon.Data.DomainObjects.ObjectID, Rubicon.Data.DomainObjects");
  }

  private LoaderUtility ()
  {
  }

  // methods and properties

}
}
