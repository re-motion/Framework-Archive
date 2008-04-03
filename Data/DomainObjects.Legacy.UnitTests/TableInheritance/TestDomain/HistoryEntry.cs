using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class HistoryEntry : DomainObject
  {
    // types

    // static members and constants

    public static HistoryEntry GetObject (ObjectID id)
    {
      return (HistoryEntry) RepositoryAccessor.GetObject (id, false);
    }

    // member fields

    // construction and disposing

    public HistoryEntry ()
    {
    }

    protected HistoryEntry (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DateTime HistoryDate
    {
      get { return (DateTime) DataContainer.GetValue ("HistoryDate"); }
      set { DataContainer.SetValue ("HistoryDate", value); }
    }

    public string Text
    {
      get { return (string) DataContainer.GetValue ("Text"); }
      set { DataContainer.SetValue ("Text", value); }
    }

    public DomainBase Owner
    {
      get { return (DomainBase) GetRelatedObject ("Owner"); }
      set { SetRelatedObject ("Owner", value); }
    }

    public new void Delete ()
    {
      base.Delete ();
    }
  }
}
