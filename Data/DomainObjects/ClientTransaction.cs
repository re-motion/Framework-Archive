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

    if (BeginDelete (domainObject))
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

      EndDelete (domainObject);
    }
  }

  private bool BeginDelete (DomainObject domainObject)
  {
    if (!domainObject.BeginDelete ())
      return false;

    foreach (RelationEndPoint relationEndPoint in domainObject.DataContainer.RelationEndPoints)
    {
      if (relationEndPoint.Definition.Cardinality == CardinalityType.One)
      {
        DomainObject oppositeDomainObject = GetRelatedObject (relationEndPoint);
        if (oppositeDomainObject != null)
        {
          RelationEndPoint oppositeEndPoint = new RelationEndPoint (
              oppositeDomainObject, relationEndPoint.OppositeEndPointDefinition);

          if (oppositeEndPoint.Definition.Cardinality == CardinalityType.Many)
          {
            DomainObjectCollection oppositeDomainObjectCollection = GetRelatedObjects (oppositeEndPoint);
            if (!oppositeDomainObjectCollection.BeginRemove (domainObject))
              return false;
          }

          if (!oppositeEndPoint.BeginRelationChange (relationEndPoint))
            return false;
        }
      }
      else
      {
        foreach (DomainObject oppositeDomainObject in GetRelatedObjects (relationEndPoint))
        {
          RelationEndPoint oppositeEndPoint = new RelationEndPoint (
              oppositeDomainObject, relationEndPoint.OppositeEndPointDefinition);

          if (!oppositeEndPoint.BeginRelationChange (relationEndPoint))
            return false;
        }
      }
    }

    return true;
  }

  private void EndDelete (DomainObject domainObject)
  {
    domainObject.EndDelete ();

    foreach (RelationEndPoint relationEndPoint in domainObject.DataContainer.RelationEndPoints)
    {
      if (relationEndPoint.Definition.Cardinality == CardinalityType.One)
      {
        DomainObject oppositeDomainObject = GetRelatedObject (relationEndPoint);
        if (oppositeDomainObject != null)
        {
          RelationEndPoint oppositeEndPoint = new RelationEndPoint (oppositeDomainObject, relationEndPoint.OppositeEndPointDefinition);

          if (oppositeEndPoint.Definition.Cardinality == CardinalityType.Many)
          {
            DomainObjectCollection oppositeDomainObjectCollection = GetRelatedObjects (oppositeEndPoint);
            oppositeDomainObjectCollection.EndRemove (domainObject);
          }

          oppositeEndPoint.EndRelationChange ();
        }
      }
      else
      {
        foreach (DomainObject oppositeDomainObject in GetRelatedObjects (relationEndPoint))
        {
          RelationEndPoint oppositeEndPoint = new RelationEndPoint (
              oppositeDomainObject, relationEndPoint.OppositeEndPointDefinition);

          oppositeEndPoint.EndRelationChange ();
        }
      }
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

  internal protected DomainObject GetRelatedObject (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    SingleObjectRelationLink relationLink = _dataManager.GetSingleObjectRelationLink (relationEndPoint);
    if (relationLink == null)
      return LoadRelatedObject (relationEndPoint);
    
    if (relationLink.DestinationObjectID != null)
      return GetObject (relationLink.DestinationObjectID);

    return null;
  }

  internal protected DomainObject GetOriginalRelatedObject (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    SingleObjectRelationLink relationLink = _dataManager.GetSingleObjectRelationLink (relationEndPoint);
    if (relationLink == null)
      return LoadRelatedObject (relationEndPoint);

    if (relationLink.OriginalDestinationObjectID != null)
      return GetObject (relationLink.OriginalDestinationObjectID);

    return null;
  }

  internal protected DomainObjectCollection GetRelatedObjects (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    DomainObjectCollection domainObjects = _dataManager.GetRelatedObjects (relationEndPoint);
    if (domainObjects != null)
      return domainObjects;

    DataContainerCollection relatedDataContainers = _persistenceManager.LoadRelatedDataContainers (relationEndPoint);
    DataContainerCollection newLoadedDataContainers = _dataManager.GetNotExisting (relatedDataContainers);
    _dataManager.Register (newLoadedDataContainers);

    domainObjects = GetDomainObjects (relationEndPoint, relatedDataContainers);

    _dataManager.RegisterInMultipleObjectsRelationLinkMap (relationEndPoint, domainObjects);

    foreach (DataContainer loadedDataContainer in newLoadedDataContainers)
      OnLoaded (new LoadedEventArgs (loadedDataContainer.DomainObject));

    return domainObjects;
  }

  internal protected DomainObjectCollection GetOriginalRelatedObjects (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    MultipleObjectsRelationLink relationLink = _dataManager.GetMultipleObjectsRelationLink (relationEndPoint);

    if (relationLink != null)
      return relationLink.OriginalDestinationDomainObjects;
    else
      return GetRelatedObjects (relationEndPoint); 
  }  

  internal protected void SetRelatedObject (RelationEndPoint relationEndPoint, DomainObject newRelatedObject)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    RelationEndPoint newRelatedEndPoint = CreateRelationEndPoint (newRelatedObject, relationEndPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = 
        CreateRelationEndPoint (GetRelatedObject (relationEndPoint), newRelatedEndPoint.Definition);

    if (object.ReferenceEquals (newRelatedEndPoint.DomainObject, oldRelatedEndPoint.DomainObject))
      return;

    if (newRelatedEndPoint.Definition.Cardinality == CardinalityType.One)
      SetRelatedObjectForOneToOneRelation (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint);
    else
      SetRelatedObjectForOneToManyRelation (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint);
  }

  protected virtual DomainObject LoadObject (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    DataContainer dataContainer = _persistenceManager.LoadDataContainer (id);
    _dataManager.Register (dataContainer);

    OnLoaded (new LoadedEventArgs (dataContainer.DomainObject));

    return dataContainer.DomainObject;
  }

  protected virtual DomainObject LoadRelatedObject (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    DataContainer relatedDataContainer = _persistenceManager.LoadRelatedDataContainer (relationEndPoint);
    if (relatedDataContainer != null)
    {
      _dataManager.Register (relatedDataContainer);
      OnLoaded (new LoadedEventArgs (relatedDataContainer.DomainObject));
      return relatedDataContainer.DomainObject;
    }
    else
    {
      _dataManager.RegisterInSingleObjectRelationLinkMap (relationEndPoint, null);
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

  private void CheckRelationEndPoint (RelationEndPoint relationEndPoint, string argumentName)  
  {
    if (relationEndPoint.IsNull)
      throw new ArgumentNullException ("argumentName", "End point cannot be null."); 
  }

  private void SetRelatedObjectForOneToOneRelation (
      RelationEndPoint relationEndPoint, 
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint)
  {
    RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint = new NullRelationEndPoint (relationEndPoint.Definition);
    if (!newRelatedEndPoint.IsNull)
    {
      oldRelatedEndPointOfNewRelatedEndPoint = CreateRelationEndPoint (
          GetRelatedObject (newRelatedEndPoint), relationEndPoint.Definition);
    }

    if (BeginRelationChange (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint))
    {
      _dataManager.WriteAssociatedPropertiesForRelationChange (
          relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

      _dataManager.ChangeLinks (relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);

      EndRelationChange (
          relationEndPoint, newRelatedEndPoint, oldRelatedEndPoint, oldRelatedEndPointOfNewRelatedEndPoint);
    }
  }

  private void SetRelatedObjectForOneToManyRelation (
      RelationEndPoint relationEndPoint, 
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint)
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

  private RelationEndPoint CreateRelationEndPoint (DomainObject domainObject, IRelationEndPointDefinition definition)
  {
    if (domainObject != null)
      return new RelationEndPoint (domainObject, definition);
    else
      return new NullRelationEndPoint (definition);
  }

  private bool BeginRelationChange (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
  {
    return relationEndPoint.BeginRelationChange (oldRelatedEndPoint, newRelatedEndPoint)
        && oldRelatedEndPoint.BeginRelationChange (relationEndPoint, new NullRelationEndPoint (relationEndPoint.Definition))
        && newRelatedEndPoint.BeginRelationChange (oldRelatedEndPointOfNewRelatedEndPoint, relationEndPoint)
        && oldRelatedEndPointOfNewRelatedEndPoint.BeginRelationChange (newRelatedEndPoint, new NullRelationEndPoint (newRelatedEndPoint.Definition));
  }

  private void EndRelationChange (
      RelationEndPoint relationEndPoint,
      RelationEndPoint newRelatedEndPoint,
      RelationEndPoint oldRelatedEndPoint,
      RelationEndPoint oldRelatedEndPointOfNewRelatedEndPoint)
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
    foreach (RelationEndPoint relationEndPoint in domainObject.DataContainer.RelationEndPoints)
    {
      if (relationEndPoint.Definition.IsMandatory)
      {
        RelationLink link = _dataManager.GetRelationLink (relationEndPoint);
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
    RelationEndPoint relationEndPoint = new RelationEndPoint (GetObject (link.ID.ObjectID), link.ID.PropertyName);

    RelationEndPoint addingEndPoint = new RelationEndPoint (
        domainObject, relationEndPoint.OppositeEndPointDefinition);

    RelationEndPoint oldRelatedEndPoint = CreateRelationEndPoint (
        GetRelatedObject (addingEndPoint), relationEndPoint.Definition);

    DomainObjectCollection oldCollection = null;
    if (!oldRelatedEndPoint.IsNull)
      oldCollection = GetRelatedObjects (oldRelatedEndPoint);
    
    if (addingEndPoint.BeginRelationChange (oldRelatedEndPoint, relationEndPoint)
        && BeginRemove (oldRelatedEndPoint, oldCollection, domainObject)
        && link.DestinationDomainObjects.BeginAdd (domainObject)
        && relationEndPoint.BeginRelationChange (new NullRelationEndPoint (addingEndPoint.Definition), addingEndPoint))
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

  private bool BeginRemove (RelationEndPoint relationEndPoint, DomainObjectCollection collection, DomainObject domainObject)
  {
    if (relationEndPoint.IsNull)
      return true;
    
    return collection.BeginRemove (domainObject)
        && relationEndPoint.BeginRelationChange (
            new RelationEndPoint (domainObject, relationEndPoint.OppositeEndPointDefinition), 
            new NullRelationEndPoint (relationEndPoint.OppositeEndPointDefinition));
  }

  private void EndRemove (RelationEndPoint relationEndPoint, DomainObjectCollection collection, DomainObject domainObject)
  {
    if (relationEndPoint.IsNull)
      return;

    collection.EndRemove (domainObject);
    relationEndPoint.EndRelationChange (); 
  }

  void ILinkChangeDelegate.PerformRemove (MultipleObjectsRelationLink link, DomainObject domainObject)
  {
    RelationEndPoint relationEndPoint = new RelationEndPoint (GetObject (link.ID.ObjectID), link.ID.PropertyName);

    RelationEndPoint removingEndPoint = new RelationEndPoint (
        domainObject, relationEndPoint.OppositeEndPointDefinition);

    if (removingEndPoint.BeginRelationChange (relationEndPoint, new NullRelationEndPoint (relationEndPoint.Definition))
        && BeginRemove (relationEndPoint, link.DestinationDomainObjects, domainObject))
    {
      removingEndPoint.SetOppositeEndPoint (new NullRelationEndPoint (relationEndPoint.Definition));
      _dataManager.ChangeLink (removingEndPoint, null);
      link.DestinationDomainObjects.PerformRemove (domainObject);

      removingEndPoint.EndRelationChange ();
      EndRemove (relationEndPoint, link.DestinationDomainObjects, domainObject);
    }
  }

  #endregion
}
}