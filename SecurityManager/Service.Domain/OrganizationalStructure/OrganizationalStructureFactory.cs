using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Security.Service.Domain.OrganizationalStructure
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
