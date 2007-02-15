using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Security.Configuration;
using Rubicon.Security.UnitTests.Configuration;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CreateSecurityClientFromConfiguration
  {
    private MockRepository _mocks;
    private SecurityConfiguration _configuration;
    private ISecurityProvider _provider;

    [SetUp]
    public void SetUp()
    {
      _mocks = new MockRepository();
      _configuration = new SecurityConfiguration ();
      SecurityConfigurationMock.SetCurrent (_configuration);
      _provider = _mocks.CreateMock<ISecurityProvider>();
      _configuration.SecurityProvider = _provider;
    }

    [TearDown]
    public void TearDown()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration());
    }

    [Test]
    public void CreateNormal()
    {
      Expect.Call (_provider.IsNull).Return (false);
      _mocks.ReplayAll();
      
      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration();
      
      _mocks.VerifyAll();
      Assert.IsInstanceOfType (typeof (SecurityClient), securityClient);
      Assert.IsFalse (((INullableObject) securityClient).IsNull);
    }

    [Test]
    public void CreateNull ()
    {
      Expect.Call (_provider.IsNull).Return (true);
      _mocks.ReplayAll ();

      SecurityClient securityClient = SecurityClient.CreateSecurityClientFromConfiguration ();

      _mocks.VerifyAll ();
      Assert.IsInstanceOfType (typeof (NullSecurityClient), securityClient);
      Assert.IsTrue (((INullableObject) securityClient).IsNull);
    }
  }
}