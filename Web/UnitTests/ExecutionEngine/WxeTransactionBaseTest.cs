using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Rubicon.Data;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

  [TestFixture]
  public class WxeTransactionBaseTest : WxeTest
  {
    private WxeTransactionMock _wxeTransaction;

    private WxeTransactionMock _rootWxeTransaction;

    private WxeTransactionMock _parentWxeTransaction;
    private TestTransaction _parentTransaction;

    private WxeTransactionMock _childWxeTransaction;

    private string _events;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _wxeTransaction = new WxeTransactionMock (null, false, false);

      _rootWxeTransaction = new WxeTransactionMock (null, false, true);
      _rootWxeTransaction.Transaction = new TestTransaction ();
      _rootWxeTransaction.PublicSetCurrentTransaction (_rootWxeTransaction.Transaction);
      ((TestTransaction) _rootWxeTransaction.Transaction).CanCreateChild = true;
      _rootWxeTransaction.TransactionCommitting += delegate { _events += " committing"; };
      _rootWxeTransaction.TransactionCommitted += delegate { _events += " committed"; };
      _rootWxeTransaction.TransactionRollingBack += delegate { _events += " rollingBack"; };
      _rootWxeTransaction.TransactionRolledBack += delegate { _events += " rolledBack"; };

      _parentWxeTransaction = new WxeTransactionMock (null, true, false); // this simulates Execute
      _parentTransaction = new TestTransaction ();
      _parentTransaction.CanCreateChild = true;
      _parentWxeTransaction.Transaction = _parentTransaction;
      _parentWxeTransaction.PublicSetCurrentTransaction (_parentTransaction);

      _childWxeTransaction = new WxeTransactionMock (null, true, false);
      _parentWxeTransaction.Add (_childWxeTransaction);

      _events = string.Empty;
    }

    [Test]
    public void TestCtor1 ()
    {
      WxeStepList steps = new WxeStepList ();
      WxeStep step = new TestStep ();
      steps.Add (step);
      bool autoCommit = true;
      bool forceRoot = false;
      WxeTransactionMock wxeTransaction = new WxeTransactionMock (steps, autoCommit, forceRoot);

      Assert.AreEqual (1, wxeTransaction.Steps.Count);
      Assert.AreSame (step, wxeTransaction.Steps[0]);
      Assert.AreEqual (autoCommit, wxeTransaction.AutoCommit);
      Assert.AreEqual (forceRoot, wxeTransaction.ForceRoot);
    }

    [Test]
    public void TestCtor2 ()
    {
      WxeStepList steps = null;
      bool autoCommit = false;
      bool forceRoot = true;
      WxeTransactionMock wxeTransaction = new WxeTransactionMock (steps, autoCommit, forceRoot);

      Assert.AreEqual (0, wxeTransaction.Steps.Count);
      Assert.AreEqual (autoCommit, wxeTransaction.AutoCommit);
      Assert.AreEqual (forceRoot, wxeTransaction.ForceRoot);
    }

    [Test]
    public void GetParentWxeTransaction ()
    {
      Assert.AreSame (_parentWxeTransaction, _childWxeTransaction.GetParentTransaction ());
    }

    [Test]
    public void GetParentTransactedFunction ()
    {
      TestFunctionWithSpecificTransaction function = new TestFunctionWithSpecificTransaction (new TestTransaction ());
      function.Execute (CurrentWxeContext);
      TestWxeTransaction transaction = function.WxeTransaction;
      Assert.IsNotNull (transaction);
      Assert.AreSame (function, transaction.GetParentTransactedFunction());
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "This object has not yet been encapsulated by a transacted function.")]
    public void GetParentTransactedFunctionThrowsIfNoFunction ()
    {
      TestWxeTransaction transaction = new TestWxeTransaction ();
      transaction.GetParentTransactedFunction ();
    }

    [Test]
    public void GetTransaction ()
    {
      Assert.AreSame (_parentTransaction, _parentWxeTransaction.Transaction);
    }

    [Test]
    public void CreateChildTransaction ()
    {
      TestTransaction parentTransaction = new TestTransaction ();
      parentTransaction.CanCreateChild = true;
      ITransaction childTransaction = _wxeTransaction.CreateChildTransaction (parentTransaction);
      Assert.IsNotNull (childTransaction);
      Assert.IsTrue (childTransaction.IsChild);
      Assert.AreSame (parentTransaction, childTransaction.Parent);
    }

    [Test]
    public void CreateChildTransactionAsRootTransaction ()
    {
      TestTransaction parentTransaction = new TestTransaction ();
      parentTransaction.CanCreateChild = false;
      ITransaction childTransaction = _wxeTransaction.CreateChildTransaction (parentTransaction);
      Assert.IsNotNull (childTransaction);
      Assert.IsFalse (childTransaction.IsChild);
    }

    [Test]
    public void CreateTransactionAsRootTransactionBecauseOfNoParentWxeTransaction ()
    {
      ITransaction transaction = _wxeTransaction.CreateTransaction ();
      Assert.IsTrue (_wxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void CreateTransactionAsRootTransactionBecauseOfNoParentTransaction ()
    {
      _parentWxeTransaction.Transaction = null;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsTrue (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void CreateTransactionAsRootTransactionBecauseOfForceRoot ()
    {
      _childWxeTransaction.ForceRoot = true;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsTrue (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void CreateTransactionAsChildTransactionWithInvalidCurrentTransaction ()
    {
      TestTransaction.Current = new TestTransaction ();
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
    }

    [Test]
    public void CreateTransactionAsChildTransactionWithNoCurrentTransaction ()
    {
      TestTransaction.Current = null;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsFalse (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void CreateTransactionAsChildTransactionWithCurrentTransaction ()
    {
      TestTransaction.Current = (TestTransaction) _parentWxeTransaction.Transaction;
      ITransaction transaction = _childWxeTransaction.CreateTransaction ();
      Assert.IsFalse (_childWxeTransaction.HasCreatedRootTransaction);
      Assert.IsNotNull (transaction);
    }

    [Test]
    public void TestOnTransactionCreated ()
    {
      _wxeTransaction.TransactionCreating += delegate { _events += " creating"; };
      _wxeTransaction.TransactionCreated += delegate { _events += " created"; };
      TestTransaction transaction = _wxeTransaction.CreateTransaction ();

      Assert.AreEqual (" creating created", _events);
    }

    [Test]
    public void TestOnTransactionCommitting ()
    {
      _rootWxeTransaction.OnTransactionCommitting ();
      Assert.AreEqual (" committing", _events);
    }

    [Test]
    public void TestOnTransactionCommitted ()
    {
      _rootWxeTransaction.OnTransactionCommitted ();
      Assert.AreEqual (" committed", _events);
    }

    [Test]
    public void TestOnTransactionRollingBack ()
    {
      _rootWxeTransaction.OnTransactionRollingBack ();
      Assert.AreEqual (" rollingBack", _events);
    }

    [Test]
    public void TestOnTransactionRolledBack ()
    {
      _rootWxeTransaction.OnTransactionRolledBack ();
      Assert.AreEqual (" rolledBack", _events);
    }

    [Test]
    public void RollBackTransaction ()
    {
      TestTransaction rootTransaction = (TestTransaction) _rootWxeTransaction.Transaction;
      TestTransaction.Current = rootTransaction;
      rootTransaction.RolledBack += delegate { _events += " txRolledBack"; };

      _rootWxeTransaction.RollbackTransaction ();
      Assert.AreEqual (" rollingBack txRolledBack rolledBack", _events);
    }

    [Test]
    public void CommitTransaction ()
    {
      TestTransaction rootTransaction = (TestTransaction) _rootWxeTransaction.Transaction;
      TestTransaction.Current = rootTransaction;
      rootTransaction.Committed += delegate { _events += " txCommitted"; };

      _rootWxeTransaction.CommitTransaction ();
      Assert.AreEqual (" committing txCommitted committed", _events);
    }

    [Test]
    public void CommitAndReleaseTransaction ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      _rootWxeTransaction.CommitAndReleaseTransaction ();

      Assert.IsTrue (transaction.IsCommitted);
      Assert.IsTrue (transaction.IsReleased);
      Assert.IsNull (_rootWxeTransaction.Transaction);
      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void RollbackAndReleaseTransaction ()
    {
      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;

      TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
      _rootWxeTransaction.RollbackAndReleaseTransaction ();

      Assert.IsTrue (transaction.IsRolledBack);
      Assert.IsTrue (transaction.IsReleased);
      Assert.IsNull (_rootWxeTransaction.Transaction);
      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void RestorePreviousCurrentTransaction ()
    {
      _wxeTransaction.StartExecution ();

      TestTransaction previousCurrentTransaction = new TestTransaction ();
      TestTransaction.Current = previousCurrentTransaction;
      _wxeTransaction.PublicSetPreviousTransaction (previousCurrentTransaction);
      _wxeTransaction.PublicSetCurrentTransaction (null);

      Assert.AreNotSame (previousCurrentTransaction, TestTransaction.Current);
      
      _wxeTransaction.RestorePreviousCurrentTransaction ();

      Assert.AreSame (previousCurrentTransaction, TestTransaction.Current);
      Assert.IsNull (_wxeTransaction.Transaction);
    }

    [Test]
    public void RestorePreviousCurrentTransactionOnlyOnce ()
    {
      _wxeTransaction.StartExecution ();

      TestTransaction.Current = new TestTransaction ();
      _wxeTransaction.PublicSetPreviousTransaction (TestTransaction.Current);
      _wxeTransaction.PublicSetCurrentTransaction (null);
      _wxeTransaction.RestorePreviousCurrentTransaction ();

      TestTransaction currentTransaction = new TestTransaction ();
      TestTransaction.Current = currentTransaction;
      _wxeTransaction.RestorePreviousCurrentTransaction (); // should not do anything

      Assert.AreSame (currentTransaction, TestTransaction.Current);
    }

    [Test]
    public void TestSerialization ()
    {
      _parentWxeTransaction.AutoCommit = true;
      _parentWxeTransaction.ForceRoot = true;
      _parentWxeTransaction.IsPreviousCurrentTransactionRestored = true;
      _parentWxeTransaction.TransactionCreating += delegate { };
      _parentWxeTransaction.TransactionCreated += delegate { };
      _parentWxeTransaction.TransactionCommitting += delegate { };
      _parentWxeTransaction.TransactionCommitted += delegate { };
      _parentWxeTransaction.TransactionRollingBack += delegate { };
      _parentWxeTransaction.TransactionRolledBack += delegate { };

      _childWxeTransaction.Transaction = (TestTransaction) _parentWxeTransaction.Transaction.CreateChild ();
      _childWxeTransaction.PreviousTransactions.Push (_parentWxeTransaction.Transaction);

      WxeTransactionMock parentWxeTransaction;
      using (Stream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, _parentWxeTransaction);
        stream.Position = 0;
        parentWxeTransaction = (WxeTransactionMock) formatter.Deserialize (stream);
      }
      WxeTransactionMock childWxeTransaction = (WxeTransactionMock) parentWxeTransaction.Steps[0];

      Assert.IsNotNull (parentWxeTransaction);
      Assert.IsNotNull (childWxeTransaction);
      Assert.IsNotNull (parentWxeTransaction.Transaction);
      Assert.IsNotNull (childWxeTransaction.Transaction);
      Assert.AreSame (parentWxeTransaction.Transaction, childWxeTransaction.PreviousTransactions.Peek());
      Assert.AreEqual (1, childWxeTransaction.PreviousTransactions.Count);
      Assert.IsTrue (parentWxeTransaction.AutoCommit);
      Assert.IsTrue (parentWxeTransaction.ForceRoot);
      Assert.IsTrue (parentWxeTransaction.IsPreviousCurrentTransactionRestored);
    }
  }
}
