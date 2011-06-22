// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Data;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders
{
  /// <summary>
  /// Builds an <see cref="IDbCommand"/> for a given <see cref="IQuery"/>.
  /// </summary>
  public class QueryDbCommandBuilder : DbCommandBuilder
  {
    private readonly IQuery _query;

    public QueryDbCommandBuilder (IQuery query, ISqlDialect sqlDialect, IValueConverter valueConverter)
        : base (sqlDialect, valueConverter)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      _query = query;
    }

    public IQuery Query
    {
      get { return _query; }
    }

    public override IDbCommand Create (IRdbmsProviderCommandExecutionContext commandExecutionContext)
    {
      ArgumentUtility.CheckNotNull ("commandExecutionContext", commandExecutionContext);

      IDbCommand command = commandExecutionContext.CreateDbCommand ();

      string statement = _query.Statement;
      foreach (QueryParameter parameter in _query.Parameters)
      {
        if (parameter.ParameterType == QueryParameterType.Text)
          statement = statement.Replace (parameter.Name, parameter.Value.ToString());
        else
          AddCommandParameter (command, parameter.Name, parameter.Value);
      }

      command.CommandText = statement;
      return command;
    }
  }
}