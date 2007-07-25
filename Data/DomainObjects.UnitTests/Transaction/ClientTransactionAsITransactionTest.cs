using System;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data.DomainObjects.Infrastructure;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class ClientTransactionAsITransactionTest : ClientTransactionBaseTest
  {
    private MockRepository _mockRepository;
    
    private ITransaction _transaction;
    private ITransaction _transactionMock;

    public override void SetUp()
    {
 	     base.SetUp();
      _mockRepository = new MockRepository();
     
      _transaction = ClientTransactionMock;
      _transactionMock = _mockRepository.CreateMock<ClientTransactionMock>();
    }

    [Test]
    public void CanCreateChild ()
    {
      Assert.IsTrue (_transaction.CanCreateChild);
    }

    [Test]
    public void CreateChild ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.IsNotNull (child);
      Assert.IsInstanceOfType (typeof (SubClientTransaction), child);
    }

    [Test]
    public void IsChild ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.IsTrue (child.IsChild);
      Assert.IsFalse (_transactionMock.IsChild);
      Assert.IsTrue (child.CreateChild().IsChild);
    }

    [Test]
    public void Parent ()
    {
      ITransaction child = _transaction.CreateChild ();
      Assert.AreSame (_transaction, child.Parent);
      Assert.AreSame (child, child.CreateChild ().Parent);
    }
  }
}