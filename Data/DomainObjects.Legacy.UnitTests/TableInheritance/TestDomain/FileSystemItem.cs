using System;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.Legacy.UnitTests.TableInheritance.TestDomain
{
  public abstract class FileSystemItem : DomainObject
  {
    // types

    // static members and constants

    public static FileSystemItem GetObject (ObjectID id)
    {
      return (FileSystemItem) RepositoryAccessor.GetObject (id, false);
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

    public string Name
    {
      get { return (string) DataContainer.GetValue ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public Folder ParentFolder
    {
      get { return (Folder) GetRelatedObject ("ParentFolder"); }
      set { SetRelatedObject ("ParentFolder", value); }
    }
  }
}
