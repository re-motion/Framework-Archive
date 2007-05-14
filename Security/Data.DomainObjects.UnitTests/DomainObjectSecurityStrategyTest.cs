using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using NUnit.Framework;
using Rhino.Mocks;
using System.Security.Principal;
using Rubicon.Security.Configuration;
using Rubicon.Collections;

namespace Rubicon.Security.Data.DomainObjects.UnitTests
{
  [TestFixture]
  public class DomainObjectSecurityStrategyTest
  {
    private MockRepository _mocks;
    private ISecurityStrategy _mockSecurityStrategy;
    private ISecurityService _stubSecurityService;
    private IDomainObjectSecurityContextFactory _stubContextFactory;
    private IPrincipal _user;
    private AccessType[] _accessTypeResult;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _mockSecurityStrategy = _mocks.CreateMock<ISecurityStrategy> ();
      _stubSecurityService = _mocks.CreateMock<ISecurityService> ();
      _stubContextFactory = _mocks.CreateMock<IDomainObjectSecurityContextFactory> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _accessTypeResult = new AccessType[] { AccessType.Get (GeneralAccessTypes.Read), AccessType.Get (GeneralAccessTypes.Edit) };
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = new NullGlobalAccessTypeCacheProvider ();
    }

    [Test]
    public void Initialize ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.New, _stubContextFactory, _mockSecurityStrategy);
      Assert.AreSame (_stubContextFactory, strategy.SecurityContextFactory);
      Assert.AreSame (_mockSecurityStrategy, strategy.SecurityStrategy);
      Assert.AreEqual (RequiredSecurityForStates.New, strategy.RequiredSecurityForStates);
    }

    [Test]
    public void Initialize_WithDefaults ()
    {
      IGlobalAccessTypeCacheProvider stubGlobalCacheProvider = _mocks.CreateMock<IGlobalAccessTypeCacheProvider> ();
      SecurityConfiguration.Current.GlobalAccessTypeCacheProvider = stubGlobalCacheProvider;
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory);

      Assert.AreSame (_stubContextFactory, strategy.SecurityContextFactory);
      Assert.IsInstanceOfType (typeof (SecurityStrategy), strategy.SecurityStrategy);
      Assert.IsInstanceOfType (typeof (Cache<string, AccessType[]>), ((SecurityStrategy) strategy.SecurityStrategy).LocalCache);
      Assert.AreSame (stubGlobalCacheProvider, ((SecurityStrategy) strategy.SecurityStrategy).GlobalCacheProvider);
    }

    [Test]
    public void HasAccess_WithAccessGranted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityService, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_StateIsNew ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_StateIsDeleted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_SecurityRequiredForNew()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.New, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityService, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_SecurityRequiredForDeleted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.Deleted, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityService, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_SecurityRequiredForNewAndDeleted ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.NewAndDeleted, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityService, _user, _accessTypeResult)).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }
    [Test]
    public void HasAccess_WithAccessGrantedAndStateIsDiscarded ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (true);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void HasAccess_WithAccessDenied ()
    {
      DomainObjectSecurityStrategy strategy = new DomainObjectSecurityStrategy (RequiredSecurityForStates.None, _stubContextFactory, _mockSecurityStrategy);
      using (_mocks.Ordered ())
      {
        Expect.Call (_stubContextFactory.IsDiscarded).Return (false);
        Expect.Call (_stubContextFactory.IsNew).Return (false);
        Expect.Call (_stubContextFactory.IsDeleted).Return (false);
        Expect.Call (_mockSecurityStrategy.HasAccess (_stubContextFactory, _stubSecurityService, _user, _accessTypeResult)).Return (false);
      }
      _mocks.ReplayAll ();

      bool hasAccess = strategy.HasAccess (_stubSecurityService, _user, _accessTypeResult);

      _mocks.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }
  }
}