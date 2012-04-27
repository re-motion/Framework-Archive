﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Linq;
using Remotion.Linq.EagerFetching;

namespace Remotion.Data.DomainObjects.Linq
{
  /// <summary>
  /// Defines an interface for objects generating <see cref="IQuery"/> objects from LINQ queries (parsed by re-linq into <see cref="QueryModel"/> 
  /// instances).
  /// </summary>
  public interface IDomainObjectQueryGenerator
  {
    /// <summary>
    /// Creates an <see cref="IQuery"/> object for a given <see cref="ClassDefinition"/> based on the given <see cref="QueryModel"/>.
    /// </summary>
    /// <param name="id">The identifier for the resulting query.</param>
    /// <param name="classDefinition">The <see cref="ClassDefinition"/> to use for creating the query. This is used to obtain the 
    /// <see cref="StorageProvider"/> for the query, and it is used to analyze the relation properties for eager fetching.</param>
    /// <param name="queryModel">The <see cref="QueryModel"/> describing the query.</param>
    /// <param name="fetchQueryModelBuilders">
    /// A number of <see cref="FetchQueryModelBuilder"/> instances for the fetch requests to be executed together with the query.</param>
    /// <param name="queryType">The type of query to create.</param>
    /// <returns>
    /// An <see cref="IQuery"/> object corresponding to the given <paramref name="queryModel"/>.
    /// </returns>
    IQuery CreateQuery (
        string id, 
        ClassDefinition classDefinition, 
        QueryModel queryModel, 
        IEnumerable<FetchQueryModelBuilder> fetchQueryModelBuilders, 
        QueryType queryType);
  }
}