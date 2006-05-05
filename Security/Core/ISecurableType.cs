using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security
{
  public interface ISecurableType
  {
    ISecurityContextFactory GetSecurityContextFactory ();
  }
}
