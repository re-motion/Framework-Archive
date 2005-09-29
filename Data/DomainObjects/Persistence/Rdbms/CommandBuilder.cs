using System;
using System.Data;
using System.Text;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public abstract class CommandBuilder
{
  // types

  // static members and constants

  // member fields

  private RdbmsProvider _provider;

  // construction and disposing

  protected CommandBuilder (RdbmsProvider provider)
  {
    ArgumentUtility.CheckNotNull ("provider", provider);
    _provider = provider;
  }

  // abstract methods and properties

  public abstract IDbCommand Create ();

  // methods and properties

  public IDataParameter AddCommandParameter (IDbCommand command, string parameterName, PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    IDataParameter commandParameter = AddCommandParameter (command, parameterName, propertyValue.Value);

    if (propertyValue.PropertyType == typeof (byte[]))
      commandParameter.DbType = DbType.Binary;

    return commandParameter;
  }

  /// <remarks>
  /// This method cannot be used for binary (BLOB) <i>parameterValues</i>. Use the overload with a <see cref="Rubicon.Data.DomainObjects.PropertyValue"/> instead.
  /// </remarks>
  public IDataParameter AddCommandParameter (IDbCommand command, string parameterName, object parameterValue)
  {
    // Note: UpdateCommandBuilder implicitly uses this method through WhereClauseBuilder.Add for Timestamp values.
    // Although Timestamp values are represented as byte arrays in ADO.NET with SQL Server they are no BLOB data type.
    // Therefore this usage is still valid.

    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);

    IDataParameter commandParameter = command.CreateParameter ();
    commandParameter.ParameterName = Provider.GetParameterName (parameterName);

    ValueConverter valueConverter = new ValueConverter ();
    if (parameterValue != null && parameterValue.GetType () == typeof (ObjectID))
      commandParameter.Value = valueConverter.GetDBValue ((ObjectID) parameterValue, Provider.ID);
    else
      commandParameter.Value = valueConverter.GetDBValue (parameterValue);

    command.Parameters.Add (commandParameter);
    return commandParameter;
  }

  
  protected IDbCommand CreateCommand ()
  {
    IDbCommand command = _provider.Connection.CreateCommand ();
    command.Connection = _provider.Connection;
    command.Transaction = _provider.Transaction;

    return command;
  }

  protected void AddObjectIDAndClassIDParameters (      
      IDbCommand command, 
      ClassDefinition classDefinition,
      PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    ClassDefinition relatedClassDefinition = null;
    object relatedIDValue = null;
    if (propertyValue.Value != null)
    {
      ObjectID relatedID = (ObjectID) propertyValue.Value;
      relatedClassDefinition = relatedID.ClassDefinition;
      relatedIDValue = GetObjectIDForParameter (relatedID);
    }
    else
    {
      relatedClassDefinition = classDefinition.GetOppositeClassDefinition (propertyValue.Name);
      relatedIDValue = null;
    }

    AddCommandParameter (command, propertyValue.Definition.ColumnName, relatedIDValue);

    if (classDefinition.StorageProviderID == relatedClassDefinition.StorageProviderID)
      AddClassIDParameter (command, relatedClassDefinition, propertyValue);
  }

  protected void AddClassIDParameter (
      IDbCommand command, 
      ClassDefinition relatedClassDefinition,
      PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNull ("relatedClassDefinition", relatedClassDefinition);
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    if (relatedClassDefinition.IsPartOfInheritanceHierarchy)
    {
      string classIDColumnName = propertyValue.Definition.ColumnName + "ClassID";
      AppendColumn (classIDColumnName, classIDColumnName);

      string classID = null;
      if (propertyValue.Value != null)
        classID = relatedClassDefinition.ID;

      AddCommandParameter (command, classIDColumnName, classID);
    }  
  }

  protected object GetObjectIDForParameter (ObjectID id)
  {
    ArgumentUtility.CheckNotNull ("id", id);

    if (id.StorageProviderID == _provider.ID)
      return id.Value;
    else
      return id.ToString ();
  }

  public RdbmsProvider Provider
  {
    get { return _provider; }
  }

  protected virtual void AppendColumn (string columnName, string parameterName)
  {
    throw new InvalidOperationException ("AppendColumn must be overridden in derived class.");
  }

  protected ArgumentException CreateArgumentException (string parameterName, string message, params object[] args)
  {
    return new ArgumentException (string.Format (message, args), parameterName);
  }
}
}
