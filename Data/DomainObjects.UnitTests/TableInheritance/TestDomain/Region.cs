using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Region")]
  [DBTable (Name = "TableInheritance_Region")]
  [NotAbstract]
  public abstract class Region: DomainObject
  {
    protected Region (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    protected Region (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [String (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Region")]
    public abstract ObjectList<Customer> Customers { get; }
  }
}