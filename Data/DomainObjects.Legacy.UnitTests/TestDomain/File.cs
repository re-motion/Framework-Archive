using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class File : FileSystemItem
  {
    // types

    // static members and constants

    public static new File GetObject (ObjectID id)
    {
      return (File) DomainObject.GetObject (id);
    }

    public static new File GetObject (ObjectID id, bool includeDeleted)
    {
      return (File) DomainObject.GetObject (id, includeDeleted);
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
