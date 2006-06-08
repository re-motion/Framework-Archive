using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using NMock2;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  public class MockObjectHelper
  {
    private Mockery _mocks;
    private IPrincipal _user;
    private SecurityContext _context;
    private ISecurityService _securityServiceMock;
    private IPermissionProvider _permissionReflectorMock;

    public MockObjectHelper (SecurityContext context, IPrincipal user)
    {
      _context = context;
      _user = user;
      _mocks = new Mockery ();
      _securityServiceMock = _mocks.NewMock<ISecurityService> ();
      _permissionReflectorMock = _mocks.NewMock<IPermissionProvider> ();
    }

    public Mockery Mocks
    {
      get { return _mocks; }
    }

    public ISecurityService CreateSecurityServiceMock (params Enum[] returnAccessTypeEnums)
    {
      AccessType[] returnAccessTypes = Array.ConvertAll<Enum, AccessType> (returnAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
      
      ISecurityService securityServiceMock = _mocks.NewMock<ISecurityService> ();

      Expect.Once.On (securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (returnAccessTypes));

      return securityServiceMock;
    }

    public IPermissionProvider CreatePermissionProviderMock (string expectedMethodName, params Enum[] returnAccessTypeEnums)
    {
      IPermissionProvider permissionProviderMock = _mocks.NewMock<IPermissionProvider> ();

      Expect.Once.On (permissionProviderMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), expectedMethodName)
          .Will (Return.Value (returnAccessTypeEnums));

      return permissionProviderMock;
    }

    public void ExpectGetRequiredMethodPermissions (string methodName, params Enum[] returnAccessTypes)
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredMethodPermissions")
          .With (typeof (SecurableObject), methodName)
          .Will (Return.Value (returnAccessTypes));
    }

    public void ExpectGetRequiredStaticMethodPermissions (string methodName, params Enum[] returnAccessTypes)
    {
      Expect.Once.On (_permissionReflectorMock)
          .Method ("GetRequiredStaticMethodPermissions")
          .With (typeof (SecurableObject), methodName)
          .Will (Return.Value (returnAccessTypes));
    }

    public void ExpectGetAccess (params Enum[] returnAccessTypeEnums)
    {
      AccessType[] returnAccessTypes = Array.ConvertAll<Enum, AccessType> (returnAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .With (_context, _user)
          .Will (Return.Value (returnAccessTypes));
    }

    public void ExpectGetAccessForStaticMethods (params Enum[] returnAccessTypeEnums)
    {
      AccessType[] returnAccessTypes = Array.ConvertAll<Enum, AccessType> (returnAccessTypeEnums, new Converter<Enum, AccessType> (AccessType.Get));
      Expect.Once.On (_securityServiceMock)
          .Method ("GetAccess")
          .Will (Return.Value (returnAccessTypes));
    }

    public ISecurityContextFactory CreateSecurityContextFactoryStub ()
    {
      ISecurityContextFactory securityContextFactoryStub = _mocks.NewMock<ISecurityContextFactory> ();

      Stub.On (securityContextFactoryStub)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_context));

      return securityContextFactoryStub;
    }

    public SecurableObject CreateSecurableObject ()
    {
      return new SecurableObject (CreateSecurityContextFactoryStub ());
    }

    public SecurityClient CreateSecurityClient ()
    {
      return new SecurityClient (_securityServiceMock, _permissionReflectorMock, new ThreadUserProvider (), new FunctionalSecurityStrategy ());
    }

    public void VerifyAllExpectationsHaveBeenMet ()
    {
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
