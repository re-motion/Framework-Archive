using System;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Persistence;
using Rubicon.Data.DomainObjects.Relations;

namespace Rubicon.Data.DomainObjects
{
public class ClientTransaction : ILinkChangeDelegate, IDisposable
{
  // types

  // static members and constants

  [ThreadStatic]
  private static ClientTransaction s_clientTransaction;

  public static ClientTransaction Current
  {
    get 
    {
      if (s_clientTransaction == null)
        s_clientTransaction = new ClientTransaction ();
      
      return s_clientTransaction;
    }
  }

  public static void SetCurrent (ClientTransaction clientTransaction)
  {
    if (s_clientTransaction != null)
      s_clientTransaction.Dispose ();

    s_clientTransaction = clientTransaction;
  }

  // member fields

  public event LoadedEventHandler Loaded;
  public event CommittedEventHandler Committed;
  
  private DataManager _dataManager;
  private PersistenceManager _persistenceManager;

  // construction and disposing

  protected ClientTransaction ()
  {
    Initialize ();
  }

  #region IDisposable Members

  public void Dispose ()
  {
    _persistenceManager.Dispose ();
  }

  #endregion

  // methods and properties

  public void Commit ()
  {
    DomainObjectCollection changedDomainObjects = _dataManager.GetChangedDomainObjects ();
    DataContainerCollection changedDataContainers = new DataContainerCollection ();

    foreach (DomainObject domainObject in changedDomainObjects)
    {
      CheckMandatoryRelations (domainObject);

      if (domainObject.DataContainer.State != StateType.Original)
        changedDataContainers.Add (domainObject.DataContainer);
    }

    _persistenceManager.Save (changedDataContainers);
    _dataManager.Commit ();
    OnCommitted (new CommittedEventArgs (new DomainObjectCollection (changedDomainObjects, true)));
  }

  public void Rollback ()
  {
    _dataManager.Rollback ();
  }

  internal DataContainer CreateNewDataContainer (Type type)
  {
    ArgumentUtility.CheckNotNull ("type", type);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions[type];
    DataContainer newDataContainer = _persistenceManager.CreateNewDataContainer (classDefinition); 
    _dataManager.RegisterNewDataContainer (newDataContainer);

    return newDataContainer;
  }

  internal protected void Delete (DomainObject domainObject)
  {
    ArgumentUtility.CheckNotNull ("domainObject", domainObject);

    // TODO: Move BeginDelete and EndDelete to DomainObject!!!

    RelationEndPointList oppositeEndPoints = domainObject.GetOppositeRelationEndPoints ();
    if (BeginDelete (domainObject, oppositeEndPoints))
    {
      /*
      foreach (RelationEndPoint relationEndPoint in domainObject.DataContainer.RelationEndPoints)
      {
        if (relationEndPoint.Definition.Cardinality == CardinalityType.One)
        {
          // TODO: opposite object can be null!
          DomainObject oppositeDomainObject = GetRelatedObject (relationEndPoint);
          RelationEndPoint oppositeEndPoint = new RelationEndPoint (oppositeDomainObject, relationEndPoint.OppositeEndPointDefinition);

          _dataManager.WriteAssociatedPropertiesForRelationChange (
              relationEndPoint, 
              new NullRelationEndPoint (oppositeEndPoint.Definition), 
              oppositeEndPoint, 
              new NullRelationEndPoint (relationEndPoint.Definition));

          _dataManager.ChangeLinks (
            relationEndPoint, 
              new NullRelationEndPoint (oppositeEndPoint.Definition), 
              oppositeEndPoint, 
              new NullRelationEndPoint (relationEndPoint.Definition));    
        }
        else
        {
          // TODO: visit every domain object of opposite collection        
        }
      }
      */

      EndDelete (domainObject, oppositeEndPoints);
    }
  }

  private bool BeginDelete (DomainObject domainObject, RelationEndPointList oppositeEndPoints)
  {
    if (!domainObject.BeginDelete ())
      return false;

    // TODO: Move code below to RelationEndPointList
    foreach (RelationEndPoint oppositeEndPoint in oppositeEndPoints)
    {
      IRelationEndPointDefinition objectEndPointDefinition = 
          oppositeEndPoint.Definition.ClassDefinition.GetOppositeEndPointDefinition (oppositeEndPoint.PropertyName);

      ObjectEndPoint oldObjectEndPoint = new ObjectEndPoint (domainObject, objectEndPointDefinition);
      ObjectEndPoint newObjectEndPoint = new NullObjectEndPoint (objectEndPointDefinition);

      if (!oppositeEndPoint.BeginRelationChange (oldObjectEndPoint, newObjectEndPoint))
        return false;
    }
    return true;
  }

  private void EndDelete (DomainObject domainObject, RelationEndPointList oppositeEndPoints)
  {
    domainObject.EndDelete ();

    // TODO: Move code below to RelationEndPointList

    foreach (RelationEndPoint oppositeEndPoint in oppositeEndPoints)
    {
      IRelationEndPointDefinition objectEndPointDefinition = 
        oppositeEndPoint.Definition.ClassDefinition.GetOppositeEndPointDefinition (oppositeEndPoint.PropertyName);

      ObjectEndPoint oldObjectEndPoint = new ObjectEndPoint (domainObject, objectEndPointDefinition);
      ObjectEndPoint newObjectEndPoint = new NullObjectEndPoint (objectEndPointDefinition);

      oppositeEndPoint.EndRelationChange ();
    }
  }

  internal protected DomainObject GetObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = _dataManager.GetObject (id);
    if (dataContainer != null)
      return dataContainer.DomainObject;

    return LoadObject (id);
  }

  internal protected DomainObject GetRelatedObject (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    SingleObjectRelationLink relationLink = _dataManager.GetSingleObjectRelationLink (objectEndPoint);
    if (relationLink == null)
      return LoadRelatedObject (objectEndPoint);
    
    if (relationLink.DestinationObjectID != null)
      return GetObject (relationLink.DestinationObjectID);

    return null;
  }

  internal protected DomainObject GetOriginalRelatedObject (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    SingleObjectRelationLink relationLink = _dataManager.GetSingleObjectRelationLink (objectEndPoint);
    if (relationLink == null)
      return LoadRelatedObject (objectEndPoint);

    if (relationLink.OriginalDestinationObjectID != null)
      return GetObject (relationLink.OriginalDestinationObjectID);

    return null;
  }

  internal protected DomainObjectCollection GetRelatedObjects (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    DomainObjectCollection domainObjects = _dataManager.GetRelatedObjects (objectEndPoint);
    if (domainObjects != null)
      return domainObjects;

    DataContainerCollection relatedDataContainers = _persistenceManager.LoadRelatedDataContainers (objectEndPoint);
    DataContainerCollection newLoadedDataContainers = _dataManager.GetNotExisting (relatedDataContainers);
    _dataManager.Register (newLoadedDataContainers);

    domainObjects = GetDomainObjects (objectEndPoint, relatedDataContainers);

    _dataManager.RegisterInMultipleObjectsRelationLinkMap (objectEndPoint, domainObjects);

    foreach (DataContainer loadedDataContainer in newLoadedDataContainers)
      OnLoaded (new LoadedEventArgs (loadedDataContainer.DomainObject));

    return domainObjects;
  }

  internal protected DomainObjectCollection GetOriginalRelatedObjects (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    MultipleObjectsRelationLink relationLink = _dataManager.GetMultipleObjectsRelationLink (objectEndPoint);

    if (relationLink != null)
      return relationLink.OriginalDestinationDomainObjects;
    else
      return GetRelatedObjects (objectEndPoint); 
  }  

  internal protected void SetRelatedObject (ObjectEndPoint objectEndPoint, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    ObjectEndPoint newRelatedEndPoint = CreateRelationEndPoint (newRelatedObject, objectEndPoint.OppositeEndPointDefinition);

    ObjectEndPoint oldRelatedEndPoint = 
        CreateRelationEndPoint (GetRelatedObject (objectEndPoint), newRelatedEndPoint.Definition);

    if (object.ReferenceEquals (newRelatedEndPoint.DomainObject, oldRelatedEndPoint.DomainObject))
      return;

    if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
      SetRelatedObjectForOneToOneRelation (objectEndPoint, newRelatedEndPoint, oldRelatedEndPoint);
    else
      SetRelatedObjectForOneToManyRelation (objectEndPoint, newRelatedEndPoint, oldRelatedEndPoint);
  }

  protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);
    _dataManager.Register (dataContainer);

    OnLoaded (new LoadedEventArgs (dataContainer.DomainObject));

    return dataContainer.DomainObject;
  }

  protected virtual DomainObject LoadRelatedObject (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (objectEndPoint);
    if (relatedDataContainer != null)
    {
      _dataManager.Register (relatedDataContainer);
      OnLoaded (new LoadedEventArgs (relatedDataContainer.DomainObject));
      return relatedDataContainer.DomainObject;
    }
    else
    {
      _dataManager.RegisterInSingleObjectRelationLinkMap (objectEndPoint, null);
      return null;
    }
  }

  protected virtual void OnLoaded (LoadedEventArgs args)
  {
    args.LoadedDomainObject.EndObjectLoading ();

    if (Loaded != null)
      Loaded (this, args);
  }

  protected virtual void OnCommitted (CommittedEventArgs args)
  {
    foreach (DomainObject domainObject in args.CommittedDomainObjects)
      domainObject.EndCommit ();

    if (Committed != null)
      Committed (this, args);
  }
  private void Initialize ()
  {
    _dataManager = new DataManager ();
    _dataManager.LinkChangeDelegate = this;
    _persistenceManager = new PersistenceManager ();
  }

  private void CheckRelationEndPoint (ObjectEndPoint objectEndPoint, string argumentName)  
  {
    if (objectEndPoint.IsNull)
      throw new ArgumentNullException ("argumentName", "End point cannot be null."); 
  }

  private void SetRelatedObjectForOneToOneRelation (
      ObjectEndPoint objectEndPoint, 
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint)
  {
    ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint = new NullObjectEndPoint (objectEndPoint.Definition);
    if (!newRelatedEndPoint.IsNull)
    {
      oldRelatedEndPointOfNewRelatedEndPoint = CreateRelationEndPoint (
          GetRelatedObject (newRelatedEndPoint), objectEndPoint.Definition);
    }

    if (BeginRelationChange (objectEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint))
    {
      _dataManager.WriteAssociatedPropertiesForRelationChange (
          objectEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

      _dataManager.ChangeLinks (objectEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

      EndRelationChange (
          objectEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);
    }
  }

  private void SetRelatedObjectForOneToManyRelation (
      ObjectEndPoint relationEndPoint, 
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint)
  {
    if (!newRelatedEndPoint.IsNull)
    {
      DomainObjectCollection collection = GetRelatedObjects (newRelatedEndPoint);
      collection.Add (relationEndPoint.DomainObject);
    }
    else
    {
      DomainObjectCollection collection = GetRelatedObjects (oldRelatedEndPoint);
      collection.Remove (relationEndPoint.DomainObject);
    }
  }

  private ObjectEndPoint CreateRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
  {
    if (domainObject != null)
      return new ObjectEndPoint (domainObject, definition);
    else
      return new NullObjectEndPoint (definition);
  }

  private bool BeginRelationChange (
      ObjectEndPoint relationEndPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    return relationEndPoint.BeginRelationChange (oldRelatedEndPoint, newRelatedEndPoint)
        && oldRelatedEndPoint.BeginRelationChange (relationEndPoint, new NullObjectEndPoint (relationEndPoint.Definition))
        && newRelatedEndPoint.BeginRelationChange (oldRelatedEndPointOfNewRelatedEndPoint, relationEndPoint)
        && oldRelatedEndPointOfNewRelatedEndPoint.BeginRelationChange (newRelatedEndPoint, new NullObjectEndPoint (newRelatedEndPoint.Definition));
  }

  private void EndRelationChange (
      ObjectEndPoint relationEndPoint,
      ObjectEndPoint newRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPoint,
      ObjectEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    relationEndPoint.EndRelationChange ();
    oldRelatedEndPoint.EndRelationChange ();
    newRelatedEndPoint.EndRelationChange ();
    oldRelatedEndPointOfNewRelatedEndPoint.EndRelationChange ();
  }

  private DomainObjectCollection GetDomainObjects (
      RelationEndPoint relationEndPoint,
      DataContainerCollection relatedDataContainers)
  {
    return DomainObjectCollection.Create (
        relationEndPoint.Definition.PropertyType,
        _dataManager.MergeWithExisting (relatedDataContainers));
  }

  private void CheckMandatoryRelations (DomainObject domainObject)
  {
    foreach (ObjectEndPoint objectEndPoint in domainObject.DataContainer.ObjectEndPoints)
    {
      if (objectEndPoint.Definition.IsMandatory)
      {
        RelationLink link = _dataManager.GetRelationLink (objectEndPoint);
        if (link != null)
          link.CheckMandatory ();
      }
    }
  }
  internal protected DataManager DataManager
  {
    get { return _dataManager; }
  }

  #region ILinkChangeDelegate Members

  void ILinkChangeDelegate.PerformAdd  (MultipleObjectsRelationLink link, DomainObject domainObject)
  {
    ObjectEndPoint relationEndPoint = new ObjectEndPoint (GetObject (link.ID.ObjectID), link.ID.PropertyName);

    ObjectEndPoint addingEndPoint = new ObjectEndPoint (
        domainObject, relationEndPoint.OppositeEndPointDefinition);

    ObjectEndPoint oldRelatedEndPoint = CreateRelationEndPoint (
        GetRelatedObject (addingEndPoint), relationEndPoint.Definition);

    DomainObjectCollection oldCollection = null;
    if (!oldRelatedEndPoint.IsNull)
      oldCollection = GetRelatedObjects (oldRelatedEndPoint);
    
    if (addingEndPoint.BeginRelationChange (oldRelatedEndPoint, relationEndPoint)
        && BeginRemove (oldRelatedEndPoint, oldCollection, domainObject)
        && link.DestinationDomainObjects.BeginAdd (domainObject)
        && relationEndPoint.BeginRelationChange (new NullObjectEndPoint (addingEndPoint.Definition), addingEndPoint))
    {
      addingEndPoint.SetOppositeEndPoint (relationEndPoint);
      _dataManager.ChangeLink (addingEndPoint, relationEndPoint.DomainObject);
      link.DestinationDomainObjects.PerformAdd (domainObject);

      if (oldCollection != null)
        oldCollection.PerformRemove (domainObject);

      addingEndPoint.EndRelationChange ();
      EndRemove (oldRelatedEndPoint, oldCollection, domainObject);
      link.DestinationDomainObjects.EndAdd (domainObject);
      relationEndPoint.EndRelationChange ();
    }
  }

  private bool BeginRemove (ObjectEndPoint objectEndPoint, DomainObjectCollection collection, DomainObject domainObject)
  {
    if (objectEndPoint.IsNull)
      return true;
    
    return collection.BeginRemove (domainObject)
        && objectEndPoint.BeginRelationChange (
            new ObjectEndPoint (domainObject, objectEndPoint.OppositeEndPointDefinition), 
            new NullObjectEndPoint (objectEndPoint.OppositeEndPointDefinition));
  }

  private void EndRemove (ObjectEndPoint objectEndPoint, DomainObjectCollection collection, DomainObject domainObject)
  {
    if (objectEndPoint.IsNull)
      return;

    collection.EndRemove (domainObject);
    objectEndPoint.EndRelationChange (); 
  }

  void ILinkChangeDelegate.PerformRemove (MultipleObjectsRelationLink link, DomainObject domainObject)
  {
    ObjectEndPoint objectEndPoint = new ObjectEndPoint (GetObject (link.ID.ObjectID), link.ID.PropertyName);

    ObjectEndPoint removingEndPoint = new ObjectEndPoint (
        domainObject, objectEndPoint.OppositeEndPointDefinition);

    if (removingEndPoint.BeginRelationChange (objectEndPoint, new NullObjectEndPoint (objectEndPoint.Definition))
        && BeginRemove (objectEndPoint, link.DestinationDomainObjects, domainObject))
    {
      removingEndPoint.SetOppositeEndPoint (new NullObjectEndPoint (objectEndPoint.Definition));
      _dataManager.ChangeLink (removingEndPoint, null);
      link.DestinationDomainObjects.PerformRemove (domainObject);

      removingEndPoint.EndRelationChange ();
      EndRemove (objectEndPoint, link.DestinationDomainObjects, domainObject);
    }
  }

  #endregion
}
}