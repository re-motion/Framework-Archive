using System;

using Rubicon.Data.DomainObjects.DataManagement;

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
    return new CollectionEndPoint (ClientTransaction.Current, endPointID, domainObjects);
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      DomainObject domainObject,
      string propertyName,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (ClientTransaction.Current, domainObject, propertyName, oppositeObjectID);
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      DataContainer dataContainer,
      string propertyName,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (ClientTransaction.Current, dataContainer, propertyName, oppositeObjectID);
  }

  protected ObjectEndPoint CreateObjectEndPoint (
      RelationEndPointID endPointID,
      ObjectID oppositeObjectID)
  {
    return new ObjectEndPoint (ClientTransaction.Current, endPointID, oppositeObjectID);
  }

}
}
