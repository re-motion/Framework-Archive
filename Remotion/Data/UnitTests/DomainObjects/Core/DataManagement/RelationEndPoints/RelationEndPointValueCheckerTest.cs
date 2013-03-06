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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  [TestFixture]
  public class RelationEndPointValueCheckerTest : ClientTransactionBaseTest
  {
    [Test]
    public void CheckClientTransaction_MatchingTransactions ()
    {
      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (TestableClientTransaction);
      var relatedObject = DomainObjectMother.CreateObjectInTransaction<OrderItem> (TestableClientTransaction);

      var endPointStub = CreateRelationEndPointStub (TestableClientTransaction, owningObject);

      CallCheckClientTransaction (endPointStub, relatedObject);
    }

    [Test]
    public void CheckClientTransaction_NullObject ()
    {
      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (TestableClientTransaction);

      var endPointStub = CreateRelationEndPointStub (TestableClientTransaction, owningObject);

      CallCheckClientTransaction (endPointStub, null);
    }

    [Test]
    [ExpectedException (typeof (ClientTransactionsDifferException), ExpectedMessage =
        @"Cannot xx DomainObject 'OrderItem\|.*@|System.Guid' from/to collection of property "
        + @"'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderItems' of DomainObject 'Order\|.*\|System.Guid'. The objects do not belong "
        + @"to the same ClientTransaction.", MatchType = MessageMatch.Regex)]
    public void CheckClientTransaction_Differ_ObjectsInDifferentTransactions ()
    {
      var owningObject = DomainObjectMother.CreateObjectInTransaction<Order> (TestableClientTransaction);
      var relatedObject = DomainObjectMother.CreateObjectInTransaction<OrderItem> (ClientTransaction.CreateRootTransaction ());

      var endPointStub = CreateRelationEndPointStub (TestableClientTransaction, owningObject);

      CallCheckClientTransaction (endPointStub, relatedObject);
    }

    private void CallCheckClientTransaction (IRelationEndPoint endPoint, DomainObject relatedObject)
    {
      RelationEndPointValueChecker.CheckClientTransaction (
          endPoint, 
          relatedObject, 
          "Cannot xx DomainObject '{0}' from/to collection of property '{1}' of DomainObject '{2}'.");
    }

    private IRelationEndPoint CreateRelationEndPointStub (ClientTransaction transaction, Order owningObject)
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (owningObject.ID, "OrderItems");

      var endPointStub = MockRepository.GenerateStub<IRelationEndPoint> ();
      endPointStub.Stub (stub => stub.ClientTransaction).Return (transaction);
      endPointStub.Stub (stub => stub.ID).Return (id);
      endPointStub.Stub (stub => stub.Definition).Return (id.Definition);
      endPointStub.Stub (stub => stub.ObjectID).Return (owningObject.ID);
      endPointStub.Stub (stub => stub.GetDomainObjectReference()).Return (owningObject);
      
      return endPointStub;
    }
  }
}