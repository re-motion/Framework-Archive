using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;

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
    private SecurityContext _context;
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
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessType.Read), AccessType.Get (GeneralAccessType.Edit) };

      SetupResult.For (_stubContextFactory.GetSecurityContext ()).Return (_context);

      _strategy = new ObjectSecurityStrategy (_stubContextFactory, _mockSecurityStrategy);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_context, _stubSecurityService, _user, _accessTypeResult)).Return (true);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      Assert.AreEqual (true, hasAccess);
      _mocks.VerifyAll ();
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (_context, _stubSecurityService, _user, _accessTypeResult)).Return (false);
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      Assert.AreEqual (false, hasAccess);
      _mocks.VerifyAll ();
    }
  }
}
