using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Region")]
  [DBTable (Name = "TableInheritance_Region")]
  [NotAbstract]
  public abstract class Region: DomainObject
  {
    public static Region NewObject ()
    {
      return DomainObject.NewObject<Region> ().With();
    }

    protected Region()
    {
    }

    protected Region (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("Region")]
    public virtual ObjectList<Customer> Customers { get { return (ObjectList<Customer>) GetRelatedObjects(); } }
  }
}