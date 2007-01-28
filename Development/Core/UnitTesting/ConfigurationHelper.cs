using System.Configuration;
using System.IO;
using System.Xml;
using Rubicon.Utilities;

namespace Rubicon.Development.UnitTesting
{
  public static class ConfigurationHelper
  {
    public static void DeserializeElement (ConfigurationElement configurationElement, string xmlFragment)
    {
      ArgumentUtility.CheckNotNull ("configurationElement", configurationElement);
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      XmlDocument document = new XmlDocument ();
      document.LoadXml (xmlFragment);

      MemoryStream stream = new MemoryStream ();
      document.Save (stream);
      stream.Position = 0;
      XmlReader reader = XmlReader.Create (stream);

      reader.IsStartElement ();
      PrivateInvoke.InvokeNonPublicMethod (configurationElement, "DeserializeElement", reader, false);
    }

    public static void DeserializeSection (ConfigurationSection configurationSection, string xmlFragment)
    {
      ArgumentUtility.CheckNotNull ("configurationSection", configurationSection);
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      XmlDocument document = new XmlDocument ();
      document.LoadXml (xmlFragment);

      MemoryStream stream = new MemoryStream ();
      document.Save (stream);
      stream.Position = 0;
      XmlReader reader = XmlReader.Create (stream);

      PrivateInvoke.InvokeNonPublicMethod (configurationSection, "DeserializeSection", reader);
    }
  }
}