using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [NotAbstract]
  public abstract class Folder : FileSystemItem
  {
    public static Folder Create ()
    {
      return DomainObject.Create<Folder> ();
    }

    protected Folder (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("ParentFolder")]
    public virtual ObjectList<FileSystemItem> FileSystemItems { get { return (ObjectList<FileSystemItem>) GetRelatedObjects(); } }

  }
}
