using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_Folder")]
  [DBTable (Name = "TableInheritance_Folder")]
  [NotAbstract]
  public abstract class Folder: FileSystemItem
  {
    public new static Folder GetObject (ObjectID id)
    {
      return (Folder) DomainObject.GetObject (id);
    }

    public static Folder NewObject()
    {
      return DomainObject.NewObject<Folder>().With();
    }

    protected Folder()
    {
    }

    protected Folder (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ParentFolder", SortExpression = "Name ASC")]
    public virtual ObjectList<FileSystemItem> FileSystemItems
    {
      get { return (ObjectList<FileSystemItem>) GetRelatedObjects(); }
    }

    [DBColumn ("FolderCreatedAt")]
    public abstract DateTime CreatedAt { get; set; }
  }
}