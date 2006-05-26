using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Security
{
  public interface ISecurableObject
  {
    IObjectSecurityStrategy GetSecurityStrategy ();
  }
}
