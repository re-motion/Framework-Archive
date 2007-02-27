using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public abstract class DomainBase : DomainObject
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    protected DomainBase ()
    {
      InitializeNew ();
    }

    protected DomainBase (ClientTransaction clientTransaction)
      : base (clientTransaction)
    {
      InitializeNew ();
    }

    protected DomainBase (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    private void InitializeNew ()
    {
      CreatedBy = "UnitTests";
      CreatedAt = DateTime.Now;
    }

    // methods and properties

    // Note: This property always returns an empty collection.
    public DomainObjectCollection AbstractClassesWithoutDerivations
    {
      get { return GetRelatedObjects ("AbstractClassesWithoutDerivations"); }
    }

    public string CreatedBy
    {
      get { return DataContainer.GetString ("CreatedBy"); }
      set { DataContainer.SetValue ("CreatedBy", value); }
    }

    public DateTime CreatedAt
    {
      get { return DataContainer.GetDateTime ("CreatedAt"); }
      set { DataContainer.SetValue ("CreatedAt", value); }
    }

    public Client Client
    {
      get { return (Client) GetRelatedObject ("Client"); }
    }

    public DomainObjectCollection HistoryEntries
    {
      get { return GetRelatedObjects ("HistoryEntries"); }
    }
  }
}
