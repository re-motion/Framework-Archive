using System;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.DataManagement
{
public class CollectionEndPointChangeWorker
{
  // types

  public enum OperationType
  {
    Add = 0,
    Remove = 1,
    Insert = 2,
    Replace = 3
  }

  // static members and constants

  public static CollectionEndPointChangeWorker CreateForAdd (
      DomainObjectCollection oppositeDomainObjects,
      RelationEndPoint oldEndPoint, 
      RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return new CollectionEndPointChangeWorker (
        oppositeDomainObjects, oldEndPoint, newEndPoint, OperationType.Add, oppositeDomainObjects.Count);
  }

  public static CollectionEndPointChangeWorker CreateForRemove (
      DomainObjectCollection oppositeDomainObjects,
      RelationEndPoint oldEndPoint, 
      RelationEndPoint newEndPoint)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return new CollectionEndPointChangeWorker (
        oppositeDomainObjects, oldEndPoint, newEndPoint, 
        OperationType.Remove, oppositeDomainObjects.IndexOf (oldEndPoint.ObjectID));
  }

  public static CollectionEndPointChangeWorker CreateForInsert (
      DomainObjectCollection oppositeDomainObjects,
      RelationEndPoint oldEndPoint, 
      RelationEndPoint newEndPoint, 
      int insertIndex)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return new CollectionEndPointChangeWorker (
        oppositeDomainObjects, oldEndPoint, newEndPoint, OperationType.Insert, insertIndex);
  }

  public static CollectionEndPointChangeWorker CreateForReplace (
      DomainObjectCollection oppositeDomainObjects,
      RelationEndPoint oldEndPoint, 
      RelationEndPoint newEndPoint, 
      int replaceIndex)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);

    return new CollectionEndPointChangeWorker (
        oppositeDomainObjects, oldEndPoint, newEndPoint, OperationType.Replace, replaceIndex);
  }

  // member fields

  private DomainObjectCollection _oppositeDomainObjects;
  private OperationType _operation;
  private RelationEndPoint _oldEndPoint;
  private DomainObject _oldRelatedObject;
  private RelationEndPoint _newEndPoint;
  private int _collectionIndex;

  // construction and disposing

  protected CollectionEndPointChangeWorker (
      DomainObjectCollection oppositeDomainObjects,
      RelationEndPoint oldEndPoint, 
      RelationEndPoint newEndPoint,
      OperationType operation,
      int collectionIndex)
  {
    ArgumentUtility.CheckNotNull ("oppositeDomainObjects", oppositeDomainObjects);
    ArgumentUtility.CheckNotNull ("oldEndPoint", oldEndPoint);
    ArgumentUtility.CheckNotNull ("newEndPoint", newEndPoint);
    
    if (!Enum.IsDefined (typeof (OperationType), operation))
      throw new ArgumentException (string.Format ("Invalid operation '{0}' provided.", operation), "operation");

    _oppositeDomainObjects = oppositeDomainObjects;
    _operation = operation;
    _oldEndPoint = oldEndPoint;
    _oldRelatedObject = oldEndPoint.GetDomainObject ();
    _newEndPoint = newEndPoint;
    _collectionIndex = collectionIndex;
  }
    
  // methods and properties

  public bool BeginRelationChange ()
  {
    if (MustRemoveObject)
    {
      if (!_oppositeDomainObjects.BeginRemove (OldRelatedObject))
        return false;
    }

    if (MustAddObject)
    {
      if (!_oppositeDomainObjects.BeginAdd (NewRelatedObject))
        return false;
    }

    return true;
  }

  public void PerformRelationChange ()
  {
    switch (_operation)
    {
      case OperationType.Remove:
        _oppositeDomainObjects.PerformRemove (OldRelatedObject);
        break;

      case OperationType.Add:
        _oppositeDomainObjects.PerformAdd (NewRelatedObject);
        break;

      case OperationType.Insert:
        _oppositeDomainObjects.PerformInsert (CollectionIndex, NewRelatedObject);
        break;

      case OperationType.Replace:
        _oppositeDomainObjects.PerformRemove (OldRelatedObject);
        _oppositeDomainObjects.PerformInsert (CollectionIndex, NewRelatedObject);
        break;
    }
  }

  public void EndRelationChange ()
  {
    if (MustRemoveObject)
      _oppositeDomainObjects.EndRemove (OldRelatedObject);

    if (MustAddObject)
      _oppositeDomainObjects.EndAdd (NewRelatedObject);
  }

  protected bool MustRemoveObject
  {
    get 
    { 
      return (_operation == OperationType.Remove 
          || _operation == OperationType.Replace); 
    }
  }

  protected bool MustAddObject
  {
    get 
    { 
      return (_operation == OperationType.Add 
          || _operation == OperationType.Insert
          || _operation == OperationType.Replace); 
    }
  }

  public OperationType Operation
  {
    get { return _operation; }
  }

  public RelationEndPoint OldEndPoint
  {
    get { return _oldEndPoint; }
  }

  public RelationEndPoint NewEndPoint
  {
    get { return _newEndPoint; }
  }

  protected DomainObject OldRelatedObject
  {
    get { return _oldRelatedObject; }
  }

  protected DomainObject NewRelatedObject 
  {
    get { return _newEndPoint.GetDomainObject (); }
  }

  protected int CollectionIndex
  {
    get { return _collectionIndex; }
  }
}
}
