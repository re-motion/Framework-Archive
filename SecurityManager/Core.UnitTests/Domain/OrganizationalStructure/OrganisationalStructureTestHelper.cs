using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  public class OrganisationalStructureTestHelper
  {
    private ClientTransaction _transaction;
    private OrganizationalStructureFactory _factory;

    public OrganisationalStructureTestHelper ()
    {
      _transaction = new ClientTransaction ();
      _factory = new OrganizationalStructureFactory ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public Client CreateClient (string name)
    {
      return CreateClient (_transaction, name);
    }

    public Client CreateClient (ClientTransaction transaction, string name)
    {
      Client client = new Client (transaction);
      client.Name = name;

      return client;
    }

    public Group CreateGroup (string name, string uniqueIdentifier, Group parent, Client client)
    {
      return CreateGroup (_transaction, name, uniqueIdentifier, parent, client);
    }

    public Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Client client)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Client = client;
      group.UniqueIdentifier = uniqueIdentifier;

      return group;
    }
    
    public User CreateUser (string userName, string firstName, string lastName, string title, Group group, Client client)
    {
      User user = _factory.CreateUser (_transaction);
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Client = client;
      user.Group = group;

      return user;
    }

    public Position CreatePosition (string name, Client client)
    {
      Position position = _factory.CreatePosition (_transaction);
      position.Name = name;
      position.Client = client;

      return position;
    }

    public Role CreateRole (User user, Group group, Position position)
    {
      Role role = new Role (_transaction);
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }

    public GroupType CreateGroupType (string name, Client client)
    {
      GroupType groupType = new GroupType (_transaction);
      groupType.Name = name;
      groupType.Client = client;

      return groupType;
    }

    public ConcretePosition CreateConcretePosition (string name, GroupType groupType, Position position)
    {
      ConcretePosition concretePosition = new ConcretePosition (_transaction);
      concretePosition.Name = name;
      concretePosition.GroupType = groupType;
      concretePosition.Position = position;

      return concretePosition;

    }
  }
}