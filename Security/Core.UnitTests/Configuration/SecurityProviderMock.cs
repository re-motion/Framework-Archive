using System;
using System.Configuration.Provider;

namespace Rubicon.Security.UnitTests.Configuration
{
  public class SecurityProviderMock : ProviderBase, ISecurityProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public SecurityProviderMock ()
    {
    }

    // methods and properties

    public AccessType[] GetAccess (SecurityContext context, System.Security.Principal.IPrincipal user)
    {
      return new AccessType[0];
    }

    public int GetRevision ()
    {
      return 0;
    }

    public bool IsNull
    {
      get { return false; }
    }
  }
}