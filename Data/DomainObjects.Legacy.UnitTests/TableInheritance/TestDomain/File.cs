using System;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public class File : FileSystemItem
  {
    // types

    // static members and constants

    public static new File GetObject (ObjectID id)
    {
      return (File) DomainObject.GetObject (id);
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

    public int Size
    {
      get { return DataContainer.GetInt32 ("Size"); }
      set { DataContainer.SetValue ("Size", value); }
    }

    public DateTime CreatedAt
    {
      get { return DataContainer.GetDateTime ("CreatedAt"); }
      set { DataContainer.SetValue ("CreatedAt", value); }
    }
  }
}
