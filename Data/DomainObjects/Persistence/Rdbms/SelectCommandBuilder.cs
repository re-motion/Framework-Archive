using System;
using System.Data;
using System.Text;

using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
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
  private string _orderClause;

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
      ClassDefinition classDefinition,
      PropertyDefinition propertyDefinition,
      object value,
      string orderClause) : this (provider, "*", classDefinition, propertyDefinition, value, orderClause)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      ClassDefinition classDefinition,
      string columnName,
      object value,
      string orderClause) : this (provider, "*", classDefinition, columnName, value, orderClause)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      PropertyDefinition propertyDefinition,
      object value) : this (provider, selectColumns, classDefinition, propertyDefinition, value, null)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      string columnName,
      object value) : this (provider, selectColumns, classDefinition, columnName, value, null)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      PropertyDefinition propertyDefinition,
      object value,
      string orderClause) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("propertyDefinition", propertyDefinition);
    SetInitialValues (selectColumns, classDefinition, propertyDefinition.ColumnName, value, orderClause);
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      string columnName,
      object value,
      string orderClause) : base (provider)
  {
    SetInitialValues (selectColumns, classDefinition, columnName, value, orderClause);
  }

  private void SetInitialValues (
      string selectColumns, 
      ClassDefinition classDefinition, 
      string columnName, 
      object value, 
      string orderClause)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
    ArgumentUtility.CheckNotNull ("value", value);

    _selectColumns = selectColumns;
    _classDefinition = classDefinition;
    _columnName = columnName;
    _value = value;
    _orderClause = orderClause;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();
    WhereClauseBuilder whereClauseBuilder = new WhereClauseBuilder (this, command);
    whereClauseBuilder.Add (_columnName, GetValue ());

    string orderExpression = string.Empty;
    if (_orderClause != null)
      orderExpression = " ORDER BY " + _orderClause;

    command.CommandText = string.Format ("SELECT {0} FROM [{1}] WHERE {2}{3};", 
        _selectColumns, _classDefinition.EntityName, whereClauseBuilder.ToString (), orderExpression);

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
