using System;

namespace Rubicon.Security
{
  public interface ISecurityContextFactory
  {
    SecurityContext CreateSecurityContext ();
  }
}
