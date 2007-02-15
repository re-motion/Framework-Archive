using System;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public interface IAccessControlListFinder
  {
    AccessControlList Find (ClientTransaction transaction, SecurityContext context);
  }
}
