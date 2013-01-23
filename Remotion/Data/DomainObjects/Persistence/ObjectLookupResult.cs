// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System;
using Remotion.Data.DomainObjects.DataManagement;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Holds the result from looking up the <see cref="DataContainer"/> for a specific <see cref="IObjectID{DomainObject}"/>.
  /// </summary>
  public struct ObjectLookupResult<T>
  {
    private readonly IObjectID<DomainObject> _objectID;
    private readonly T _locatedObject;

    public ObjectLookupResult (IObjectID<DomainObject> objectID, T locatedObject)
    {
      _objectID = objectID;
      _locatedObject = locatedObject;
    }

    public IObjectID<DomainObject> ObjectID
    {
      get { return _objectID; }
    }

    public T LocatedObject
    {
      get { return _locatedObject; }
    }
  }
}