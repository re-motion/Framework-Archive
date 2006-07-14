using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;

using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CheckMethodAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private IPrincipal _user;
    private SecurityContext _context;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      _testHelper = new SecurityClientTestHelper (_context, _user);

      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void InstanceMethod_ShouldAllowAccess ()
    {
      _testHelper.ExpectGetRequiredMethodPermissions ("Record", GeneralAccessType.Edit);
      _testHelper.ExpectGetAccess (GeneralAccessType.Edit);
      ISecurableObject securableObject = _testHelper.CreateSecurableObject ();

      _securityClient.CheckMethodAccess (securableObject, "Record", _user);

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void InstanceMethod_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectGetRequiredMethodPermissions ("Record", GeneralAccessType.Edit);
      _testHelper.ExpectGetAccess (GeneralAccessType.Read);

      _securityClient.CheckMethodAccess (_testHelper.CreateSecurableObject (), "Record", _user);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The member 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void InstanceMethodWithoutDefinedPermissions_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectGetRequiredMethodPermissions ("Save", new Enum[0]);

      _securityClient.CheckMethodAccess (_testHelper.CreateSecurableObject (), "Save", _user);
    }

    [Test]
    public void StaticMethod_ShouldAllowAccess ()
    {
      _testHelper.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", GeneralAccessType.Edit);
      _testHelper.ExpectGetAccessForStaticMethods (GeneralAccessType.Edit);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void StaticMethod_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", GeneralAccessType.Edit);
      _testHelper.ExpectGetAccessForStaticMethods (GeneralAccessType.Read);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The member 'CreateForSpecialCase' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void StaticMethodWithoutDefinedPermissions_ShouldThrowArgumentException ()
    {
      _testHelper.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", new Enum[0]);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }
  }
}
