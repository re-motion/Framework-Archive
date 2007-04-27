using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  [ClassID ("TI_FileSystemItem")]
  [TableInheritanceTestDomain]
  public abstract class FileSystemItem : DomainObject
  {
    protected FileSystemItem()
    {
    }

    protected FileSystemItem (DataContainer dataContainer)
        : base (dataContainer)
    {
    }

    [StringProperty (IsNullable = false, MaximumLength = 100)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("FileSystemItems")]
    public abstract Folder ParentFolder { get; set; }
  }
}