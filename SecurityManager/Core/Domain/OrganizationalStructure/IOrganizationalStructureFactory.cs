using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public interface IOrganizationalStructureFactory
  {
    Client CreateClient (ClientTransaction CurrentTransaction);
    Group CreateGroup (ClientTransaction transaction);
    User CreateUser (ClientTransaction transaction);
    Position CreatePosition (ClientTransaction transaction);
    Type GetClientType ();
    Type GetGroupType ();
    Type GetUserType ();
    Type GetPositionType ();

  }
}
