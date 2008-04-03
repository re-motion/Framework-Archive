using System;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class Folder : FileSystemItem
  {
    // types

    // static members and constants

    public static new Folder GetObject (ObjectID id)
    {
      return (Folder) RepositoryAccessor.GetObject (id, false);
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
      get { return GetRelatedObjects ("FileSystemItems"); }
    }

    public DateTime CreatedAt
    {
      get { return (DateTime) DataContainer.GetValue ("CreatedAt"); }
      set { DataContainer.SetValue ("CreatedAt", value); }
    }
  }
}
