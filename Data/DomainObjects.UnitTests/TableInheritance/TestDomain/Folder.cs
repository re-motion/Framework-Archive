using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.UnitTests.TableInheritance.TestDomain
{
  public class Folder : FileSystemItem
  {
    // types

    // static members and constants

    public static new Folder GetObject (ObjectID id)
    {
      return (Folder) DomainObject.GetObject (id);
    }

    // member fields

    // construction and disposing

    public Folder ()
    {
    }

    protected Folder (DataContainer dataContainer)
      : base (dataContainer)
    {
    }

    // methods and properties

    public DomainObjectCollection FileSystemItems
    {
      get { return GetRelatedObjects ("FileSystemItems"); }
    }
  }
}
