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
using System.Linq;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;

namespace Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications
{
  public class NullEndPointModificationCommand : IDataManagementCommand
  {
    private readonly IRelationEndPoint _affectedEndPoint;

    public NullEndPointModificationCommand (IRelationEndPoint affectedEndPoint)
    {
      _affectedEndPoint = affectedEndPoint;
    }

    public IRelationEndPoint AffectedEndPoint
    {
      get { return _affectedEndPoint; }
    }

    public IEnumerable<Exception> GetAllExceptions ()
    {
      return Enumerable.Empty<Exception> ();
    }

    public void Begin ()
    {
      // do nothing
    }

    public void Perform ()
    {
      // do nothing
    }

    public void End ()
    {
      // do nothing
    }

    public void NotifyClientTransactionOfBegin ()
    {
      // do nothing
    }

    public void NotifyClientTransactionOfEnd ()
    {
      // do nothing
    }

    ExpandedCommand IDataManagementCommand.ExpandToAllRelatedObjects ()
    {
      return new ExpandedCommand (this);
    }
  }
}
