using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Rubicon.Configuration;
using Rubicon.Security.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Configuration
{

  public class SecurityConfigurationMock : SecurityConfiguration
  {
    // types

    // static members

    public static new void SetCurrent (SecurityConfiguration configuration)
    {
      SecurityConfiguration.SetCurrent (configuration);
    }

    // member fields

    // construction and disposing

    public SecurityConfigurationMock ()
    {
    }

    // methods and properties

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