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

    public static Folder Create()
    {
      return Create<Folder>();
    }

    protected Folder (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    [DBBidirectionalRelation ("ParentFolder", SortExpression = "Name ASC")]
    public virtual ObjectList<FileSystemItem> FileSystemItems
    {
      get { return (ObjectList<FileSystemItem>) GetRelatedObjects(); }
    }

    public abstract DateTime CreatedAt { get; set; }
  }
}