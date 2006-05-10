using System;
using Rubicon.Security;

namespace Rubicon.Security.UnitTests.Domain
{
  [SecurityState]
  public enum FileState
  {
    New = 0,
    Normal = 1,
    Archived = 2
  }
}