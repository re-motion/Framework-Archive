using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security
{
  public interface ISecurityService
  {
    Enum[] GetAccess (SecurityContext context, string userName);
  }
}
