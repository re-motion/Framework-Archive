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
  private string _whereClauseColumnName;
  private object _whereClauseValue;
  private string _orderClause;

  // construction and disposing

  public SelectCommandBuilder (
      RdbmsProvider provider,
      ClassDefinition classDefinition,
      PropertyDefinition whereClausePropertyDefinition,
      object whereClauseValue)
    : this (provider, "*", classDefinition, whereClausePropertyDefinition, whereClauseValue)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      ClassDefinition classDefinition,
      string whereClauseColumnName,
      object whereClauseValue)
    : this (provider, "*", classDefinition, whereClauseColumnName, whereClauseValue)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      ClassDefinition classDefinition,
      PropertyDefinition whereClausePropertyDefinition,
      object whereClauseValue,
      string orderClause)
    : this (provider, "*", classDefinition, whereClausePropertyDefinition, whereClauseValue, orderClause)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      ClassDefinition classDefinition,
      string whereClauseColumnName,
      object whereClauseValue,
      string orderClause)
    : this (provider, "*", classDefinition, whereClauseColumnName, whereClauseValue, orderClause)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      PropertyDefinition whereClausePropertyDefinition,
      object whereClauseValue)
    : this (provider, selectColumns, classDefinition, whereClausePropertyDefinition, whereClauseValue, null)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      string whereClauseColumnName,
      object whereClauseValue)
    : this (provider, selectColumns, classDefinition, whereClauseColumnName, whereClauseValue, null)
  {
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      PropertyDefinition whereClausePropertyDefinition,
      object whereClauseValue,
      string orderClause) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("whereClausePropertyDefinition", whereClausePropertyDefinition);
    SetInitialValues (selectColumns, classDefinition, whereClausePropertyDefinition.ColumnName, whereClauseValue, orderClause);
  }

  public SelectCommandBuilder (
      RdbmsProvider provider,
      string selectColumns,
      ClassDefinition classDefinition,
      string whereClauseColumnName,
      object whereClauseValue,
      string orderClause) : base (provider)
  {
    SetInitialValues (selectColumns, classDefinition, whereClauseColumnName, whereClauseValue, orderClause);
  }

  private void SetInitialValues (
      string selectColumns, 
      ClassDefinition classDefinition,
      string whereClauseColumnName,
      object whereClauseValue, 
      string orderClause)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("selectColumns", selectColumns);
    ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
    ArgumentUtility.CheckNotNullOrEmpty ("whereClauseColumnName", whereClauseColumnName);
    ArgumentUtility.CheckNotNull ("whereClauseValue", whereClauseValue);

    _selectColumns = selectColumns;
    _classDefinition = classDefinition;
    _whereClauseColumnName = whereClauseColumnName;
    _whereClauseValue = whereClauseValue;
    _orderClause = orderClause;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();
    WhereClauseBuilder whereClauseBuilder = new WhereClauseBuilder (this, command);
    whereClauseBuilder.Add (_whereClauseColumnName, GetValue ());

    string orderExpression = string.Empty;
    if (_orderClause != null)
      orderExpression = " ORDER BY " + _orderClause;

    // TODO: Implement concrete table inheritance!
    command.CommandText = string.Format ("SELECT {0} FROM [{1}] WHERE {2}{3};", 
        _selectColumns, _classDefinition.GetEntityName (), whereClauseBuilder.ToString (), orderExpression);

    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    throw new NotSupportedException ("'AppendColumn' is not supported by 'SelectCommandBuilder'.");
  }

  private object GetValue ()
  {
    if (_whereClauseValue.GetType () == typeof (ObjectID))
      return GetObjectIDForParameter ((ObjectID) _whereClauseValue);
    else
      return _whereClauseValue;
  }
}
}
