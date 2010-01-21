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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.EndPointModifications;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.EndPointModifications
{
  [TestFixture]
  public class NullEndPointModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private IEndPoint _endPointMock;
    private IEndPoint _oldEndPointStub;
    private IEndPoint _newEndPointStub;
    private Order _oldRelatedObject;
    private Order _newRelatedObject;
    private NullEndPointModification _modification;
    private RelationEndPointID _id;

    public override void SetUp ()
    {
      base.SetUp ();
      _mockRepository = new MockRepository ();
      _id = new RelationEndPointID (
          DomainObjectIDs.Computer1,
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Computer), "Employee"));

      _endPointMock = _mockRepository.StrictMock<IEndPoint> ();

      _oldRelatedObject = Order.GetObject (DomainObjectIDs.Order1);
      _newRelatedObject = Order.GetObject (DomainObjectIDs.Order2);

      _oldEndPointStub = _mockRepository.Stub<IEndPoint> ();
      _oldEndPointStub.Stub (stub => stub.ObjectID).Return (_oldRelatedObject.ID);
      _oldEndPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);
      _oldEndPointStub.Replay ();

      _newEndPointStub = _mockRepository.Stub<IEndPoint> ();
      _newEndPointStub.Stub (stub => stub.ObjectID).Return (_newRelatedObject.ID);
      _newEndPointStub.Stub (stub => stub.ClientTransaction).Return (ClientTransactionMock);
      _newEndPointStub.Replay ();

      _modification = new NullEndPointModification (_endPointMock, _oldRelatedObject, _newRelatedObject);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.ModifiedEndPoint);
      Assert.AreSame (_oldRelatedObject, _modification.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, _modification.NewRelatedObject);
    }

    [Test]
    public void Initialization_FromNullObjectEndPoint ()
    {
      var endPoint = new NullObjectEndPoint (ClientTransactionMock, _id.Definition);
      var modification = (RelationEndPointModification) endPoint.CreateSetModification (_newRelatedObject);
      Assert.IsInstanceOfType (typeof (NullEndPointModification), modification);
      Assert.AreSame (endPoint, modification.ModifiedEndPoint);
      Assert.IsNull (modification.OldRelatedObject);
      Assert.AreSame (_newRelatedObject, modification.NewRelatedObject);
    }

    [Test]
    public void BeginDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.Begin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void PerformDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.Perform ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void EndDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.End ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfBeginDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.NotifyClientTransactionOfBegin ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void NotifyClientTransactionOfEndDoesNothing ()
    {
      _mockRepository.ReplayAll ();

      _modification.NotifyClientTransactionOfEnd ();

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ExtendToAllRelatedObjects ()
    {
      var result = _modification.ExtendToAllRelatedObjects ();

      Assert.That (result, Is.SameAs (_modification));
    }
  }
}
