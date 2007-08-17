using System;

namespace Rubicon.Security.UnitTests.Core.SampleDomain
{
  [SecurityState]
  public enum TestSecurityState
  {
    Public,
    Confidential,
    Secret
  }
}
