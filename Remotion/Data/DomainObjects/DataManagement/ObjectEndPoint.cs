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
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  public abstract class ObjectEndPoint : RelationEndPoint, IObjectEndPoint
  {
    protected ObjectEndPoint (ClientTransaction clientTransaction, RelationEndPointID id)
        : base (clientTransaction, id)
    {
    }

    public abstract ObjectID OppositeObjectID { get; set; }
    public abstract ObjectID OriginalOppositeObjectID { get; }

    public override bool IsDataComplete
    {
      get { return true; }
    }

    public override void EnsureDataComplete ()
    {
      // nothing to do, ObjectEndPoints' data is always complete
      Assertion.IsTrue (IsDataComplete);
    }

    public override void CheckMandatory ()
    {
      if (OppositeObjectID == null)
      {
        throw CreateMandatoryRelationNotSetException (
            GetDomainObjectReference(),
            PropertyName,
            "Mandatory relation property '{0}' of domain object '{1}' cannot be null.",
            PropertyName,
            ObjectID);
      }
    }

    public override IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject)
    {
      ArgumentUtility.CheckNotNull ("removedRelatedObject", removedRelatedObject);

      var currentRelatedObject = this.GetOppositeObject (true);
      if (removedRelatedObject != currentRelatedObject)
      {
        string removedID = removedRelatedObject.ID.ToString ();
        string currentID = currentRelatedObject != null ? currentRelatedObject.ID.ToString () : "<null>";

        var message = string.Format ("Cannot remove object '{0}' from object end point '{1}' - it currently holds object '{2}'.", 
            removedID, PropertyName, currentID);
        throw new InvalidOperationException (message);
      }

      return CreateSetCommand (null);
    }

    public override IDataManagementCommand CreateDeleteCommand ()
    {
      return new AdHocCommand { PerformHandler = () => OppositeObjectID = null };
    }

    public virtual IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition ();

      var newRelatedObjectID = newRelatedObject != null ? newRelatedObject.ID : null;
      if (OppositeObjectID == newRelatedObjectID)
        return new ObjectEndPointSetSameCommand (this);
      else if (oppositeEndPointDefinition.IsAnonymous)
        return new ObjectEndPointSetUnidirectionalCommand (this, newRelatedObject);
      else if (oppositeEndPointDefinition.Cardinality == CardinalityType.One)
        return new ObjectEndPointSetOneOneCommand (this, newRelatedObject);
      else 
        return new ObjectEndPointSetOneManyCommand (this, newRelatedObject);
    }

    public override void SetValueFrom (IRelationEndPoint source)
    {
      var sourceObjectEndPoint = ArgumentUtility.CheckNotNullAndType<ObjectEndPoint> ("source", source);

      if (Definition != sourceObjectEndPoint.Definition)
      {
        var message = string.Format (
            "Cannot set this end point's value from '{0}'; the end points do not have the same end point definition.",
            source.ID);
        throw new ArgumentException (message, "source");
      }

// ReSharper disable RedundantCheckBeforeAssignment
      if (OppositeObjectID != sourceObjectEndPoint.OppositeObjectID)
        OppositeObjectID = sourceObjectEndPoint.OppositeObjectID;
// ReSharper restore RedundantCheckBeforeAssignment

      if (sourceObjectEndPoint.HasBeenTouched || HasChanged)
        Touch ();
    }

    public override IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager)
    {
      ArgumentUtility.CheckNotNull ("dataManager", dataManager);

      var oppositeEndPointDefinition = Definition.GetOppositeEndPointDefinition ();
      if (oppositeEndPointDefinition.IsAnonymous || OppositeObjectID == null)
      {
        return Enumerable.Empty<IRelationEndPoint> ();
      }
      else
      {
        var oppositeEndPointID = new RelationEndPointID (OppositeObjectID, oppositeEndPointDefinition);
        return new[] { dataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (oppositeEndPointID) };
      }
    }

    #region Serialization

    protected ObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
      // currently, there's nothing to do here
    }

    protected override void SerializeIntoFlatStructure (FlattenedSerializationInfo info)
    {
      // currently, there's nothing to do here
    }

    #endregion
  }
}
