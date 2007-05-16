using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Threading;
using Rubicon.SecurityManager.Domain;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class ValidationTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }

    [Test]
    public void Validate_IsValid ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject (_testHelper.Transaction);

      AccessControlEntryValidationResult result = ace.Validate ();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateSpecificTenant_IsValid ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficTenant (tenant);

      AccessControlEntryValidationResult result = ace.Validate ();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateSpecificTenant_IsNull ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficTenant (tenant);
      ace.SpecificTenant = null;

      AccessControlEntryValidationResult result = ace.Validate ();

      Assert.IsFalse (result.IsValid);
      Assert.IsTrue (result.IsSpecificTenantMissing);
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException), ExpectedMessage =
       "The access control entry has the Tenant property set to SpecificTenant, but no Tenant is assigned.")]
    public void Commit_SpecificTenantIsNull ()
    {
      Tenant tenant = _testHelper.CreateTenant ("TestTenant");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficTenant (tenant);
      ace.SpecificTenant = null;

      _testHelper.Transaction.Commit ();
    }
  }
}
