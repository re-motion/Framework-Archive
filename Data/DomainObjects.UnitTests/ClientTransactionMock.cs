using System;

using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests
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

  protected override DomainObject LoadRelatedObject (RelationEndPointID relationEndPointID)
  {
    _numberOfCallsToLoadRelatedObject++;
    return base.LoadRelatedObject (relationEndPointID);
  }

  public DomainObject GetObject (ObjectID id)
  {
    return GetObject (id, false);
  }

  public new DomainObject GetObject (ObjectID id, bool includeDeleted)
  {
    return base.GetObject (id, includeDeleted);
  }

  public new DomainObject GetRelatedObject (RelationEndPointID relationEndPointID)
  {
    return base.GetRelatedObject (relationEndPointID);
  }

  public new DomainObjectCollection GetOriginalRelatedObjects (RelationEndPointID relationEndPointID)
  {
    return base.GetOriginalRelatedObjects (relationEndPointID);
  }

  public new DomainObjectCollection GetRelatedObjects (RelationEndPointID relationEndPointID)
  {
    return base.GetRelatedObjects (relationEndPointID);
  }

  public new void SetRelatedObject (RelationEndPointID relationEndPointID, DomainObject newRelatedObject)
  {
    base.SetRelatedObject (relationEndPointID, newRelatedObject);
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
