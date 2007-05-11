using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [Instantiable]
  public abstract class Folder : FileSystemItem
  {
    public new static Folder NewObject ()
    {
      return NewObject<Folder> ().With();
    }

    protected Folder ()
    {
    }

    protected Folder (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("ParentFolder")]
    public abstract ObjectList<FileSystemItem> FileSystemItems { get; }

  }
}
