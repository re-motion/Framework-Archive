using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [NotAbstract]
  public abstract class Folder : FileSystemItem
  {
    public new static Folder NewObject ()
    {
      return DomainObject.NewObject<Folder> ().With();
    }

    protected Folder ()
    {
    }

    protected Folder (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ParentFolder")]
    public virtual ObjectList<FileSystemItem> FileSystemItems { get { return (ObjectList<FileSystemItem>) GetRelatedObjects(); } }

  }
}
