using System;

namespace Rubicon.Security.UnitTests.TestDomain
{
  [SecurityState]
  public enum Confidentiality
  {
    Normal = 0,
    Confidential = 1,
    Private = 2
  }
}