using System;

using Rubicon.Data.DomainObjects.DataManagement;

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

  protected override DomainObject LoadRelatedObject (RelationEndPoint relationEndPoint)
  {
    _numberOfCallsToLoadRelatedObject++;
    return base.LoadRelatedObject (relationEndPoint);
  }

  public new DomainObject GetObject (ObjectID id)
  {
    return base.GetObject (id);
  }

  public new DomainObject GetRelatedObject (RelationEndPoint relationEndPoint)
  {
    return base.GetRelatedObject (relationEndPoint);
  }

  public new DomainObjectCollection GetOriginalRelatedObjects (RelationEndPoint relationEndPoint)
  {
    return base.GetOriginalRelatedObjects (relationEndPoint);
  }

  public new DomainObjectCollection GetRelatedObjects (RelationEndPoint relationEndPoint)
  {
    return base.GetRelatedObjects (relationEndPoint);
  }

  public new void SetRelatedObject (RelationEndPoint relationEndPoint, DomainObject newRelatedObject)
  {
    base.SetRelatedObject (relationEndPoint, newRelatedObject);
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
