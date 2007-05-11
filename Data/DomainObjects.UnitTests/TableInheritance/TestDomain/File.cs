using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_File")]
  [DBTable ("TableInheritance_File")]
  [Instantiable]
  public abstract class File: FileSystemItem
  {
    public new static File GetObject (ObjectID id)
    {
      return (File) DomainObject.GetObject (id);
    }

    public static File NewObject()
    {
      return NewObject<File>().With();
    }

    protected File ()
    {
    }

    protected File (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    public abstract int Size { get; set; }

    [DBColumn ("FileCreatedAt")]
    public abstract DateTime CreatedAt { get; set; }
  }
}