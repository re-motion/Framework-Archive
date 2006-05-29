using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

using Rubicon.Security.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Configuration
{

  public class SecurityConfigurationMock : SecurityConfiguration
  {
    // types

    // static members

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

    public new TypeElement<ISecurityService> CustomService
    {
      get { return base.CustomService; }
      set { base.CustomService = value; }
    }

    public new SecurityServiceType SecurityServiceType
    {
      get { return base.SecurityServiceType; }
      set { base.SecurityServiceType = value; }
    }

    public new TypeElement<IUserProvider> CustomUserProvider
    {
      get { return base.CustomUserProvider; }
      set { base.CustomUserProvider = value; }
    }

    public new UserProviderType UserProviderType
    {
      get { return base.UserProviderType; }
      set { base.UserProviderType = value; }
    }
  }
}