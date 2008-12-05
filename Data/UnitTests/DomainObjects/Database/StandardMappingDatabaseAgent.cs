// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using System.Data.SqlClient;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Core.Resources;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Development.UnitTesting.Data.SqlClient;

namespace Remotion.Data.UnitTests.DomainObjects.Database
{
  public class StandardMappingDatabaseAgent : DatabaseAgent
  {
    public StandardMappingDatabaseAgent (string connectionString)
        : base (connectionString)
    {
    }

    protected override int ExecuteBatch (IDbConnection connection, string sqlFileName, IDbTransaction transaction)
    {
      int batch =  base.ExecuteBatch (connection, sqlFileName, transaction);
      LoadBlobs (connection, transaction);
      return batch;
    }

    private void LoadBlobs (IDbConnection connection, IDbTransaction transaction)
    {
      DomainObjectIDs domainObjectIDs = StandardConfiguration.Instance.GetDomainObjectIDs();
      UpdateClassWithAllDataTypes (connection, transaction, domainObjectIDs.ClassWithAllDataTypes1, ResourceManager.GetImage1 ());
      UpdateClassWithAllDataTypes (connection, transaction, domainObjectIDs.ClassWithAllDataTypes2, ResourceManager.GetImage2 ());
    }

    private void UpdateClassWithAllDataTypes (IDbConnection connection, IDbTransaction transaction, ObjectID id, byte[] binary)
    {
      string updateText = "Update [TableWithAllDataTypes] set [Binary] = @binary where [ID] = @id";
      using (SqlCommand command = (SqlCommand) CreateCommand (connection, updateText, transaction))
      {
        command.Parameters.AddWithValue ("@binary", binary);
        command.Parameters.AddWithValue ("@id", id.Value);
        command.ExecuteNonQuery ();
      }
    }
  }
}
