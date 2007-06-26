using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class Folder : FileSystemItem
  {
    // types

    // static members and constants

    public static new Folder GetObject (ObjectID id)
    {
      return (Folder) DomainObject.GetObject (id);
    }

    public static new Folder GetObject (ObjectID id, bool includeDeleted)
    {
      return (Folder) DomainObject.GetObject (id, includeDeleted);
    }

    // member fields

    // construction and disposing

    public Folder ()
    {
    }

    protected Folder (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DomainObjectCollection FileSystemItems
    {
      get { return (DomainObjectCollection) GetRelatedObjects ("FileSystemItems"); }
    }

  }
}
