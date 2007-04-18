using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
  [Serializable]
  [DBTable]
  [TestDomain]
  [NotAbstract]
  public abstract class FileSystemItem : TestDomainBase
  {
    public static FileSystemItem Create ()
    {
      return DomainObject.Create<FileSystemItem> ();
    }

    protected FileSystemItem (ClientTransaction clientTransaction, ObjectID objectID)
        : base (clientTransaction, objectID)
    {
    }

    [DBBidirectionalRelation ("FileSystemItems")]
    public abstract Folder ParentFolder { get; set; }

  }
}
