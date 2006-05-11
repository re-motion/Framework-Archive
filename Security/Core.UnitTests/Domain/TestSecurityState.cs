using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security;

namespace Rubicon.Security.UnitTests.SampleDomain
{
  [SecurityState]
  public enum TestSecurityState
  {
    Public,
    Confidential,
    Secret
  }
}
