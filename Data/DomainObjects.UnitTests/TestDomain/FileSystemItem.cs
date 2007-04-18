using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class FileSystemItem : TestDomainBase
  {
    public static FileSystemItem NewObject ()
    {
      return DomainObject.NewObject<FileSystemItem> ().With ();
    }

    protected FileSystemItem()
    {
    }

    protected FileSystemItem (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [DBBidirectionalRelation ("FileSystemItems")]
    public abstract Folder ParentFolder { get; set; }

  }
}
