using System;
using System.Security.Principal;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class NewObjectCreatingTest : BaseTest
  {
    private SecurityClientTransactionExtensionTestHelper _testHelper;
    private IClientTransactionExtension _extension;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new SecurityClientTransactionExtensionTestHelper ();
      _extension = new SecurityClientTransactionExtension ();

      _testHelper.SetupSecurityConfiguration ();
    }

    [TearDown]
    public void TearDown ()
    {
      _testHelper.TearDownSecurityConfiguration ();
    }

    [Test]
    public void Test_AccessGranted ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, false);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (typeof (SecurableObject));
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.NewObjectCreating (typeof (SecurableObject));
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (typeof (NonSecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      IObjectSecurityStrategy objectSecurityStrategy = _testHelper.CreateObjectSecurityStrategy ();
      _testHelper.AddExtension (_extension);
      HasStatelessAccessDelegate hasAccess = delegate (Type type, ISecurityProvider securityService, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        new SecurableObject (_testHelper.Transaction, objectSecurityStrategy);
        return true;
      };
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, hasAccess);
      _testHelper.ReplayAll ();

      _extension.NewObjectCreating (typeof (SecurableObject));

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      IObjectSecurityStrategy objectSecurityStrategy = _testHelper.CreateObjectSecurityStrategy ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectFunctionalSecurityStrategyHasAccess (typeof (SecurableObject), GeneralAccessTypes.Create, true);
      _testHelper.ReplayAll ();

      new SecurableObject (_testHelper.Transaction, objectSecurityStrategy);

      _testHelper.VerifyAll ();
    }
  }
}