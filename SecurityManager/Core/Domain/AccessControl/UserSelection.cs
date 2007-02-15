using System;

namespace Rubicon.SecurityManager.Domain.AccessControl
{
  public enum UserSelection
  {
    All = 0,
    Owner = 1,
    SpecificUser = 2,
    SpecificPosition = 3
  }
}
