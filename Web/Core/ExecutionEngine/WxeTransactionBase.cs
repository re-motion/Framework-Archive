using System;
using log4net;
using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> Creates a scope for a transaction. </summary>
[Serializable]
public abstract class WxeTransactionBase: WxeStepList
{
  /// <summary> Finds out wheter the specified step is part of a <b>WxeTransactionBase</b>. </summary>
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

  /// <summary> Gets the current <see cref="ITransaction"/>. </summary>
  /// <value> 
  ///   An instance of a type implementing <see cref="ITransaction"/> or <see langword="null"/> if no current
  ///   transaction exists.
  /// </value>
  /// <remarks> 
  ///   <note type="inheritinfo">
  ///     If the <see cref="ITransaction"/> implementation does not support the concept of a current transaction,
  ///     it is valid to always return <see langword="null"/>.
  ///   </note>
  /// </remarks>
  protected abstract ITransaction CurrentTransaction { get; }

  /// <summary> Sets the current <see cref="ITransaction"/> to <paramref name="transaction"/>. </summary>
  /// <param name="transaction"> The new current transaction. </param>
  /// <remarks> 
  ///   <note type="inheritinfo">
  ///     It the <see cref="ITransaction"/> implementation does not support the concept of a current transaction,
  ///     it is valid to implement an empty method.
  ///   </note>
  /// </remarks>
  protected abstract void SetCurrentTransaction (ITransaction transaction);

  /// <summary> Creates a new transaction. </summary>
  /// <returns> A new instance of a type implementing <see cref="ITransaction"/>. </returns>
  protected ITransaction CreateTransaction()
  {
    WxeTransactionBase parentTransaction = ParentTransaction;
    ITransaction transaction = null;

    bool isParentTransactionNull = parentTransaction == null || parentTransaction.Transaction == null;
    bool hasParentTransaction = ! _forceRoot && ! isParentTransactionNull;
    if (hasParentTransaction)
    {
      bool hasCurrentTransaction = CurrentTransaction != null;
      if (hasCurrentTransaction && parentTransaction.Transaction != CurrentTransaction)
        throw new InvalidOperationException ("The parent transaction does not match the current transaction.");

      transaction = CreateChildTransaction (parentTransaction.Transaction);
      s_log.Debug ("Created child " + this.GetType().Name + ".");
    }
    else
    {
      transaction = CreateRootTransaction();
      s_log.Debug ("Created root " + this.GetType().Name + ".");
    }

    return transaction;
  }

  /// <summary> Creates a new root transaction. </summary>
  /// <returns> A new instance of a type implementing <see cref="ITransaction"/>. </returns>
  protected abstract ITransaction CreateRootTransaction();

  /// <summary> Creates a new <see cref="ITransaction"/> using the <paramref name="parentTransaction"/> as parent. </summary>
  /// <param name="parentTransaction"> The <see cref="ITransaction"/> to be used as parent. </param>
  /// <returns> A new instance of a type implementing <see cref="ITransaction"/>. </returns>
  /// <remarks> 
  ///   The created transaction will be created as a root transaction if the <see cref="ITransaction"/> 
  ///   implementation of the <paramref name="parentTransaction"/> does not support child transactions.
  /// </remarks>
  protected ITransaction CreateChildTransaction (ITransaction parentTransaction)
  {
    ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

    if (parentTransaction.CanCreateChild)
      return parentTransaction.CreateChild();
    else
      return CreateRootTransaction();
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

  /// <summary> Gets the encapsulated <see cref="ITransaction"/>. </summary>
  public ITransaction Transaction
  {
    get { return _transaction; } 
  }

  public override void Execute (WxeContext context)
  {
    s_log.Debug ("Entering Execute of " + this.GetType().Name);
    
    if (! ExecutionStarted)
    {
      _previousTransaction = CurrentTransaction;
      if (_transaction == null)
        _transaction = CreateTransaction();
    }

    SetCurrentTransaction (_transaction);

    try
    {
      base.Execute (context);
    }
    catch (Exception e)
    {
      if (e is System.Threading.ThreadAbortException)
        throw;

      RollbackAndReleaseTransaction();
      RestorePreviousTransaction();
      s_log.Debug ("Aborted Execute of " + this.GetType().Name + " because of exception: \"" + e.Message + "\" (" + e.GetType().FullName + ").");
  
      throw;
    }

    if (_autoCommit)
      CommitAndReleaseTransaction();
    else
      RollbackAndReleaseTransaction();
    RestorePreviousTransaction();
   
    s_log.Debug ("Leaving Execute of " + this.GetType().Name);
  }

  protected override void AbortRecursive()
  {
    s_log.Debug ("Aborting " + this.GetType().Name);
    base.AbortRecursive();
    RollbackAndReleaseTransaction();
    RestorePreviousTransaction();
  }


  protected void CommitAndReleaseTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Committing " + _transaction.GetType().Name + ".");
      _transaction.Commit();
      _transaction.Release();
      _transaction = null;
    }
  }

  protected void RollbackAndReleaseTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Rolling back " + _transaction.GetType().Name + ".");
      _transaction.Rollback();
      _transaction.Release();
      _transaction = null;
    }
  }

  protected void RestorePreviousTransaction()
  {
    if (ExecutionStarted && ! _isPreviousTransactionRestored)
    {
      SetCurrentTransaction (_previousTransaction);
      _previousTransaction = null;
      _isPreviousTransactionRestored = true;
    }
  }

  /// <summary> Sets the encapsualted transaction. </summary>
  /// <remarks> Make this method protected virtual once the requirement arises to use an external transaction. </remarks>
  private void SetTransaction (ITransaction transaction)
  {
    ArgumentUtility.CheckNotNull ("transaction", transaction);
    if (ExecutionStarted)
      throw new InvalidOperationException ("Cannot set the Transaction after the execution has started.");
    _transaction = transaction;
  }
}

}
