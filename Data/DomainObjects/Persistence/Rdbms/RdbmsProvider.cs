using System;
using System.Data;
using System.Text;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.Configuration.StorageProviders;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.Persistence
{
public abstract class RdbmsProvider : StorageProvider
{
  // types

  // static members and constants

  // member fields

  private IDbConnection _connection;
  private IDbTransaction _transaction;

  private bool _disposed = false;

  // construction and disposing

  protected RdbmsProvider (RdbmsProviderDefinition rdbmsProviderDefinition) : base (rdbmsProviderDefinition)
  {
  }

  protected override void Dispose (bool disposing)
  {
    if (!_disposed)
    {
      try
      {
        if (disposing)
          Disconnect ();
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
    DisposeTransaction ();
    DisposeConnection ();
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

  public override DataContainerCollection LoadDataContainersByRelatedID (
      ClassDefinition classDefinition, 
      string propertyName, 
      ObjectID relatedID)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);
    ArgumentUtility.CheckNotNull ("relatedID", relatedID);

    Connect ();

    PropertyDefinition property = classDefinition.GetPropertyDefinition (propertyName);
    if (property == null)
    {
      throw CreateStorageProviderException ("Class '{0}' does not contain property '{1}'.",
          classDefinition.ID, propertyName);
    }

    SelectCommandBuilder commandBuilder = new SelectCommandBuilder (this, classDefinition, property, relatedID);
    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (classDefinition, command, CommandBehavior.SingleResult))
      {
        DataContainerFactory dataContainerFactory = new DataContainerFactory (classDefinition, reader);
        return dataContainerFactory.CreateCollection ();
      }
    }
  }

  public override DataContainer LoadDataContainer (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    ClassDefinition classDefinition = MappingConfiguration.Current.ClassDefinitions.GetByClassID (id.ClassID);
    if (classDefinition == null)
    {
      throw CreateStorageProviderException ("Mapping does not contain a class definition with ID '{0}'.", 
          id.ClassID);
    }

    Connect();

    SelectCommandBuilder commandBuilder = new SelectCommandBuilder (this, classDefinition, "ID", id.Value);
    using (IDbCommand command = commandBuilder.Create ())
    {
      using (IDataReader reader = ExecuteReader (classDefinition, command, CommandBehavior.SingleRow))
      {
        DataContainerFactory dataContainerFactory = new DataContainerFactory (classDefinition, reader);
        return dataContainerFactory.CreateDataContainer ();
      }
    }
  }

  public override void Save (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    Connect ();

    foreach (DataContainer dataContainer in dataContainers.GetByState (StateType.New))
    {
      CommandBuilder commandBuilder = new InsertCommandBuilder (this, dataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Save (command, dataContainer);
      }
    }

    foreach (DataContainer dataContainer in dataContainers)
    {
      CommandBuilder commandBuilder = new UpdateCommandBuilder (this, dataContainer);
      using (IDbCommand command = commandBuilder.Create ())
      {
        Save (command, dataContainer);
      }
    }
  }

  public override void SetTimestamp (DataContainerCollection dataContainers)
  {
    ArgumentUtility.CheckNotNull ("dataContainers", dataContainers);

    Connect ();

    foreach (DataContainer dataContainer in dataContainers)
      SetTimestamp (dataContainer);
  }

  public override DataContainer CreateNewDataContainer (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    return DataContainer.CreateNew (CreateNewObjectID (classDefinition));
  }

  protected virtual ObjectID CreateNewObjectID (ClassDefinition classDefinition)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);

    return new ObjectID (this.ID, classDefinition.ID, Guid.NewGuid ());
  }

  protected virtual IDataReader ExecuteReader (ClassDefinition classDefinition, IDbCommand command, CommandBehavior behavior)
  {
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("command", command);
    if (!Enum.IsDefined (typeof (CommandBehavior), behavior)) throw new ArgumentException (string.Format ("Invalid command behavior '{0}' provided.", behavior), "behavior");

    try
    {
      return command.ExecuteReader (behavior);
    }
    catch (Exception e)
    {
      throw CreateStorageProviderException (
          e, "Error while executing SQL command for class '{0}'.", classDefinition.ID);
    }
  }
  
  protected virtual void SetTimestamp (DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

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

  protected virtual void Save (IDbCommand command, DataContainer dataContainer)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    if (command == null)
      return;

    if (dataContainer.State == StateType.Original)
      return; 

    int recordsAffected = 0;
    try
    {
      recordsAffected = command.ExecuteNonQuery ();
    }
    catch (Exception e)
    {
      throw CreateStorageProviderException (e, "Error while saving object '{0}'.", dataContainer.ID);
    }

    if (recordsAffected != 1)
    {
      throw CreateConcurrencyViolationException (
          "Concurrency violation encountered. Object '{0}' has already been changed by someone else.", 
          dataContainer.ID);
    }
  }

  protected virtual IDbCommand CreateCommand (DataContainer dataContainer)
  {
    CommandBuilder commandBuilder = null;

    if (dataContainer.State == StateType.New)
      commandBuilder = new InsertCommandBuilder (this, dataContainer);
    else
      commandBuilder = new UpdateCommandBuilder (this, dataContainer);

    return commandBuilder.Create ();
  }

  protected new RdbmsProviderDefinition StorageProviderDefinition
  {
    get { return (RdbmsProviderDefinition) base.StorageProviderDefinition; }
  }
  
  public IDbConnection Connection
  {
    get { return _connection; }
  }

  public IDbTransaction Transaction
  {
    get { return _transaction; }
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

  private void DisposeTransaction ()
  {
    if (_transaction != null)
      _transaction.Dispose ();

    _transaction = null;
  }

  private void DisposeConnection ()
  {
    if (_connection != null)
      _connection.Close();
    
    _connection = null;
  }
}
}
