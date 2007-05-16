using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public interface IOrganizationalStructureFactory
  {
    Tenant CreateTenant (ClientTransaction CurrentTransaction);
    Group CreateGroup (ClientTransaction transaction);
    User CreateUser (ClientTransaction transaction);
    Position CreatePosition (ClientTransaction transaction);
    Type GetTenantType ();
    Type GetGroupType ();
    Type GetUserType ();
    Type GetPositionType ();

  }
}
