using System;
using System.Collections.Generic;
using System.Security.Principal;
using Rhino.Mocks;
using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  public class SecurityClientTestHelper
  {
    public static SecurityClientTestHelper CreateForStatelessSecurity ()
    {
      return new SecurityClientTestHelper (new SecurityContext (typeof (SecurableObject)));
    }
    
    public static SecurityClientTestHelper CreateForStatefulSecurity ()
    {
      SecurityContext context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      return new SecurityClientTestHelper (context);
    }

    private MockRepository _mocks;
    private IPrincipal _user;
    private SecurityContext _context;
    private ISecurityProvider _mockSecurityProvider;
    private IPermissionProvider _mockPermissionReflector;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private IUserProvider _stubUserProvider;
    private SecurableObject _securableObject;

    private SecurityClientTestHelper (SecurityContext context)
    {
      _context = context;
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);

      _mocks = new MockRepository ();
      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      _mockPermissionReflector = _mocks.CreateMock<IPermissionProvider> ();
      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();
      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();
      _stubUserProvider = _mocks.CreateMock<IUserProvider> ();
      SetupResult.For (_stubUserProvider.GetUser ()).Return (_user);

      _securableObject = new SecurableObject (_mockObjectSecurityStrategy);
    }

    public SecurityClient CreateSecurityClient ()
    {
      return new SecurityClient (_mockSecurityProvider, _mockPermissionReflector, _stubUserProvider, _mockFunctionalSecurityStrategy);
    }

    public SecurableObject SecurableObject
    {
      get { return _securableObject; }
    }

    public void ExpectPermissionReflectorGetRequiredMethodPermissions (string methodName, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredMethodPermissions (typeof (SecurableObject), methodName)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredStaticMethodPermissions (string methodName, params Enum[] returnValue)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredStaticMethodPermissions (typeof (SecurableObject), methodName)).Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyWritePermissions (string propertyName, params Enum[] accessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyName)).Return (accessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyReadPermissions (string propertyName, params Enum[] accessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyName)).Return (accessTypes);
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectObjectSecurityStrategyHasAccess (new Enum[] { requiredAccessType }, returnValue);
    }

    public void ExpectObjectSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      Expect
          .Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityProvider, _user, ConvertAccessTypeEnums (requiredAccessTypes)))
          .Return (returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum requiredAccessType, bool returnValue)
    {
      ExpectFunctionalSecurityStrategyHasAccess (new Enum[] { requiredAccessType }, returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Enum[] requiredAccessTypes, bool returnValue)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (typeof (SecurableObject), _mockSecurityProvider, _user, ConvertAccessTypeEnums (requiredAccessTypes)))
          .Return (returnValue);
    }

    public void ExpectSecurityServiceGetAccess (params Enum[] returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (returnValue, new Converter<Enum, AccessType> (AccessType.Get));
      Expect.Call (_mockSecurityProvider.GetAccess (_context, _user)).Return (accessTypes);
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }
 
    private AccessType[] ConvertAccessTypeEnums (Enum[] accessTypeEnums)
    {
      return Array.ConvertAll (accessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
    }
  }
}
