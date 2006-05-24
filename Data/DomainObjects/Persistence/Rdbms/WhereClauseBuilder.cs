using System;
using System.Data;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence.Rdbms
{
public class WhereClauseBuilder
{
  // types

  // static members and constants

  // member fields

  private CommandBuilder _commandBuilder;
  private IDbCommand _command;
  private StringBuilder _whereClauseBuilder;

  // construction and disposing

  public WhereClauseBuilder (CommandBuilder commandBuilder, IDbCommand command)
  {
    ArgumentUtility.CheckNotNull ("commandBuilder", commandBuilder);
    ArgumentUtility.CheckNotNull ("command", command);

    _commandBuilder = commandBuilder;
    _command = command;
    _whereClauseBuilder = new StringBuilder ();
  }

  // methods and properties

  public void Add (string columnName, object value)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
    ArgumentUtility.CheckNotNull ("value", value);

    if (_whereClauseBuilder.Length > 0)
      _whereClauseBuilder.Append (" AND ");

    string parameterName = _commandBuilder.Provider.GetParameterName (columnName);
    _whereClauseBuilder.AppendFormat ("[{0}] = {1}", columnName, parameterName);
    _commandBuilder.AddCommandParameter (_command, columnName, value);
  }

  public void SetInExpression (string columnName, object[] values)
  {
    ArgumentUtility.CheckNotNullOrEmpty ("columnName", columnName);
    ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("values", values);

    if (_whereClauseBuilder.Length > 0)
      throw new InvalidOperationException ("SetInExpression can only be used with an empty WhereClauseBuilder.");

    _whereClauseBuilder.AppendFormat ("[{0}] IN (", columnName);

    for (int i = 0; i < values.Length; i++)
    {
      if (i > 0)
        _whereClauseBuilder.Append (", ");

      string incrementedColumnName = string.Format ("{0}{1}", columnName, i + 1);
      string parameterName = _commandBuilder.Provider.GetParameterName (incrementedColumnName);

      _whereClauseBuilder.Append (parameterName);
      _commandBuilder.AddCommandParameter (_command, parameterName, values[i]);
    }

    _whereClauseBuilder.Append (")");
  }

  public override string ToString ()
  {
    return _whereClauseBuilder.ToString ();
  }
}
}
