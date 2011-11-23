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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence
{
  /// <summary>
  /// Thrown when one or multiple objects can't be found in the underlying data source.
  /// </summary>
  [Serializable]
  public class ObjectNotFoundException : StorageProviderException
  {
    private static string BuildMessage (IEnumerable<ObjectID> ids)
    {
      return string.Format ("Object(s) could not be found: {0}.", SeparatedStringBuilder.Build (", ", ids, id => "'" + id + "'"));
    }
    
    private readonly ObjectID[] _ids;

    public ObjectNotFoundException (ObjectID[] ids)
      : this (ids, null)
    {
    }

    public ObjectNotFoundException (ObjectID[] ids, Exception inner)
        : this (BuildMessage(ids), ids, inner)
    {
    }

    public ObjectNotFoundException (string message, ObjectID[] ids, Exception inner)
      : base (message, inner)
    {
      ArgumentUtility.CheckNotNull ("ids", ids);

      _ids = ids;
    }

    protected ObjectNotFoundException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
      _ids = (ObjectID[]) info.GetValue ("_ids", typeof (ObjectID[]));
    }

    [Obsolete ("This property is obsolete. Use IDs instead. (1.13.131)", true)]
    public ObjectID ID
    {
      get { throw new NotImplementedException(); }
    }

    public ReadOnlyCollection<ObjectID> IDs
    {
      get { return Array.AsReadOnly (_ids); }
    }

    public override void GetObjectData (SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData (info, context);

      info.AddValue ("_ids", _ids);
    }
  }
}
