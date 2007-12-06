using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TestDomain
{
  public class FileSystemItem : TestDomainBase
  {
    // types

    // static members and constants

    public static FileSystemItem GetObject (ObjectID id)
    {
      return (FileSystemItem) RepositoryAccessor.GetObject (id, false);
    }

    public static new FileSystemItem GetObject (ObjectID id, bool includeDeleted)
    {
      return (FileSystemItem) RepositoryAccessor.GetObject (id, includeDeleted);
    }

    // member fields

    // construction and disposing

    public FileSystemItem ()
    {
    }

    protected FileSystemItem (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public Folder ParentFolder
    {
      get { return (Folder) GetRelatedObject ("ParentFolder"); }
      set { SetRelatedObject ("ParentFolder", value); }
    }

  }
}
