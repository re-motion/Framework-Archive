using System;

using Rubicon.Data.DomainObjects;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public class Region : DomainObject
  {
    // types

    // static members and constants

    public static new Region GetObject (ObjectID id)
    {
      return (Region) DomainObject.GetObject (id);
    }

    public static new Region GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Region) DomainObject.GetObject (id, clientTransaction);
    }

    // member fields

    // construction and disposing

    public Region ()
    {
    }

    public Region (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected Region (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public DomainObjectCollection Customers
    {
      get { return GetRelatedObjects ("Customers"); }
    }
  }
}
