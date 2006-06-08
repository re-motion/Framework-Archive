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
  public class CheckConstructorAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private IPrincipal _user;
    private SecurityContext _statelessContext;

    [SetUp]
    public void SetUp ()
    {
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _statelessContext = new SecurityContext (typeof (SecurableObject));
      _testHelper = new SecurityClientTestHelper (_statelessContext, _user);
    }

    [Test]
    public void OneReturnedAccessType_ShouldAllowAccess ()
    {
      _testHelper.ExpectPermissionReflectorToBeNeverCalled ();
      _testHelper.ExpectGetAccessForStaticMethods (GeneralAccessType.Create);

      SecurityClient securityClient = _testHelper.CreateSecurityClient ();
      securityClient.CheckConstructorAccess (typeof (SecurableObject), _user);

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
    }

    [Test, ExpectedException (typeof (PermissionDeniedException))]
    public void OneReturnedAccessType_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectPermissionReflectorToBeNeverCalled ();
      _testHelper.ExpectGetAccessForStaticMethods (GeneralAccessType.Read);

      SecurityClient securityClient = _testHelper.CreateSecurityClient ();
      securityClient.CheckConstructorAccess (typeof (SecurableObject), _user);
    }

    [Test]
    public void MultipleReturnedAccessTypes_ShouldAllowAccess ()
    {
      _testHelper.ExpectPermissionReflectorToBeNeverCalled ();
      _testHelper.ExpectGetAccessForStaticMethods (GeneralAccessType.Edit, GeneralAccessType.Create);

      SecurityClient securityClient = _testHelper.CreateSecurityClient ();
      securityClient.CheckConstructorAccess (typeof (SecurableObject), _user);

      _testHelper.VerifyAllExpectationsHaveBeenMet ();
    }
  }
}
