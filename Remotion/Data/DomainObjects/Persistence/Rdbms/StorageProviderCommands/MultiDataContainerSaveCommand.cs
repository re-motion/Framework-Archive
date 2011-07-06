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
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Persistence.Rdbms.DbCommandBuilders;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.StorageProviderCommands
{
  /// <summary>
  /// The <see cref="MultiDataContainerSaveCommand"/> creates an <see cref="IDbCommand"/> and executes it for all given tuples.
  /// </summary>
  public class MultiDataContainerSaveCommand : IStorageProviderCommand<IRdbmsProviderCommandExecutionContext>
  {
    private readonly Tuple<ObjectID, IDbCommandBuilder>[] _tuples;

    public MultiDataContainerSaveCommand (IEnumerable<Tuple<ObjectID, IDbCommandBuilder>> tuples)
    {
      ArgumentUtility.CheckNotNull ("tuples", tuples);

      _tuples = tuples.ToArray();
    }

    public void Execute (IRdbmsProviderCommandExecutionContext executionContext)
    {
      ArgumentUtility.CheckNotNull ("executionContext", executionContext);

      foreach (var tuple in _tuples)
      {
        var command = tuple.Item2.Create (executionContext);
        if (command == null)
          continue;

        int recordsAffected;
        try
        {
          recordsAffected = command.ExecuteNonQuery();
        }
        catch (Exception e)
        {
          throw new RdbmsProviderException (string.Format("Error while saving object '{0}'.", tuple.Item1), e);
        }

        if (recordsAffected != 1)
        {
          throw new ConcurrencyViolationException (
              string.Format ("Concurrency violation encountered. Object '{0}' has already been changed by someone else.", tuple.Item1));
        }
      }
    }
  }
}