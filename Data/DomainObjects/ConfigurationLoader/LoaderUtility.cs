using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Configuration.Loader
{
public sealed class LoaderUtility
{
  // types

  // static members and constants

  public static string GetExecutingAssemblyPath ()
  {
    AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName ();

    Uri codeBaseUri = new Uri (assemblyName.CodeBase);
    return Path.GetDirectoryName (codeBaseUri.LocalPath);
  }

  public static Type MapType (XmlNode node)
  {
    ArgumentUtility.CheckNotNull ("node", node);

    return MappingUtility.MapType (node.InnerText.Trim ());    
  }

  public static Type MapType (XmlNode node, string xPath, XmlNamespaceManager namespaceManager)
  {
    ArgumentUtility.CheckNotNull ("node", node);
    ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);

    return MapType (node.SelectSingleNode (xPath, namespaceManager));
  }

  public static Type MapOptionalType (XmlNode selectionNode, string xPath, XmlNamespaceManager namespaceManager)
  {
    ArgumentUtility.CheckNotNull ("selectionNode", selectionNode);
    ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
    ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);
    
    XmlNode typeNode = selectionNode.SelectSingleNode (xPath, namespaceManager);

    if (typeNode != null)
      return MapType (typeNode);
    else
      return null;
  }

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

    return Path.Combine (GetExecutingAssemblyPath (), defaultFileName);
  }

  // member fields

  // construction and disposing

  private LoaderUtility ()
  {
  }

  // methods and properties

}
}
