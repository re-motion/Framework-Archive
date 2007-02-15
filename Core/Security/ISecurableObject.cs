using System;

namespace Rubicon.Security
{
  public interface ISecurableObject
  {
    IObjectSecurityStrategy GetSecurityStrategy ();
  }
}
