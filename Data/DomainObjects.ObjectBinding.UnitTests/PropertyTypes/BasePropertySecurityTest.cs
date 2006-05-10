using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Data.DomainObjects.ObjectBinding.PropertyTypes;
using Rubicon.NullableValueTypes;
using Rubicon.Security;
using Rubicon.Security.Configuration;

using Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.ObjectBinding.UnitTests.PropertyTypes
{
  [TestFixture]
  public class BasePropertySecurityTest
  {
    private Mockery _mocks;
    private ISecurityService _securityService;
    private IUserProvider _userProvider;
    private SecurityContext _securityContext;
    private ISecurityContextFactory _securityContextFactory;
    private StringProperty _property;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new Mockery ();
      _securityService = _mocks.NewMock<ISecurityService> ();
      _userProvider = _mocks.NewMock<IUserProvider> ();
      SecurityConfiguration.Current.SecurityService = _securityService;
      SecurityConfiguration.Current.UserProvider = _userProvider;
      Stub.On (_userProvider)
          .Method ("GetUser")
          .Will (Return.Value (new GenericPrincipal (new GenericIdentity ("owner"), new string[0])));

      _securityContext = new SecurityContext (typeof (SecurableSearchObject), "owner", "group", "client", null, null);

      _securityContextFactory = _mocks.NewMock<ISecurityContextFactory> ();
      Stub.On (_securityContextFactory)
          .Method ("GetSecurityContext")
          .Will (Return.Value (_securityContext));

      Type domainObjectType = typeof (SecurableSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("StringProperty");
      _property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));
    }

    [Test]
    public void IsAccessible ()
    {
      Stub.On (_securityService)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create), AccessType.Get (GeneralAccessType.Delete) }));

      SecurableSearchObject testSearchObject = new SecurableSearchObject (_securityContextFactory);
      Assert.AreEqual (false, _property.IsAccessible (testSearchObject));
    }

    [Test]
    public void IsNotAccessible ()
    {
      Expect.Once.On (_securityService)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read) }));

      SecurableSearchObject testSearchObject = new SecurableSearchObject (_securityContextFactory);
      Assert.AreEqual (true, _property.IsAccessible (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsAccessibleForNonSecurableType ()
    {
      Expect.Never.On (_securityService)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read) }));

      Type domainObjectType = typeof (TestSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("StringProperty");
      BaseProperty property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));

      TestSearchObject testSearchObject = new TestSearchObject ();
      Assert.AreEqual (true, property.IsAccessible (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsReadOnly ()
    {
      Stub.On (_securityService)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Create), AccessType.Get (GeneralAccessType.Delete) }));

      SecurableSearchObject testSearchObject = new SecurableSearchObject (_securityContextFactory);
      Assert.AreEqual (true, _property.IsReadOnly (testSearchObject));
    }

    [Test]
    public void IsNotReadOnly ()
    {
      Expect.Once.On (_securityService)
          .Method ("GetAccess")
          .Will (Return.Value (new AccessType[] { AccessType.Get (GeneralAccessType.Edit), AccessType.Get (GeneralAccessType.Read) }));

      SecurableSearchObject testSearchObject = new SecurableSearchObject (_securityContextFactory);
      Assert.AreEqual (false, _property.IsReadOnly (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsNotReadOnlyForNonSecurableType ()
    {
      Expect.Never.On (_securityService)
          .Method ("GetAccess");

      Type domainObjectType = typeof (TestSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("StringProperty");
      BaseProperty property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));

      TestSearchObject testSearchObject = new TestSearchObject ();
      Assert.AreEqual (false, property.IsReadOnly (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    public void IsReadOnlyForNonSecurableType ()
    {
      Expect.Never.On (_securityService)
          .Method ("GetAccess");

      Type domainObjectType = typeof (TestSearchObject);
      PropertyInfo stringPropertyInfo = domainObjectType.GetProperty ("ReadOnlyStringProperty");
      BaseProperty property = new StringProperty (stringPropertyInfo, false, typeof (string), false, new NaInt32 (200));

      TestSearchObject testSearchObject = new TestSearchObject ();
      Assert.AreEqual (true, property.IsReadOnly (testSearchObject));
      _mocks.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
