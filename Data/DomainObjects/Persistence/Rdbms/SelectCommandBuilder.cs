using System;
using System.Data;
using System.Text;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class SelectCommandBuilder : CommandBuilder
{
  // types

  // static members and constants

  // member fields

  private string _selectColumns;
  private ClassDefinition _classDefinition;
  private string _columnName;
  private object _value;

  // construction and disposing

  public SelectCommandBuilder (
      RdbmsProvider provider,
      ClassDefinition classDefinition,
      PropertyDefinition propertyDefinition,
      object value) : this (provider, "*", classDefinition, propertyDefinition, value)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      ClassDefinition classDefinition,
      string columnName,
      object value) : this (provider, "*", classDefinition, columnName, value)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      PropertyDefinition propertyDefinition,
      object value) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    SetInitialValues (selectColumns, classDefinition, propertyDefinition.ColumnName, value);
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      string columnName,
      object value) : base (provider)
  {
    SetInitialValues (selectColumns, classDefinition, columnName, value);
  }

  private void SetInitialValues (string selectColumns, ClassDefinition classDefinition, string columnName, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
    ArgumentUtility.CheckNotNull ("value", value);

    _selectColumns = selectColumns;
    _classDefinition = classDefinition;
    _columnName = columnName;
    _value = value;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();
    WhereClauseBuilder whereClauseBuilder = new WhereClauseBuilder (this, command);
    whereClauseBuilder.Add (_columnName, GetValue ());

    command.CommandText = string.Format ("SELECT {0} FROM [{1}] WHERE {2};", 
        _selectColumns, _classDefinition.EntityName, whereClauseBuilder.ToString ());

    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    throw new NotSupportedException ("'AppendColumn' is not supported by 'SelectCommandBuilder'.");
  }

  private object GetValue ()
  {
    if (_value.GetType () == typeof (ObjectID))
      return GetObjectIDForParameter ((ObjectID) _value);
    else
      return _value;
  }
}
}
