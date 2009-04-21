// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Cloning
{
  public class CloneContext
  {
    private readonly DomainObjectCloner _cloner;
    private readonly SimpleDataStore<DomainObject, DomainObject> _clones = new SimpleDataStore<DomainObject, DomainObject> ();
    private readonly Queue<Tuple<DomainObject, DomainObject>> _cloneHulls = new Queue<Tuple<DomainObject, DomainObject>> ();

    public CloneContext (DomainObjectCloner cloner)
    {
      ArgumentUtility.CheckNotNull ("cloner", cloner);
      _cloner = cloner;
    }

    public virtual Queue<Tuple<DomainObject, DomainObject>> CloneHulls
    {
      get { return _cloneHulls; }
    }

    public virtual T GetCloneFor<T> (T domainObject)
        where T : DomainObject
    {
      return (T) _clones.GetOrCreateValue (domainObject, delegate (DomainObject cloneTemplate)
      {
        DomainObject clone = _cloner.CreateCloneHull (cloneTemplate);
        CloneHulls.Enqueue (Tuple.NewTuple (cloneTemplate, clone));
        return clone;
      });
    }
  }
}
