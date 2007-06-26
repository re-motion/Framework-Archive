using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Client : DomainObject
  {
    // types

    // static members and constants

    public static new Client GetObject (ObjectID id)
    {
      return (Client) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Client ()
    {
    }

    protected Client (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DomainObjectCollection AssignedObjects
    {
      get { return GetRelatedObjects ("AssignedObjects"); }
    }

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }
  }
}
