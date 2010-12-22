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
using System.Collections.ObjectModel;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Persistence.Rdbms.Model
{
  // TODO Review 3606: Implement INullObject on IEntityDefinition and IColumnDefinition
  /// <summary>
  /// The <see cref="NullEntityDefinition"/> represents a non-existing entity.
  /// </summary>
  public class NullEntityDefinition : IEntityDefinition
  {
    private readonly StorageProviderDefinition _storageProviderDefinition;

    public NullEntityDefinition (StorageProviderDefinition storageProviderDefinition)
    {
      ArgumentUtility.CheckNotNull ("storageProviderDefinition", storageProviderDefinition);

      _storageProviderDefinition = storageProviderDefinition;
    }

    public string LegacyEntityName
    {
      get { return null; }
    }

    public string LegacyViewName
    {
      get { return null; }
    }

    public string StorageProviderID
    {
      get { return _storageProviderDefinition.Name; }
    }

    public StorageProviderDefinition StorageProviderDefinition
    {
      get { return _storageProviderDefinition; }
    }

    public string ViewName
    {
      get { return null; }
    }

    public ReadOnlyCollection<IColumnDefinition> GetColumns ()
    {
      return new ReadOnlyCollection<IColumnDefinition> (new IColumnDefinition[0]);
    }

    public void Accept (IEntityDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);

      visitor.VisitNullEntityDefinition (this);
    }
  }
}