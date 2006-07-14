using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class ObjectSecurityStrategyTest
  {
    private MockRepository _mocks;
    private ISecurityStrategy _mockSecurityStrategy;
    private ISecurityService _stubSecurityService;
    private ISecurityContextFactory _stubContextFactory;
    private IPrincipal _user;
    private AccessType[] _accessTypeResult;
    private ObjectSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.CreateMock<ISecurityStrategy> ();
      _stubSecurityService = _mocks.CreateMock<ISecurityService> ();
      _stubContextFactory = _mocks.CreateMock<ISecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessType.Read), AccessType.Get (GeneralAccessType.Edit) };

      _strategy = new ObjectSecurityStrategy (_stubContextFactory, _mockSecurityStrategy);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_stubContextFactory, _strategy.SecurityContextFactory);
      Assert.AreSame (_mockSecurityStrategy, _strategy.SecurityStrategy);
    }

    [Test]
    public void Initialize_WithDefaults ()
    {
      ObjectSecurityStrategy strategy = new ObjectSecurityStrategy (_stubContextFactory);
      Assert.AreSame (_stubContextFactory, strategy.SecurityContextFactory);
      Assert.IsInstanceOfType (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (NullAccessTypeCache<string>), ((SecurityStrategy)strategy.SecurityStrategy).LocalCache);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityService, _user, _accessTypeResult)).Return (true);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityService, _user, _accessTypeResult)).Return (false);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
  }
}
