using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Data;
using Rubicon.Web.ExecutionEngine;
using Mocks_Is = Rhino.Mocks.Constraints.Is;
using Mocks_List = Rhino.Mocks.Constraints.List;
using Mocks_Property = Rhino.Mocks.Constraints.Property;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{
  public interface IWxeTransactionFunctionEventSink
  {
    void Handler (object sender, EventArgs e);
    void Handler (object sender, ITransaction transaction);
  }

  [TestFixture]
  public class WxeTransactedFunctionBaseTest : WxeTest
  {
    private MockRepository _mocks;
    private WxeTransactedFunctionMock _wxeTransactedFunction;
    private ProxyWxeTransaction _mockWxeTransaction;
    private ITransaction _mockTransaction;
    private ITransaction _mockPreviousTransaction;
    private IWxeTransactionFunctionEventSink _mockEventSink;

    //private WxeTransactionMock _rootWxeTransaction;

    //private WxeTransactionMock _parentWxeTransaction;
    //private TestTransaction _parentTransaction;

    //private WxeTransactionMock _childWxeTransaction;

    private string _events;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _mocks = new MockRepository ();
      _mockWxeTransaction = _mocks.PartialMock<ProxyWxeTransaction> ();
      _wxeTransactedFunction = new WxeTransactedFunctionMock (_mockWxeTransaction);
      _wxeTransactedFunction.Committing += delegate { _events += " committing"; };
      _wxeTransactedFunction.Committed += delegate { _events += " committed"; };
      _wxeTransactedFunction.RollingBack += delegate { _events += " rollingBack"; };
      _wxeTransactedFunction.RolledBack += delegate { _events += " rolledBack"; };

      _mockTransaction = _mocks.CreateMock<ITransaction> ();
      _mockPreviousTransaction = _mocks.CreateMock<ITransaction> ();

      _mockEventSink = _mocks.CreateMock<IWxeTransactionFunctionEventSink> ();
    }

    [Test]
    public void Execute ()
    {
      using (_mocks.Ordered ())
      {
        Expect.Call (_mockWxeTransaction.Proxy_CurrentTransaction).Return (_mockPreviousTransaction);
        Expect.Call (_mockWxeTransaction.Proxy_CreateRootTransaction ()).Return (_mockTransaction);
        _mockWxeTransaction.Proxy_SetCurrentTransaction (_mockTransaction);
        Expect.Call (_mockWxeTransaction.Proxy_CurrentTransaction).Return (_mockTransaction);
        ExpectCommitAndReleaseTransaction (_mockWxeTransaction, _mockTransaction, _mockTransaction);
        ExpectRestorePreviousCurrentTransaction (_mockWxeTransaction, _mockPreviousTransaction);
      }

      _mocks.ReplayAll ();

      _wxeTransactedFunction.Execute (CurrentWxeContext);

      _mocks.VerifyAll ();
    }

    [Test]
    public void TestOnCreating ()
    {
      _wxeTransactedFunction.TransactionCreating += _mockEventSink.Handler;
      _mockWxeTransaction.Execute (CurrentWxeContext);
      _mockEventSink.Handler (_wxeTransactedFunction, EventArgs.Empty);
      _mocks.ReplayAll ();
      _wxeTransactedFunction.Execute (CurrentWxeContext);

      _mockWxeTransaction.Proxy_OnTransactionCreating ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void TestOnCreated ()
    {
      _wxeTransactedFunction.TransactionCreated += _mockEventSink.Handler;
      _mockWxeTransaction.Execute (CurrentWxeContext);
      _mockEventSink.Handler (null, (WxeTransactedFunctionEventArgs<ITransaction>) null);
      LastCall.Constraints (
          Mocks_Is.Same (_wxeTransactedFunction),
          Mocks_Is.NotNull () & Mocks_Property.Value ("Transaction", _mockTransaction));
      _mocks.ReplayAll ();
      _wxeTransactedFunction.Execute (CurrentWxeContext);

      _mockWxeTransaction.Proxy_OnTransactionCreated (_mockTransaction);

      _mocks.VerifyAll ();
    }

    [Test]
    public void TestOnCommitting ()
    {
      _wxeTransactedFunction.Committing += _mockEventSink.Handler;
      _mockWxeTransaction.Execute (CurrentWxeContext);
      _mockEventSink.Handler (_wxeTransactedFunction, EventArgs.Empty);
      _mocks.ReplayAll ();
      _wxeTransactedFunction.Execute (CurrentWxeContext);

      _mockWxeTransaction.Proxy_OnTransactionCommitting ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void TestOnCommitted ()
    {
      _wxeTransactedFunction.Committed += _mockEventSink.Handler;
      _mockWxeTransaction.Execute (CurrentWxeContext);
      _mockEventSink.Handler (_wxeTransactedFunction, EventArgs.Empty);
      _mocks.ReplayAll ();
      _wxeTransactedFunction.Execute (CurrentWxeContext);

      _mockWxeTransaction.Proxy_OnTransactionCommitted ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void TestOnRollingBack ()
    {
      _wxeTransactedFunction.RollingBack += _mockEventSink.Handler;
      _mockWxeTransaction.Execute (CurrentWxeContext);
      _mockEventSink.Handler (_wxeTransactedFunction, EventArgs.Empty);
      _mocks.ReplayAll ();
      _wxeTransactedFunction.Execute (CurrentWxeContext);

      _mockWxeTransaction.Proxy_OnTransactionRollingBack ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void TestOnTransactionRolledBack ()
    {
      _wxeTransactedFunction.RolledBack += _mockEventSink.Handler;
      _mockWxeTransaction.Execute (CurrentWxeContext);
      _mockEventSink.Handler (_wxeTransactedFunction, EventArgs.Empty);
      _mocks.ReplayAll ();
      _wxeTransactedFunction.Execute (CurrentWxeContext);

      _mockWxeTransaction.Proxy_OnTransactionRolledBack ();

      _mocks.VerifyAll ();
    }

    [Test]
    public void TestSerialization ()
    {
      WxeTransactedFunctionMock wxeTransactedFunction = new WxeTransactedFunctionMock (null);
      wxeTransactedFunction.TransactionCreating += _mockEventSink.Handler;
      wxeTransactedFunction.TransactionCreated += _mockEventSink.Handler;
      wxeTransactedFunction.Committing += _mockEventSink.Handler;
      wxeTransactedFunction.Committed += _mockEventSink.Handler;
      wxeTransactedFunction.RollingBack += _mockEventSink.Handler;
      wxeTransactedFunction.RolledBack += _mockEventSink.Handler;

      WxeTransactedFunctionMock deserialized;
      using (Stream stream = new MemoryStream ())
      {
        BinaryFormatter formatter = new BinaryFormatter ();
        formatter.Serialize (stream, wxeTransactedFunction);
        stream.Position = 0;
        deserialized = (WxeTransactedFunctionMock) formatter.Deserialize (stream);
      }
      Assert.IsNotNull (deserialized);
    }

    private void ExpectCommitAndReleaseTransaction (ProxyWxeTransaction wxeTransaction, ITransaction newTransaction, ITransaction currentTransaction)
    {
      wxeTransaction.Proxy_SetCurrentTransaction (newTransaction);

      wxeTransaction.Proxy_OnTransactionCommitting ();
      newTransaction.Commit ();
      wxeTransaction.Proxy_OnTransactionCommitted ();
      
      wxeTransaction.Proxy_SetPreviousCurrentTransaction (currentTransaction);
      Expect.Call (wxeTransaction.Proxy_CurrentTransaction).Return (currentTransaction);
      newTransaction.Release ();
    }

    private void ExpectRestorePreviousCurrentTransaction (ProxyWxeTransaction wxeTransaction, ITransaction previousTransaction)
    {
      wxeTransaction.Proxy_SetPreviousCurrentTransaction (previousTransaction);
      Expect.Call (wxeTransaction.Proxy_CurrentTransaction).Return (previousTransaction);
    }

    [Test]
    public void MyTransactionAndExecutionTransaction ()
    {
      ProxyWxeTransaction outerTransaction = _mocks.CreateMock<ProxyWxeTransaction> ();
      outerTransaction.Transaction = _mocks.CreateMock<ITransaction> ();
      Assert.IsNotNull (outerTransaction.Transaction);
      WxeTransactedFunctionMock outerFunction = new WxeTransactedFunctionMock (outerTransaction);
      outerFunction.InitiateCreateTransaction ();

      WxeTransactedFunctionMock mediumNullFunction = new WxeTransactedFunctionMock (null);
      outerFunction.Add (mediumNullFunction);
      mediumNullFunction.InitiateCreateTransaction ();

      ProxyWxeTransaction innerTransaction = _mocks.CreateMock<ProxyWxeTransaction> ();
      innerTransaction.Transaction = _mocks.CreateMock<ITransaction> ();
      Assert.IsNotNull (innerTransaction.Transaction);
      WxeTransactedFunctionMock innerFunction = new WxeTransactedFunctionMock (innerTransaction);
      mediumNullFunction.Add (innerFunction);
      innerFunction.InitiateCreateTransaction ();

      Assert.AreSame (outerTransaction.Transaction, outerFunction.MyTransaction);
      Assert.AreSame (outerTransaction.Transaction, outerFunction.Transaction);

      Assert.IsNull (mediumNullFunction.MyTransaction);
      Assert.AreSame (outerTransaction.Transaction, mediumNullFunction.Transaction);

      Assert.AreSame (innerTransaction.Transaction, innerFunction.MyTransaction);
      Assert.AreSame (innerTransaction.Transaction, innerFunction.Transaction);
    }

    [Test]
    public void NullMyTransactionAndExecutionTransaction ()
    {
      WxeTransactedFunctionMock outerFunction = new WxeTransactedFunctionMock (null);
      outerFunction.InitiateCreateTransaction ();

      Assert.IsNull (outerFunction.MyTransaction);
      Assert.IsNull (outerFunction.Transaction);
    }

    [Test]
    public void ThreadAbortExceptionInNestedFunction ()
    {
      TestTransaction originalTransaction = new TestTransaction ();
      TestTransaction.Current = originalTransaction;

      TestTransactedFunctionWithThreadAbort nestedFunction = new TestTransactedFunctionWithThreadAbort ();
      TestTransactedFunctionWithNestedFunction parentFunction = new TestTransactedFunctionWithNestedFunction (originalTransaction, nestedFunction);

      try
      {
        parentFunction.Execute (CurrentWxeContext);
        Assert.Fail ("Expected ThreadAbortException");
      }
      catch (ThreadAbortException)
      {
        Thread.ResetAbort ();
      }

      Assert.AreSame (originalTransaction, TestTransaction.Current);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsFalse (nestedFunction.SecondStepExecuted);
      Assert.IsTrue (nestedFunction.ThreadAborted);

      parentFunction.Execute (CurrentWxeContext);

      Assert.IsTrue (nestedFunction.FirstStepExecuted);
      Assert.IsTrue (nestedFunction.SecondStepExecuted);

      Assert.AreSame (originalTransaction, TestTransaction.Current);
    }
  }
}
