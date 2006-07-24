using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.AccessControl;

namespace Rubicon.SecurityManager.UnitTests.Domain.AccessControl
{
  [TestFixture]
  public class SecurityTokenTest : DomainTest
  {
    private OrganizationalStructureFactory _factory;

    public override void SetUp ()
    {
      base.SetUp ();
      _factory = new OrganizationalStructureFactory ();
    }

    [Test]
    public void Initialize_Empty ()
    {
      List<Group> owningGroups = new List<Group> ();
      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (null, owningGroups, abstractRoles);
      
      Assert.IsNull (token.User);
      Assert.IsEmpty (token.OwningGroups);
      Assert.IsEmpty (token.AbstractRoles);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void Initialize_WithoutUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group1);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (null, owningGroups, abstractRoles);

      Assert.IsNull (token.User);
      Assert.AreEqual (1, token.OwningGroups.Count);
      Assert.Contains (group1, token.OwningGroups);
      Assert.IsEmpty (token.AbstractRoles);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void GetOwningGroupRoles_Empty ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group = CreateGroup (transaction, "Testgroup", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, owningGroups, abstractRoles);

      Assert.AreEqual (0, token.OwningGroupRoles.Count);
    }

    [Test]
    public void GetOwningGroupRoles_WithRoles ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group1);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, owningGroups, abstractRoles);

      Assert.AreEqual (2, token.OwningGroupRoles.Count);
      Assert.Contains (officialInGroup1, token.OwningGroupRoles);
      Assert.Contains (managerInGroup1, token.OwningGroupRoles);
    }

    [Test]
    public void GetUserGroups_WithoutUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Group1", null, client);
      Group group2 = CreateGroup (transaction, "Group2", null, client);
      User user = null;

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group2);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, owningGroups, abstractRoles);

      Assert.AreEqual (0, token.UserGroups.Count);
    }

    [Test]
    public void GetUserGroups_WithUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group parentGroup1 = CreateGroup (transaction, "ParentGroup1", null, client);
      Group group1 = CreateGroup (transaction, "Group1", parentGroup1, client);
      Group group2 = CreateGroup (transaction, "Group2", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group2);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, owningGroups, abstractRoles);

      Assert.AreEqual (2, token.UserGroups.Count);
      Assert.Contains (group1, token.UserGroups);
      Assert.Contains (parentGroup1, token.UserGroups);
    }

    [Test]
    public void ContainsRoleForOwningGroupAndPosition_DoesNotContain ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group2);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, owningGroups, abstractRoles);

      Assert.IsFalse (token.ContainsRoleForOwningGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForOwningGroupAndPosition_Contains ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group1);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, owningGroups, abstractRoles);

      Assert.IsTrue (token.ContainsRoleForOwningGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForUserGroupAndPosition_DoesNotContain ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user1 = CreateUser (transaction, "test.user1", "Test", "User 1", "Dipl.Ing.(FH)", group1, client);
      User user2 = CreateUser (transaction, "test.user2", "Test", "User 2", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role user1OfficialInGroup1 = CreateRole (transaction, user1, group1, officialPosition);
      Role user2ManagerInGroup1 = CreateRole (transaction, user2, group1, managerPosition);
      Role user1OfficialInGroup2 = CreateRole (transaction, user1, group2, officialPosition);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group1);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user1, owningGroups, abstractRoles);

      Assert.IsFalse (token.ContainsRoleForUserGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForUserGroupAndPosition_Contains ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user1 = CreateUser (transaction, "test.user1", "Test", "User 1", "Dipl.Ing.(FH)", group1, client);
      User user2 = CreateUser (transaction, "test.user2", "Test", "User 2", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role user1OfficialInGroup1 = CreateRole (transaction, user1, group1, officialPosition);
      Role user2ManagerInGroup1 = CreateRole (transaction, user2, group1, managerPosition);
      Role user1OfficialInGroup2 = CreateRole (transaction, user1, group2, officialPosition);

      List<Group> owningGroups = new List<Group> ();
      owningGroups.Add (group1);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user1, owningGroups, abstractRoles);

      Assert.IsTrue (token.ContainsRoleForUserGroupAndPosition (officialPosition));
    }

    private Client CreateClient (ClientTransaction transaction, string name)
    {
      Client client = new Client (transaction);
      client.Name = name;

      return client;
    }

    private Group CreateGroup (ClientTransaction transaction, string name, Group parent, Client client)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Client = client;

      return group;
    }

    private User CreateUser (ClientTransaction transaction, string userName, string firstName, string lastName, string title, Group group, Client client)
    {
      User user = _factory.CreateUser (transaction);
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Client = client;
      user.Group = group;

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
      Role role = new Role (transaction);
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }
  }
}
