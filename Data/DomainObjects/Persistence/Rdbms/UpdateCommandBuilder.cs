using System;
using System.Data;
using System.Text;

using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class UpdateCommandBuilder : CommandBuilder
{
  // types

  // static members and constants

  // member fields

  private DataContainer _dataContainer;
  private StringBuilder _updateBuilder;

  // construction and disposing

  public UpdateCommandBuilder (RdbmsProvider provider, DataContainer dataContainer) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    if (dataContainer.State == StateType.Unchanged)
      throw CreateArgumentException ("dataContainer", "State of provided DataContainer must not be 'Unchanged'.");

    _dataContainer = dataContainer;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();
    _updateBuilder = new StringBuilder ();

    foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
    {
      if (MustBeUpdated (propertyValue))
        AddPropertyValue (command, propertyValue);
    }

    if (command.Parameters.Count == 0)
    {
      command.Dispose ();
      command = null;
      return null;
    }

    WhereClauseBuilder whereClauseBuilder = new WhereClauseBuilder (this, command);
    whereClauseBuilder.Add ("ID", _dataContainer.ID.Value);

    if (_dataContainer.State != StateType.New)
      whereClauseBuilder.Add ("Timestamp", _dataContainer.Timestamp);

    command.CommandText = string.Format ("UPDATE [{0}] SET {1} WHERE {2};",
        _dataContainer.ClassDefinition.EntityName,
        _updateBuilder.ToString (),
        whereClauseBuilder.ToString ());

    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    if (_updateBuilder.Length > 0)
      _updateBuilder.Append (", ");

    _updateBuilder.AppendFormat ("[{0}] = {1}", columnName, parameterName);
  }

  private void AddPropertyValue (IDbCommand command, PropertyValue propertyValue)
  {
    string parameterName = Provider.GetParameterName (propertyValue.Definition.ColumnName);
    AppendColumn (propertyValue.Definition.ColumnName, parameterName);

    if (propertyValue.PropertyType != typeof (ObjectID))
      AddCommandParameter (command, parameterName, propertyValue);
    else
      AddObjectIDParameter (command, _dataContainer.ClassDefinition, propertyValue);
  }

  private bool MustBeUpdated (PropertyValue propertyValue)
  {
    return (_dataContainer.State == StateType.New && propertyValue.PropertyType == typeof (ObjectID)) 
        || (_dataContainer.State == StateType.Deleted && propertyValue.PropertyType == typeof (ObjectID)) 
        || (_dataContainer.State == StateType.Changed && propertyValue.HasChanged);
  }
}
}
