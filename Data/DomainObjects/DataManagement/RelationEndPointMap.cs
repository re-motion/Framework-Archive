using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class RelationEndPointMap : RelationEndPointCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public RelationEndPointMap ()
  {
  }

  public RelationEndPointMap (RelationEndPointCollection collection, bool isCollectionReadOnly) 
      : base (collection, isCollectionReadOnly)
  {
  }

  // methods and properties

  public override void Add (RelationEndPoint endPoint)
  {
    ArgumentUtility.CheckNotNull ("endPoint", endPoint);
    if (endPoint.IsNull)
      throw new ArgumentNullException ("endPoint", "A NullRelationEndPoint cannot be added to a RelationEndPointMap.");

    base.Add (endPoint);
  }

  public void Register (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    ClassDefinition classDefinition = dataContainer.ClassDefinition;

    foreach (RelationDefinition relationDefinition in classDefinition.GetAllRelationDefinitions ())
    {
      foreach (IRelationEndPointDefinition endPointDefinition in relationDefinition.EndPointDefinitions)
      {
        if (!endPointDefinition.IsVirtual)
        {
          if (classDefinition.IsRelationEndPoint (endPointDefinition))
          {
            ObjectID oppositeObjectID = dataContainer.GetObjectID (endPointDefinition.PropertyName);
            ObjectEndPoint endPoint = new ObjectEndPoint (dataContainer, endPointDefinition, oppositeObjectID);
            Add (endPoint);

            if (endPoint.OppositeEndPointDefinition.Cardinality == CardinalityType.One && endPoint.OppositeObjectID != null)
            {
              ObjectEndPoint oppositeEndPoint = new ObjectEndPoint (
                  endPoint.OppositeObjectID, endPoint.OppositeEndPointDefinition, endPoint.ObjectID);

              Add (oppositeEndPoint);
            }
          }
        }
      }
    }
  }
}
}
