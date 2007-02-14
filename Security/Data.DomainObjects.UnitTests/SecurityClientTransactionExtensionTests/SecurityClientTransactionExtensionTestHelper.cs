using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rhino.Mocks;
using System.Security.Principal;
using Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Security.Configuration;
using Rubicon.Security.Metadata;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  public delegate bool HasAccessDelegate (ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);
  public delegate bool HasStatelessAccessDelegate (Type type, ISecurityService securityService, IPrincipal user, params AccessType[] requiredAccessTypes);

  public class SecurityClientTransactionExtensionTestHelper
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IPrincipal _user;
    private ISecurityService _mockSecurityService;
    private IUserProvider _stubUserProvider;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private IPermissionProvider _mockPermissionReflector;
    private ClientTransaction _transaction;

    // construction and disposing

    public SecurityClientTransactionExtensionTestHelper ()
    {
      _mocks = new MockRepository ();
      _mockSecurityService = _mocks.CreateMock<ISecurityService> ();
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _stubUserProvider = _mocks.CreateMock<IUserProvider> ();
      SetupResult.For (_stubUserProvider.GetUser ()).Return (_user);
      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();
      _mockPermissionReflector = _mocks.CreateMock<IPermissionProvider> ();
      _transaction = new ClientTransaction ();

      SetupResult.For (_mockSecurityService.IsNull).Return (false);
    }

    // methods and properties

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return new SecurableObject (_transaction, CreateObjectSecurityStrategy ());
    }

    public NonSecurableObject CreateNonSecurableObject ()
    {
      return new NonSecurableObject (_transaction);
    }

    public IObjectSecurityStrategy CreateObjectSecurityStrategy ()
    {
      return _mocks.CreateMock<IObjectSecurityStrategy> ();
    }

    public void SetupSecurityConfiguration ()
    {
      SecurityConfiguration.Current.SecurityService = _mockSecurityService;
      SecurityConfiguration.Current.UserProvider = _stubUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
      SecurityConfiguration.Current.PermissionProvider = _mockPermissionReflector;
    }

    public void TearDownSecurityConfiguration ()
    {
      SecurityConfiguration.Current.SecurityService = new NullSecurityService ();
      SecurityConfiguration.Current.UserProvider = new ThreadUserProvider ();
      SecurityConfiguration.Current.FunctionalSecurityStrategy = new FunctionalSecurityStrategy ();
      SecurityConfiguration.Current.PermissionProvider = new PermissionReflector ();
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public IDisposable Ordered ()
    {
      return _mocks.Ordered ();
    }

    public void AddExtension (IClientTransactionExtension extension)
    {
      ArgumentUtility.CheckNotNullAndType<SecurityClientTransactionExtension> ("extension", extension);

      _transaction.Extensions.Add (typeof (SecurityClientTransactionExtension).FullName, extension);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, HasAccessDelegate doDelegate)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      Expect.Call (objectSecurityStrategy.HasAccess (_mockSecurityService, _user, AccessType.Get (accessTypeEnum))).Do (doDelegate);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, bool returnValue)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      Expect.Call (objectSecurityStrategy.HasAccess (_mockSecurityService, _user, AccessType.Get (accessTypeEnum))).Return (returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, HasStatelessAccessDelegate doDelegate)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (securableObjectType, _mockSecurityService, _user, AccessType.Get (accessTypeEnum)))
          .Do (doDelegate);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (securableObjectType, _mockSecurityService, _user, AccessType.Get (accessTypeEnum)))
          .Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyWritePermissions (string propertyName, params Enum[] returnedAccessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyName)).Return (returnedAccessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyReadPermissions (string propertyName, params Enum[] returnedAccessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyName)).Return (returnedAccessTypes);
    }

    public void ExpectSecurityServiceGetAccess (SecurityContext context, params Enum[] returnedAccessTypes)
    {
      Expect.Call (_mockSecurityService.GetAccess (context, _user)).Return (Array.ConvertAll<Enum, AccessType> (returnedAccessTypes, AccessType.Get));
    }
  }
}