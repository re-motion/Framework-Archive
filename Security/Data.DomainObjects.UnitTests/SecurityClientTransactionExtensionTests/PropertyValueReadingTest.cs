using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using System.Security.Principal;
using Rubicon.Data.DomainObjects;
using Rubicon.Security.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Security.Data.DomainObjects.UnitTests.SecurityClientTransactionExtensionTests
{
  [TestFixture]
  public class PropertyValueReadingTest : BaseTest
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
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (dataContainer, dataContainer.PropertyValues["StringProperty"], ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (PermissionDeniedException))]
    public void Test_AccessDenied ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, false);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (dataContainer, dataContainer.PropertyValues["StringProperty"], ValueAccess.Current);
    }

    [Test]
    public void Test_AccessGranted_WithinSecurityFreeSection ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      using (new SecurityFreeSection ())
      {
        _extension.PropertyValueReading (dataContainer, dataContainer.PropertyValues["StringProperty"], ValueAccess.Current);
      }

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_WithNonSecurableObject ()
    {
      NonSecurableObject nonSecurableObject = _testHelper.CreateNonSecurableObject ();
      DataContainer dataContainer = nonSecurableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (dataContainer, dataContainer.PropertyValues["StringProperty"], ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_RecursiveSecurity ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      DataContainer dataContainer = securableObject.GetDataContainer ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      HasAccessDelegate hasAccess = delegate (ISecurityService securityService, IPrincipal user, AccessType[] requiredAccessTypes)
      {
        string dummy = securableObject.OtherStringProperty;
        return true;
      };
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, hasAccess);
      _testHelper.ReplayAll ();

      _extension.PropertyValueReading (dataContainer, dataContainer.PropertyValues["StringProperty"], ValueAccess.Current);

      _testHelper.VerifyAll ();
    }

    [Test]
    public void Test_AccessedViaDomainObject ()
    {
      SecurableObject securableObject = _testHelper.CreateSecurableObject ();
      _testHelper.AddExtension (_extension);
      _testHelper.ExpectPermissionReflectorGetRequiredPropertyReadPermissions ("StringProperty", TestAccessTypes.First);
      _testHelper.ExpectObjectSecurityStrategyHasAccess (securableObject, TestAccessTypes.First, true);
      _testHelper.ReplayAll ();

      object dummy = securableObject.StringProperty;

      _testHelper.VerifyAll ();
    }
  }
}