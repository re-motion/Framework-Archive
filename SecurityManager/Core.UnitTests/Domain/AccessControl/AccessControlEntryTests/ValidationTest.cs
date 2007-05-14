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
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);

      AccessControlEntryValidationResult result = ace.Validate ();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateSpecificClient_IsValid ()
    {
      Client client = _testHelper.CreateClient ("TestClient");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficClient (client);

      AccessControlEntryValidationResult result = ace.Validate ();

      Assert.IsTrue (result.IsValid);
    }

    [Test]
    public void ValidateSpecificClient_IsNull ()
    {
      Client client = _testHelper.CreateClient ("TestClient");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficClient (client);
      ace.SpecificClient = null;

      AccessControlEntryValidationResult result = ace.Validate ();

      Assert.IsFalse (result.IsValid);
      Assert.IsTrue (result.IsSpecificClientMissing);
    }

    [Test]
    [ExpectedException (typeof (ConstraintViolationException),
       "The access control entry has the Client property set to SpecificClient, but no Client is assigned.")]
    public void Commit_SpecificClientIsNull ()
    {
      Client client = _testHelper.CreateClient ("TestClient");
      AccessControlEntry ace = _testHelper.CreateAceWithSpecficClient (client);
      ace.SpecificClient = null;

      _testHelper.Transaction.Commit ();
    }
  }
}
