using System;

using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class FileSystemItem : DomainObject
{
  // types

  // static members and constants

  public static new FileSystemItem GetObject (ObjectID id)
  {
    return (FileSystemItem) DomainObject.GetObject (id);
  }

  public static new FileSystemItem GetObject (ObjectID id, bool includeDeleted)
  {
    return (FileSystemItem) DomainObject.GetObject (id, includeDeleted);
  }

  public static new FileSystemItem GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (FileSystemItem) DomainObject.GetObject (id, clientTransaction);
  }

  public static new FileSystemItem GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return (FileSystemItem) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  // construction and disposing

  public FileSystemItem ()
  {
  }

  public FileSystemItem (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected FileSystemItem (DataContainer dataContainer) : base (dataContainer)
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
