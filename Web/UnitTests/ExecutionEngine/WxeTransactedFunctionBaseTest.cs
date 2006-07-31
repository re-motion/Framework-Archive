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
using Rhino.Mocks;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;
using Rubicon.Collections;
using Rubicon.Data;
using Rubicon.Web.UnitTests.AspNetFramework;

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
    private ITransaction _mockOtherTransaction;
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
      _mockOtherTransaction = _mocks.CreateMock<ITransaction> ();
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
        ExpectCommitAndReleaseTransaction (_mockWxeTransaction, _mockTransaction, _mockOtherTransaction);
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
          Is.Same (_wxeTransactedFunction), 
          Is.NotNull () & Property.Value ("Transaction", _mockTransaction));
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

      BinaryFormatter formatter = new BinaryFormatter ();
      Stream stream = new MemoryStream ();
      formatter.Serialize (stream, wxeTransactedFunction);
      stream.Position = 0;
      WxeTransactedFunctionMock deserialized = (WxeTransactedFunctionMock) formatter.Deserialize (stream);

      Assert.IsNotNull (deserialized);
    }

    private void ExpectCommitAndReleaseTransaction (ProxyWxeTransaction wxeTransaction, ITransaction transaction, ITransaction currentTransaction)
    {
      Expect.Call (wxeTransaction.Proxy_CurrentTransaction).Return (currentTransaction);
      wxeTransaction.Proxy_SetCurrentTransaction (transaction);
      wxeTransaction.Proxy_OnTransactionCommitting ();
      transaction.Commit ();
      wxeTransaction.Proxy_OnTransactionCommitted ();
      wxeTransaction.Proxy_SetCurrentTransaction (currentTransaction);
      transaction.Release ();
    }

    private void ExpectRestorePreviousCurrentTransaction (ProxyWxeTransaction wxeTransaction, ITransaction previousTransaction)
    {
      wxeTransaction.Proxy_SetCurrentTransaction (previousTransaction);
    }
  }
}
