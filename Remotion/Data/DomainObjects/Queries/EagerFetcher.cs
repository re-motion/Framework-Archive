// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// This framework is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with this framework; if not, see http://www.gnu.org/licenses.
// 
using System;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries
{
  public class EagerFetcher
  {
    private readonly IQueryManager _queryManager;
    private readonly DomainObject[] _originalResults;

    public EagerFetcher (IQueryManager queryManager, DomainObject[] originalResults)
    {
      ArgumentUtility.CheckNotNull ("queryManager", queryManager);
      ArgumentUtility.CheckNotNull ("originalResults", originalResults);

      _queryManager = queryManager;
      _originalResults = originalResults;
    }

    public void FetchRelatedObjects (IRelationEndPointDefinition relationEndPointDefinition, IQuery fetchQuery)
    {
      ArgumentUtility.CheckNotNull ("relationEndPointDefinition", relationEndPointDefinition);
      ArgumentUtility.CheckNotNull ("fetchQuery", fetchQuery);

      if (relationEndPointDefinition.Cardinality != CardinalityType.Many)
        throw new ArgumentException ("Eager fetching is only supported for collection-valued relation properties.", "relationEndPointDefinition");

      var fetchedResult = _queryManager.GetCollection (fetchQuery);

      // TODO: revursiveness, 
    }
  }
}