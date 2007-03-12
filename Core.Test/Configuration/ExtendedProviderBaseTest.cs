using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Rubicon.Configuration;

namespace Rubicon.Core.UnitTests.Configuration
{
  [TestFixture]
  public class ExtendedProviderBaseTest
  {
    [Test]
    public void Initialize()
    {
      NameValueCollection config = new NameValueCollection();
      config.Add ("description", "The Description");

      ExtendedProviderBase provider = new StubExtendedProvider ("Provider", config);

      Assert.AreEqual ("Provider", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
  }
}