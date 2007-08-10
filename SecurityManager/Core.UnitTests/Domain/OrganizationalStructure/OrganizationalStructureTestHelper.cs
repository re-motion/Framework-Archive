using System;
using Rubicon.Data.DomainObjects;
using Rubicon.SecurityManager.Domain.OrganizationalStructure;

namespace Rubicon.SecurityManager.UnitTests.Domain.OrganizationalStructure
{
  public class OrganizationalStructureTestHelper
  {
    private readonly ClientTransaction _transaction;
    private readonly OrganizationalStructureFactory _factory;

    public OrganizationalStructureTestHelper ()
    {
      _transaction = ClientTransaction.NewTransaction();
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
      using (_transaction.EnterScope())
      {
        Tenant tenant = _factory.CreateTenant (transaction);
        tenant.UniqueIdentifier = uniqueIdentifier;
        tenant.Name = name;

        return tenant;
      }
    }

    public Group CreateGroup (string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      return CreateGroup (_transaction, name, uniqueIdentifier, parent, tenant);
    }

    public Group CreateGroup (ClientTransaction transaction, string name, string uniqueIdentifier, Group parent, Tenant tenant)
    {
      using (transaction.EnterScope())
      {
        Group group = _factory.CreateGroup (transaction);
        group.Name = name;
        group.Parent = parent;
        group.Tenant = tenant;
        group.UniqueIdentifier = uniqueIdentifier;

        return group;
      }
    }

    public User CreateUser (string userName, string firstName, string lastName, string title, Group owningGroup, Tenant tenant)
    {
      using (_transaction.EnterScope())
      {
        User user = _factory.CreateUser (ClientTransactionScope.CurrentTransaction);
        user.UserName = userName;
        user.FirstName = firstName;
        user.LastName = lastName;
        user.Title = title;
        user.Tenant = tenant;
        user.OwningGroup = owningGroup;

        return user;
      }
    }

    public Position CreatePosition (string name)
    {
      using (_transaction.EnterScope())
      {
        Position position = _factory.CreatePosition (ClientTransactionScope.CurrentTransaction);
        position.Name = name;

        return position;
      }
    }

    public Role CreateRole (User user, Group group, Position position)
    {
      using (_transaction.EnterScope())
      {
        Role role = Role.NewObject (ClientTransactionScope.CurrentTransaction);
        role.User = user;
        role.Group = group;
        role.Position = position;

        return role;
      }
    }

    public GroupType CreateGroupType (string name)
    {
      using (_transaction.EnterScope())
      {
        GroupType groupType = GroupType.NewObject (ClientTransactionScope.CurrentTransaction);
        groupType.Name = name;

        return groupType;
      }
    }

    public GroupTypePosition CreateGroupTypePosition (GroupType groupType, Position position)
    {
      using (_transaction.EnterScope ())
      {
        GroupTypePosition concretePosition = GroupTypePosition.NewObject (ClientTransactionScope.CurrentTransaction);
        concretePosition.GroupType = groupType;
        concretePosition.Position = position;

        return concretePosition;
      }
    }
  }
}