using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests
{
  [SecurityState]
  public enum TestSecurityState
  {
    Public,
    Confidential,
    Secret
  }
}
