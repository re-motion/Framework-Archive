using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors
{
  public abstract class BaseClass: DomainObject
  {
    protected BaseClass ()
    {
    }

    public abstract int Int32 { get; set; }
  }
}