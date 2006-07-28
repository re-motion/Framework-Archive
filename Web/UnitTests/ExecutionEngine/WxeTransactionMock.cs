using System;
using System.Collections;
using System.Web;
using System.Web.UI;
using System.Reflection;
using Rubicon.NullableValueTypes;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Development.UnitTesting;
using Rubicon.Data;

namespace Rubicon.Web.UnitTests.ExecutionEngine
{

/// <summary> Provides a test implementation of the <see langword="abstract"/> <see cref="WxeTransactionBase"/> type. </summary>
[Serializable]
public class WxeTransactionMock: WxeTransactionBase<TestTransaction>
{
  private bool _hasCreatedRootTransaction;

  public WxeTransactionMock (WxeStepList steps, bool autoCommit, bool forceRoot)
    : base (steps, autoCommit, forceRoot)
  {
  }

  protected override TestTransaction CreateRootTransaction()
  {
    _hasCreatedRootTransaction = true;
    return new TestTransaction();
  }

  protected override TestTransaction CurrentTransaction
  {
    get { return TestTransaction.Current; }
  }

  protected override void SetCurrentTransaction (TestTransaction transaction)
  {
    TestTransaction.Current = (TestTransaction) transaction;
  }

  public ArrayList Steps
  {
    get { return (ArrayList) PrivateInvoke.GetNonPublicField (this, "_steps"); }
  }

  public void StartExecution()
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

  public TestTransaction PreviousCurrentTransaction
  {
    get { return (TestTransaction) PrivateInvoke.GetNonPublicField (this, "_previousCurrentTransaction"); }
    set { PrivateInvoke.SetNonPublicField (this, "_previousCurrentTransaction", value); }
  }

  public new TestTransaction CreateTransaction ()
  {
    return base.CreateTransaction();
  }

  public new TestTransaction CreateChildTransaction (TestTransaction parentTransaction)
  {
    return base.CreateChildTransaction (parentTransaction);
  }

  public new WxeTransactionBase<TestTransaction> ParentTransaction
  {
    get { return base.ParentTransaction; }
  }

  public bool HasCreatedRootTransaction
  {
    get { return _hasCreatedRootTransaction; }
  }

  public new void OnTransactionCommitting()
  {
    base.OnTransactionCommitting ();
  }

  public new void OnTransactionCommitted()
  {
    base.OnTransactionCommitted ();
  }

  public new void OnTransactionRollingBack()
  {
    base.OnTransactionRollingBack ();
  }

  public new void OnTransactionRolledBack()
  {
    base.OnTransactionRolledBack ();
  }

  public void CommitTransaction()
  {
    base.CommitTransaction (Transaction);
  }

  public void RollbackTransaction()
  {
    base.RollbackTransaction (Transaction);
  }

  public new void CommitAndReleaseTransaction()
  {
    base.CommitAndReleaseTransaction();
  }

  public new void RollbackAndReleaseTransaction()
  {
    base.RollbackAndReleaseTransaction();
  }

  public new void RestorePreviousCurrentTransaction()
  {
    base.RestorePreviousCurrentTransaction();
  }
}

}