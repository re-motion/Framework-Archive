using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Web.UI;

using NMock2;
using NUnit.Framework;

using Rubicon.Security.Configuration;
using Rubicon.Security.Web.UI;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.UI;

namespace Rubicon.Security.Web.UnitTests.UI.WebSecurityProviderTests
{
  [TestFixture]
  public class PermissionFromSecurableObjectTest
  {
    private IWebSecurityProvider _securityProvider;
    private Mockery _mocks;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private IPrincipal _user;

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new WebSecurityProvider ();

      _mocks = new Mockery ();

      _securityService = _mocks.NewMock<ISecurityService> ();
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _userProvider = _mocks.NewMock<IUserProvider> ();
      Stub.On (_userProvider)
          .Method ("GetUser")
          .WithNoArguments ()
          .Will (Return.Value (_user));

      _mockObjectSecurityStrategy = _mocks.NewMock<IObjectSecurityStrategy> ();

      SecurityConfiguration.Current.SecurityService = _securityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
    }

    [TearDown]
    public void TearDown ()
    {
      SecurityConfiguration.Current.SecurityService = null;
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
    }

    [Test]
    public void HasAccessGranted ()
    {

      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (true));

      WebSecurityProvider securityProvider = new WebSecurityProvider ();

      SecurableObject securableObject = new SecurableObject (_mockObjectSecurityStrategy);
      bool hasAccess = securityProvider.HasAccess (securableObject, new EventHandler (TestEventHandler));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessDenied ()
    {
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (false));

      WebSecurityProvider securityProvider = new WebSecurityProvider ();

      SecurableObject securableObject = new SecurableObject (_mockObjectSecurityStrategy);
      bool hasAccess = securityProvider.HasAccess (securableObject, new EventHandler (TestEventHandler));

      _mocks.VerifyAllExpectationsHaveBeenMet ();
      Assert.IsFalse (hasAccess);
    }

    [DemandTargetMethodPermission ("Show")]
    private void TestEventHandler (object sender, EventArgs args)
    {
    }
  }
}