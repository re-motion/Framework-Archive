using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityAdapterRegistryTest
  {
    // types

    // static members

    // member fields

    private SecurityAdapterRegistry _securityAdapterRegistry;
    private MockRepository _mocks;

    // construction and disposing

    public SecurityAdapterRegistryTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _securityAdapterRegistry = new SecurityProviderRegistryMock ();
    }

    [Test]
    public void GetInstance ()
    {
      Assert.IsNotNull (SecurityAdapterRegistry.Instance);
    }

    [Test]
    public void SetAndGetProvider ()
    {
      ISecurityAdapter exptectedAdapter = _mocks.CreateMock<ISecurityAdapter> ();
      _mocks.ReplayAll ();
      
      _securityAdapterRegistry.SetAdapter<ISecurityAdapter> (exptectedAdapter);

      Assert.AreSame (exptectedAdapter, _securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());
    }

    [Test]
    public void GetProviderNotSet ()
    {
      Assert.IsNull (_securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());
    }

    [Test]
    public void SetProviderNull ()
    {
      ISecurityAdapter adapter = _mocks.CreateMock<ISecurityAdapter> ();
      _mocks.ReplayAll ();

      _securityAdapterRegistry.SetAdapter<ISecurityAdapter>(adapter);
      Assert.IsNotNull (_securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());

      _securityAdapterRegistry.SetAdapter<ISecurityAdapter> (null);
      Assert.IsNull (_securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());
    }
  }
}