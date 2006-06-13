using System;
using System.Collections.Generic;
using System.Text;

using NMock2;

using Rubicon.Utilities;
using System.Security.Principal;
using Rubicon.Security.Metadata;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.UI.WebSecurityProviderTests
{
  public class WebPermissionProviderTestHelper
  {
    // types

    // static members

    // member fields

    private Mockery _mocks;
    private IPrincipal _user;
    private ISecurityService _mockSecurityService;
    private IUserProvider _mockUserProvider;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private IWxeSecurityProvider _mockWxeSecurityProvider;

    // construction and disposing

    public WebPermissionProviderTestHelper ()
    {
      _mocks = new Mockery ();
      
      _mockSecurityService = _mocks.NewMock<ISecurityService> ();
      _mockObjectSecurityStrategy = _mocks.NewMock<IObjectSecurityStrategy> ();
      _mockFunctionalSecurityStrategy = _mocks.NewMock<IFunctionalSecurityStrategy> ();
      _mockWxeSecurityProvider = _mocks.NewMock<IWxeSecurityProvider> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _mockUserProvider = _mocks.NewMock<IUserProvider> ();
      Stub.On (_mockUserProvider)
          .Method ("GetUser")
          .WithNoArguments ()
          .Will (Return.Value (_user));
    }

    // methods and properties

    public void ExpectObjectSecurityStrategyToBeNeverCalled ()
    {
      Expect.Never.On (_mockObjectSecurityStrategy);
    }

    public void ExpectFunctionalSecurityStrategyToBeNeverCalled ()
    {
      Expect.Never.On (_mockObjectSecurityStrategy);
    }

    public void ExpectWxeSecurityProviderToBeNeverCalled ()
    {
      Expect.Never.On (_mockWxeSecurityProvider);
    }

    public void ExpectHasAccess (Enum[] accessTypes, bool returnValue)
    {
      Expect.Once.On (_mockObjectSecurityStrategy)
         .Method ("HasAccess")
         .With (_mockSecurityService, _user, Array.ConvertAll<Enum, AccessType> (accessTypes, AccessType.Get))
         .Will (Return.Value (returnValue));
    }

    public void ExpectHasStatelessAccessForSecurableObject (Enum[] accessTypes, bool returnValue)
    {
      Expect.Once.On (_mockFunctionalSecurityStrategy)
         .Method ("HasAccess")
         .With (typeof (SecurableObject), _mockSecurityService, _user, Array.ConvertAll<Enum, AccessType> (accessTypes, AccessType.Get))
         .Will (Return.Value (returnValue));
    }

    public void ExpectHasStatelessAccessForWxeFunction (Type functionType, bool returnValue)
    {
      Expect.Once.On (_mockWxeSecurityProvider)
         .Method ("HasStatelessAccess")
         .With (functionType)
         .Will (Return.Value (returnValue));
    }

    public void VerifyAllExpectationsHaveBeenMet ()
    {
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    public ISecurityService SecurityService
    {
      get { return _mockSecurityService; }
    }

    public IUserProvider UserProvider
    {
      get { return _mockUserProvider; }
    }

    public IObjectSecurityStrategy ObjectSecurityStrategy
    {
      get { return _mockObjectSecurityStrategy; }
    }

    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get { return _mockFunctionalSecurityStrategy; }
    }

    public IWxeSecurityProvider WxeSecurityProvider
    {
      get { return _mockWxeSecurityProvider; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return new SecurableObject (_mockObjectSecurityStrategy);
    }
  }
}
