using System;
using System.Data;
using System.Text;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Persistence.Configuration;
using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Data.DomainObjects.Queries.Configuration;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public abstract class RdbmsProvider : StorageProvider
{
  // types

  // static members and constants

  // member fields

  private IDbConnection _connection;
  private IDbTransaction _transaction;

  // construction and disposing

  protected RdbmsProvider (RdbmsProviderDefinition rdbmsProviderDefinition) : base (rdbmsProviderDefinition)
  {
  }

  protected override void Dispose (bool disposing)
  {
    if (!IsDisposed)
    {
      try
      {
        if (disposing)
        {
          DisposeTransaction ();
          DisposeConnection ();
        }
      }
      finally
      {
        base.Dispose (disposing);
      }
    }
  }

  // abstract methods and properties

  public abstract string GetParameterName (string name);
  protected abstract IDbConnection CreateConnection ();

  // methods and properties

  public virtual void Connect ()
  {
    CheckDisposed ();

    if (!IsConnected)
    {
      try
      {
        _connection = CreateConnection ();
        if (_connection.ConnectionString == null || _connection.ConnectionString == string.Empty)
          _connection.ConnectionString = this.StorageProviderDefinition.ConnectionString;

        _connection.Open ();
      }
      catch (Exception e)
      {
        throw CreateStorageProviderException (e, "Error while opening connection.");
      }
    }
  }

  public virtual void Disconnect()
  {
    Dispose ();
  }

  public virtual bool IsConnected
  {
    get 
    {
      if (_connection == null)
        return false;

      return _connection.State != ConnectionState.Closed;
    }
  }

  public override void BeginTransaction ()
  {
    CheckDisposed ();

    Connect ();

    if (_transaction != null)
      throw new InvalidOperationException ("Cannot call BeginTransaction when a transaction is already in progress.");

    try
    {
      _transaction = _connection.BeginTransaction (IsolationLevel.Serializable);
    }
    catch (Exception e)
    {
      throw CreateStorageProviderException (e, "Error while executing BeginTransaction.");
    }
  }

  public override void Commit ()
  {
    CheckDisposed ();

    if (_transaction == null)
    {
      throw new InvalidOperationException (
          "Commit cannot be called without calling BeginTransaction first.");
    }

    try
    {
      _transaction.Commit ();
    }
    catch (Exception e)
    {
      throw CreateStorageProviderException (e, "Error while executing Commit.");
    }
    finally 
    {
      DisposeTransaction ();
    }
  }

  public override void Rollback ()
  {
    CheckDisposed ();

    if (_transaction == null)
    {
      throw new InvalidOperationException (
          "Rollback cannot be called without calling BeginTransaction first.");
    }

    try
    {
      _transaction.Rollback ();
    }
    catch (Exception e)
    {
      throw CreateStorageProviderException (e, "Error while executing Rollback.");
    }
    finally
    {
      DisposeTransaction ();
    }
  }

  public override DataContainerCollection ExecuteCollectionQuery (Query query)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("query", query);
    CheckQuery (query, "query");

    if (query.QueryType == QueryType.Scalar)
      throw CreateArgumentException ("query", "A scalar query cannot be used with ExecuteCollectionQuery.");

    Connect ();

    QueryCommandBuilder commandBuilder = new QueryCommandBuilder (this, query);
    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (command, CommandBehavior.SingleResult))
      {
        DataContainerFactory dataContainerFactory = new DataContainerFactory (reader);
        return dataContainerFactory.CreateCollection ();
      }
    }    
  }

  public override object ExecuteScalarQuery (Query query)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("query", query);
    CheckQuery (query, "query");

    if (query.QueryType == QueryType.Collection)
      throw CreateArgumentException ("query", "A collection query cannot be used with ExecuteScalarQuery.");

    Connect ();

    QueryCommandBuilder commandBuilder = new QueryCommandBuilder (this, query);
    using (IDbCommand command = commandBuilder.Create ())
    {
      try
      {
        return command.ExecuteScalar ();
      }
      catch (Exception e)
      {
        throw CreateStorageProviderException (
            e, "Error while executing SQL command for query '{0}'.", query.QueryID);
      }
    }
  }

  public override DataContainerCollection LoadDataContainersByRelatedID (
      ClassDefinition classDefinition, 
      string propertyName, 
      ObjectID relatedID)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("relatedID", relatedID);
    CheckClassDefinition (classDefinition, "classDefinition");
    CheckObjectIDValue (relatedID, "relatedID");

    Connect ();

    PropertyDefinition property = classDefinition.GetPropertyDefinition (propertyName);
    if (property == null)
    {
      throw CreateStorageProviderException ("Class '{0}' does not contain property '{1}'.",
          classDefinition.ID, propertyName);
    }

    VirtualRelationEndPointDefinition oppositeRelationEndPointDefinition = 
        (VirtualRelationEndPointDefinition) classDefinition.GetOppositeEndPointDefinition (property.PropertyName);

    SelectCommandBuilder commandBuilder = new SelectCommandBuilder (
        this, classDefinition, property, relatedID, oppositeRelationEndPointDefinition.SortExpression);

    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (command, CommandBehavior.SingleResult))
      {
        DataContainerFactory dataContainerFactory = new DataContainerFactory (reader);
        return dataContainerFactory.CreateCollection ();
      }
    }
  }

  public override DataContainer LoadDataContainer (ObjectID id)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("id", id);
    CheckStorageProviderID (id, "id");
    CheckObjectIDValue (id, "id");

    Connect();

    SelectCommandBuilder commandBuilder = new SelectCommandBuilder (this, id.ClassDefinition, "ID", id.Value);
    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (command, CommandBehavior.SingleRow))
      {
        DataContainerFactory dataContainerFactory = new DataContainerFactory (reader);
        return dataContainerFactory.CreateDataContainer ();
      }
    }
  }

  public override void Save (DataContainerCollection dataContainers)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    Connect ();

    foreach (DataContainer dataContainer in dataContainers.GetByState (StateType.New))
    {
      CommandBuilder commandBuilder = new InsertCommandBuilder (this, dataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Save (command, dataContainer.ID);
      }
    }

    foreach (DataContainer dataContainer in dataContainers)
    {
      if (dataContainer.State != StateType.Unchanged)
      {
        CommandBuilder commandBuilder = new UpdateCommandBuilder (this, dataContainer);
        using (IDbCommand command = commandBuilder.Create ())
        {
          Save (command, dataContainer.ID);
        }
      }
    }

    foreach (DataContainer dataContainer in dataContainers.GetByState (StateType.Deleted))
    {
      CommandBuilder commandBuilder = new DeleteCommandBuilder (this, dataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Save (command, dataContainer.ID);
      }
    }
  }

  public override void SetTimestamp (DataContainerCollection dataContainers)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    Connect ();

    foreach (DataContainer dataContainer in dataContainers)
    {
      if (dataContainer.State != StateType.Deleted)
        SetTimestamp (dataContainer);
    }
  }

  public override DataContainer CreateNewDataContainer (ClassDefinition classDefinition)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    CheckClassDefinition (classDefinition, "classDefinition");

    return DataContainer.CreateNew (CreateNewObjectID (classDefinition));
  }
  
  public IDbConnection Connection
  {
    get 
    { 
      CheckDisposed ();
      return _connection; 
    }
  }

  public IDbTransaction Transaction
  {
    get 
    { 
      CheckDisposed ();
      return _transaction; 
    }
  }

  protected virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    CheckClassDefinition (classDefinition, "classDefinition");

    return new ObjectID (classDefinition.ID, Guid.NewGuid ());
  }

  protected virtual IDataReader ExecuteReader (IDbCommand command, CommandBehavior behavior)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckValidEnumValue (behavior, "behavior");

    try
    {
      return command.ExecuteReader (behavior);
    }
    catch (Exception e)
    {
      throw CreateStorageProviderException (e, "Error while executing SQL command.");
    }
  }
  
  protected virtual void SetTimestamp (DataContainer dataContainer)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    if (dataContainer.State == StateType.Deleted)
      throw CreateArgumentException ("dataContainer", "Timestamp cannot be set for a deleted DataContainer.");

    SelectCommandBuilder commandBuilder = new SelectCommandBuilder (
        this, "Timestamp", dataContainer.ClassDefinition, "ID", dataContainer.ID.Value);

    using (IDbCommand command = commandBuilder.Create ())
    {
      try
      {
        dataContainer.SetTimestamp (command.ExecuteScalar ());
      }
      catch (Exception e)
      {
        throw CreateStorageProviderException (e, "Error while setting timestamp for object '{0}'.", dataContainer.ID);
      }
    }
  }

  protected virtual void Save (IDbCommand command, ObjectID id)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("id", id);
    CheckStorageProviderID (id, "id");
    CheckObjectIDValue (id, "id");

    if (command == null)
      return;

    int recordsAffected = 0;
    try
    {
      recordsAffected = command.ExecuteNonQuery ();
    }
    catch (Exception e)
    {
      throw CreateStorageProviderException (e, "Error while saving object '{0}'.", id);
    }

    if (recordsAffected != 1)
    {
      throw CreateConcurrencyViolationException (
          "Concurrency violation encountered. Object '{0}' has already been changed by someone else.", id);
    }
  }

  protected virtual bool ValidateObjectID (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    if (id.StorageProviderID == ID && id.Value != null && id.Value.GetType () != typeof (System.Guid))
      return false;

    return true;
  }

  protected new RdbmsProviderDefinition StorageProviderDefinition
  {
    get 
    { 
      CheckDisposed ();
      return (RdbmsProviderDefinition) base.StorageProviderDefinition; 
    }
  }

  protected StorageProviderException CreateStorageProviderException (
      string formatString,
      params object[] args)
  {
    return CreateStorageProviderException (null, formatString, args);
  }

  protected StorageProviderException CreateStorageProviderException (
      Exception innerException,
      string formatString,
      params object[] args)
  {
    return new StorageProviderException (string.Format (formatString, args), innerException);
  }


  protected ConcurrencyViolationException CreateConcurrencyViolationException (
      string formatString,
      params object[] args)
  {
    return CreateConcurrencyViolationException (null, formatString, args);
  }

  protected ConcurrencyViolationException CreateConcurrencyViolationException (
      Exception innerException,
      string formatString,
      params object[] args)
  {
    return new ConcurrencyViolationException (string.Format (formatString, args), innerException);
  }

  protected ArgumentException CreateArgumentException (string argumentName, string formatString, params object[] args)
  {
    return new ArgumentException (string.Format (formatString, args), argumentName);
  }

  private void DisposeTransaction ()
  {
    if (_transaction != null)
      _transaction.Dispose ();

    _transaction = null;
  }

  private void DisposeConnection ()
  {
    if (_connection != null)
      _connection.Close ();
    
    _connection = null;
  }

  private void CheckQuery (Query query, string argumentName)
  {
    if (query.StorageProviderID != ID)
    {
      throw CreateArgumentException (
          "query", 
          "The StorageProviderID '{0}' of the provided query '{1}' does not match with this StorageProvider's ID '{2}'.",
          query.StorageProviderID, 
          query.QueryID,
          ID);
    }

    foreach (QueryParameter parameter in query.Parameters)
    {
      if (parameter.Value != null && parameter.Value.GetType () == typeof (ObjectID))
      {
        ObjectID id = (ObjectID) parameter.Value;
        if (!ValidateObjectID (id))
        {
          throw CreateArgumentException (
              argumentName,
              "The query parameter '{0}' is of type 'Rubicon.Data.DomainObjects.ObjectID'."
                  + " The value of this parameter is of type '{1}', but only 'System.Guid' is supported.",
              parameter.Name,
              id.Value.GetType ());
              
        }
      }
    }
  }

  private void CheckStorageProviderID (ObjectID id, string argumentName)
  {
    if (id.StorageProviderID != ID)
    {
      throw CreateArgumentException (
          argumentName,
          "The StorageProviderID '{0}' of the provided ObjectID does not match with this StorageProvider's ID '{1}'.",
          id.StorageProviderID,
          ID);
    }

    CheckObjectIDValue (id, argumentName);
  }

  private void CheckObjectIDValue (ObjectID id, string argumentName)
  {
    if (!ValidateObjectID (id))
    {
      throw CreateArgumentException (
          argumentName,
          "The value of the provided ObjectID is of type '{0}', but only 'System.Guid' is supported.",
          id.Value.GetType ());
    }  
  }

  private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
  {
    if (classDefinition.StorageProviderID != ID)
    {
      throw CreateArgumentException (
          argumentName,
          "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
          classDefinition.StorageProviderID,
          ID);
    }
  }
}
}
