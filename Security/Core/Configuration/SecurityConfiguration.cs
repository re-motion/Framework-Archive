using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security.Configuration
{
  public class SecurityConfiguration
  {
    private static SecurityConfiguration s_current = null;

    public static SecurityConfiguration Current
    {
      get
      {
        if (s_current == null)
        {
          lock (typeof (SecurityConfiguration))
          {
            if (s_current == null)
            {
              s_current = new SecurityConfiguration ();
            }
          }
        }

        return s_current;
      }
    }

    private ISecurityService _securityService;
    private IUserProvider _userProvider;

    public ISecurityService SecurityService
    {
      get { return _securityService; }
      set { _securityService = value; }
    }

    public IUserProvider UserProvider
    {
      get { return _userProvider; }
      set { _userProvider = value; }
    }
  }
}
