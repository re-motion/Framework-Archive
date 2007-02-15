using System;
using System.Security.Principal;
using Rhino.Mocks;
using Rubicon.Security.Web.UnitTests.Domain;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Security.Web.UnitTests.UI.WebSecurityAdapterTests
{
  public class WebPermissionProviderTestHelper
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IPrincipal _user;
    private ISecurityService _mockSecurityService;
    private IUserProvider _mockUserProvider;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private IWxeSecurityAdapter _mockWxeSecurityAdapter;

    // construction and disposing

    public WebPermissionProviderTestHelper ()
    {
      _mocks = new MockRepository ();
      
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      SetupResult.For (_mockSecurityService.IsNull).Return (false);
      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();
      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();
      _mockWxeSecurityAdapter = _mocks.CreateMock<IWxeSecurityAdapter> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _mockUserProvider = _mocks.CreateMock<IUserProvider> ();
      SetupResult.For (_mockUserProvider.GetUser()).Return (_user);
    }

    // methods and properties

    public void ExpectHasAccess (Enum[] accessTypeEnums, bool returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, AccessType.Get);
      Expect.Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityService, _user, accessTypes)).Return (returnValue);
    }

    public void ExpectHasStatelessAccessForSecurableObject (Enum[] accessTypeEnums, bool returnValue)
    {
        AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, AccessType.Get);
        Expect
            .Call (_mockFunctionalSecurityStrategy.HasAccess (typeof (SecurableObject), _mockSecurityService, _user, accessTypes))
            .Return (returnValue);
    }

    public void ExpectHasStatelessAccessForWxeFunction (Type functionType, bool returnValue)
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (functionType)).Return (returnValue);
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public ISecurityService SecurityService
    {
      get { return _mockSecurityService; }
    }

    public IUserProvider UserProvider
    {
      get { return _mockUserProvider; }
    }

    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get { return _mockFunctionalSecurityStrategy; }
    }

    public IWxeSecurityAdapter WxeSecurityAdapter
    {
      get { return _mockWxeSecurityAdapter; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return new SecurableObject (_mockObjectSecurityStrategy);
    }
  }
}
