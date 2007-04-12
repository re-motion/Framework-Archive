using System;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  public class RelationEndPointBaseTest : ClientTransactionBaseTest
  {
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
      return new ObjectEndPoint (domainObject, propertyName, oppositeObjectID);
    }

    protected ObjectEndPoint CreateObjectEndPoint (
        DataContainer dataContainer,
        string propertyName,
        ObjectID oppositeObjectID)
    {
      return new ObjectEndPoint (dataContainer, propertyName, oppositeObjectID);
    }

    protected ObjectEndPoint CreateObjectEndPoint (
        RelationEndPointID endPointID,
        ObjectID oppositeObjectID)
    {
      return new ObjectEndPoint (ClientTransactionMock, endPointID, oppositeObjectID);
    }
  }
}
