using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
public sealed class LoaderUtility
{
  // types

  // static members and constants

  public static Type GetType (XmlNode node)
  {
    ArgumentUtility.CheckNotNull ("node", node);

    return Type.GetType (node.InnerText.Trim (), true);    
  }

  public static Type GetType (XmlNode node, string xPath, XmlNamespaceManager namespaceManager)
  {
    ArgumentUtility.CheckNotNull ("node", node);
    ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);

    return GetType (node.SelectSingleNode (xPath, namespaceManager));
  }

  public static Type GetOptionalType (XmlNode selectionNode, string xPath, XmlNamespaceManager namespaceManager)
  {
    ArgumentUtility.CheckNotNull ("selectionNode", selectionNode);
    ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);
    
    XmlNode typeNode = selectionNode.SelectSingleNode (xPath, namespaceManager);

    if (typeNode != null)
      return GetType (typeNode);
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

    return Path.Combine (ReflectionUtility.GetExecutingAssemblyPath (), defaultFileName);
  }

  // member fields

  // construction and disposing

  private LoaderUtility ()
  {
  }

  // methods and properties

}
}
