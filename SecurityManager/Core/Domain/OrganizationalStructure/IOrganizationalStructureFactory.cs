using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public interface IOrganizationalStructureFactory
  {
    Tenant CreateTenant ();
    Group CreateGroup ();
    User CreateUser ();
    Position CreatePosition ();
    Type GetTenantType ();
    Type GetGroupType ();
    Type GetUserType ();
    Type GetPositionType ();

  }
}
