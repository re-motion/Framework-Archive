using System;
using log4net.Config;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class CollectionEndPointModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private CollectionEndPoint _endPointMock;
    private IEndPoint _oldEndPointMock;
    private IEndPoint _newEndPointMock;
    private CollectionEndPointModification _modification;
    private RelationEndPointID _id;
    private CollectionEndPointChangeAgent _changeAgentMock;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();
      _id = new RelationEndPointID (
          DomainObjectIDs.Order1,
          ReflectionUtility.GetPropertyName (typeof (Order), "OrderItems"));

      _endPointMock = _mockRepository.CreateMock<CollectionEndPoint> (ClientTransactionMock, _id, new DomainObjectCollection());
      _oldEndPointMock = _mockRepository.Stub<IEndPoint>();
      _newEndPointMock = _mockRepository.Stub<IEndPoint>();
      _changeAgentMock = _mockRepository.CreateMock<CollectionEndPointChangeAgent>(new DomainObjectCollection(), _oldEndPointMock, _newEndPointMock,
          CollectionEndPointChangeAgent.OperationType.Add, 0);

      _modification = new CollectionEndPointModification (_endPointMock, _changeAgentMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, _modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, _modification.NewEndPoint);
      Assert.AreSame (_changeAgentMock, _modification.ChangeAgent);
    }

    [Test]
    public void Initialization_FromEndPoint_Add ()
    {
      RelationEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection());
      CollectionEndPointModification modification = (CollectionEndPointModification) endPoint.CreateModification (RelationEndPoint.CreateNullRelationEndPoint (_id.Definition), _newEndPointMock);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
      Assert.IsTrue (modification.OldEndPoint.IsNull);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Add, modification.ChangeAgent.Operation);
    }

    [Test]
    public void Initialization_FromEndPoint_Remove ()
    {
      RelationEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection ());
      CollectionEndPointModification modification = (CollectionEndPointModification) endPoint.CreateModification (_oldEndPointMock, RelationEndPoint.CreateNullRelationEndPoint (_id.Definition));
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.IsTrue (modification.NewEndPoint.IsNull);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Remove, modification.ChangeAgent.Operation);
    }

    [Test]
    public void Initialization_FromEndPoint_Insert ()
    {
      CollectionEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection ());
      CollectionEndPointModification modification = endPoint.CreateInsertModification (_oldEndPointMock,_newEndPointMock, 3);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Insert, modification.ChangeAgent.Operation);
      Assert.AreEqual (3, PrivateInvoke.GetNonPublicField (modification.ChangeAgent, "_collectionIndex"));
    }

    [Test]
    public void Initialization_FromEndPoint_Replace ()
    {
      CollectionEndPoint endPoint = new CollectionEndPoint (ClientTransactionMock, _id, new DomainObjectCollection ());
      CollectionEndPointModification modification = endPoint.CreateReplaceModification (_oldEndPointMock, _newEndPointMock);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (modification.OldEndPoint, modification.ChangeAgent.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
      Assert.AreSame (modification.NewEndPoint, modification.ChangeAgent.NewEndPoint);
      Assert.AreEqual (CollectionEndPointChangeAgent.OperationType.Replace, modification.ChangeAgent.Operation);
    }

    [Test]
    public void BeginInvokesBeginRelationChange ()
    {
      _changeAgentMock.BeginRelationChange();
      _endPointMock.BeginRelationChange (_oldEndPointMock, _newEndPointMock);

      _mockRepository.ReplayAll();

      _modification.Begin();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PerformInvokesPerformRelationChange ()
    {
      _endPointMock.PerformRelationChange ();

      _mockRepository.ReplayAll();

      _modification.Perform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void EndInvokesEndRelationChange ()
    {
      _changeAgentMock.EndRelationChange ();
      _endPointMock.EndRelationChange ();

      _mockRepository.ReplayAll();

      _modification.End();

      _mockRepository.VerifyAll();
    }
  }
}