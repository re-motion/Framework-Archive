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
using NUnit.Framework;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class NonNotifyingBidirectionalRelationModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ObjectEndPoint _endPointMock;
    private RelationEndPointModification _modificationMock1;
    private RelationEndPointModification _modificationMock2;
    private RelationEndPointModification _modificationMock3;
    private Order _oldRelatedObject;
    private Order _newRelatedObject;
    private NonNotifyingBidirectionalRelationModification _collection;
    private RelationEndPointID _id;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _id = new RelationEndPointID (
          DomainObjectIDs.Computer1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "Employee"));

      _endPointMock = _mockRepository.StrictMock<ObjectEndPoint> (ClientTransactionMock, _id, DomainObjectIDs.Employee3);

      _oldRelatedObject = Order.GetObject (DomainObjectIDs.Order1);
      _newRelatedObject = Order.GetObject (DomainObjectIDs.Order2);

      _modificationMock1 = _mockRepository.StrictMock<RelationEndPointModification> (_endPointMock, _oldRelatedObject, _newRelatedObject);
      _modificationMock2 = _mockRepository.StrictMock<RelationEndPointModification> (_endPointMock, _oldRelatedObject, _newRelatedObject);
      _modificationMock3 = _mockRepository.StrictMock<RelationEndPointModification> (_endPointMock, _oldRelatedObject, _newRelatedObject);

      _collection = new NonNotifyingBidirectionalRelationModification (_modificationMock1, _modificationMock2, _modificationMock3);
    }

    [Test]
    public void Perform ()
    {
      _modificationMock1.Perform ();
      _modificationMock2.Perform ();
      _modificationMock3.Perform ();

      _mockRepository.ReplayAll ();

      _collection.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExecuteAllSteps ()
    {
      _modificationMock1.Expect (mock => mock.Perform());
      _modificationMock2.Expect (mock => mock.Perform ());
      _modificationMock3.Expect (mock => mock.Perform ());

      _mockRepository.ReplayAll ();

      _collection.ExecuteAllSteps ();

      _mockRepository.VerifyAll ();
    }
  }
}