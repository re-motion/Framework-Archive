using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_File")]
  [DBTable (Name = "TableInheritance_File")]
  [NotAbstract]
  public abstract class File: FileSystemItem
  {
    public new static File GetObject (ObjectID id)
    {
      return (File) DomainObject.GetObject (id);
    }

    public static File Create()
    {
      return Create<File>();
    }

    protected File (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    public abstract int Size { get; set; }

    public abstract DateTime CreatedAt { get; set; }
  }
}