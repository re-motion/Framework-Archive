using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using Rhino.Mocks;

using Rubicon.Security;
using Rubicon.Security.UnitTests.Configuration;
using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Utilities;
using Rubicon.Security.UnitTests.MockConstraints;
using Rubicon.Security.Configuration;
using Rubicon.Collections;

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
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };

      _strategy = new FunctionalSecurityStrategy (_mockSecurityStrategy);

      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfigurationMock.SetCurrent (new SecurityConfiguration ());
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_mockSecurityStrategy, _strategy.SecurityStrategy);
    }
    
    [Test]
    public void Initialize_WithDefaults ()
    {
      IGlobalAccessTypeCacheProvider stubGlobalCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = stubGlobalCacheProvider;
      FunctionalSecurityStrategy strategy = new FunctionalSecurityStrategy ();

      Assert.IsInstanceOfType (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (NullCache<string, AccessType[]>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, ((SecurityStrategy) strategy.SecurityStrategy).GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      Expect.Call (_mockSecurityStrategy.HasAccess (null, null, null, null)).Return (true).Constraints (
          new FunctionalSecurityContextFactoryConstraint ("Rubicon.Security.UnitTests.SampleDomain.SecurableObject, Rubicon.Security.UnitTests"),
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
          new FunctionalSecurityContextFactoryConstraint ("Rubicon.Security.UnitTests.SampleDomain.SecurableObject, Rubicon.Security.UnitTests"),
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
