using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.DataManagement
{
  [TestFixture]
  public class ObjectEndPointModificationTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    private ObjectEndPoint _endPointMock;
    private IEndPoint _oldEndPointMock;
    private IEndPoint _newEndPointMock;
    private ObjectEndPointModification _modification;
    private RelationEndPointID _id;

    public override void SetUp ()
    {
      base.SetUp();
      _mockRepository = new MockRepository();
      _id = new RelationEndPointID (
          DomainObjectIDs.Computer1,
          ReflectionUtility.GetPropertyName (typeof (Computer), "Employee"));

      _endPointMock = _mockRepository.CreateMock<ObjectEndPoint> (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      _oldEndPointMock = _mockRepository.CreateMock<IEndPoint>();
      _newEndPointMock = _mockRepository.CreateMock<IEndPoint>();

      _modification = new ObjectEndPointModification (_endPointMock, _oldEndPointMock, _newEndPointMock);
    }

    [Test]
    public void Initialization ()
    {
      Assert.AreSame (_endPointMock, _modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, _modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, _modification.NewEndPoint);
    }

    [Test]
    public void Initialization_FromEndPoint ()
    {
      RelationEndPoint endPoint = new ObjectEndPoint (ClientTransactionMock, _id, DomainObjectIDs.Employee3);
      RelationEndPointModification modification = endPoint.CreateModification (_oldEndPointMock, _newEndPointMock);
      Assert.IsInstanceOfType (typeof (ObjectEndPointModification), modification);
      Assert.AreSame (endPoint, modification.AffectedEndPoint);
      Assert.AreSame (_oldEndPointMock, modification.OldEndPoint);
      Assert.AreSame (_newEndPointMock, modification.NewEndPoint);
    }

    [Test]
    public void BeginInvokesBeginRelationChange ()
    {
      _endPointMock.BeginRelationChange (_oldEndPointMock, _newEndPointMock);

      _mockRepository.ReplayAll();

      _modification.Begin();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void PerformInvokesPerformRelationChange ()
    {
      _endPointMock.PerformRelationChange (_newEndPointMock);

      _mockRepository.ReplayAll();

      _modification.Perform();

      _mockRepository.VerifyAll();
    }

    [Test]
    public void EndInvokesEndRelationChange ()
    {
      _endPointMock.EndRelationChange();

      _mockRepository.ReplayAll();

      _modification.End();

      _mockRepository.VerifyAll();
    }
  }
}