using System;
using System.Data;

using Rubicon.Data.DomainObjects.Queries;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Persistence
{
public class QueryCommandBuilder : CommandBuilder
{
  // types

  // static members and constants

  // member fields

  private Query _query;

  // construction and disposing

  public QueryCommandBuilder (RdbmsProvider provider, Query query) : base (provider)
  {
    ArgumentUtility.CheckNotNull ("query", query);

    _query = query;
  }

  // methods and properties

  public override IDbCommand Create ()
  {
    IDbCommand command = CreateCommand ();

    string statement = _query.Statement;
    foreach (QueryParameter parameter in _query.Parameters)
    { 
      if (parameter.ParameterType == QueryParameterType.Text)
        statement = statement.Replace (parameter.Name, parameter.Value.ToString ());
      else
        AddCommandParameter (command, parameter.Name, parameter.Value);
    }

    command.CommandText = statement;
    return command;
  }

  protected override void AppendColumn (string columnName, string parameterName)
  {
    throw new NotSupportedException ("'AppendColumn' is not supported by 'QueryCommandBuilder'.");
  }
}
}
