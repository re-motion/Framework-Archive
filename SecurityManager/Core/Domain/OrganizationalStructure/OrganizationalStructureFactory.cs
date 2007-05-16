using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public class OrganizationalStructureFactory : IOrganizationalStructureFactory
  {
    public virtual Tenant CreateTenant (ClientTransaction transaction)
    {
      return Tenant.NewObject (transaction);
    }

    public virtual Group CreateGroup (ClientTransaction transaction)
    {
      return Group.NewObject (transaction);
    }

    public virtual User CreateUser (ClientTransaction transaction)
    {
      return User.NewObject (transaction);
    }

    public virtual Position CreatePosition (ClientTransaction transaction)
    {
      return Position.NewObject (transaction);
    }

    public virtual Type GetTenantType ()
    {
      return typeof (Tenant);
    }

    public virtual Type GetGroupType ()
    {
      return typeof (Group);
    }

    public virtual Type GetUserType ()
    {
      return typeof (User);
    }

    public virtual Type GetPositionType ()
    {
      return typeof (Position);
    }
  }
}
