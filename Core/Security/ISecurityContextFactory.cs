using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security
{
  public interface ISecurityContextFactory
  {
    SecurityContext CreateSecurityContext ();
  }
}
