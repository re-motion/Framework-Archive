using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Relations;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
public class ClientTransactionMock : ClientTransaction
{
  // types

  // static members and constants

  // member fields

  private int _numberOfCallsToLoadDataContainer;
  private int _numberOfCallsToLoadRelatedObject;

  // construction and disposing

  public ClientTransactionMock ()
  {
    Initialize ();
  }

  // methods and properties

  private void Initialize ()
  {
    _numberOfCallsToLoadDataContainer = 0;
    _numberOfCallsToLoadRelatedObject = 0;
  }

  protected override DomainObject LoadObject (ObjectID id)
  {
    _numberOfCallsToLoadDataContainer++;
    return base.LoadObject (id);
  }

  protected override DomainObject LoadRelatedObject (ObjectEndPoint objectEndPoint)
  {
    _numberOfCallsToLoadRelatedObject++;
    return base.LoadRelatedObject (objectEndPoint);
  }

  public new DomainObject GetObject (ObjectID id)
  {
    return base.GetObject (id);
  }

  public new DomainObject GetRelatedObject (ObjectEndPoint objectEndPoint)
  {
    return base.GetRelatedObject (objectEndPoint);
  }

  public new DomainObjectCollection GetOriginalRelatedObjects (ObjectEndPoint objectEndPoint)
  {
    return base.GetOriginalRelatedObjects (objectEndPoint);
  }

  public new DomainObjectCollection GetRelatedObjects (ObjectEndPoint objectEndPoint)
  {
    return base.GetRelatedObjects (objectEndPoint);
  }

  public new void SetRelatedObject (ObjectEndPoint objectEndPoint, DomainObject newRelatedObject)
  {
    base.SetRelatedObject (objectEndPoint, newRelatedObject);
  }

  public int NumberOfCallsToLoadDataContainer
  {
    get { return _numberOfCallsToLoadDataContainer; }
  }

  public int NumberOfCallsToLoadRelatedObject
  {
    get { return _numberOfCallsToLoadRelatedObject; }
  }

  public new DataManager DataManager
  {
    get { return base.DataManager; }
  }
}
}
