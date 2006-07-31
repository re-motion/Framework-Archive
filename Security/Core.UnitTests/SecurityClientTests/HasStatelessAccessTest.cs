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
  public class HasStatelessAccessTest
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
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll ();
      Assert.AreEqual (true, hasAccess);
    }

    [Test]
    public void Test_AccessDenied ()
    {
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      bool hasAccess = _securityClient.HasStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));

      _testHelper.VerifyAll ();
      Assert.AreEqual (false, hasAccess);
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted ()
    {
      _testHelper.ReplayAll ();

      bool hasAccess;
      using (new SecurityFreeSection ())
      {
        hasAccess = _securityClient.HasStatelessAccess (typeof (SecurableObject), AccessType.Get (TestAccessTypes.First));
      }

      _testHelper.VerifyAll ();
      Assert.IsTrue (hasAccess);
    }
  }
}
