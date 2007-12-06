using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class File : FileSystemItem
  {
    // types

    // static members and constants

    public static new File GetObject (ObjectID id)
    {
      return (File) RepositoryAccessor.GetObject (id, false);
    }

    public static new File GetObject (ObjectID id, bool includeDeleted)
    {
      return (File) RepositoryAccessor.GetObject (id, includeDeleted);
    }

    // member fields

    // construction and disposing

    public File ()
    {
    }

    protected File (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

  }
}
