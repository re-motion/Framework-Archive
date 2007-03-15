using System.Configuration;
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

      using (XmlTextReader reader = new XmlTextReader (xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        reader.IsStartElement();
        PrivateInvoke.InvokeNonPublicMethod (configurationElement, "DeserializeElement", reader, false);
      }
    }

    public static void DeserializeSection (ConfigurationSection configurationSection, string xmlFragment)
    {
      ArgumentUtility.CheckNotNull ("configurationSection", configurationSection);
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      using (XmlTextReader reader = new XmlTextReader (xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        PrivateInvoke.InvokeNonPublicMethod (configurationSection, "DeserializeSection", reader);
      }
    }
  }
}