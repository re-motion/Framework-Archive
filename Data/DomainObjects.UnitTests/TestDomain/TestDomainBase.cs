using System;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.TestDomain
{
public class TestDomainBase : DomainObject
{
  // types

  // static members and constants

  public static new TestDomainBase GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (TestDomainBase) DomainObject.GetObject (id, clientTransaction);
  }

  public static new TestDomainBase GetObject (ObjectID id, bool includeDeleted)
  {
    return (TestDomainBase) DomainObject.GetObject (id, includeDeleted);
  }

  public static new TestDomainBase GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return (TestDomainBase) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  // construction and disposing

  protected TestDomainBase ()
  {
  }

  protected TestDomainBase (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected TestDomainBase (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public new DataContainer DataContainer 
  {
    get { return base.DataContainer; }
  }

  public new DomainObject GetRelatedObject (string propertyName)
  {
    return base.GetRelatedObject (propertyName);
  }

  public new DomainObjectCollection GetRelatedObjects (string propertyName)
  {
    return base.GetRelatedObjects (propertyName);
  }

  public new DomainObject GetOriginalRelatedObject (string propertyName)
  {
    return base.GetOriginalRelatedObject (propertyName);
  }

  public new DomainObjectCollection GetOriginalRelatedObjects (string propertyName)
  {
    return base.GetOriginalRelatedObjects (propertyName);
  }

  public new void SetRelatedObject (string propertyName, DomainObject newRelatedObject)
  {
    base.SetRelatedObject (propertyName, newRelatedObject);
  }
}
}
