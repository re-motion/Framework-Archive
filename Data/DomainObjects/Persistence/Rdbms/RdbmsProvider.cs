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

  protected RdbmsProvider (RdbmsProviderDefinition definition) : base (definition)
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
          _connection.ConnectionString = this.Definition.ConnectionString;

        _connection.Open ();
      }
      catch (Exception e)
      {
        throw CreateRdbmsProviderException (e, "Error while opening connection.");
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
      throw CreateRdbmsProviderException (e, "Error while executing BeginTransaction.");
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
      throw CreateRdbmsProviderException (e, "Error while executing Commit.");
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
      throw CreateRdbmsProviderException (e, "Error while executing Rollback.");
    }
    finally
    {
      DisposeTransaction ();
    }
  }

  public override DataContainerCollection ExecuteCollectionQuery (IQuery query)
  {
    CheckQuery (query, QueryType.Collection, "query");

    Connect ();

    QueryCommandBuilder commandBuilder = new QueryCommandBuilder (this, query);
    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (command, CommandBehavior.SingleResult))
      {
        IDataContainerFactory dataContainerFactory = CreateDataContainerFactory (reader, query);
        return dataContainerFactory.CreateCollection ();
      }
    }    
  }

  public override object ExecuteScalarQuery (IQuery query)
  {
    CheckQuery (query, QueryType.Scalar, "query");

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
        throw CreateRdbmsProviderException (
            e, "Error while executing SQL command for query '{0}'.", query.ID);
      }
    }
  }

  public override DataContainerCollection LoadDataContainersByRelatedID (ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("relatedID", relatedID);
    CheckClassDefinition (classDefinition, "classDefinition");

    Connect ();

    PropertyDefinition property = classDefinition.GetPropertyDefinition (propertyName);
    if (property == null)
    {
      throw CreateRdbmsProviderException ("Class '{0}' does not contain property '{1}'.",
          classDefinition.ID, propertyName);
    }

    VirtualRelationEndPointDefinition oppositeRelationEndPointDefinition = 
        (VirtualRelationEndPointDefinition) classDefinition.GetMandatoryOppositeEndPointDefinition (property.PropertyName);

    // TODO: ClassDefinition does not have to have an entity assigned => abstract base class in a concrete table inheritance scenario =>
    // Search for all concrete entities and look for objects.

    SelectCommandBuilder commandBuilder = new SelectCommandBuilder (
        this, classDefinition, property, relatedID, oppositeRelationEndPointDefinition.SortExpression);

    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (command, CommandBehavior.SingleResult))
      {
        IDataContainerFactory dataContainerFactory = CreateDataContainerFactory (reader);
        return dataContainerFactory.CreateCollection ();
      }
    }
  }

  public override DataContainer LoadDataContainer (ObjectID id)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("id", id);
    CheckStorageProviderID (id, "id");

    Connect();

    SelectCommandBuilder commandBuilder = new SelectCommandBuilder (this, id.ClassDefinition, "ID", id.Value);
    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (command, CommandBehavior.SingleRow))
      {
        IDataContainerFactory dataContainerFactory = CreateDataContainerFactory (reader);
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

  protected virtual IDataContainerFactory CreateDataContainerFactory (IDataReader reader, IQuery query)
  {
    ArgumentUtility.CheckNotNull ("reader", reader);
    
    return new DataContainerFactory (reader);
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
      throw CreateRdbmsProviderException (e, "Error while executing SQL command.");
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
        throw CreateRdbmsProviderException (e, "Error while setting timestamp for object '{0}'.", dataContainer.ID);
      }
    }
  }

  protected virtual void Save (IDbCommand command, ObjectID id)
  {
    CheckDisposed ();
    ArgumentUtility.CheckNotNull ("id", id);
    CheckStorageProviderID (id, "id");

    if (command == null)
      return;

    int recordsAffected = 0;
    try
    {
      recordsAffected = command.ExecuteNonQuery ();
    }
    catch (Exception e)
    {
      throw CreateRdbmsProviderException (e, "Error while saving object '{0}'.", id);
    }

    if (recordsAffected != 1)
    {
      throw CreateConcurrencyViolationException (
          "Concurrency violation encountered. Object '{0}' has already been changed by someone else.", id);
    }
  }

  public new RdbmsProviderDefinition Definition
  {
    get 
    {
      // CheckDisposed is not necessary here, because StorageProvider.Definition already checks this.
      return (RdbmsProviderDefinition) base.Definition; 
    }
  }

  protected RdbmsProviderException CreateRdbmsProviderException (
      string formatString,
      params object[] args)
  {
    return CreateRdbmsProviderException (null, formatString, args);
  }

  protected RdbmsProviderException CreateRdbmsProviderException (
      Exception innerException,
      string formatString,
      params object[] args)
  {
    return new RdbmsProviderException (string.Format (formatString, args), innerException);
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

  private IDataContainerFactory CreateDataContainerFactory (IDataReader reader)
  {
    ArgumentUtility.CheckNotNull ("reader", reader);

    return CreateDataContainerFactory (reader, null);
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
