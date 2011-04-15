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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer
{
  public class TestableSqlStorageObjectFactory : SqlStorageObjectFactory
  {
    private readonly TableBuilder _tableBuilder;
    private readonly ViewBuilder _viewBuilder;
    private readonly ConstraintBuilder _constraintBuilder;
    private readonly IndexBuilder _indexBuilder;

    public TestableSqlStorageObjectFactory (
        TableBuilder tableBuilder, ViewBuilder viewBuilder, ConstraintBuilder constraintBuilder, IndexBuilder indexBuilder)
    {
      _indexBuilder = indexBuilder;
      _constraintBuilder = constraintBuilder;
      _viewBuilder = viewBuilder;
      _tableBuilder = tableBuilder;
    }

    protected override TableBuilder CreateTableBuilder ()
    {
      return _tableBuilder;
    }

    protected override ViewBuilder CreateViewBuilder ()
    {
      return _viewBuilder;
    }

    protected override ConstraintBuilder CreateConstraintBuilder ()
    {
      return _constraintBuilder;
    }

    protected override IndexBuilder CreateIndexBuilder ()
    {
      return _indexBuilder;
    }
  }
}