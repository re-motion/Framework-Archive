using System;
using System.Data;
using System.Text;

using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
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

  public void AddCommandParameter (IDbCommand command, string parameterName, PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    AddCommandParameter (command, parameterName, propertyValue.Value);
  }

  public void AddCommandParameter (IDbCommand command, string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);

    IDataParameter commandParameter = command.CreateParameter ();
    commandParameter.ParameterName = Provider.GetParameterName (parameterName);
    commandParameter.Value = ConvertToDBType (parameterValue);

    command.Parameters.Add (commandParameter);
  }

  
  protected IDbCommand CreateCommand ()
  {
    IDbCommand command = _provider.Connection.CreateCommand ();
    command.Connection = _provider.Connection;
    command.Transaction = _provider.Transaction;

    return command;
  }

  protected void AddObjectIDParameter (      
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
      relatedClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (relatedID.ClassID);
      relatedIDValue = GetObjectIDForParameter (relatedID);
    }
    else
    {
      relatedClassDefinition = classDefinition.GetRelatedClassDefinition (propertyValue.Name);
      relatedIDValue = null;
    }

    AddCommandParameter (command, propertyValue.Definition.ColumnName, relatedIDValue);
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

    ClassDefinitionCollection derivedClasses =  
        MappingConfiguration.Current.ClassDefinitions.GetDirectlyDerivedClassDefinitions (relatedClassDefinition);

    if (relatedClassDefinition.BaseClass != null || derivedClasses.Count > 0)
    {
      string classIDColumnName = propertyValue.Definition.ColumnName + "ClassID";
      AppendColumn (classIDColumnName, classIDColumnName);

      object classID = null;
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

  protected object ConvertToDBType (object value)
  {
    if (value == null)
      return DBNull.Value;

    Type type = value.GetType ();

    if (type == typeof (NaBoolean))
    {
      NaBoolean naBooleanValue = (NaBoolean) value;
      if (!naBooleanValue.IsNull)
        return naBooleanValue.Value;
      else
        return DBNull.Value;
    }

    if (type == typeof (NaDateTime))
    {
      NaDateTime naDateTimeValue = (NaDateTime) value;
      if (!naDateTimeValue.IsNull)
        return naDateTimeValue.Value;
      else
        return DBNull.Value;
    }

    if (type == typeof (NaDouble))
    {
      NaDouble naDoubleValue = (NaDouble) value;
      if (!naDoubleValue.IsNull)
        return naDoubleValue.Value;
      else
        return DBNull.Value;
    }

    if (type == typeof (NaInt32))
    {
      NaInt32 naInt32Value = (NaInt32) value;
      if (!naInt32Value.IsNull)
        return naInt32Value.Value;
      else
        return DBNull.Value;
    }

    if (type.IsEnum)
      return (int) value;

    if (type == typeof (char))
      return value.ToString ();
    
    return value;
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
