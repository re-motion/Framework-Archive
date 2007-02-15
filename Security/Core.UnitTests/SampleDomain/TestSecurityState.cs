using System;

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
