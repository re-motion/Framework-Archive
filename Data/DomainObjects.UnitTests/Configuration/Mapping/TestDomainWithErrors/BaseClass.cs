using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors
{
  public abstract class BaseClass: DomainObject
  {
    protected BaseClass ()
    {
    }

    protected BaseClass (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    public abstract int Int32 { get; set; }
  }
}