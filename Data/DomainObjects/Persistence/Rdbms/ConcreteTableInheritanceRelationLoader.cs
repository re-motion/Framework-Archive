using System;
using System.Collections.Generic;
using System.Text;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;
using Rubicon.Data.DomainObjects.DataManagement;
using System.Data;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
  public class ConcreteTableInheritanceRelationLoader
  {
    // types

    // static members and constants

    // member fields

    private RdbmsProvider _provider;
    private ClassDefinition _classDefinition;
    private PropertyDefinition _propertyDefinition;
    private ObjectID _relatedID;

    // construction and disposing

    public ConcreteTableInheritanceRelationLoader (
        RdbmsProvider provider,
        ClassDefinition classDefinition,
        PropertyDefinition propertyDefinition,
        ObjectID relatedID)
    {
      ArgumentUtility.CheckNotNull ("provider", provider);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
      ArgumentUtility.CheckNotNull ("relatedID", relatedID);

      _provider = provider;
      _classDefinition = classDefinition;
      _propertyDefinition = propertyDefinition;
      _relatedID = relatedID;
    }

    // methods and properties

    public RdbmsProvider Provider
    {
      get { return _provider; }
    }

    public DataContainerCollection LoadDataContainers ()
    {
      List<ObjectID> objectIDsInCorrectOrder = GetObjectIDsInCorrectOrder ();
      if (objectIDsInCorrectOrder.Count == 0)
        return new DataContainerCollection ();

      Dictionary<string, List<ObjectID>> objectIDsPerEntityName = GetObjectIDsPerEntityName (objectIDsInCorrectOrder);
      DataContainerCollection allDataContainers = new DataContainerCollection ();
      foreach (string entityName in objectIDsPerEntityName.Keys)
        allDataContainers = DataContainerCollection.Join (allDataContainers, GetDataContainers (entityName, objectIDsPerEntityName[entityName]));

      return GetOrderedDataContainers (objectIDsInCorrectOrder, allDataContainers);
    }

    private List<ObjectID> GetObjectIDsInCorrectOrder ()
    {
      UnionSelectCommandBuilder builder = UnionSelectCommandBuilder.CreateForRelatedIDLookup (
          _provider, _classDefinition, _propertyDefinition, _relatedID);

      using (IDbCommand command = builder.Create ())
      {
        if (command == null)
          return new List<ObjectID> ();
           
        using (IDataReader reader = Provider.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          List<ObjectID> objectIDsInCorrectOrder = new List<ObjectID> ();

          ValueConverter valueConverter = new ValueConverter ();
          while (reader.Read ())
          {
            objectIDsInCorrectOrder.Add (valueConverter.GetID (reader));
          }

          return objectIDsInCorrectOrder;
        }
      } 
    }

    private DataContainerCollection GetOrderedDataContainers (List<ObjectID> objectIDsInCorrectOrder, DataContainerCollection unorderedDataContainers)
    {
      DataContainerCollection orderedDataContainers = new DataContainerCollection ();
      foreach (ObjectID objectID in objectIDsInCorrectOrder)
        orderedDataContainers.Add (unorderedDataContainers[objectID]);

      return orderedDataContainers;
    }

    private DataContainerCollection GetDataContainers (string entityName, List<ObjectID> objectIDs)
    {
      SelectCommandBuilder commandBuilder = SelectCommandBuilder.CreateForIDLookup (Provider, entityName, objectIDs.ToArray ());

      using (IDbCommand command = commandBuilder.Create ())
      {
        using (IDataReader reader = Provider.ExecuteReader (command, CommandBehavior.SingleResult))
        {
          DataContainerFactory dataContainerFactory = new DataContainerFactory (Provider, reader);
          return dataContainerFactory.CreateCollection ();
        }
      }
    }

    private Dictionary<string, List<ObjectID>> GetObjectIDsPerEntityName (List<ObjectID> objectIDsInCorrectOrder)
    {
      Dictionary<string, List<ObjectID>> objectIDsPerEntityName = new Dictionary<string, List<ObjectID>> ();
      foreach (ObjectID objectID in objectIDsInCorrectOrder)
      {
        string entityName = objectID.ClassDefinition.GetEntityName ();
        if (!objectIDsPerEntityName.ContainsKey (entityName))
          objectIDsPerEntityName.Add (entityName, new List<ObjectID> ());

        objectIDsPerEntityName[entityName].Add (objectID);
      }
      return objectIDsPerEntityName;
    }
  }
}
