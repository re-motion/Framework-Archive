using System;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public abstract class FileSystemItem : DomainObject
  {
    // types

    // static members and constants

    public static new FileSystemItem GetObject (ObjectID id)
    {
      return (FileSystemItem) DomainObject.GetObject (id);
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
      get { return DataContainer.GetString ("Name"); }
      set { DataContainer.SetValue ("Name", value); }
    }

    public Folder ParentFolder
    {
      get { return (Folder) GetRelatedObject ("ParentFolder"); }
      set { SetRelatedObject ("ParentFolder", value); }
    }
  }
}
