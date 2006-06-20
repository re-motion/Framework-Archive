using System;
using System.Security.Principal;

using Rubicon.Data.DomainObjects;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public interface ISecurityTokenBuilder
  {
    SecurityToken CreateToken (ClientTransaction transaction, IPrincipal user, SecurityContext context);
  }
}
