using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Rubicon.Security.Configuration;
using Rubicon.Security.Metadata;
using Rubicon.Utilities;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.Configuration
{
  [TestFixture]
  public class SecurityConfigurationTest
  {
    [Test]
    public void DeserializeSecurityConfigurationWithDomainObjectGlobalAccessTypeCacheProvider ()
    {
      SecurityConfigurationMock configuration = new SecurityConfigurationMock ();
      string xmlFragment = @"<rubicon.security globalAccessTypeCacheProvider=""ClientTransaction"" />";
      configuration.DeserializeSection (xmlFragment);

      Assert.IsInstanceOfType (typeof (ClientTransactionAccessTypeCacheProvider), configuration.GlobalAccessTypeCacheProvider);
    }
  }
}