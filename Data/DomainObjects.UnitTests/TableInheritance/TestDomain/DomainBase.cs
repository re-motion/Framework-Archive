using System;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_DomainBase")]
  [TestDomain]
  public abstract class DomainBase: DomainObject
  {
    protected DomainBase (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
      if (objectID == null)
        InitializeNew();
    }

    protected DomainBase (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    private void InitializeNew()
    {
      CreatedBy = "UnitTests";
      CreatedAt = DateTime.Now;
    }

    // Note: This property always returns an empty collection.
    [DBBidirectionalRelation ("DomainBase")]
    public abstract ObjectList<AbstractClassWithoutDerivations> AbstractClassesWithoutDerivations { get; }

    [String (IsNullable = false, MaximumLength = 100)]
    public abstract string CreatedBy { get; set; }

    public abstract DateTime CreatedAt { get; set; }

    [DBBidirectionalRelation ("AssignedObjects")]
    public abstract Client Client { get; }

    [DBBidirectionalRelation ("Owner", SortExpression = "HistoryDate desc")]
    public abstract ObjectList<HistoryEntry> HistoryEntries { get; }
  }
}