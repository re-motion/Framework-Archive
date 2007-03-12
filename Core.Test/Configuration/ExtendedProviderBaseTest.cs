using System;
using System.Collections.Specialized;
using NUnit.Framework;

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

      StubExtendedProvider provider = new StubExtendedProvider ("Stub", config);

      Assert.AreEqual ("Stub", provider.Name);
      Assert.AreEqual ("The Description", provider.Description);
    }
  }
}