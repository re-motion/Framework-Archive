using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.Metadata;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain
{
  public class DatabaseFixtures
  {
    private OrganizationalStructureFactory _factory;

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
