using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

using NUnit.Framework;

using Rubicon.Security.Metadata;
using Rubicon.Security.UnitTests.TestDomain;
using Rubicon.Security.UnitTests.SampleDomain;

namespace Rubicon.Security.UnitTests.SecurityClientTests
{
  [TestFixture]
  public class CheckStatelessAccessTest
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
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessType.First, true);
      _testHelper.ReplayAll ();

      _securityClient.CheckStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessType.First));

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied_ShouldThrowPermissionDeniedException ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessType.First, false);
      _testHelper.ReplayAll ();

      _securityClient.CheckStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessType.First));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _securityClient.CheckStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessType.First));
      }

      _testHelper.VerifyAll ();
    }
  }
}
