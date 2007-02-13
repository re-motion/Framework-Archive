using System.Configuration.Provider;
using System.Security.Principal;

namespace Rubicon.Security
{
  public class NullSecurityService : ProviderBase, ISecurityService
  {
    public AccessType[] GetAccess (SecurityContext context, IPrincipal user)
    {
      return new AccessType[0];
    }

    public int GetRevision ()
    {
      return 0;
    }
  }
}