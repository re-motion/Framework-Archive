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
  public class FunctionalSecurityStrategyTest
  {
    private MockRepository _mocks;
    private ISecurityStrategy _mockSecurityStrategy;
    private ISecurityService _stubSecurityService;
    private IPrincipal _user;
    private AccessType[] _accessTypeResult;
    private FunctionalSecurityStrategy _strategy;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.CreateMock<ISecurityStrategy> ();
      _stubSecurityService = _mocks.CreateMock<ISecurityService> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessType.Read), AccessType.Get (GeneralAccessType.Edit) };

      _strategy = new FunctionalSecurityStrategy (_mockSecurityStrategy);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_mockSecurityStrategy, _strategy.SecurityStrategy);
    }
    
    [Test]
    public void Initialize_WithDefaults ()
    {
      FunctionalSecurityStrategy strategy = new FunctionalSecurityStrategy ();
      Assert.IsInstanceOfType (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (NullAccessTypeCache<string>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (null, null, null, null)).Return (true).Constraints (
          Is.NotNull () 
          & Property.Value ("Class", "Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.SecurableObject, Rubicon.Security.UnitTests"),
          Is.Same (_stubSecurityService),
          Is.Same (_user),
          List.Equal (_accessTypeResult));
      _mocks.ReplayAll();

      bool hasAccess = _strategy.HasAccess (typeof (SecurableObject), _stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (null, null, null, null)).Return (false).Constraints (
          Is.NotNull ()
          & Property.Value ("Class", "Rubicon.Security.UnitTests.SampleDomain.PermissionReflection.SecurableObject, Rubicon.Security.UnitTests"),
          Is.Same (_stubSecurityService),
          Is.Same (_user),
          List.Equal (_accessTypeResult));
      _mocks.ReplayAll ();

      bool hasAccess = _strategy.HasAccess (typeof (SecurableObject), _stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
  }
}
