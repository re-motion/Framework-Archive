using System;

using Rubicon.Data.DomainObjects.Relations;
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

  public DataContainerCollection LoadRelatedDataContainers (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    if (!objectEndPoint.IsVirtual)
    {
      throw CreatePersistenceException (
          "A DataContainerCollection cannot be loaded for a relation with a non-virtual end point,"
          + " relation: '{0}', property: '{1}'. Check your mapping configuration.",
          objectEndPoint.RelationDefinition.ID, objectEndPoint.PropertyName);
    }

    if (objectEndPoint.Definition.Cardinality == CardinalityType.One)
    {
      throw CreatePersistenceException (
          "Cannot load multiple related data containers for one-to-one relation '{0}'.",
          objectEndPoint.RelationDefinition.ID);
    }

    IRelationEndPointDefinition oppositeEndPointDefinition = objectEndPoint.OppositeEndPointDefinition;

    StorageProvider oppositeProvider = _storageProviderManager.GetMandatoryStorageProvider (
        oppositeEndPointDefinition.ClassDefinition.StorageProviderID);

    DataContainerCollection dataContainers = oppositeProvider.LoadDataContainersByRelatedID (
        oppositeEndPointDefinition.ClassDefinition,
        oppositeEndPointDefinition.PropertyName,
        objectEndPoint.ObjectID);

    if (objectEndPoint.Definition.IsMandatory && dataContainers.Count == 0)
    {
      throw CreatePersistenceException (
          "Collection for mandatory relation '{0}' (property: '{1}') contains no elements.", 
          objectEndPoint.RelationDefinition.ID, objectEndPoint.PropertyName);
    }

    return dataContainers;
  }

  public DataContainer LoadRelatedDataContainer (ObjectEndPoint objectEndPoint)
  {
    ArgumentUtility.CheckNotNull ("objectEndPoint", objectEndPoint);
    CheckRelationEndPoint (objectEndPoint, "objectEndPoint");

    if (!objectEndPoint.IsVirtual)
      return GetOppositeDataContainerForEndPoint (objectEndPoint);

    return GetOppositeDataContainerForVirtualEndPoint (objectEndPoint);
  }


  private void CheckRelationEndPoint (ObjectEndPoint objectEndPoint, string argumentName)  
  {
    if (objectEndPoint.IsNull)
      throw new ArgumentNullException ("argumentName", "End point cannot be null."); 
  }

  private DataContainer GetOppositeDataContainerForVirtualEndPoint (ObjectEndPoint objectEndPoint)
  {
    if (objectEndPoint.Definition.Cardinality == CardinalityType.Many)
    {
      throw CreatePersistenceException (
          "Cannot load a single related data container for one-to-many relation '{0}'.", 
          objectEndPoint.RelationDefinition.ID);
    }

    StorageProvider oppositeProvider = _storageProviderManager.GetMandatoryStorageProvider (
        objectEndPoint.OppositeEndPointDefinition.ClassDefinition.StorageProviderID);

    DataContainerCollection oppositeDataContainers = oppositeProvider.LoadDataContainersByRelatedID (
        objectEndPoint.OppositeEndPointDefinition.ClassDefinition, 
        objectEndPoint.OppositeEndPointDefinition.PropertyName, 
        objectEndPoint.ObjectID);

    if (oppositeDataContainers.Count == 0)
      return GetNullDataContainerWithRelationCheck (objectEndPoint);
    else
      return oppositeDataContainers[0];
  }

  private DataContainer GetOppositeDataContainerForEndPoint (ObjectEndPoint objectEndPoint)
  {
    IRelationEndPointDefinition endPointDefinition = objectEndPoint.Definition;

    ObjectID id = objectEndPoint.DataContainer.GetObjectID (endPointDefinition.PropertyName);
    if (id == null)
      return GetNullDataContainerWithRelationCheck (objectEndPoint);

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

  private DataContainer GetNullDataContainerWithRelationCheck (ObjectEndPoint objectEndPoint)
  {
    if (!objectEndPoint.Definition.IsMandatory)
      return null;

    throw CreatePersistenceException (
        "Cannot load related DataContainer of object '{0}' over mandatory relation '{1}'.",
        objectEndPoint.DataContainer.ID, objectEndPoint.RelationDefinition.ID);     
  }

  private PersistenceException CreatePersistenceException (string message, params object[] args)
  {
    return new PersistenceException (string.Format (message, args));
  }
}
}
