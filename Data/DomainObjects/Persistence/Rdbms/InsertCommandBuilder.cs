using System;
using System.Data;
using System.Text;

using Rubicon.NullableValueTypes;
using Rubicon.Data.DomainObjects.Configuration.Mapping;
using Rubicon.Data.DomainObjects.DataManagement;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class InsertCommandBuilder : CommandBuilder
{
  // types

  // static members and constants

  // member fields

  private DataContainer _dataContainer;
  private StringBuilder _columnBuilder;
  private StringBuilder _valueBuilder;

  // construction and disposing

  public InsertCommandBuilder (RdbmsProvider provider, DataContainer dataContainer) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("dataContainer", dataContainer);

    if (dataContainer.State != StateType.New)
      throw CreateArgumentException ("dataContainer", "State of provided dataContainer must be 'New', but is '{0}'.", dataContainer.State);

    _dataContainer = dataContainer;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();

    _columnBuilder = new StringBuilder ();
    _valueBuilder = new StringBuilder ();

    string idParameter = Provider.GetParameterName ("id");
    string classIDParameter = Provider.GetParameterName ("classID");

    AppendColumn ("ID", idParameter);
    AppendColumn ("ClassID", classIDParameter);

    AddCommandParameter (command, idParameter, _dataContainer.ID.Value);
    AddCommandParameter (command, classIDParameter, _dataContainer.ID.ClassID);

    foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
    {
      if (propertyValue.PropertyType != typeof (ObjectID))
      {
        string parameterName = Provider.GetParameterName (propertyValue.Definition.ColumnName);
        AppendColumn (propertyValue.Definition.ColumnName, parameterName);
        AddCommandParameter (command, parameterName, propertyValue);
      }
    }

    command.CommandText = string.Format ("INSERT INTO [{0}] ({1}) VALUES ({2});",
        _dataContainer.ClassDefinition.EntityName, _columnBuilder.ToString (), _valueBuilder.ToString ());

    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    if (_columnBuilder.Length > 0)
      _columnBuilder.Append (", ");

    _columnBuilder.Append ("[");
    _columnBuilder.Append (columnName);
    _columnBuilder.Append ("]");

    if (_valueBuilder.Length > 0)
      _valueBuilder.Append (", ");

    _valueBuilder.Append (parameterName);    
  }
}
}
