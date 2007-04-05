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

    protected Folder (ClientTransaction clientTransaction, ObjectID id)
        : base (clientTransaction, id)
    {
    }

    protected Folder (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ParentFolder", SortExpression = "Name ASC")]
    public abstract ObjectList<FileSystemItem> FileSystemItems { get; }

    public abstract DateTime CreatedAt { get; set; }
  }
}