using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.AccessControl;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class SecurityTokenTest : DomainTest
  {
    private OrganizationalStructureFactory _factory;
    ClientTransaction _transaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _factory = new OrganizationalStructureFactory ();
      _transaction = ClientTransaction.NewTransaction();
      _transaction.EnterScope ();
    }

    [Test]
    public void Initialize_Empty ()
    {
      SecurityToken token = new SecurityToken (null, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsNull (token.OwningTenant);
      Assert.IsNull (token.User);
      Assert.IsEmpty (token.OwningGroups);
      Assert.IsEmpty (token.AbstractRoles);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void GetOwningTenant ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      User user = null;

      SecurityToken token = new SecurityToken (user, tenant, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.AreSame (tenant, token.OwningTenant);
    }

    [Test]
    public void GetOwningGroups_Empty ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      User user = null;

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsEmpty (token.OwningGroups);
    }

    [Test]
    public void GetOwningGroups ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group = CreateGroup (_transaction, "Testgroup", null, tenant);
      User user = null;

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group), CreateAbstractRoles ());

      Assert.AreEqual (1, token.OwningGroups.Count);
      Assert.Contains (group, token.OwningGroups);
    }

    [Test]
    public void GetOwningGroupRoles_Empty ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group = CreateGroup (_transaction, "Testgroup", null, tenant);
      User user = CreateUser (_transaction, "test.user", group, tenant);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group), CreateAbstractRoles ());

      Assert.AreEqual (0, token.OwningGroupRoles.Count);
    }

    [Test]
    public void GetOwningGroupRoles_WithoutUser ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group1 = CreateGroup (_transaction, "Testgroup", null, tenant);
      User user = CreateUser (_transaction, "test.user", group1, tenant);
      Position officialPosition = CreatePosition (_transaction, "Official");
      Role officialInGroup1 = CreateRole (_transaction, user, group1, officialPosition);

      SecurityToken token = new SecurityToken (null, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.IsNull (token.User);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void GetOwningGroupRoles_WithRoles ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group1 = CreateGroup (_transaction, "Testgroup", null, tenant);
      Group group2 = CreateGroup (_transaction, "Other group", null, tenant);
      User user = CreateUser (_transaction, "test.user", group1, tenant);
      Position officialPosition = CreatePosition (_transaction, "Official");
      Position managerPosition = CreatePosition (_transaction, "Manager");
      Role officialInGroup1 = CreateRole (_transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (_transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (_transaction, user, group2, officialPosition);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.AreEqual (2, token.OwningGroupRoles.Count);
      Assert.Contains (officialInGroup1, token.OwningGroupRoles);
      Assert.Contains (managerInGroup1, token.OwningGroupRoles);
    }

    [Test]
    public void GetUserGroups_WithoutUser ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group1 = CreateGroup (_transaction, "Group1", null, tenant);
      Group group2 = CreateGroup (_transaction, "Group2", null, tenant);
      User user = null;

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group2), CreateAbstractRoles ());

      Assert.IsEmpty (token.UserGroups);
    }

    [Test]
    public void GetUserGroups_WithUser ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group parentGroup1 = CreateGroup (_transaction, "ParentGroup1", null, tenant);
      Group group1 = CreateGroup (_transaction, "Group1", parentGroup1, tenant);
      Group group2 = CreateGroup (_transaction, "Group2", null, tenant);
      User user = CreateUser (_transaction, "test.user", group1, tenant);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group2), CreateAbstractRoles ());

      Assert.AreEqual (2, token.UserGroups.Count);
      Assert.Contains (group1, token.UserGroups);
      Assert.Contains (parentGroup1, token.UserGroups);
    }

    [Test]
    public void MatchesUserTenant_MatchesUserInTenant ()
    {
      Tenant tenant = CreateTenant (_transaction, "TestTenant");
      Group group = CreateGroup (_transaction, "Testgroup", null, tenant);
      User user = CreateUser (_transaction, "test.user", group, tenant);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsTrue (token.MatchesUserTenant (tenant));
    }

    [Test]
    public void MatchesUserTenant_MatchesUserInParentTenant ()
    {
      Tenant parentTenant = CreateTenant (_transaction, "ParentTenant");
      Tenant tenant = CreateTenant (_transaction, "TestTenant");
      tenant.Parent = parentTenant;
      Group group = CreateGroup (_transaction, "Testgroup", null, parentTenant);
      User user = CreateUser (_transaction, "test.user", group, parentTenant);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsTrue (token.MatchesUserTenant (tenant));
    }

    [Test]
    public void MatchesUserTenant_DoesNotMatchWithoutUser ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");

      SecurityToken token = new SecurityToken (null, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsFalse (token.MatchesUserTenant (tenant));
    }

    [Test]
    public void ContainsRoleForOwningGroupAndPosition_DoesNotContain ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group1 = CreateGroup (_transaction, "Testgroup", null, tenant);
      Group group2 = CreateGroup (_transaction, "Other group", null, tenant);
      User user = CreateUser (_transaction, "test.user", group1, tenant);
      Position officialPosition = CreatePosition (_transaction, "Official");
      Position managerPosition = CreatePosition (_transaction, "Manager");
      Role officialInGroup1 = CreateRole (_transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (_transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (_transaction, user, group2, officialPosition);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group2), CreateAbstractRoles ());

      Assert.IsFalse (token.ContainsRoleForOwningGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForOwningGroupAndPosition_Contains ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group1 = CreateGroup (_transaction, "Testgroup", null, tenant);
      Group group2 = CreateGroup (_transaction, "Other group", null, tenant);
      User user = CreateUser (_transaction, "test.user", group1, tenant);
      Position officialPosition = CreatePosition (_transaction, "Official");
      Position managerPosition = CreatePosition (_transaction, "Manager");
      Role officialInGroup1 = CreateRole (_transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (_transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (_transaction, user, group2, officialPosition);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.IsTrue (token.ContainsRoleForOwningGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForUserGroupAndPosition_DoesNotContain ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group1 = CreateGroup (_transaction, "Testgroup", null, tenant);
      Group group2 = CreateGroup (_transaction, "Other group", null, tenant);
      User user1 = CreateUser (_transaction, "test.user1", group1, tenant);
      User user2 = CreateUser (_transaction, "test.user2", group1, tenant);
      Position officialPosition = CreatePosition (_transaction, "Official");
      Position managerPosition = CreatePosition (_transaction, "Manager");
      Role user1OfficialInGroup1 = CreateRole (_transaction, user1, group1, officialPosition);
      Role user2ManagerInGroup1 = CreateRole (_transaction, user2, group1, managerPosition);
      Role user1OfficialInGroup2 = CreateRole (_transaction, user1, group2, officialPosition);

      SecurityToken token = new SecurityToken (user1, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.IsFalse (token.ContainsRoleForUserGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForUserGroupAndPosition_Contains ()
    {
      Tenant tenant = CreateTenant (_transaction, "Testtenant");
      Group group1 = CreateGroup (_transaction, "Testgroup", null, tenant);
      Group group2 = CreateGroup (_transaction, "Other group", null, tenant);
      User user1 = CreateUser (_transaction, "test.user1", group1, tenant);
      User user2 = CreateUser (_transaction, "test.user2", group1, tenant);
      Position officialPosition = CreatePosition (_transaction, "Official");
      Position managerPosition = CreatePosition (_transaction, "Manager");
      Role user1OfficialInGroup1 = CreateRole (_transaction, user1, group1, officialPosition);
      Role user2ManagerInGroup1 = CreateRole (_transaction, user2, group1, managerPosition);
      Role user1OfficialInGroup2 = CreateRole (_transaction, user1, group2, officialPosition);

      SecurityToken token = new SecurityToken (user1, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.IsTrue (token.ContainsRoleForUserGroupAndPosition (officialPosition));
    }

    private Tenant CreateTenant (ClientTransaction transaction, string name)
    {
      Tenant tenant = _factory.CreateTenant (transaction);
      tenant.Name = name;

      return tenant;
    }

    private Group CreateGroup (ClientTransaction transaction, string name, Group parent, Tenant tenant)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Tenant = tenant;

      return group;
    }

    private User CreateUser (ClientTransaction transaction, string userName, Group owningGroup, Tenant tenant)
    {
      User user = _factory.CreateUser (transaction);
      user.UserName = userName;
      user.FirstName = "First Name";
      user.LastName = "Last Name";
      user.Title = "Title";
      user.Tenant = tenant;
      user.OwningGroup = owningGroup;

      return user;
    }

    private Position CreatePosition (ClientTransaction transaction, string name)
    {
      Position position = _factory.CreatePosition (transaction);
      position.Name = name;

      return position;
    }

    private Role CreateRole (ClientTransaction transaction, User user, Group group, Position position)
    {
      Role role = Role.NewObject (transaction);
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }

    private List<AbstractRoleDefinition> CreateAbstractRoles ()
    {
      return new List<AbstractRoleDefinition> ();
    }

    private List<Group> CreateOwningGroups (params Group[] groups)
    {
      return new List<Group> (groups);
    }

    private List<Tenant> CreateOwningTenants (params Tenant[] tenants)
    {
      return new List<Tenant> (tenants);
    }
  }
}
