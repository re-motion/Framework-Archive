using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Xml;
using Rubicon.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public static class LoaderUtility
  {
    public static Type GetType (string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      return Type.GetType (typeName.Trim (), true);    
    }

    public static Type GetType (XmlNode node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      return GetType (node.InnerText);    
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

    public static string GetConfigurationFileName (string appSettingKey, string defaultFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("appSettingKey", appSettingKey);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultFileName", defaultFileName);

      string fileName = ConfigurationWrapper.Current.GetAppSetting (appSettingKey, false);
      if (fileName != null)
        return fileName;

      return Path.Combine (ReflectionUtility.GetExecutingAssemblyPath (), defaultFileName);
    }
  }
}
