using System;
using log4net;
using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Data
{
  public interface ITransaction
  {
    void Commit();
    void Rollback();
    bool CanCreateChild { get; }
    ITransaction CreateChild();
    ITransaction CreateAndRegisterChild();
    ITransaction UnregisterChild();
  }
}
namespace Rubicon.Web.ExecutionEngine
{

/// <summary>
/// Creates a scope for a transaction.
/// </summary>
[Serializable]
public abstract class WxeTransactionBase: WxeStepList
{
  /// <summary>
  ///   Finds out wheter the specified step is part of a WxeTransactionBase.
  /// </summary>
  /// <returns> True, if one of the parents of the specified Step is a WxeTransactionBase, false otherwise. </returns>
  public static bool HasWxeTransaction (WxeStep step)
  {
    return WxeStep.GetStepByType (step, typeof (WxeTransactionBase)) != null;
  }

  private static ILog s_log = LogManager.GetLogger (typeof (WxeTransactionBase));

  private ITransaction _transaction = null;
  private ITransaction _previousTransaction = null;
  private bool _autoCommit;
  private bool _forceRoot;
  private bool _hasExecutionStarted = false;
  private bool _isPreviousTransactionRestored = false;

  /// <summary> Creates a new instance. </summary>
  /// <param name="steps"> Initial step list. Can be <see langword="null"/>. </param>
  /// <param name="autoCommit">
  ///   If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back. 
  /// </param>
  /// <param name="forceRoot"> 
  ///   If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists. 
  /// </param>
  /// <remarks>
  ///   If the <see cref="ITransaction"/> implementation does not support child transactions 
  /// </remarks>
  public WxeTransactionBase (WxeStepList steps, bool autoCommit, bool forceRoot)
  {
    _autoCommit = autoCommit;
    _forceRoot = forceRoot;
    if (steps != null)
      AddStepList (steps);
  }

  protected abstract ITransaction CurrentTransaction { get; set; }
  protected abstract ITransaction CreateRootTransaction();

  protected virtual ITransaction CreateChildTransaction (ITransaction transaction)
  {
    ArgumentUtility.CheckNotNull ("transaction", transaction);

    if (transaction.CanCreateChild)
      return transaction.CreateChild();
    else
      return CreateRootTransaction();
  }

  protected ITransaction CreateTransaction()
  {
    WxeTransactionBase parentTransaction = ParentTransaction;
    ITransaction transaction = null;

    bool isParentTransactionNull = parentTransaction == null || parentTransaction.Transaction == null;
    if (_forceRoot || isParentTransactionNull)
    {
      transaction = CreateRootTransaction();
      s_log.Debug ("Created root " + this.GetType().Name + ".");
    }
    else
    {
      if (parentTransaction.Transaction != CurrentTransaction)
        throw new InvalidOperationException ("The parent transaction does not match the current transaction.");

      transaction = CreateChildTransaction (parentTransaction.Transaction);
      s_log.Debug ("Created child " + this.GetType().Name + ".");
    }

    return transaction;
  }

  /// <summary> Gets the first parent of type <see cref="WxeTransactionBase"/>. </summary>
  /// <value> 
  ///   A <see cref="WxeTransactionBase"/> object or <see langword="null"/> if the current transaction is the 
  ///   topmost transaction.
  /// </value>
  protected WxeTransactionBase ParentTransaction
  {
    get { return (WxeTransactionBase) WxeStep.GetStepByType (this, typeof (WxeTransactionBase)); }
  }

  public ITransaction Transaction
  {
    get { return _transaction; } 
  }

  /// <summary> Sets the encapsualted transaction. </summary>
  /// <remarks> Make this method protected virtual once the requirement arises to use an existing transaction. </remarks>
  /// <exclude/>
  protected virtual void SetTransaction (ITransaction transaction)
  {
    ArgumentUtility.CheckNotNull ("transaction", transaction);
    if (_hasExecutionStarted) throw new InvalidOperationException ("Cannot set the Transaction after the execution has started.");
    _transaction = transaction;
  }

  public override void Execute (WxeContext context)
  {
    s_log.Debug ("Entering Execute of " + this.GetType().Name);
    
    if (! _hasExecutionStarted)
    {
      _previousTransaction = CurrentTransaction;
      if (_transaction == null)
        _transaction = CreateTransaction();
      _hasExecutionStarted = true;
    }

    CurrentTransaction = _transaction;

    try
    {
      base.Execute (context);
    }
    catch (Exception e)
    {
      if (e is System.Threading.ThreadAbortException)
        throw;

      RollbackTransaction();
      RestorePreviousTransaction();
      s_log.Debug ("Aborted Execute of " + this.GetType().Name + " because of exception: \"" + e.Message + "\" (" + e.GetType().FullName + ").");
  
      throw;
    }

    if (_autoCommit)
      CommitTransaction();
    else
      RollbackTransaction();
    RestorePreviousTransaction();
    
    s_log.Debug ("Leaving Execute of " + this.GetType().Name);
  }

  protected override void AbortRecursive()
  {
    s_log.Debug ("Aborting " + this.GetType().Name);
    base.AbortRecursive();
    RollbackTransaction();
    RestorePreviousTransaction();
  }

  protected void CommitTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Committing " + _transaction.GetType().Name + ".");
      CommitTransaction(_transaction);
      _transaction = null;
    }
  }

  private void CommitTransaction (ITransaction transaction)
  {
    transaction.Commit();
  }
  
  private void RollbackTransaction (ITransaction transaction)
  {
    transaction.Rollback();
  }

  protected void RollbackTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Rolling back " + _transaction.GetType().Name + ".");
      RollbackTransaction(_transaction);
      _transaction = null;
    }
  }

  protected void RestorePreviousTransaction()
  {
    if (_hasExecutionStarted && ! _isPreviousTransactionRestored)
    {
      CurrentTransaction = _previousTransaction;
      _previousTransaction = null;
      _isPreviousTransactionRestored = true;
    }
  }
}

}
