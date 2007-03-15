using System;
using System.IO;
using System.Xml;
using Rubicon.SecurityManager.Configuration;
using Rubicon.Utilities;

namespace Rubicon.SecurityManager.UnitTests.Configuration
{
  internal class TestSecurityManagerConfiguration : SecurityManagerConfiguration
  {
    public void DeserializeSection (string xmlFragment)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      XmlDocument document = new XmlDocument ();
      document.LoadXml (xmlFragment);

      using (MemoryStream stream = new MemoryStream ())
      {
        document.Save (stream);
        stream.Position = 0;
        using (XmlReader reader = XmlReader.Create (stream))
        {
          DeserializeSection (reader);
        }
      }
    }
  }
}
