using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;
using Rubicon.SecurityManager.Configuration;

namespace Rubicon.SecurityManager.UnitTests.Domain
{
  public class DatabaseFixtures
  {
    private Client _currentClient;
    private OrganizationalStructureFactory _factory;

    public Client CurrentClient
    {
      get { return _currentClient; }
    }

    public DatabaseFixtures ()
    {
      _factory = new OrganizationalStructureFactory ();
    }

    public void CreateTestData ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      ClientTransaction transaction = new ClientTransaction ();

      AbstractRoleDefinition qualityManagerRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "QualityManager|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.UnitTests", 0);
      AbstractRoleDefinition developerRole = new AbstractRoleDefinition (transaction, Guid.NewGuid (), "Developer|Rubicon.SecurityManager.UnitTests.TestDomain.ProjectRole, Rubicon.SecurityManager.UnitTests", 1);

      Client client = CreateClient (transaction, "Testclient");
      Group rootGroup = CreateGroup (transaction, "rootGroup", null, client);
      Group parentOfOwnerGroup = CreateGroup (transaction, "parentOfOwnerGroup", rootGroup, client);
      Group ownerGroup = CreateGroup (transaction, "ownerGroup", parentOfOwnerGroup, client);
      Group group = CreateGroup (transaction, "Testgroup", ownerGroup, client);
      User user = CreateUser (transaction, "test.user", "test", "user", "Dipl.Ing.(FH)", group, client);
      Position officialPosition = CreatePosition (transaction, "Official", client);
      Position managerPosition = CreatePosition (transaction, "Manager", client);

      Role officialInGroup = CreateRole (transaction, user, group, officialPosition);
      Role managerInGroup = CreateRole (transaction, user, group, managerPosition);
      Role managerInOwnerGroup = CreateRole (transaction, user, ownerGroup, managerPosition);
      Role officialInRootGroup = CreateRole (transaction, user, rootGroup, officialPosition);
      
      transaction.Commit ();
    }

    public void CreateUsersWithDifferentClients ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      ClientTransaction transaction = new ClientTransaction ();

      _currentClient = CreateClient (transaction, "client 1");
      Client client2 = CreateClient (transaction, "client 2");

      Group group1 = CreateGroup (transaction, "group 1", null, _currentClient);
      Group group2 = CreateGroup (transaction, "group 2", null, client2);

      User user1 = CreateUser (transaction, "Hans", "Huber", "Huber.Hans", string.Empty, group1, _currentClient);
      User user2 = CreateUser (transaction, "Martha", "Hauser", "Hauser.Martha", string.Empty, group1, _currentClient);
      User user3 = CreateUser (transaction, "Heinz", "Zuber", "Zuber.Heinz", string.Empty, group2, client2);

      transaction.Commit ();
    }

    public void CreateGroupsWithDifferentClients ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      ClientTransaction transaction = new ClientTransaction ();

      _currentClient = CreateClient (transaction, "client 1");
      Client client2 = CreateClient (transaction, "client 2");

      Group group1 = CreateGroup (transaction, "group 1", null, _currentClient);
      Group group2 = CreateGroup (transaction, "group 2", null, _currentClient);
      Group group3 = CreateGroup (transaction, "group 3", null, client2);

      transaction.Commit ();
    }

    public void CreateGroupTypesWithDifferentClients ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      ClientTransaction transaction = new ClientTransaction ();

      _currentClient = CreateClient (transaction, "client 1");
      Client client2 = CreateClient (transaction, "client 2");

      GroupType groupType1 = CreateGroupType (transaction, "groupType 1", _currentClient);
      GroupType groupType2 = CreateGroupType (transaction, "groupType 2", _currentClient);
      GroupType groupType3 = CreateGroupType (transaction, "groupType 3", client2);

      transaction.Commit ();
    }

    public void CreatePositionsWithDifferentClients ()
    {
      DatabaseHelper dbHelper = new DatabaseHelper ();
      dbHelper.SetupDB ();

      ClientTransaction transaction = new ClientTransaction ();

      _currentClient = CreateClient (transaction, "client 1");
      Client client2 = CreateClient (transaction, "client 2");

      Position position1 = CreatePosition (transaction, "position 1", _currentClient);
      Position position2 = CreatePosition (transaction, "position 2", _currentClient);
      Position position3 = CreatePosition (transaction, "position 3", client2);

      transaction.Commit ();
    }

    private Group CreateGroup (ClientTransaction transaction, string name, Group parent, Client client)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Client = client;

      return group;
    }

    private Client CreateClient (ClientTransaction transaction, string name)
    {
      Client client = new Client (transaction);
      client.Name = name;

      return client;
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

    private GroupType CreateGroupType (ClientTransaction transaction, string name, Client client)
    {
      GroupType groupType = new GroupType (transaction);
      groupType.Name = name;
      groupType.Client = client;

      return groupType;
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
