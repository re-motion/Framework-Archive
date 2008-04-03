using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Client : DomainObject
  {
    // types

    // static members and constants

    public static Client GetObject (ObjectID id)
    {
      return (Client) RepositoryAccessor.GetObject (id, false);
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
