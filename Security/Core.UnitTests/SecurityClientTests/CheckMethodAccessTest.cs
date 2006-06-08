using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CheckMethodAccessTest
  {
    private MockObjectHelper _mockHelper;
    private IPrincipal _user;
    private SecurityContext _context;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      _mockHelper = new MockObjectHelper (_context, _user);

      _securityClient = _mockHelper.CreateSecurityClient ();
    }

    [Test]
    public void CheckSuccessfulAccess ()
    {
      _mockHelper.ExpectGetRequiredMethodPermissions ("Record", GeneralAccessType.Edit);
      _mockHelper.ExpectGetAccess (GeneralAccessType.Edit);
      ISecurableObject securableObject = _mockHelper.CreateSecurableObject ();

      _securityClient.CheckMethodAccess (securableObject, "Record", _user);

      _mockHelper.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckDeniedAccess ()
    {
      _mockHelper.ExpectGetRequiredMethodPermissions ("Record", GeneralAccessType.Edit);
      _mockHelper.ExpectGetAccess (GeneralAccessType.Read);

      _securityClient.CheckMethodAccess (_mockHelper.CreateSecurableObject (), "Record", _user);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The method 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForMethodWithoutRequiredPermissions ()
    {
      _mockHelper.ExpectGetRequiredMethodPermissions ("Save", new Enum[0]);

      _securityClient.CheckMethodAccess (_mockHelper.CreateSecurableObject (), "Save", _user);
    }

    [Test]
    public void CheckSuccessfulStaticMethodAccess ()
    {
      _mockHelper.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", GeneralAccessType.Edit);
      _mockHelper.ExpectGetAccessForStaticMethods (GeneralAccessType.Edit);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _mockHelper.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckDeniedAccessForStaticMethod ()
    {
      _mockHelper.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", GeneralAccessType.Edit);
      _mockHelper.ExpectGetAccessForStaticMethods (GeneralAccessType.Read);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The method 'CreateForSpecialCase' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForStaticMethodWithoutRequiredPermissions ()
    {
      _mockHelper.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", new Enum[0]);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }
  }
}
