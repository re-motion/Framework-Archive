using System;
using System.Data;
using System.Text;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

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
  protected abstract void AppendColumn (string columnName, string parameterName);

  // methods and properties

  public void AddCommandParameter (IDbCommand command, string parameterName, PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    if (propertyValue.PropertyType.IsEnum)
      AddCommandParameter (command, parameterName, (int) propertyValue.Value);
    else if (propertyValue.PropertyType == typeof (char))
      AddCommandParameter (command, parameterName, propertyValue.Value.ToString ());
    else 
      AddCommandParameter (command, parameterName, ConvertToDBType (propertyValue));
  }

  public void AddCommandParameter (IDbCommand command, string parameterName, object parameterValue)
  {
    ArgumentUtility.CheckNotNull ("command", command);
    ArgumentUtility.CheckNotNullOrEmpty ("parameterName", parameterName);
    ArgumentUtility.CheckNotNull ("parameterValue", parameterValue);

    IDataParameter commandParameter = command.CreateParameter ();
    commandParameter.ParameterName = parameterName;
    commandParameter.Value = parameterValue;

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
      relatedIDValue = DBNull.Value;
    }

    AddCommandParameter (command, _provider.GetParameterName (propertyValue.Definition.ColumnName), relatedIDValue);
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
      string classIDParameterName = _provider.GetParameterName (classIDColumnName);
      AppendColumn (classIDColumnName, classIDParameterName);

      object classID = null;
      if (propertyValue.Value != null)
        classID = relatedClassDefinition.ID;
      else
        classID = DBNull.Value;

      AddCommandParameter (command, classIDParameterName, classID);
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

  protected object ConvertToDBType (PropertyValue propertyValue)
  {
    ArgumentUtility.CheckNotNull ("propertyValue", propertyValue);

    if (propertyValue.PropertyType == typeof (NaBoolean))
    {
      NaBoolean naBooleanValue = (NaBoolean) propertyValue.Value;
      if (!naBooleanValue.IsNull)
        return naBooleanValue.Value;
      else
        return DBNull.Value;
    }

    if (propertyValue.PropertyType == typeof (NaDateTime))
    {
      NaDateTime naDateTimeValue = (NaDateTime) propertyValue.Value;
      if (!naDateTimeValue.IsNull)
        return naDateTimeValue.Value;
      else
        return DBNull.Value;
    }

    if (propertyValue.PropertyType == typeof (NaDouble))
    {
      NaDouble naDoubleValue = (NaDouble) propertyValue.Value;
      if (!naDoubleValue.IsNull)
        return naDoubleValue.Value;
      else
        return DBNull.Value;
    }

    if (propertyValue.PropertyType == typeof (NaInt32))
    {
      NaInt32 naInt32Value = (NaInt32) propertyValue.Value;
      if (!naInt32Value.IsNull)
        return naInt32Value.Value;
      else
        return DBNull.Value;
    }

    if (propertyValue.Value != null)
      return propertyValue.Value;
    else
      return DBNull.Value;
  }

  public RdbmsProvider Provider
  {
    get { return _provider; }
  }
}
}
