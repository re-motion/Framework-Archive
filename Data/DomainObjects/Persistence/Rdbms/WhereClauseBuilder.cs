using System;
using System.Data;
using System.Text;

namespace Rubicon.Data.DomainObjects.Persistence
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
    if (_whereClauseBuilder.Length > 0)
      _whereClauseBuilder.Append (" AND ");

    string parameterName = _commandBuilder.Provider.GetParameterName (columnName);
    _whereClauseBuilder.AppendFormat ("[{0}] = {1}", columnName, parameterName);
    _commandBuilder.AddCommandParameter (_command, parameterName, value);
  }

  public override string ToString ()
  {
    return _whereClauseBuilder.ToString ();
  }
}
}
