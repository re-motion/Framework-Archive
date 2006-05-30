using System;
using System.Collections.Generic;
using System.Text;

using NMock2;
using NUnit.Framework;

using Rubicon.Security;
using Rubicon.Security.UnitTests.SampleDomain;
using System.Security.Principal;
using Rubicon.Security.Configuration;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class ObjectSecurityProviderTest
  {
    // types

    // static members

    // member fields

    private IObjectSecurityProvider _securityProvider;
    private Mockery _mocks;
    private ISecurableObject _mockSecurableObject;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private IPrincipal _user;

    // construction and disposing

    public ObjectSecurityProviderTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _securityProvider = new ObjectSecurityProvider ();

      _mocks = new Mockery ();

      _securityService = _mocks.NewMock<ISecurityService> ();
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _userProvider = _mocks.NewMock<IUserProvider> ();
      Stub.On (_userProvider)
          .Method ("GetUser")
          .WithNoArguments ()
          .Will (Return.Value (_user));
      
      SecurityConfiguration.Current.SecurityService = _securityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      
      _mockObjectSecurityStrategy = _mocks.NewMock<IObjectSecurityStrategy> ();
      _mockSecurableObject = _mocks.NewMock<ISecurableObject> ();
      Stub.On (_mockSecurableObject)
          .Method ("GetSecurityStrategy")
          .WithNoArguments ()
          .Will (Return.Value (_mockObjectSecurityStrategy));
    }

    [Test]
    public void HasAccessOnGetAccessorWithValidAccessType ()
    {
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (true));

      bool hasAccess = _securityProvider.HasAccessOnGetAccessor (_mockSecurableObject);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnGetAccessorWithInvalidAccessType ()
    {
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Read) })
          .Will (Return.Value (false));

      bool hasAccess = _securityProvider.HasAccessOnGetAccessor (_mockSecurableObject);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsFalse (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessorWithValidAccessType ()
    {
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Edit) })
          .Will (Return.Value (true));

      bool hasAccess = _securityProvider.HasAccessOnSetAccessor (_mockSecurableObject);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsTrue (hasAccess);
    }

    [Test]
    public void HasAccessOnSetAccessorWithInvalidAccessType ()
    {
      Expect.Once.On (_mockObjectSecurityStrategy)
          .Method ("HasAccess")
          .With (_securityService, _user, new AccessType[] { AccessType.Get (GeneralAccessType.Edit) })
          .Will (Return.Value (false));

      bool hasAccess = _securityProvider.HasAccessOnSetAccessor (_mockSecurableObject);

      _mocks.VerifyAllExpectationsHaveBeenMet ();

      Assert.IsFalse (hasAccess);
    }
  }
}