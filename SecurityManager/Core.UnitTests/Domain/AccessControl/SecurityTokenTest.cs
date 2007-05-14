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

    public override void SetUp ()
    {
      base.SetUp ();
      _factory = new OrganizationalStructureFactory ();
    }

    [Test]
    public void Initialize_Empty ()
    {
      SecurityToken token = new SecurityToken (null, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsNull (token.OwningClient);
      Assert.IsNull (token.User);
      Assert.IsEmpty (token.OwningGroups);
      Assert.IsEmpty (token.AbstractRoles);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void GetOwningClient ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      User user = null;

      SecurityToken token = new SecurityToken (user, client, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.AreSame (client, token.OwningClient);
    }

    [Test]
    public void GetOwningGroups_Empty ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      User user = null;

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsEmpty (token.OwningGroups);
    }

    [Test]
    public void GetOwningGroups ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group = CreateGroup (transaction, "Testgroup", null, client);
      User user = null;

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group), CreateAbstractRoles ());

      Assert.AreEqual (1, token.OwningGroups.Count);
      Assert.Contains (group, token.OwningGroups);
    }

    [Test]
    public void GetOwningGroupRoles_Empty ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group = CreateGroup (transaction, "Testgroup", null, client);
      User user = CreateUser (transaction, "test.user", group, client);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group), CreateAbstractRoles ());

      Assert.AreEqual (0, token.OwningGroupRoles.Count);
    }

    [Test]
    public void GetOwningGroupRoles_WithoutUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      User user = CreateUser (transaction, "test.user", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);

      SecurityToken token = new SecurityToken (null, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.IsNull (token.User);
      Assert.IsEmpty (token.OwningGroupRoles);
    }

    [Test]
    public void GetOwningGroupRoles_WithRoles ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group1), CreateAbstractRoles ());

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

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group2), CreateAbstractRoles ());

      Assert.IsEmpty (token.UserGroups);
    }

    [Test]
    public void GetUserGroups_WithUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group parentGroup1 = CreateGroup (transaction, "ParentGroup1", null, client);
      Group group1 = CreateGroup (transaction, "Group1", parentGroup1, client);
      Group group2 = CreateGroup (transaction, "Group2", null, client);
      User user = CreateUser (transaction, "test.user", group1, client);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group2), CreateAbstractRoles ());

      Assert.AreEqual (2, token.UserGroups.Count);
      Assert.Contains (group1, token.UserGroups);
      Assert.Contains (parentGroup1, token.UserGroups);
    }

    [Test]
    public void MatchesUserClient_MatchesUserInClient ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "TestClient");
      Group group = CreateGroup (transaction, "Testgroup", null, client);
      User user = CreateUser (transaction, "test.user", group, client);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsTrue (token.MatchesUserClient (client));
    }

    [Test]
    public void MatchesUserClient_MatchesUserInParentClient ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client parentClient = CreateClient (transaction, "ParentClient");
      Client client = CreateClient (transaction, "TestClient");
      client.Parent = parentClient;
      Group group = CreateGroup (transaction, "Testgroup", null, parentClient);
      User user = CreateUser (transaction, "test.user", group, parentClient);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsTrue (token.MatchesUserClient (client));
    }

    [Test]
    public void MatchesUserClient_DoesNotMatchWithoutUser ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");

      SecurityToken token = new SecurityToken (null, null, CreateOwningGroups (), CreateAbstractRoles ());

      Assert.IsFalse (token.MatchesUserClient (client));
    }

    [Test]
    public void ContainsRoleForOwningGroupAndPosition_DoesNotContain ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group2), CreateAbstractRoles ());

      Assert.IsFalse (token.ContainsRoleForOwningGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForOwningGroupAndPosition_Contains ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user = CreateUser (transaction, "test.user", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role officialInGroup1 = CreateRole (transaction, user, group1, officialPosition);
      Role managerInGroup1 = CreateRole (transaction, user, group1, managerPosition);
      Role officialInGroup2 = CreateRole (transaction, user, group2, officialPosition);

      SecurityToken token = new SecurityToken (user, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.IsTrue (token.ContainsRoleForOwningGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForUserGroupAndPosition_DoesNotContain ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user1 = CreateUser (transaction, "test.user1", group1, client);
      User user2 = CreateUser (transaction, "test.user2", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role user1OfficialInGroup1 = CreateRole (transaction, user1, group1, officialPosition);
      Role user2ManagerInGroup1 = CreateRole (transaction, user2, group1, managerPosition);
      Role user1OfficialInGroup2 = CreateRole (transaction, user1, group2, officialPosition);

      SecurityToken token = new SecurityToken (user1, null, CreateOwningGroups (group1), CreateAbstractRoles ());

      Assert.IsFalse (token.ContainsRoleForUserGroupAndPosition (managerPosition));
    }

    [Test]
    public void ContainsRoleForUserGroupAndPosition_Contains ()
    {
      ClientTransaction transaction = new ClientTransaction ();
      Client client = CreateClient (transaction, "Testclient");
      Group group1 = CreateGroup (transaction, "Testgroup", null, client);
      Group group2 = CreateGroup (transaction, "Other group", null, client);
      User user1 = CreateUser (transaction, "test.user1", group1, client);
      User user2 = CreateUser (transaction, "test.user2", group1, client);
      Position officialPosition = CreatePosition (transaction, "Official");
      Position managerPosition = CreatePosition (transaction, "Manager");
      Role user1OfficialInGroup1 = CreateRole (transaction, user1, group1, officialPosition);
      Role user2ManagerInGroup1 = CreateRole (transaction, user2, group1, managerPosition);
      Role user1OfficialInGroup2 = CreateRole (transaction, user1, group2, officialPosition);

      SecurityToken token = new SecurityToken (user1, null, CreateOwningGroups (group1), CreateAbstractRoles ());

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

    private User CreateUser (ClientTransaction transaction, string userName, Group group, Client client)
    {
      User user = _factory.CreateUser (transaction);
      user.UserName = userName;
      user.FirstName = "First Name";
      user.LastName = "Last Name";
      user.Title = "Title";
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

    private List<AbstractRoleDefinition> CreateAbstractRoles ()
    {
      return new List<AbstractRoleDefinition> ();
    }

    private List<Group> CreateOwningGroups (params Group[] groups)
    {
      return new List<Group> (groups);
    }

    private List<Client> CreateOwningClients (params Client[] clients)
    {
      return new List<Client> (clients);
    }
  }
}
