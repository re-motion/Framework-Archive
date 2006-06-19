using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects;
using Rubicon.Security;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public interface IAccessControlListFinder
  {
    AccessControlList Find (ClientTransaction transaction, SecurityContext context);
  }
}
