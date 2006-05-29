using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace Rubicon.Security
{
  public interface IUserProvider
  {
    IPrincipal GetUser ();
  }
}
