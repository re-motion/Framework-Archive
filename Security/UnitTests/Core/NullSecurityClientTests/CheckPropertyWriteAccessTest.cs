using System;
using NUnit.Framework;
using Remotion.Security.UnitTests.Core.SampleDomain;

namespace Remotion.Security.UnitTests.Core.NullSecurityClientTests
{
  [TestFixture]
  public class CheckPropertyWriteAccessTest
  {
    private NullSecurityClientTestHelper _testHelper;
    private SecurityClient _securityClient;

    [SetUp]
    public void SetUp()
    {
      _testHelper = NullSecurityClientTestHelper.CreateForStatefulSecurity();
      _securityClient = _testHelper.CreateSecurityClient();
    }

    [Test]
    public void Test_AccessGranted()
    {
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");

      _testHelper.VerifyAll();
    }

    [Test]
    public void Test_WithinSecurityFreeSection_AccessGranted()
    {
      _testHelper.ReplayAll();

      using (new SecurityFreeSection())
      {
        _securityClient.CheckPropertyWriteAccess (_testHelper.SecurableObject, "InstanceProperty");
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithSecurityStrategyIsNull()
    {
      _testHelper.ReplayAll();

      _securityClient.CheckPropertyWriteAccess (new SecurableObject (null), "InstanceProperty");

      _testHelper.VerifyAll ();
    }
  }
}