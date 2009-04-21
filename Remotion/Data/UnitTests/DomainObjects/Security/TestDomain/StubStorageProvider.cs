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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Security.TestDomain
{
  public class StubStorageProvider : StorageProvider
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public StubStorageProvider (StorageProviderDefinition definition)
      : base (definition)
    {
    }

    // methods and properties

    public override DataContainer LoadDataContainer (ObjectID id)
    {
      throw new NotImplementedException ();
    }

    public override DataContainerCollection LoadDataContainers (IEnumerable<ObjectID> ids)
    {
      throw new NotImplementedException ();
    }

    public override DataContainer[] ExecuteCollectionQuery (IQuery query)
    {
      ArgumentUtility.CheckNotNull ("query", query);

      List<DataContainer> collection = new List<DataContainer> ();
      if (query.ID == "GetSecurableObjects")
        collection.Add (DataContainer.CreateNew (CreateNewObjectID  (MappingConfiguration.Current.ClassDefinitions[typeof (SecurableObject)])));

      return collection.ToArray ();
      ;
    }

    public override object ExecuteScalarQuery (IQuery query)
    {
      throw new NotImplementedException ();
    }

    public override void Save (DataContainerCollection dataContainers)
    {
    }

    public override void SetTimestamp (DataContainerCollection dataContainers)
    {
    }

    public override DataContainerCollection LoadDataContainersByRelatedID (ClassDefinition classDefinition, string propertyName, ObjectID relatedID)
    {
      throw new NotImplementedException ();
    }

    public override void BeginTransaction ()
    {
    }

    public override void Commit ()
    {
    }

    public override void Rollback ()
    {
    }

    public override ObjectID CreateNewObjectID (ClassDefinition classDefinition)
    {
      CheckDisposed ();
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      CheckClassDefinition (classDefinition, "classDefinition");

      return new ObjectID (classDefinition.ID, Guid.NewGuid ());
    }

    private void CheckClassDefinition (ClassDefinition classDefinition, string argumentName)
    {
      if (classDefinition.StorageProviderID != ID)
      {
        throw CreateArgumentException (
            argumentName,
            "The StorageProviderID '{0}' of the provided ClassDefinition does not match with this StorageProvider's ID '{1}'.",
            classDefinition.StorageProviderID,
            ID);
      }
    }
  }
}
