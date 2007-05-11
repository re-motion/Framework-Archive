using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_HistoryEntry")]
  [DBTable ("TableInheritance_HistoryEntry")]
  [TableInheritanceTestDomain]
  [Instantiable]
  public abstract class HistoryEntry: DomainObject
  {
    public new static HistoryEntry GetObject (ObjectID id)
    {
      return (HistoryEntry) DomainObject.GetObject (id);
    }

    public static HistoryEntry NewObject()
    {
      return NewObject<HistoryEntry>().With();
    }

    protected HistoryEntry()
    {
    }

    protected HistoryEntry (DataContainer dataContainer)
        : base (dataContainer)
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