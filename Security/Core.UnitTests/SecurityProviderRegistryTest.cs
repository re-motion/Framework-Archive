using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityProviderRegistryTest
  {
    // types

    // static members

    // member fields

    private SecurityProviderRegistry _securityProviderRegistry;

    // construction and disposing

    public SecurityProviderRegistryTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityProviderRegistry = new SecurityProviderRegistryMock ();
    }

    [Test]
    public void GetInstance ()
    {
      Assert.IsNotNull (SecurityProviderRegistry.Instance);
    }

    [Test]
    public void SetAndGetProvider ()
    {
      IObjectSecurityProvider exptectedProvider = new ObjectSecurityProvider ();
      _securityProviderRegistry.SetProvider<IObjectSecurityProvider> (exptectedProvider);
      IObjectSecurityProvider actualProvider = _securityProviderRegistry.GetProvider<IObjectSecurityProvider> ();

      Assert.AreSame (exptectedProvider, actualProvider);
    }

    [Test]
    public void GetProviderNotSet ()
    {
      Assert.IsNull (_securityProviderRegistry.GetProvider<IObjectSecurityProvider> ());
    }
  }
}