using System;
using Rubicon.Data;

namespace Rubicon.Security.UnitTests.TestDomain
{
  [AccessType]
  public enum DomainAccessType
  {
    [PermanentGuid ("00000002-0001-0000-0000-000000000000")]
    Journalize,
    [PermanentGuid ("00000002-0002-0000-0000-000000000000")]
    Archive
  }
}