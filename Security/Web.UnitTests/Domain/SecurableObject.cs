using System;

namespace Rubicon.Security.Web.UnitTests.Domain
{
  public class SecurableObject : ISecurableObject
  {
    public enum Method
    {
      Delete,
      Show,
      Search
    }

    [DemandMethodPermission (GeneralAccessTypes.Search)]
    public static void Search ()
    {
    }

    private IObjectSecurityStrategy _securityStrategy;

    public SecurableObject (IObjectSecurityStrategy securityStrategy)
    {
      _securityStrategy = securityStrategy;
    }

    [DemandMethodPermission (GeneralAccessTypes.Read)]
    public void Show ()
    {
    }

    [DemandMethodPermission (GeneralAccessTypes.Delete)]
    public void Delete ()
    {
    }

    public IObjectSecurityStrategy GetSecurityStrategy ()
    {
      return _securityStrategy;
    }

    public Type GetSecurableType ()
    {
      return GetType ();
    }
  }
}
