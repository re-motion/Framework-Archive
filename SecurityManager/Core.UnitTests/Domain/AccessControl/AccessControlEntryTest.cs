using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using System.Threading;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class AccessControlEntryTest : DomainTest
  {
    private AccessControlTestHelper _testHelper;

    public override void SetUp ()
    {
      base.SetUp ();
      _testHelper = new AccessControlTestHelper ();
    }

    [Test]
    public void MatchesToken_EmptyTokenAndEmptyAce ()
    {
      SecurityToken token = _testHelper.CreateEmptyToken ();
      AccessControlEntry entry = new AccessControlEntry (_testHelper.Transaction);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void MatchesToken_EmptyTokenAndAceForAbstractRole ()
    {
      SecurityToken token = _testHelper.CreateEmptyToken ();
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void MatchesToken_TokenWithAbstractRoleAndAceForAbstractRole ()
    {
      AccessControlEntry entry = _testHelper.CreateAceWithAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (entry.SpecificAbstractRole);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void MatchesToken_TokenWithAbstractRoleAndEmptyAce ()
    {
      AccessControlEntry entry = new AccessControlEntry (_testHelper.Transaction);
      SecurityToken token = _testHelper.CreateTokenWithAbstractRole (_testHelper.CreateTestAbstractRole ());

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void MatchesToken_EmptyTokenAndAceWithPosition ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager", client);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition);
      SecurityToken token = _testHelper.CreateEmptyToken ();

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void MatchesToken_TokenWithRoleAndAceWithPosition ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager", client);
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition);
      SecurityToken token = _testHelper.CreateTokenWithGroups (user, group);

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void MatchesToken_TokenWithRoleAndAbstractRoleAndAceWithPosition ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager", client);
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition);
      SecurityToken token = _testHelper.CreateToken (user, new Group[] { group }, new AbstractRoleDefinition[] { _testHelper.CreateTestAbstractRole() });

      Assert.IsTrue (entry.MatchesToken (token));
    }

    [Test]
    public void MatchesToken_TokenWithRoleAndAceWithPositionAndAbstractRole ()
    {
      Client client = _testHelper.CreateClient ("Testclient");
      Position managerPosition = _testHelper.CreatePosition ("Manager", client);
      Group group = _testHelper.CreateGroup ("Testgroup", null, client);
      User user = _testHelper.CreateUser ("test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);
      Role role = _testHelper.CreateRole (user, group, managerPosition);
      AccessControlEntry entry = _testHelper.CreateAceWithPosition (managerPosition);
      entry.SpecificAbstractRole = _testHelper.CreateTestAbstractRole ();
      SecurityToken token = _testHelper.CreateTokenWithGroups (user, group);

      Assert.IsFalse (entry.MatchesToken (token));
    }

    [Test]
    public void GetAllowedAccessTypes_EmptyAce ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes ();

      Assert.AreEqual (0, accessTypes.Length);
    }

    [Test]
    public void GetAllowedAccessTypes_ReadAllowed ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition readAccessType = _testHelper.CreateReadAccessType (ace, true);
      AccessTypeDefinition writeAccessType = _testHelper.CreateWriteAccessType (ace, null);
      AccessTypeDefinition deleteAccessType = _testHelper.CreateDeleteAccessType (ace, null);

      AccessTypeDefinition[] accessTypes = ace.GetAllowedAccessTypes ();

      Assert.AreEqual (1, accessTypes.Length);
      Assert.Contains (readAccessType, accessTypes);
    }

    [Test]
    public void AllowAccess_Read ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessType (ace, null);

      ace.AllowAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes ();
      Assert.AreEqual (1, allowedAccessTypes.Length);
      Assert.Contains (accessType, allowedAccessTypes);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The access type 'Test' is not assigned to this access control entry.\r\nParameter name: accessType")]
    public void AllowAccess_InvalidAccessType ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition accessType = new AccessTypeDefinition (_testHelper.Transaction, Guid.NewGuid (), "Test", 42);

      ace.AllowAccess (accessType);
    }

    [Test]
    public void RemoveAccess_Read ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition accessType = _testHelper.CreateReadAccessType (ace, true);

      ace.RemoveAccess (accessType);

      AccessTypeDefinition[] allowedAccessTypes = ace.GetAllowedAccessTypes ();
      Assert.AreEqual (0, allowedAccessTypes.Length);
    }

    [Test]
    public void AttachAccessType_TwoNewAccessType ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition accessType0 = new AccessTypeDefinition (_testHelper.Transaction, Guid.NewGuid (), "Access Type 0", 0);
      AccessTypeDefinition accessType1 = new AccessTypeDefinition (_testHelper.Transaction, Guid.NewGuid (), "Access Type 1", 1);
      DateTime changedAt = ace.ChangedAt;
      Thread.Sleep (50);

      ace.AttachAccessType (accessType0);
      ace.AttachAccessType (accessType1);

      Assert.AreEqual (2, ace.Permissions.Count);
      Permission permission0 = (Permission) ace.Permissions[0];
      Assert.AreSame (accessType0, permission0.AccessType);
      Assert.AreEqual (0, permission0.Index);
      Permission permission1 = (Permission) ace.Permissions[1];
      Assert.AreSame (accessType1, permission1.AccessType);
      Assert.AreEqual (1, permission1.Index);
      Assert.Greater ((decimal) ace.ChangedAt.Ticks, (decimal) changedAt.Ticks);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), "The access type 'Test' has already been attached to this access control entry.\r\nParameter name: accessType")]
    public void AttachAccessType_ExistingAccessType ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      AccessTypeDefinition accessType = new AccessTypeDefinition (_testHelper.Transaction, Guid.NewGuid (), "Test", 42);

      ace.AttachAccessType (accessType);
      ace.AttachAccessType (accessType);
    }

    [Test]
    public void GetActualPriority_CustomPriority ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      ace.Priority = 42;

      Assert.AreEqual (42, ace.ActualPriority);
    }

    [Test]
    public void GetActualPriority_EmptyAce ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);

      Assert.AreEqual (0, ace.ActualPriority);
    }

    [Test]
    public void GetActualPriority_AceWithAbstractRole ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      ace.SpecificAbstractRole = new AbstractRoleDefinition (_testHelper.Transaction, Guid.NewGuid (), "Test", 42);

      Assert.AreEqual (AccessControlEntry.AbstractRolePriority, ace.ActualPriority);
    }

    [Test]
    public void GetActualPriority_AceWithUser ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      ace.User = UserSelection.Owner;

      Assert.AreEqual (AccessControlEntry.UserPriority, ace.ActualPriority);
    }

    [Test]
    public void GetActualPriority_AceWithGroup ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      ace.Group = GroupSelection.GroupOfOwner;

      Assert.AreEqual (AccessControlEntry.GroupPriority, ace.ActualPriority);
    }

    [Test]
    public void GetActualPriority_AceWithClient ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      ace.Client = ClientSelection.ClientOfOwner;

      Assert.AreEqual (AccessControlEntry.ClientPriority, ace.ActualPriority);
    }

    [Test]
    public void GetActualPriority_AceWithUserAndGroup ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      ace.User = UserSelection.Owner;
      ace.Group = GroupSelection.GroupOfOwner;

      int expectedPriority = AccessControlEntry.UserPriority + AccessControlEntry.GroupPriority;
      Assert.AreEqual (expectedPriority, ace.ActualPriority);
    }

    [Test]
    public void GetActualPriority_AceWithUserAndAbstractRoleAndGroupAndClient ()
    {
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);
      ace.User = UserSelection.Owner;
      ace.SpecificAbstractRole = new AbstractRoleDefinition (_testHelper.Transaction, Guid.NewGuid (), "Test", 42);
      ace.Group = GroupSelection.GroupOfOwner;
      ace.Client = ClientSelection.ClientOfOwner;

      int expectedPriority = AccessControlEntry.UserPriority + AccessControlEntry.AbstractRolePriority + AccessControlEntry.GroupPriority
          + AccessControlEntry.ClientPriority;

      Assert.AreEqual (expectedPriority, ace.ActualPriority);
    }

    [Test]
    public void Get_PermissionsFromDatabase ()
    {
      DatabaseFixtures dbFixtures = new DatabaseFixtures ();
      ObjectID aceID = dbFixtures.CreateAccessControlEntryWith10Permissions ();

      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry ace = AccessControlEntry.GetObject (aceID, transaction);

      Assert.AreEqual (10, ace.Permissions.Count);
      for (int i = 0; i < 10; i++)
        Assert.AreEqual (string.Format ("Access Type {0}", i), ((Permission) ace.Permissions[i]).AccessType.Name);
    }

    [Test]
    public void GetChangedAt_AfterCreation ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);

      Assert.AreNotEqual (DateTime.MinValue, ace.ChangedAt);
    }

    [Test]
    public void Touch_AfterCreation ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);

      DateTime creationDate = ace.ChangedAt;

      Thread.Sleep (50);
      ace.Touch ();

      Assert.Greater ((decimal) ace.ChangedAt.Ticks, (decimal)creationDate.Ticks);
    }

    [Test]
    public void SetAndGet_Index ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      AccessControlEntry ace = new AccessControlEntry (_testHelper.Transaction);

      ace.Index = 1;
      Assert.AreEqual (1, ace.Index);
    }
  }
}
