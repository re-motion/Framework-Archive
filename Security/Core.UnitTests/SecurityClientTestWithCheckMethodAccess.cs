using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.SampleDomain.PermissionReflection;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests
{
  [TestFixture]
  public class SecurityClientTestWithCheckMethodAccess
  {
    private MockObjectCreator _mockMother;
    private IPrincipal _user;
    private SecurityContext _context;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _context = new SecurityContext (typeof (SecurableObject), "owner", "group", "client", new Dictionary<string, Enum> (), new Enum[0]);
      _mockMother = new MockObjectCreator (_context, _user);

      _securityClient = _mockMother.CreateSecurityClient ();
    }

    [Test]
    public void CheckSuccessfulAccess ()
    {
      _mockMother.ExpectGetRequiredMethodPermissions ("Record", GeneralAccessType.Edit);
      _mockMother.ExpectGetAccess (GeneralAccessType.Edit);

      _securityClient.CheckMethodAccess (_mockMother.CreateSecurableObject (), "Record", _user);

      _mockMother.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckDeniedAccess ()
    {
      _mockMother.ExpectGetRequiredMethodPermissions ("Record", GeneralAccessType.Edit);
      _mockMother.ExpectGetAccess (GeneralAccessType.Read);

      _securityClient.CheckMethodAccess (_mockMother.CreateSecurableObject (), "Record", _user);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The method 'Save' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForMethodWithoutRequiredPermissions ()
    {
      _mockMother.ExpectGetRequiredMethodPermissions ("Save", new Enum[0]);

      _securityClient.CheckMethodAccess (_mockMother.CreateSecurableObject (), "Save", _user);
    }

    [Test]
    public void CheckSuccessfulStaticMethodAccess ()
    {
      _mockMother.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", GeneralAccessType.Edit);
      _mockMother.ExpectGetAccessForStaticMethods (GeneralAccessType.Edit);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);

      _mockMother.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void CheckDeniedAccessForStaticMethod ()
    {
      _mockMother.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", GeneralAccessType.Edit);
      _mockMother.ExpectGetAccessForStaticMethods (GeneralAccessType.Read);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException),
        "The method 'CreateForSpecialCase' does not define required permissions.\r\nParameter name: requiredAccessTypeEnums")]
    public void CheckAccessForStaticMethodWithoutRequiredPermissions ()
    {
      _mockMother.ExpectGetRequiredStaticMethodPermissions ("CreateForSpecialCase", new Enum[0]);

      _securityClient.CheckStaticMethodAccess (typeof (SecurableObject), "CreateForSpecialCase", _user);
    }
  }
}
