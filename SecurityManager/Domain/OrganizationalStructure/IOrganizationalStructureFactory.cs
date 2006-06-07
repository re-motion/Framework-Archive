using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Data.DomainObjects;

namespace Rubicon.SecurityManager.Domain.OrganizationalStructure
{
  public interface IOrganizationalStructureFactory
  {
    Group CreateGroup (ClientTransaction transaction);
    User CreateUser (ClientTransaction transaction);
    Position CreatePosition (ClientTransaction transaction);
  }
}
