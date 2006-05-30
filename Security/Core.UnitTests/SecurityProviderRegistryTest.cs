using System;
using System.Collections.Generic;
using System.Text;

using NMock2;
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
    private Mockery _mocks;

    // construction and disposing

    public SecurityProviderRegistryTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
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
      ISecurityProvider exptectedProvider = _mocks.NewMock<ISecurityProvider> ();
      _securityProviderRegistry.SetProvider<ISecurityProvider> (exptectedProvider);

      Assert.AreSame (exptectedProvider, _securityProviderRegistry.GetProvider<ISecurityProvider> ());
    }

    [Test]
    public void GetProviderNotSet ()
    {
      Assert.IsNull (_securityProviderRegistry.GetProvider<ISecurityProvider> ());
    }

    [Test]
    public void SetProviderNull ()
    {
      ISecurityProvider provider = _mocks.NewMock<ISecurityProvider> ();
      _securityProviderRegistry.SetProvider<ISecurityProvider>(provider);
      Assert.IsNotNull (_securityProviderRegistry.GetProvider<ISecurityProvider> ());

      _securityProviderRegistry.SetProvider<ISecurityProvider> (null);
      Assert.IsNull (_securityProviderRegistry.GetProvider<ISecurityProvider> ());
    }
  }
}