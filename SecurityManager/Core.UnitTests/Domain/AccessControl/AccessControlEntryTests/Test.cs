using System;
using System.Threading;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl.AccessControlEntryTests
{
  [TestFixture]
  public class Test : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp();
      _testHelper = new AccessControlTestHelper();
      _testHelper.Transaction.EnterScope();
    }

    [Test]
    public void GetAllowedAccessTypes_EmptyAce ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes();

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAllowedAccessTypes_ReadAllowed ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace, null);

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes();

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (readAccessType, accessTypes);
    }

    [Test]
    public void AllowAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessType (ace, null);

      ace.AllowAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (1, allowedAccessTypes.Length);
      Assert.Contains (accessType, allowedAccessTypes);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The access type 'Test' is not assigned to this access control entry.\r\nParameter name: accessType")]
    public void AllowAccess_InvalidAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject  (Guid.NewGuid(), "Test", 42);

      ace.AllowAccess (accessType);
    }

    [Test]
    public void RemoveAccess_Read ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessType (ace, true);

      ace.RemoveAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes();
      Assert.AreEqual (0, allowedAccessTypes.Length);
    }

    [Test]
    public void AttachAccessType_TwoNewAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType0 = AccessTypeDefinition.NewObject  (Guid.NewGuid(), "Access Type 0", 0);
      AccessTypeDefinition accessType1 = AccessTypeDefinition.NewObject  (Guid.NewGuid(), "Access Type 1", 1);
      DateTime changedAt = ace.ChangedAt;
      Thread.Sleep (50);

      ace.AttachAccessType (accessType0);
      ace.AttachAccessType (accessType1);

      Assert.AreEqual (2, ace.Permissions.Count);
      Permission permission0 = ace.Permissions[0];
      Assert.AreSame (accessType0, permission0.AccessType);
      Assert.AreEqual (0, permission0.Index);
      Permission permission1 = (Permission) ace.Permissions[1];
      Assert.AreSame (accessType1, permission1.AccessType);
      Assert.AreEqual (1, permission1.Index);
      Assert.Greater ((decimal) ace.ChangedAt.Ticks, (decimal) changedAt.Ticks);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The access type 'Test' has already been attached to this access control entry.\r\nParameter name: accessType")]
    public void AttachAccessType_ExistingAccessType ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();
      AccessTypeDefinition accessType = AccessTypeDefinition.NewObject  (Guid.NewGuid(), "Test", 42);

      ace.AttachAccessType (accessType);
      ace.AttachAccessType (accessType);
    }

    [Test]
    public void Get_PermissionsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      ObjectID aceID = dbFixtures.CreateAccessControlEntryWithPermissions (10, ClientTransaction.NewTransaction());

      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);

      Assert.AreEqual (10, ace.Permissions.Count);
      for (int i = 0; i < 10; i++)
        Assert.AreEqual (string.Format ("Access Type {0}", i), (ace.Permissions[i]).AccessType.Name);
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      Assert.AreNotEqual (DateTime.MinValue, ace.ChangedAt);
    }

    [Test]
    public void Touch_AfterCreation ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      DateTime creationDate = ace.ChangedAt;

      Thread.Sleep (50);
      ace.Touch();

      Assert.Greater ((decimal) ace.ChangedAt.Ticks, (decimal) creationDate.Ticks);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      AccessControlEntry ace = AccessControlEntry.NewObject();

      ace.Index = 1;
      Assert.AreEqual (1, ace.Index);
    }

    [Test]
    public void ClearSpecificTenantOnCommit ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      ObjectID aceID = dbFixtures.CreateAccessControlEntryWithPermissions (0, ClientTransaction.NewTransaction());
      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);
      ace.Tenant = TenantSelection.OwningTenant;
      ace.SpecificTenant = _testHelper.CreateTenant ("TestTenant");

      Assert.IsNotNull (ace.SpecificTenant);
      ClientTransactionScope.CurrentTransaction.Commit ();
      Assert.IsNull (ace.SpecificTenant);      
    }

    [Test]
    public void ClearSpecificTenantOnCommitWhenObjectIsDeleted ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures();
      ObjectID aceID = dbFixtures.CreateAccessControlEntryWithPermissions (0, ClientTransaction.NewTransaction());
      AccessControlEntry ace = AccessControlEntry.GetObject (aceID);
      ace.Tenant = TenantSelection.SpecificTenant;
      ace.SpecificTenant = _testHelper.CreateTenant ("TestTenant");
      ClientTransactionScope.CurrentTransaction.Commit();
      using (new ClientTransactionScope())
      {
        AccessControlEntry aceActual = AccessControlEntry.GetObject (aceID);
        aceActual.Tenant = TenantSelection.OwningTenant;

        Assert.IsNotNull (aceActual.SpecificTenant);
        aceActual.Delete();
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
    }
  }
}