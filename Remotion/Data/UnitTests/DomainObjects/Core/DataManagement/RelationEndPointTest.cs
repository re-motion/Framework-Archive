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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  [TestFixture]
  public class RelationEndPointTest : ClientTransactionBaseTest
  {
    private Order _invalidObject;
    private RelationEndPointID _endPointID;

    private RelationEndPoint _endPoint;
    private TestableRelationEndPoint _endPointWithNullObject;
    private TestableRelationEndPoint _endPointWithInvalidObject;

    public override void SetUp ()
    {
      base.SetUp ();

      _invalidObject = Order.NewObject ();
      _invalidObject.Delete ();

      Assert.That (_invalidObject.State, Is.EqualTo (StateType.Invalid));

      _endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");

      _endPoint = RelationEndPointObjectMother.CreateVirtualObjectEndPoint (_endPointID, null);
      _endPointWithNullObject = new TestableRelationEndPoint (ClientTransactionMock, RelationEndPointID.Create(null, _endPointID.Definition));
      _endPointWithInvalidObject = new TestableRelationEndPoint (ClientTransactionMock, RelationEndPointID.Create(_invalidObject.ID, _endPointID.Definition));
    }

    [Test]
    public void RelationDefinition ()
    {
      Assert.That (_endPoint.RelationDefinition, Is.SameAs (_endPointID.Definition.RelationDefinition));
    }

    [Test]
    public void IsVirtual ()
    {
      Assert.That (_endPoint.IsVirtual, Is.True);
    }

    [Test]
    public void ID ()
    {
      Assert.That (_endPoint.ID, Is.SameAs (_endPointID));
    }

    [Test]
    public void GetDomainObject ()
    {
      var domainObject = _endPoint.GetDomainObject ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Unchanged));
      Assert.That (domainObject, Is.SameAs (Order.GetObject (DomainObjectIDs.Order1)));
    }

    [Test]
    public void GetDomainObject_Null ()
    {
      var domainObject = _endPointWithNullObject.GetDomainObject ();

      Assert.That (domainObject, Is.Null);
    }

    [Test]
    public void GetDomainObject_Deleted ()
    {
      var order1 = Order.GetObject (_endPoint.ObjectID);
      order1.Delete ();

      Assert.That (order1.State, Is.EqualTo (StateType.Deleted));

      var domainObject = _endPoint.GetDomainObject ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Deleted));
      Assert.That (domainObject, Is.SameAs (order1));
    }

    [Test]
    public void GetDomainObject_Invalid ()
    {
      var domainObject = _endPointWithInvalidObject.GetDomainObject ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Invalid));
      Assert.That (domainObject, Is.SameAs (_invalidObject));
    }

    [Test]
    public void GetDomainObjectReference ()
    {
      var domainObject = _endPoint.GetDomainObjectReference ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.NotLoadedYet));
      Assert.That (domainObject, Is.SameAs (LifetimeService.GetObjectReference (ClientTransactionMock, DomainObjectIDs.Order1)));
    }

    [Test]
    public void GetDomainObjectReference_Null ()
    {
      var domainObject = _endPointWithNullObject.GetDomainObjectReference ();

      Assert.That (domainObject, Is.Null);
    }

    [Test]
    public void GetDomainObjectReference_Invalid ()
    {
      var domainObject = _endPointWithInvalidObject.GetDomainObjectReference ();

      Assert.That (domainObject.State, Is.EqualTo (StateType.Invalid));
      Assert.That (domainObject, Is.SameAs (_invalidObject));
    }

  }
}