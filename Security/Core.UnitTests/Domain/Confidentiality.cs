using System;
using Rubicon.Security;

namespace Rubicon.Security.UnitTests.Domain
{
  [SecurityState]
  public enum Confidentiality
  {
    Normal = 0,
    Confidential = 1,
    Private = 2
  }
}