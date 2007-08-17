using System;

namespace Rubicon.Security.UnitTests.TestDomain
{
  [SecurityState]
  public enum FileState
  {
    New = 0,
    Normal = 1,
    Archived = 2
  }
}