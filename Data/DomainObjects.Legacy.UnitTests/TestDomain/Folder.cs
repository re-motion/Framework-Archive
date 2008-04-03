using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class Folder : FileSystemItem
  {
    // types

    // static members and constants

    public static new Folder GetObject (ObjectID id)
    {
      return (Folder) RepositoryAccessor.GetObject (id, false);
    }

    public static new Folder GetObject (ObjectID id, bool includeDeleted)
    {
      return (Folder) RepositoryAccessor.GetObject (id, includeDeleted);
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
