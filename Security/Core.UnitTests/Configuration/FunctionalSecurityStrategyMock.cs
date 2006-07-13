using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.UnitTests.Configuration
{
  public class FunctionalSecurityStrategyMock : IFunctionalSecurityStrategy
  {
    public bool HasAccess (Type type, ISecurityService securityService, System.Security.Principal.IPrincipal user, params AccessType[] requiredAccessTypes)
    {
      throw new NotImplementedException ();
    }
  }
}
