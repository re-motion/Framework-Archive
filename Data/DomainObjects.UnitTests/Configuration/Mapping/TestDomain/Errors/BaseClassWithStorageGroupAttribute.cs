using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  [DBStorageGroup]
  public abstract class BaseClassWithStorageGroupAttribute: DomainObject
  {
    protected BaseClassWithStorageGroupAttribute ()
    {
    }
  }
}