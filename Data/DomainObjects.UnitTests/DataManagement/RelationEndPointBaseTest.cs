using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.Transaction;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
public class RelationEndPointBaseTest : ClientTransactionBaseTest
{
  // types

  // static members and constants

  // member fields

  private ClientTransactionMock _clientTransactionMock;

  // construction and disposing

  protected RelationEndPointBaseTest ()
  {
  }

  // methods and properties

  public override void SetUp()
  {
    base.SetUp ();
    
    _clientTransactionMock = new ClientTransactionMock ();
    ClientTransaction.SetCurrent (_clientTransactionMock);
  }

  protected CollectionEndPoint CreateCollectionEndPoint (
      RelationEndPointID endPointID, 
      DomainObjectCollection domainObjects)
  {
    CollectionEndPoint newCollectionEndPoint = new CollectionEndPoint (
        _clientTransactionMock, endPointID, domainObjects);

    newCollectionEndPoint.ChangeDelegate = _clientTransactionMock.DataManager.RelationEndPointMap;

    return newCollectionEndPoint;
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      DomainObject domainObject,
      string propertyName,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (_clientTransactionMock, domainObject, propertyName, oppositeObjectID);
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      DataContainer dataContainer,
      string propertyName,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (_clientTransactionMock, dataContainer, propertyName, oppositeObjectID);
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      RelationEndPointID endPointID,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (_clientTransactionMock, endPointID, oppositeObjectID);
  }
}
}
