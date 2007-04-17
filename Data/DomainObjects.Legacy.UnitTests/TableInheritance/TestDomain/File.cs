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
      get { return (int) DataContainer.GetValue ("Size"); }
      set { DataContainer.SetValue ("Size", value); }
    }

    public DateTime CreatedAt
    {
      get { return (DateTime) DataContainer.GetValue ("CreatedAt"); }
      set { DataContainer.SetValue ("CreatedAt", value); }
    }
  }
}
