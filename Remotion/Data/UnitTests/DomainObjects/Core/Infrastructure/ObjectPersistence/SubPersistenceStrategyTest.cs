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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Data.DomainObjects;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectPersistence
{
  [TestFixture]
  public class SubPersistenceStrategyTest : ClientTransactionBaseTest
  {
    private IUnlockedParentTransactionContext _unlockedParentTransactionContextMock;
    private SubPersistenceStrategy _persistenceStrategy;
    private IQuery _queryStub;
    private IParentTransactionContext _parentTransactionContextMock;

    public override void SetUp ()
    {
      base.SetUp ();

      _parentTransactionContextMock = MockRepository.GenerateStrictMock<IParentTransactionContext> ();
      _unlockedParentTransactionContextMock = MockRepository.GenerateStrictMock<IUnlockedParentTransactionContext> ();
      _persistenceStrategy = new SubPersistenceStrategy (_parentTransactionContextMock);

      _queryStub = MockRepository.GenerateStub<IQuery>();
    }

    [Test]
    public void ExecuteCustomQuery ()
    {
      var fakeResult = new IQueryResultRow[0];
      
      _parentTransactionContextMock
          .Expect (mock => mock.ExecuteCustomQuery (_queryStub))
          .Return (fakeResult);

      var result = _persistenceStrategy.ExecuteCustomQuery (_queryStub);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void ExecuteScalarQuery ()
    {
      var fakeResult = new object();

      _parentTransactionContextMock
          .Expect (mock => mock.ExecuteScalarQuery (_queryStub))
          .Return (fakeResult);

      var result = _persistenceStrategy.ExecuteScalarQuery (_queryStub);

      _parentTransactionContextMock.VerifyAllExpectations ();
      Assert.That (result, Is.SameAs (fakeResult));
    }

    [Test]
    public void PersistData_NewDataContainer ()
    {
      var instance = DomainObjectMother.CreateFakeObject<Order> ();
      var dataContainer = DataContainer.CreateNew (instance.ID);
      SetPropertyValue (dataContainer, typeof (Order), "OrderNumber", 12);
      dataContainer.SetDomainObject (instance);

      var persistableData = new PersistableData (instance, StateType.New, dataContainer, new IRelationEndPoint[0]);

      _parentTransactionContextMock.Stub (mock => mock.IsInvalid (instance.ID)).Return (true);
      _parentTransactionContextMock.Expect (mock => mock.UnlockParentTransaction ()).Return (_unlockedParentTransactionContextMock);
      _parentTransactionContextMock.Stub (stub => stub.GetDataContainerWithoutLoading (instance.ID)).Return (null);

      using (_unlockedParentTransactionContextMock.GetMockRepository ().Ordered ())
      {
        _unlockedParentTransactionContextMock.Expect (mock => mock.MarkNotInvalid (instance.ID));
        _unlockedParentTransactionContextMock
            .Expect (mock => mock.RegisterDataContainer (Arg<DataContainer>.Is.Anything))
            .WhenCalled (
                mi =>
                {
                  var dc = (DataContainer) mi.Arguments[0];
                  Assert.That (dc.ID, Is.EqualTo (instance.ID));
                  Assert.That (dc.State, Is.EqualTo (StateType.New));
                  Assert.That (dc.HasDomainObject, Is.True);
                  Assert.That (dc.DomainObject, Is.SameAs (instance));
                  Assert.That (GetPropertyValue (dc, typeof (Order), "OrderNumber"), Is.EqualTo (12));
                }
            );
        _unlockedParentTransactionContextMock.Expect (mock => mock.Dispose());
      }

      _persistenceStrategy.PersistData (Array.AsReadOnly (new[] { persistableData }));

      _parentTransactionContextMock.VerifyAllExpectations ();
      _unlockedParentTransactionContextMock.VerifyAllExpectations ();
    }
  }
}