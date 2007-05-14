using System;
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

    public Client CreateClient (string name, string uniqueIdentifier)
    {
      return CreateClient (_transaction, name, uniqueIdentifier);
    }

    public Client CreateClient (ClientTransaction transaction, string name, string uniqueIdentifier)
    {
      Client client = new Client (transaction);
      client.UniqueIdentifier = uniqueIdentifier;
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

    public Position CreatePosition (string name)
    {
      Position position = _factory.CreatePosition (_transaction);
      position.Name = name;

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

    public GroupType CreateGroupType (string name)
    {
      GroupType groupType = new GroupType (_transaction);
      groupType.Name = name;

      return groupType;
    }

    public GroupTypePosition CreateGroupTypePosition (GroupType groupType, Position position)
    {
      GroupTypePosition concretePosition = new GroupTypePosition (_transaction);
      concretePosition.GroupType = groupType;
      concretePosition.Position = position;

      return concretePosition;

    }
  }
}