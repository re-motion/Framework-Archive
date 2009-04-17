// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class CollectionEndPointSelfReplaceModificationTest : CollectionEndPointModificationTestBase
  {
    private CollectionEndPointSelfReplaceModification _modification;
    private Order _replacedRelatedObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _replacedRelatedObject = Order.GetObject (DomainObjectIDs.Order1);

      _modification =
          new CollectionEndPointSelfReplaceModification (CollectionEndPoint, _replacedRelatedObject, CollectionDataMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_modification.ModifiedEndPoint, Is.SameAs (CollectionEndPoint));
      Assert.That (_modification.OldRelatedObject, Is.SameAs (_replacedRelatedObject));
      Assert.That (_modification.NewRelatedObject, Is.SameAs (_replacedRelatedObject));
      Assert.That (_modification.ModifiedCollection, Is.SameAs (CollectionEndPoint.OppositeDomainObjects));
      Assert.That (_modification.ModifiedCollectionData, Is.SameAs (CollectionDataMock));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Modified end point is null, a NullEndPointModification is needed.\r\n"
                                                                      + "Parameter name: modifiedEndPoint")]
    public void Initialization_FromNullEndPoint ()
    {
      var endPoint = new NullCollectionEndPoint (RelationEndPointID.Definition);
      new CollectionEndPointSelfReplaceModification (endPoint, _replacedRelatedObject, CollectionDataMock);
    }

    [Test]
    public void Begin ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.Begin ();

      Assert.That (relationChangingCalled, Is.False); // no change notification
      Assert.That (relationChangedCalled, Is.False); // no change notification
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // no change notification
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // no change notification
    }

    [Test]
    public void End ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      _modification.End ();

      Assert.That (relationChangingCalled, Is.False); // no change notification
      Assert.That (relationChangedCalled, Is.False); // no change notification
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // no change notification
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // no change notification
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // no change notification
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // no change notification
    }

    [Test]
    public void Perform ()
    {
      bool relationChangingCalled = false;
      bool relationChangedCalled = false;

      DomainObject.RelationChanging += (sender, args) => relationChangingCalled = true;
      DomainObject.RelationChanged += (sender, args) => relationChangedCalled = true;

      CollectionDataMock.BackToRecord ();
      CollectionDataMock.Replay ();

      _modification.Perform ();

      CollectionDataMock.VerifyAllExpectations ();

      Assert.That (relationChangingCalled, Is.False); // operation was not started
      Assert.That (relationChangedCalled, Is.False); // operation was not finished
      Assert.That (CollectionEventReceiver.AddingDomainObject, Is.Null); // operation was not started
      Assert.That (CollectionEventReceiver.AddedDomainObject, Is.Null); // operation was not finished
      Assert.That (CollectionEventReceiver.RemovingDomainObjects, Is.Empty); // operation was not started
      Assert.That (CollectionEventReceiver.RemovedDomainObjects, Is.Empty); // operation was not finished
      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);

      Assert.That (CollectionEndPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void CreateBidirectionalModification ()
    {
      var bidirectionalModification = _modification.CreateBidirectionalModification ();
      Assert.That (bidirectionalModification, Is.InstanceOfType (typeof (NonNotifyingBidirectionalRelationModification)));

      var oppositeEndPoint = ClientTransactionMock.DataManager.RelationEndPointMap.GetRelationEndPointWithLazyLoad (
          _replacedRelatedObject, CollectionEndPoint.OppositeEndPointDefinition);

      var steps = bidirectionalModification.GetModificationSteps ();
      Assert.That (steps.Count, Is.EqualTo (2));

      // customer.Orders.Touch()
      Assert.That (steps[0], Is.SameAs (_modification));

      // customer.Orders[index].Touch()
      Assert.That (steps[1], Is.InstanceOfType (typeof (ObjectEndPointSetSameModification)));
      Assert.That (steps[1].ModifiedEndPoint, Is.SameAs (oppositeEndPoint));
      Assert.That (steps[1].NewRelatedObject, Is.SameAs (DomainObject));
    }
  }
}