using System;
using System.IO;
using System.Web;
using System.Web.SessionState;
using System.Collections.Specialized;
using System.Reflection;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Data;
using Rubicon.Web.UnitTests.AspNetFramework;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

[TestFixture]
public class WxeTransactionBaseTest: WxeTest
{
  private WxeTransactionMock _wxeTransaction;
  
  private WxeTransactionMock _rootWxeTransaction;

  private WxeTransactionMock _parentWxeTransaction;
  private TestTransaction _parentTransaction;

  private WxeTransactionMock _childWxeTransaction;

  private string _events;

  [SetUp]
  public override void SetUp()
  {
    base.SetUp();

    _wxeTransaction = new WxeTransactionMock (null, false, false);

    _rootWxeTransaction = new WxeTransactionMock (null, false, true);
    _rootWxeTransaction.Transaction = new TestTransaction();
    ((TestTransaction) _rootWxeTransaction.Transaction).CanCreateChild = true;
    _rootWxeTransaction.TransactionCommitting += new EventHandler (WxeTransaction_TransactionCommitting);
    _rootWxeTransaction.TransactionCommitted += new EventHandler(WxeTransaction_TransactionCommitted);
    _rootWxeTransaction.TransactionRollingBack += new EventHandler(WxeTransaction_TransactionRollingBack);
    _rootWxeTransaction.TransactionRolledBack += new EventHandler(WxeTransaction_TransactionRolledBack);

    _parentWxeTransaction = new WxeTransactionMock (null, true, false);
    _parentTransaction = new TestTransaction();
    _parentTransaction.CanCreateChild = true;
    _parentWxeTransaction.Transaction = _parentTransaction;
    _childWxeTransaction = new WxeTransactionMock (null, true, false);
    _parentWxeTransaction.Add (_childWxeTransaction);

    _events = string.Empty;
  }

  [Test]
  public void TestCtor1()
  {
    WxeStepList steps = new WxeStepList();
    WxeStep step = new TestStep();
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
  public void TestCtor2()
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
  public void GetParentWxeTransaction()
  {
    Assert.AreSame (_parentWxeTransaction, _childWxeTransaction.ParentTransaction);
  }

  [Test]
  public void GetTransaction()
  {
    Assert.AreSame (_parentTransaction, _parentWxeTransaction.Transaction);
  }

  [Test]
  public void CreateChildTransaction()
  {
    TestTransaction parentTransaction = new TestTransaction();
    parentTransaction.CanCreateChild = true;
    ITransaction childTransaction = _wxeTransaction.CreateChildTransaction (parentTransaction);
    Assert.IsNotNull (childTransaction);
    Assert.IsTrue (childTransaction.IsChild);
    Assert.AreSame (parentTransaction, childTransaction.Parent);
  }

  [Test]
  public void CreateChildTransactionAsRootTransaction()
  {
    TestTransaction parentTransaction = new TestTransaction();
    parentTransaction.CanCreateChild = false;
    ITransaction childTransaction = _wxeTransaction.CreateChildTransaction (parentTransaction);
    Assert.IsNotNull (childTransaction);
    Assert.IsFalse (childTransaction.IsChild);
  }

  [Test]
  public void CreateTransactionAsRootTransactionBecauseOfNoParentWxeTransaction()
  {
    ITransaction transaction = _wxeTransaction.CreateTransaction();
    Assert.IsTrue (_wxeTransaction.HasCreatedRootTransaction);
    Assert.IsNotNull (transaction);
  }

  [Test]
  public void CreateTransactionAsRootTransactionBecauseOfNoParentTransaction()
  {
    _parentWxeTransaction.Transaction = null;
    ITransaction transaction = _childWxeTransaction.CreateTransaction();
    Assert.IsTrue (_childWxeTransaction.HasCreatedRootTransaction);
    Assert.IsNotNull (transaction);
  }

  [Test]
  public void CreateTransactionAsRootTransactionBecauseOfForceRoot()
  {
    _childWxeTransaction.ForceRoot = true;
    ITransaction transaction = _childWxeTransaction.CreateTransaction();
    Assert.IsTrue (_childWxeTransaction.HasCreatedRootTransaction);
    Assert.IsNotNull (transaction);
  }

  [Test]
  [ExpectedException (typeof (InvalidOperationException))]
  public void CreateTransactionAsChildTransactionWithInvalidCurrentTransaction()
  {
    TestTransaction.Current = new TestTransaction();
    ITransaction transaction = _childWxeTransaction.CreateTransaction();
    Assert.Fail();
  }

  [Test]
  public void CreateTransactionAsChildTransactionWithNoCurrentTransaction()
  {
    TestTransaction.Current = null;
    ITransaction transaction = _childWxeTransaction.CreateTransaction();
    Assert.IsFalse (_childWxeTransaction.HasCreatedRootTransaction);
    Assert.IsNotNull (transaction);
  }

  [Test]
  public void CreateTransactionAsChildTransactionWithCurrentTransaction()
  {
    TestTransaction.Current = (TestTransaction) _parentWxeTransaction.Transaction;
    ITransaction transaction = _childWxeTransaction.CreateTransaction();
    Assert.IsFalse (_childWxeTransaction.HasCreatedRootTransaction);
    Assert.IsNotNull (transaction);
  }

  [Test]
  public void TestOnTransactionCommitting()
  {
    _rootWxeTransaction.OnTransactionCommitting();
    Assert.AreEqual (" committing", _events);
  }

  [Test]
  public void TestOnTransactionCommitted()
  {
    _rootWxeTransaction.OnTransactionCommitted();
    Assert.AreEqual (" committed", _events);
  }

  [Test]
  public void TestOnTransactionRollingBack()
  {
    _rootWxeTransaction.OnTransactionRollingBack();
    Assert.AreEqual (" rollingBack", _events);
  }

  [Test]
  public void TestOnTransactionRolledBack()
  {
    _rootWxeTransaction.OnTransactionRolledBack();
    Assert.AreEqual (" rolledBack", _events);
  }

  [Test]
  public void RollBackTransaction()
  {
    TestTransaction rootTransaction = (TestTransaction) _rootWxeTransaction.Transaction;
    TestTransaction.Current = rootTransaction;
    rootTransaction.RolledBack += new EventHandler(Transaction_TransactionRolledBack);
    
    _rootWxeTransaction.RollbackTransaction();
    Assert.AreEqual (" rollingBack txRolledBack rolledBack", _events);
  }

  [Test]
  public void CommitTransaction()
  {
    TestTransaction rootTransaction = (TestTransaction) _rootWxeTransaction.Transaction;
    TestTransaction.Current = rootTransaction;
    rootTransaction.Committed += new EventHandler(Transaction_TransactionCommitted);
    
    _rootWxeTransaction.CommitTransaction();
    Assert.AreEqual (" committing txCommitted committed", _events);
  }

  [Test]
  public void CommitAndReleaseTransaction()
  {
    TestTransaction currentTransaction = new TestTransaction();
    TestTransaction.Current = currentTransaction;

    TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
    _rootWxeTransaction.CommitAndReleaseTransaction();

    Assert.IsTrue (transaction.IsCommitted);
    Assert.IsTrue (transaction.IsReleased);
    Assert.IsNull (_rootWxeTransaction.Transaction);
    Assert.AreSame (currentTransaction, TestTransaction.Current);
  }

  [Test]
  public void RollbackAndReleaseTransaction()
  {
    TestTransaction currentTransaction = new TestTransaction();
    TestTransaction.Current = currentTransaction;

    TestTransaction transaction = (TestTransaction) _rootWxeTransaction.Transaction;
    _rootWxeTransaction.RollbackAndReleaseTransaction();

    Assert.IsTrue (transaction.IsRolledBack);
    Assert.IsTrue (transaction.IsReleased);
    Assert.IsNull (_rootWxeTransaction.Transaction);
    Assert.AreSame (currentTransaction, TestTransaction.Current);
  }

  [Test]
  public void RestorePreviousCurrentTransaction()
  {
    _wxeTransaction.StartExecution();

    TestTransaction.Current = new TestTransaction();
    _wxeTransaction.Transaction = null;
    ITransaction previousCurrentTransaction = new TestTransaction();
    _wxeTransaction.PreviousCurrentTransaction = previousCurrentTransaction;
    _wxeTransaction.RestorePreviousCurrentTransaction();

    Assert.AreSame (previousCurrentTransaction, TestTransaction.Current);
    Assert.IsNull (_wxeTransaction.Transaction);
  }

  [Test]
  public void RestorePreviousCurrentTransactionOnlyOnce()
  {
    _wxeTransaction.StartExecution();

    TestTransaction.Current = new TestTransaction();
    _wxeTransaction.Transaction = null;
    _wxeTransaction.PreviousCurrentTransaction = new TestTransaction();
    _wxeTransaction.RestorePreviousCurrentTransaction();

    TestTransaction currentTransaction = new TestTransaction();
    TestTransaction.Current = currentTransaction;
    _wxeTransaction.RestorePreviousCurrentTransaction();

    Assert.AreSame (currentTransaction, TestTransaction.Current);
  }

  [Test]
  public void TestSerialization()
  {
    _parentWxeTransaction.AutoCommit = true;
    _parentWxeTransaction.ForceRoot = true;
    _parentWxeTransaction.IsPreviousCurrentTransactionRestored = true;
    _childWxeTransaction.Transaction = _parentWxeTransaction.Transaction.CreateChild();
    _childWxeTransaction.PreviousCurrentTransaction = _parentWxeTransaction.Transaction;

    BinaryFormatter formatter = new BinaryFormatter();
    Stream stream = new MemoryStream();
    formatter.Serialize(stream, _parentWxeTransaction);
    stream.Position = 0;
    WxeTransactionMock parentWxeTransaction = (WxeTransactionMock) formatter.Deserialize (stream);
    WxeTransactionMock childWxeTransaction = (WxeTransactionMock) parentWxeTransaction.Steps[0];

    Assert.IsNotNull (parentWxeTransaction);
    Assert.IsNotNull (childWxeTransaction);
    Assert.IsNotNull (parentWxeTransaction.Transaction);
    Assert.IsNotNull (childWxeTransaction.Transaction);
    Assert.AreSame (parentWxeTransaction.Transaction, childWxeTransaction.PreviousCurrentTransaction);
    Assert.IsTrue (parentWxeTransaction.AutoCommit);
    Assert.IsTrue (parentWxeTransaction.ForceRoot);
    Assert.IsTrue (parentWxeTransaction.IsPreviousCurrentTransactionRestored);
  }

  private void WxeTransaction_TransactionCommitting (object sender, EventArgs e)
  {
    _events += " committing";
  }

  private void WxeTransaction_TransactionCommitted (object sender, EventArgs e)
  {
    _events += " committed";
  }

  private void Transaction_TransactionCommitted (object sender, EventArgs e)
  {
    _events += " txCommitted";
  }

  private void WxeTransaction_TransactionRollingBack (object sender, EventArgs e)
  {
    _events += " rollingBack";
  }

  private void WxeTransaction_TransactionRolledBack (object sender, EventArgs e)
  {
    _events += " rolledBack";
  }
  private void Transaction_TransactionRolledBack (object sender, EventArgs e)
  {
    _events += " txRolledBack";
  }
}

}
