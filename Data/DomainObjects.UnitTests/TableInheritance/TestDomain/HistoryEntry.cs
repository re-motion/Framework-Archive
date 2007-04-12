using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_HistoryEntry")]
  [DBTable (Name = "TableInheritance_HistoryEntry")]
  [NotAbstract]
  public abstract class HistoryEntry: DomainObject
  {
    public new static HistoryEntry GetObject (ObjectID id)
    {
      return (HistoryEntry) DomainObject.GetObject (id);
    }

    public static HistoryEntry Create()
    {
      return Create<HistoryEntry>();
    }

    protected HistoryEntry (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    public abstract DateTime HistoryDate { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 250)]
    public abstract string Text { get; set; }

    [DBBidirectionalRelation ("HistoryEntries")]
    [Mandatory]
    public abstract DomainBase Owner { get; set; }
  }
}