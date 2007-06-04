using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class HistoryEntry : DomainObject
  {
    // types

    // static members and constants

    public static new HistoryEntry GetObject (ObjectID id)
    {
      return (HistoryEntry) DomainObject.GetObject (id);
    }

    public static new HistoryEntry GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (HistoryEntry) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public HistoryEntry ()
    {
    }

    public HistoryEntry (ClientTransaction clientTransaction) : base (clientTransaction)
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
