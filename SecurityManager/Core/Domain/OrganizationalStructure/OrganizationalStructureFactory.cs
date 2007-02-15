using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public class OrganizationalStructureFactory : IOrganizationalStructureFactory
  {
    public virtual Group CreateGroup (ClientTransaction transaction)
    {
      return new Group (transaction);
    }

    public virtual User CreateUser (ClientTransaction transaction)
    {
      return new User (transaction);
    }

    public virtual Position CreatePosition (ClientTransaction transaction)
    {
      return new Position (transaction);
    }
  }
}
