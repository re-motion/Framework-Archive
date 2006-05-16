using System;
using Rubicon.Data;

namespace Rubicon.Security.UnitTests.TestDomain
{
  [AbstractRole]
  public enum DomainAbstractRole
  {
    [PermanentGuid ("00000003-0001-0000-0000-000000000000")]
    Clerk,
    [PermanentGuid ("00000003-0002-0000-0000-000000000000")]
    Secretary
  }
}