using System;
using System.Data;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
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
      throw CreateArgumentException ("dataContainer", "State of provided DataContainer must be 'New', but is '{0}'.", dataContainer.State);

    _dataContainer = dataContainer;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = Provider.CreateDbCommand ();

    _columnBuilder = new StringBuilder ();
    _valueBuilder = new StringBuilder ();

    string idColumn = "ID";
    string classIDColumn = "ClassID";

    AppendColumn (idColumn, idColumn);
    AppendColumn (classIDColumn, classIDColumn);

    AddCommandParameter (command, idColumn, _dataContainer.ID);
    AddCommandParameter (command, classIDColumn, _dataContainer.ID.ClassID);

    foreach (PropertyValue propertyValue in _dataContainer.PropertyValues)
    {
      if (propertyValue.PropertyType != typeof (ObjectID))
      {
        AppendColumn (propertyValue.Definition.ColumnName, propertyValue.Definition.ColumnName);
        AddCommandParameter (command, propertyValue.Definition.ColumnName, propertyValue);
      }
    }
    
    command.CommandText = string.Format (
        "INSERT INTO {0} ({1}) VALUES ({2}){3}",
        Provider.DelimitIdentifier (_dataContainer.ClassDefinition.GetEntityName ()),
        _columnBuilder.ToString (), 
        _valueBuilder.ToString (),
        Provider.StatementDelimiter);

    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    if (_columnBuilder.Length > 0)
      _columnBuilder.Append (", ");

    _columnBuilder.Append (Provider.DelimitIdentifier (columnName));

    if (_valueBuilder.Length > 0)
      _valueBuilder.Append (", ");

    _valueBuilder.Append (Provider.GetParameterName (parameterName));    
  }
}
}
