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

  // construction and disposing

  protected RelationEndPointBaseTest ()
  {
  }

  // methods and properties

  protected CollectionEndPoint CreateCollectionEndPoint (
      RelationEndPointID endPointID, 
      DomainObjectCollection domainObjects)
  {
    CollectionEndPoint newCollectionEndPoint = new CollectionEndPoint (
        ClientTransactionMock, endPointID, domainObjects);

    newCollectionEndPoint.ChangeDelegate = ClientTransactionMock.DataManager.RelationEndPointMap;

    return newCollectionEndPoint;
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      DomainObject domainObject,
      string propertyName,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (ClientTransactionMock, domainObject, propertyName, oppositeObjectID);
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      DataContainer dataContainer,
      string propertyName,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (ClientTransactionMock, dataContainer, propertyName, oppositeObjectID);
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      RelationEndPointID endPointID,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (ClientTransactionMock, endPointID, oppositeObjectID);
  }
}
}
