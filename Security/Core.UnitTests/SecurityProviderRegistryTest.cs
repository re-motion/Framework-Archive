using System;
using System.Collections.Generic;
using System.Text;

using Rhino.Mocks;
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
    private MockRepository _mocks;

    // construction and disposing

    public SecurityProviderRegistryTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
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
      ISecurityProvider exptectedProvider = _mocks.CreateMock<ISecurityProvider> ();
      _mocks.ReplayAll ();
      
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
      ISecurityProvider provider = _mocks.CreateMock<ISecurityProvider> ();
      _mocks.ReplayAll ();

      _securityProviderRegistry.SetProvider<ISecurityProvider>(provider);
      Assert.IsNotNull (_securityProviderRegistry.GetProvider<ISecurityProvider> ());

      _securityProviderRegistry.SetProvider<ISecurityProvider> (null);
      Assert.IsNull (_securityProviderRegistry.GetProvider<ISecurityProvider> ());
    }
  }
}