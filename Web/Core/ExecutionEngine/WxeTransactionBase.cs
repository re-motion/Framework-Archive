using System;
using System.ComponentModel;
using log4net;
using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

/// <summary> Creates a scope for a transaction. </summary>
[Serializable]
public abstract class WxeTransactionBase: WxeStepList
{
  private static readonly ILog s_log = LogManager.GetLogger (typeof (WxeTransactionBase));

  /// <summary> Finds out wheter the specified step is part of a <b>WxeTransactionBase</b>. </summary>
  /// <returns> True, if one of the parents of the specified Step is a WxeTransactionBase, false otherwise. </returns>
  public static bool HasWxeTransaction (WxeStep step)
  {
    return WxeStep.GetStepByType (step, typeof (WxeTransactionBase)) != null;
  }

  private ITransaction _transaction = null;
  private ITransaction _previousCurrentTransaction = null;
  private bool _autoCommit;
  private bool _forceRoot;
  private bool _isPreviousCurrentTransactionRestored = false;

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
  /// <exception cref="InvalidOperationException"> 
  ///   Thrown if <see langword="null"/> where to be returned as the child transaction.
  /// </exception>
  protected ITransaction CreateTransaction()
  {
    WxeTransactionBase parentTransaction = ParentTransaction;
    ITransaction transaction;

    bool isParentTransactionNull = parentTransaction == null || parentTransaction.Transaction == null;
    bool useParentTransaction = ! _forceRoot && ! isParentTransactionNull;
    if (useParentTransaction)
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
      if (transaction == null)
      {
        throw new InvalidOperationException (string.Format (
            "{0}.CreateRootTransaction() evaluated and returned null.", GetType().Name));
      }
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
  /// <exception cref="InvalidOperationException"> 
  ///   Thrown if <see langword="null"/> where to be returned as the child transaction.
  /// </exception>
  protected ITransaction CreateChildTransaction (ITransaction parentTransaction)
  {
    ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

    ITransaction transaction;
    if (parentTransaction.CanCreateChild)
    {
      transaction = parentTransaction.CreateChild();
      if (transaction == null)
      {
        throw new InvalidOperationException (string.Format (
            "{0}.CreateChild() evaluated and returned null.", parentTransaction.GetType().Name));
      }
    }
    else
    {
      transaction = CreateRootTransaction();
      if (transaction == null)
      {
        throw new InvalidOperationException (string.Format (
            "{0}.CreateRootTransaction() evaluated and returned null.", GetType().Name));
      }
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
    get { return (WxeTransactionBase) WxeStep.GetStepByType (ParentStep, typeof (WxeTransactionBase)); }
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
      _previousCurrentTransaction = CurrentTransaction;
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
      RestorePreviousCurrentTransaction();
      s_log.Debug ("Aborted Execute of " + this.GetType().Name + " because of exception: \"" + e.Message + "\" (" + e.GetType().FullName + ").");
  
      throw;
    }

    if (_autoCommit)
      CommitAndReleaseTransaction();
    else
      RollbackAndReleaseTransaction();
    RestorePreviousCurrentTransaction();
   
    s_log.Debug ("Leaving Execute of " + this.GetType().Name);
  }

  protected override void AbortRecursive()
  {
    s_log.Debug ("Aborting " + this.GetType().Name);
    base.AbortRecursive();
    RollbackAndReleaseTransaction();
    RestorePreviousCurrentTransaction();
  }

  /// <summary> Commits encasulated <see cref="ITransaction"/> and releases it afterwards. </summary>
  protected void CommitAndReleaseTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Committing " + _transaction.GetType().Name + ".");
      ITransaction currentTransaction = CurrentTransaction;
      SetCurrentTransaction (_transaction);
      try
      {
        CommitTransaction (_transaction);
      }
      catch (Exception)
      {
        SetCurrentTransaction (currentTransaction);
        throw;
      }
      SetCurrentTransaction (currentTransaction);
      _transaction.Release();
      _transaction = null;
    }
  }

  /// <summary> Commits the <paramref name="transaction"/>. </summary>
  /// <param name="transaction"> The <see cref="ITransaction"/> to be committed. </param>
  /// <remarks> 
  ///   Calls the <see cref="OnTransactionCommitting"/> method before committing the transaction
  ///   and the <see cref="OnTransactionCommitted"/> method after the transaction has been committed.
  /// </remarks>
  protected void CommitTransaction (ITransaction transaction)
  {
    ArgumentUtility.CheckNotNull ("transaction", transaction);

    OnTransactionCommitting();
    transaction.Commit();
    OnTransactionCommitted();
  }

  /// <summary> Called before the transaction is committed. </summary>
  /// <remarks> Raises the <see cref="TransactionCommitting"/> event. </remarks>
  protected virtual void OnTransactionCommitting()
  {
    if (TransactionCommitting != null)
      TransactionCommitting (this, EventArgs.Empty);
  }

  /// <summary> Called after the transaction has been committed. </summary>
  /// <remarks> Raises the <see cref="TransactionCommitted"/> event. </remarks>
  protected virtual void OnTransactionCommitted()
  {
    if (TransactionCommitted != null)
      TransactionCommitted (this, EventArgs.Empty);
  }

  /// <summary> Rolls the encasulated <see cref="ITransaction"/> back and relases it afterwards. </summary>
  protected void RollbackAndReleaseTransaction()
  {
    if (_transaction != null)
    {
      s_log.Debug ("Rolling back " + _transaction.GetType().Name + ".");
      ITransaction currentTransaction = CurrentTransaction;
      SetCurrentTransaction (_transaction);
      try
      {
        RollbackTransaction (_transaction);
      }
      catch (Exception)
      {
        SetCurrentTransaction (currentTransaction);
        throw;
      }
      SetCurrentTransaction (currentTransaction);
      _transaction.Release();
      _transaction = null;
    }
  }

  /// <summary> Rolls the <paramref name="transaction"/> back. </summary>
  /// <param name="transaction"> The <see cref="ITransaction"/> to be rolled back. </param>
  /// <remarks> 
  ///   Calls the <see cref="OnTransactionRollingBack"/> method before rolling back the transaction 
  ///   and the <see cref="OnTransactionRolledBack"/> method after the transaction has been rolled back.
  /// </remarks>
  protected void RollbackTransaction (ITransaction transaction)
  {
    ArgumentUtility.CheckNotNull ("transaction", transaction);
    
    OnTransactionRollingBack();
    transaction.Rollback();
    OnTransactionRolledBack();
  }

  /// <summary> Called before the transaction is rolled back. </summary>
  /// <remarks> Raises the <see cref="TransactionRollingBack"/> event. </remarks>
  protected virtual void OnTransactionRollingBack()
  {
    if (TransactionRollingBack != null)
      TransactionRollingBack (this, EventArgs.Empty);
  }

  /// <summary> Called after the transaction has been rolled back. </summary>
  /// <remarks> Raises the <see cref="TransactionRolledBack"/> event. </remarks>
  protected virtual void OnTransactionRolledBack()
  {
    if (TransactionRolledBack != null)
      TransactionRolledBack (this, EventArgs.Empty);
  }

  /// <summary> Sets the backed up transaction as the old and new current transaction. </summary>
  protected void RestorePreviousCurrentTransaction()
  {
    if (ExecutionStarted && ! _isPreviousCurrentTransactionRestored)
    {
      SetCurrentTransaction (_previousCurrentTransaction);
      _previousCurrentTransaction = null;
      _isPreviousCurrentTransactionRestored = true;
    }
  }

  protected bool AutoCommit
  {
    get { return _autoCommit; }
  }

  protected bool ForceRoot
  {
    get { return _forceRoot; }
  }

  /// <summary> Is raises before the transaction is committed. </summary>
  /// <remarks>
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactionBase"/> has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler TransactionCommitting;
  
  /// <summary> Is fired after the transaction has been committed. </summary>
  /// <remarks>
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactionBase"/> has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler TransactionCommitted;
  
  /// <summary> Is fired before the transaction is rolled back. </summary>
  /// <remarks>
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactionBase"/> has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler TransactionRollingBack;

  /// <summary> Is fired after the transaction has been rolled back. </summary>
  /// <remarks>
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactionBase"/> has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler TransactionRolledBack;
}

}
