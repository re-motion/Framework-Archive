using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Folder")]
  [DBTable ("TableInheritance_Folder")]
  [Instantiable]
  public abstract class Folder: FileSystemItem
  {
    public new static Folder GetObject (ObjectID id)
    {
      return (Folder) DomainObject.GetObject (id);
    }

    public static Folder NewObject()
    {
      return NewObject<Folder>().With();
    }

    protected Folder()
    {
    }

    protected Folder (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ParentFolder", SortExpression = "Name ASC")]
    public abstract ObjectList<FileSystemItem> FileSystemItems { get; }

    [DBColumn ("FolderCreatedAt")]
    public abstract DateTime CreatedAt { get; set; }
  }
}