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
    public void GetRoles_Empty ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group = CreateGroup (transaction, "Testgroup", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group, client);

      List<Group> groups = new List<Group> ();
      groups.Add (group);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, groups, abstractRoles);

      Assert.AreEqual (0, token.Roles.Count);
    }

    [Test]
    public void GetRoles_WithRoles ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official", client);
      Position managerPosition = CreatePosition (transaction, "Manager", client);
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      List<Group> groups = new List<Group> ();
      groups.Add (group1);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, groups, abstractRoles);

      Assert.AreEqual (2, token.Roles.Count);
      Assert.Contains (officialInGroup1, token.Roles);
      Assert.Contains (managerInGroup1, token.Roles);
    }

    [Test]
    public void ContainsRoleForPosition_DoesNotContain ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official", client);
      Position managerPosition = CreatePosition (transaction, "Manager", client);
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      List<Group> groups = new List<Group> ();
      groups.Add (group2);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, groups, abstractRoles);

      Assert.IsFalse (token.ContainsRoleForPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForPosition_Contains ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", "Test", "User", "Dipl.Ing.(FH)", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official", client);
      Position managerPosition = CreatePosition (transaction, "Manager", client);
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      List<Group> groups = new List<Group> ();
      groups.Add (group1);

      List<AbstractRoleDefinition> abstractRoles = new List<AbstractRoleDefinition> ();

      SecurityToken token = new SecurityToken (user, groups, abstractRoles);

      Assert.IsTrue (token.ContainsRoleForPosition (managerPosition));
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

    private Position CreatePosition (ClientTransaction transaction, string name, Client client)
    {
      Position position = _factory.CreatePosition (transaction);
      position.Name = name;
      position.Client = client;

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
