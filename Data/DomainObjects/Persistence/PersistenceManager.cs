using System;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Configuration.Mapping;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class PersistenceManager : IDisposable
{
  // types

  // static members and constants

  // member fields

  private StorageProviderManager _storageProviderManager;

  // construction and disposing

  public PersistenceManager ()
  {
    _storageProviderManager = new StorageProviderManager ();
  }

  #region IDisposable Members

  public void Dispose()
  {
    _storageProviderManager.Dispose ();
  }

  #endregion

  // methods and properties

  public DataContainer CreateNewDataContainer (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    StorageProvider provider = _storageProviderManager.GetMandatoryStorageProvider (classDefinition.StorageProviderID);
    return provider.CreateNewDataContainer (classDefinition);
  }

  public void Save (DataContainerCollection dataContainers)
  {
    if (dataContainers.Count == 0)
      return;

    string storageProviderID = dataContainers[0].ClassDefinition.StorageProviderID;
    foreach (DataContainer dataContainer in dataContainers)
    {
      if (dataContainer.ClassDefinition.StorageProviderID != storageProviderID)
        throw CreatePersistenceException ("Save does not support multiple storage providers.");
    }

    StorageProvider provider = _storageProviderManager.GetMandatoryStorageProvider (storageProviderID);

    provider.BeginTransaction ();
    provider.Save (dataContainers);
    provider.SetTimestamp (dataContainers);
    provider.Commit ();
  }

  public DataContainer LoadDataContainer (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    StorageProvider provider = _storageProviderManager.GetMandatoryStorageProvider (id.StorageProviderID);
    DataContainer dataContainer = provider.LoadDataContainer (id);

    if (dataContainer == null)
      throw new ObjectNotFoundException (id);

    return dataContainer;
  }

  public DataContainerCollection LoadRelatedDataContainers (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    if (!relationEndPoint.IsVirtual)
    {
      throw CreatePersistenceException (
          "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point,"
          + " relation: '{0}', property: '{1}'. Check your mapping configuration.",
          relationEndPoint.RelationDefinition.ID, relationEndPoint.PropertyName);
    }

    if (relationEndPoint.Definition.Cardinality == CardinalityType.One)
    {
      throw CreatePersistenceException (
          "Cannot load multiple related data containers for one-to-one relation '{0}'.",
          relationEndPoint.RelationDefinition.ID);
    }

    IRelationEndPointDefinition oppositeEndPointDefinition = relationEndPoint.OppositeEndPointDefinition;

    StorageProvider oppositeProvider = _storageProviderManager.GetMandatoryStorageProvider (
        oppositeEndPointDefinition.ClassDefinition.StorageProviderID);

    DataContainerCollection dataContainers = oppositeProvider.LoadDataContainersByRelatedID (
        oppositeEndPointDefinition.ClassDefinition,
        oppositeEndPointDefinition.PropertyName,
        relationEndPoint.ObjectID);

    if (relationEndPoint.Definition.IsMandatory && dataContainers.Count == 0)
    {
      throw CreatePersistenceException (
          "Collection for mandatory relation '{0}' (property: '{1}') contains no elements.", 
          relationEndPoint.RelationDefinition.ID, relationEndPoint.PropertyName);
    }

    return dataContainers;
  }

  public DataContainer LoadRelatedDataContainer (RelationEndPoint relationEndPoint)
  {
    ArgumentUtility.CheckNotNull ("relationEndPoint", relationEndPoint);
    CheckRelationEndPoint (relationEndPoint, "relationEndPoint");

    if (!relationEndPoint.IsVirtual)
      return GetOppositeDataContainerForEndPoint (relationEndPoint);

    return GetOppositeDataContainerForVirtualEndPoint (relationEndPoint);
  }


  private void CheckRelationEndPoint (RelationEndPoint relationEndPoint, string argumentName)  
  {
    if (relationEndPoint.IsNull)
      throw new ArgumentNullException ("argumentName", "End point cannot be null."); 
  }

  private DataContainer GetOppositeDataContainerForVirtualEndPoint (RelationEndPoint relationEndPoint)
  {
    if (relationEndPoint.Definition.Cardinality == CardinalityType.Many)
    {
      throw CreatePersistenceException (
          "Cannot load a single related data container for one-to-many relation '{0}'.", 
          relationEndPoint.RelationDefinition.ID);
    }

    StorageProvider oppositeProvider = _storageProviderManager.GetMandatoryStorageProvider (
        relationEndPoint.OppositeEndPointDefinition.ClassDefinition.StorageProviderID);

    DataContainerCollection oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID (
        relationEndPoint.OppositeEndPointDefinition.ClassDefinition, 
        relationEndPoint.OppositeEndPointDefinition.PropertyName, 
        relationEndPoint.ObjectID);

    if (oppositeDataContainers.Count == 0)
      return GetNullDataContainerWithRelationCheck (relationEndPoint);
    else
      return oppositeDataContainers[0];
  }

  private DataContainer GetOppositeDataContainerForEndPoint (RelationEndPoint relationEndPoint)
  {
    IRelationEndPointDefinition endPointDefinition = relationEndPoint.Definition;

    ObjectID id = relationEndPoint.DataContainer.GetObjectID (endPointDefinition.PropertyName);
    if (id == null)
      return GetNullDataContainerWithRelationCheck (relationEndPoint);

    StorageProvider oppositeProvider = _storageProviderManager.GetMandatoryStorageProvider (id.StorageProviderID);
    DataContainer oppositeDataContainer = oppositeProvider.LoadDataContainer (id);
    if (oppositeDataContainer == null)
    {
      throw CreatePersistenceException (
          "Property '{0}' of class '{1}' refers to non-existing object with ID '{2}'.",
          endPointDefinition.PropertyName,
          endPointDefinition.ClassDefinition.ID, 
          id.Value);
    }

    return oppositeDataContainer;
  }

  private DataContainer GetNullDataContainerWithRelationCheck (RelationEndPoint relationEndPoint)
  {
    if (!relationEndPoint.Definition.IsMandatory)
      return null;

    throw CreatePersistenceException (
        "Cannot load related DataContainer of object '{0}' over mandatory relation '{1}'.",
        relationEndPoint.DataContainer.ID, relationEndPoint.RelationDefinition.ID);     
  }

  private PersistenceException CreatePersistenceException (string message, params object[] args)
  {
    return new PersistenceException (string.Format (message, args));
  }
}
}
