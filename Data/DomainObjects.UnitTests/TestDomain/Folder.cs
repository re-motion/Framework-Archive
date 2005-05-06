using System;

using Rubicon.Data.DomainObjects;
using Rubicon.NullableValueTypes;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class Folder : FileSystemItem
{
  // types

  // static members and constants

  public static new Folder GetObject (ObjectID id)
  {
    return (Folder) DomainObject.GetObject (id);
  }

  public static new Folder GetObject (ObjectID id, bool includeDeleted)
  {
    return (Folder) DomainObject.GetObject (id, includeDeleted);
  }

  public static new Folder GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (Folder) DomainObject.GetObject (id, clientTransaction);
  }

  public static new Folder GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return (Folder) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  // construction and disposing

  public Folder ()
  {
  }

  public Folder (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected Folder (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public DomainObjectCollection FileSystemItems
  {
    get { return (DomainObjectCollection) GetRelatedObjects ("FileSystemItems"); }
  }

}
}
