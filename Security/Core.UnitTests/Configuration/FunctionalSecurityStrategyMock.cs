using System;

namespace Rubicon.Security.UnitTests.Core.Configuration
{
  public class FunctionalSecurityStrategyMock : IFunctionalSecurityStrategy
  {
    public bool HasAccess (Type type, ISecurityProvider securityProvider, System.Security.Principal.IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      throw new NotImplementedException ();
    }
  }
}
