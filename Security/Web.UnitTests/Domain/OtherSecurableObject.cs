using System;

namespace Rubicon.Security.Web.UnitTests.Domain
{
  public class OtherSecurableObject : ISecurableObject
  {
    private IObjectSecurityStrategy _securityStrategy;

    public OtherSecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public void Show ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }
  }
}
