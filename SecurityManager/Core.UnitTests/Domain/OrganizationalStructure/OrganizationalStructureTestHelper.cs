using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  public class OrganizationalStructureTestHelper
  {
    private ClientTransaction _transaction;
    private OrganizationalStructureFactory _factory;

    public OrganizationalStructureTestHelper ()
    {
      _transaction = new ClientTransaction ();
      _factory = new OrganizationalStructureFactory ();
    }

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public Tenant CreateTenant (string name, string uniqueIdentifier)
    {
      return CreateTenant (_transaction, name, uniqueIdentifier);
    }

    public Tenant CreateTenant (ClientTransaction transaction, string name, string uniqueIdentifier)
    {
      Tenant tenant = _factory.CreateTenant (transaction);
      tenant.UniqueIdentifier = uniqueIdentifier;
      tenant.Name = name;

      return tenant;
    }

    public Group CreateGroup (string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      return CreateGroup (_transaction, name, uniqueIdentifier, parent, tenant);
    }

    public Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      Group group = _factory.CreateGroup (transaction);
      group.Name = name;
      group.Parent = parent;
      group.Tenant = tenant;
      group.UniqueIdentifier = uniqueIdentifier;

      return group;
    }
    
    public User CreateUser (string userName, string firstName, string lastName, string title, Group owningGroup, Tenant tenant)
    {
      User user = _factory.CreateUser (_transaction);
      user.UserName = userName;
      user.FirstName = firstName;
      user.LastName = lastName;
      user.Title = title;
      user.Tenant = tenant;
      user.OwningGroup = owningGroup;

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
      Role role = Role.NewObject (_transaction);
      role.User = user;
      role.Group = group;
      role.Position = position;

      return role;
    }

    public GroupType CreateGroupType (string name)
    {
      GroupType groupType = GroupType.NewObject (_transaction);
      groupType.Name = name;

      return groupType;
    }

    public GroupTypePosition CreateGroupTypePosition (GroupType groupType, Position position)
    {
      GroupTypePosition concretePosition = GroupTypePosition.NewObject (_transaction);
      concretePosition.GroupType = groupType;
      concretePosition.Position = position;

      return concretePosition;

    }
  }
}