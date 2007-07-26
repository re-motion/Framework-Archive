using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Development.UnitTesting;
using Rubicon.Web.ExecutionEngine;
using NUnit.Framework;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

  /// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactionBase{TTransaction}"/> type. </summary>
  [Serializable]
  public class WxeTransactionMock : WxeTransactionBase<TestTransaction>
  {
    private bool _hasCreatedRootTransaction;
    private Stack<TestTransaction> _previousTransactions = new Stack<TestTransaction> ();

    public WxeTransactionMock (WxeStepList steps, bool autoCommit, bool forceRoot)
      : base (steps, autoCommit, forceRoot)
    {
    }

    protected override TestTransaction CreateRootTransaction ()
    {
      _hasCreatedRootTransaction = true;
      return new TestTransaction ();
    }

    protected override TestTransaction CurrentTransaction
    {
      get { return TestTransaction.Current; }
    }

    protected override void SetCurrentTransaction (TestTransaction transaction)
    {
      _previousTransactions.Push (TestTransaction.Current);
      TestTransaction.Current = transaction;
    }

    public void PublicSetCurrentTransaction (TestTransaction transaction)
    {
      SetCurrentTransaction (transaction);
    }

    protected override void RestorePreviousTransaction ()
    {
      Assert.IsNotEmpty (_previousTransactions);
      TestTransaction.Current = _previousTransactions.Pop ();
    }

    public ArrayList Steps
    {
      get { return (ArrayList) PrivateInvoke.GetNonPublicField (this, "_steps"); }
    }

    public void StartExecution ()
    {
      PrivateInvoke.SetNonPublicField (this, "_lastExecutedStep", 0);
    }

    public new bool AutoCommit
    {
      get { return base.AutoCommit; }
      set { PrivateInvoke.SetNonPublicField (this, "_autoCommit", value); }
    }

    public new bool ForceRoot
    {
      get { return base.ForceRoot; }
      set { PrivateInvoke.SetNonPublicField (this, "_forceRoot", value); }
    }

    public bool IsPreviousCurrentTransactionRestored
    {
      get { return (bool) PrivateInvoke.GetNonPublicField (this, "_isPreviousCurrentTransactionRestored"); }
      set { PrivateInvoke.SetNonPublicField (this, "_isPreviousCurrentTransactionRestored", value); }
    }

    public new TestTransaction Transaction
    {
      get { return base.Transaction; }
      set { PrivateInvoke.SetNonPublicField (this, "_transaction", value); }
    }

    public new TestTransaction CreateTransaction ()
    {
      return base.CreateTransaction ();
    }

    public new TestTransaction CreateChildTransaction (TestTransaction parentTransaction)
    {
      return base.CreateChildTransaction (parentTransaction);
    }

    public new WxeTransactionBase<TestTransaction> GetParentTransaction ()
    {
      return base.GetParentTransaction ();
    }

    public bool HasCreatedRootTransaction
    {
      get { return _hasCreatedRootTransaction; }
    }

    public Stack<TestTransaction> PreviousTransactions
    {
      get { return _previousTransactions; }
    }

    public new void OnTransactionCommitting ()
    {
      base.OnTransactionCommitting ();
    }

    public new void OnTransactionCommitted ()
    {
      base.OnTransactionCommitted ();
    }

    public new void OnTransactionRollingBack ()
    {
      base.OnTransactionRollingBack ();
    }

    public new void OnTransactionRolledBack ()
    {
      base.OnTransactionRolledBack ();
    }

    public void CommitTransaction ()
    {
      base.CommitTransaction (Transaction);
    }

    public void RollbackTransaction ()
    {
      base.RollbackTransaction (Transaction);
    }

    public new void CommitAndReleaseTransaction ()
    {
      base.CommitAndReleaseTransaction ();
    }

    public new void RollbackAndReleaseTransaction ()
    {
      base.RollbackAndReleaseTransaction ();
    }

    public new void RestorePreviousCurrentTransaction ()
    {
      base.RestorePreviousCurrentTransaction ();
    }

  }

}