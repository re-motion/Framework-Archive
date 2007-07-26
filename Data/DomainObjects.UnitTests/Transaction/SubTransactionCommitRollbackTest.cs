using System;
using NUnit.Framework;
using Rubicon.Data.DomainObjects.DataManagement;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Transaction
{
  [TestFixture]
  public class SubTransactionCommitRollbackTest : ClientTransactionBaseTest
  {
    private ClientTransaction _subTransaction;

    public override void SetUp ()
    {
      base.SetUp ();
      _subTransaction = ClientTransactionMock.CreateSubTransaction ();
    }

    [Test]
    public void ReturnToParentTransactionReturnsTrue ()
    {
      Assert.AreEqual (true, _subTransaction.ReturnToParentTransaction ());
    }

    [Test]
    public void ReturnToParentTransactionMakesParentWriteable ()
    {
      Assert.IsTrue (_subTransaction.ParentTransaction.IsReadOnly);
      Assert.IsFalse (_subTransaction.IsDiscarded);
      _subTransaction.ReturnToParentTransaction ();
      Assert.IsTrue (_subTransaction.IsDiscarded);
      Assert.IsFalse (_subTransaction.ParentTransaction.IsReadOnly);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "The subtransaction can no longer be used because control has "
        + "returned to its parent transaction.")]
    public void ReturnToParentTransactionRendersSubTransactionUnusable ()
    {
      _subTransaction.ReturnToParentTransaction ();
      using (_subTransaction.EnterScope ())
      {
        Order.NewObject ();
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterRollback ()
    {
      _subTransaction.Rollback ();
      Assert.IsFalse (_subTransaction.IsDiscarded);
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }

    [Test]
    public void SubTransactionCanContinueToBeUsedAfterCommit ()
    {
      _subTransaction.Commit ();
      Assert.IsFalse (_subTransaction.IsDiscarded);
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.NewObject ();
        Assert.IsNotNull (order);
      }
    }

    [Test]
    [ExpectedException (typeof (ObjectDiscardedException), ExpectedMessage = "Object 'Order.*' is already discarded.", MatchType = MessageMatch.Regex)]
    public void RollbackResetsNewedObjects ()
    {
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.NewObject();
        _subTransaction.Rollback();
        int i = order.OrderNumber;
      }
    }

    [Test]
    public void RollbackResetsLoadedObjects ()
    {
      using (_subTransaction.EnterScope ())
      {
        Order order = Order.GetObject (DomainObjectIDs.Order1);
        order.OrderNumber = 5;

        _subTransaction.Rollback ();

        Assert.AreNotEqual (5, order.OrderNumber);
      }
    }

    [Test]
    public void SubRollbackDoesNotRollbackParent ()
    {
      _subTransaction.ReturnToParentTransaction ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      Assert.AreEqual (1, order.OrderNumber);
      order.OrderNumber = 3;
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        order.OrderNumber = 5;
        ClientTransactionScope.CurrentTransaction.Rollback ();
        Assert.AreEqual (3, order.OrderNumber);
      }
      Assert.AreEqual (3, order.OrderNumber);
      ClientTransactionMock.Rollback ();
      Assert.AreEqual (1, order.OrderNumber);
    }


    [Test]
    public void ParentTransactionStillReadOnlyAfterCommit ()
    {
      using (_subTransaction.EnterScope ())
      {
        Assert.IsTrue (ClientTransactionMock.IsReadOnly);
        ClassWithAllDataTypes classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
        Assert.AreNotEqual (7, classWithAllDataTypes.Int32Property);
        classWithAllDataTypes.Int32Property = 7;
        _subTransaction.Commit ();
        Assert.IsTrue (ClientTransactionMock.IsReadOnly);
      }
    }
    
    [Test]
    public void CommitPropagatesNewObjectsToParentTransaction ()
    {
      ClassWithAllDataTypes classWithAllDataTypes;
      using (_subTransaction.EnterScope ())
      {
        classWithAllDataTypes = ClassWithAllDataTypes.NewObject ();
        Assert.AreNotEqual (7, classWithAllDataTypes.Int32Property);
        classWithAllDataTypes.Int32Property = 7;
        _subTransaction.Commit ();
        Assert.AreEqual (7, classWithAllDataTypes.Int32Property);
      }
      Assert.IsNotNull (classWithAllDataTypes);
      Assert.AreEqual (7, classWithAllDataTypes.Int32Property);
    }

    [Test]
    public void CommitPropagatesChangedObjectsToParentTransaction ()
    {
      Order order;
      using (_subTransaction.EnterScope ())
      {
        order = Order.GetObject (DomainObjectIDs.Order1);
        order.OrderNumber = 5;

        _subTransaction.Commit ();

        Assert.AreEqual (5, order.OrderNumber);
      }

      Assert.IsNotNull (order);
      Assert.AreEqual (5, order.OrderNumber);
    }

    [Test]
    public void SubCommitDoesNotCommitParent ()
    {
      _subTransaction.ReturnToParentTransaction ();
      Order order = Order.GetObject (DomainObjectIDs.Order1);
      using (ClientTransactionMock.CreateSubTransaction ().EnterScope ())
      {
        order.OrderNumber = 5;
        ClientTransactionScope.CurrentTransaction.Commit ();
      }
      Assert.AreEqual (5, order.OrderNumber);
      ClientTransactionMock.Rollback ();
      Assert.AreEqual (1, order.OrderNumber);
    }
  }
}