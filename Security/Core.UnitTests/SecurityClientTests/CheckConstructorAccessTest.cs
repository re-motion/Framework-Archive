using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using NUnit.Framework;
using NMock2;

using Rubicon.Security.UnitTests.SampleDomain;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CheckConstructorAccessTest
  {
    private SecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = SecurityClientTestHelper.CreateForStatelessSecurity ();
      _securityClient = _testHelper.CreateSecurityClient ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (GeneralAccessType.Create, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckConstructorAccess (typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (GeneralAccessType.Create, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckConstructorAccess (typeof (SecurableObject));
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckConstructorAccess (typeof (SecurableObject));
      }

      _testHelper.VerifyAll ();
    }
  }
}
