using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Rubicon.Utilities;

using Rubicon.Security.Service.Domain.Configuration;

namespace Rubicon.Security.Service.Domain.UnitTests.Configuration
{
  internal class TestSecurityDomainConfiguration : SecurityDomainConfiguration
  {
    public void DeserializeSection (string xmlFragment)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      XmlDocument document = new XmlDocument ();
      document.LoadXml (xmlFragment);

      MemoryStream stream = new MemoryStream ();
      document.Save (stream);
      stream.Position = 0;
      XmlReader reader = XmlReader.Create (stream);

      DeserializeSection (reader);
    }
  }
}
