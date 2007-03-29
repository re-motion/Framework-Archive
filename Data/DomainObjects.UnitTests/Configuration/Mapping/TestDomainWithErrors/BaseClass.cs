using System;
using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors
{
  public abstract class BaseClass: DomainObject
  {
    protected BaseClass (ClientTransaction clientTransaction, ObjectID objectID)
      : base (clientTransaction, objectID)
    {
    }

    public abstract int Int32 { get; set; }
  }
}