using System;
using Rubicon;

namespace Rubicon.Security.UnitTests.TestDomain
{
  [AbstractRole]
  public enum SpecialAbstractRoles
  {
    [PermanentGuid ("00000004-0001-0000-0000-000000000000")]
    Administrator,
  }
}