using System;
using Rubicon.Data;
using Rubicon.Logging;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

  /// <summary> Creates a scope for a transaction. </summary>
  /// <typeparam name="TTransaction">The <see cref="ITransaction"/> implementation wrapped by this <see cref="WxeTransactionBase{TTransaction}"/>. </typeparam>
  [Serializable]
  public abstract class WxeTransactionBase<TTransaction> : WxeStepList
    where TTransaction : class, ITransaction
  {
    private static readonly ILog s_log = LogManager.GetLogger ("Rubicon.Web.ExecutionEngine.WxeTransactionBase");

    /// <summary> Finds out wheter the specified step is part of a <see cref="WxeTransactionBase{TTransaction}"/>. </summary>
    /// <returns> 
    ///   <see langword="true"/>, if one of the parents of the specified Step is a <see cref="WxeTransactionBase{TTransaction}"/>, 
    ///   otherwise <see langword="false"/>. 
    /// </returns>
    public static bool HasWxeTransaction (WxeStep step)
    {
      return WxeStep.GetStepByType (step, typeof (WxeTransactionBase<TTransaction>)) != null;
    }

    private TTransaction _transaction = null;
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
    ///   If the <typeparamref name="TTransaction"/> implementation does not support child transactions 
    /// </remarks>
    public WxeTransactionBase (WxeStepList steps, bool autoCommit, bool forceRoot)
    {
      _autoCommit = autoCommit;
      _forceRoot = forceRoot;
      if (steps != null)
        AddStepList (steps);
    }

    /// <summary> Gets the current <typeparamref name="TTransaction"/>. </summary>
    /// <value> 
    ///   An instance of a type implementing <typeparamref name="TTransaction"/> or <see langword="null"/> if no current
    ///   transaction exists.
    /// </value>
    /// <remarks> 
    ///   <note type="inotes">
    ///     If the <typeparamref name="TTransaction"/> implementation does not support the concept of a current transaction,
    ///     it is valid to always return <see langword="null"/>.
    ///   </note>
    /// </remarks>
    protected abstract TTransaction CurrentTransaction { get; }

    /// <summary> Sets the current <typeparamref name="TTransaction"/> to <paramref name="transaction"/> and stores the previous transaction
    /// for later restoration. </summary>
    /// <param name="transaction"> The new current transaction. </param>
    /// <remarks> 
    ///   <note type="inotes">
    ///     It the <typeparamref name="TTransaction"/> implementation does not support the concept of a current transaction,
    ///     it is valid to implement an empty method.
    ///   </note>
    ///   <para>
    ///    Implementers can rely on each call to this method being paired with a call to <see cref="RestorePreviousTransaction"/>.
    ///    However, <see cref="SetCurrentTransaction"/> can be called multiple times, bevore <see cref="RestorePreviousTransaction"/>
    ///    is invoked. In that case, <see cref="RestorePreviousTransaction"/> must operate in a Last-In-First-Out (stack-like) fashion.
    ///  </para>
    /// </remarks>
    protected abstract void SetCurrentTransaction (TTransaction transaction);

    /// <summary> Resets the current <typeparamref name="TTransaction"/> to the transaction previously replaced via 
    /// <see cref="SetCurrentTransaction"/>. </summary>
    /// <remarks> 
    ///   <note type="inotes">
    ///     It the <typeparamref name="TTransaction"/> implementation does not support the concept of a current transaction,
    ///     it is valid to implement an empty method.
    ///   </note>
    ///   <para>
    ///     Implementes can rely on this method being called exactly once for each call of <see cref="SetCurrentTransaction"/>.
    ///    However, <see cref="SetCurrentTransaction"/> can be called multiple times, bevore <see cref="RestorePreviousTransaction"/>
    ///    is invoked. In that case, <see cref="RestorePreviousTransaction"/> must operate in a Last-In-First-Out (stack-like) fashion.
    ///   </para>
    /// </remarks>
    protected abstract void RestorePreviousTransaction ();

    /// <summary> Creates a new transaction. </summary>
    /// <returns> A new instance of type <typeparamref name="TTransaction"/>. </returns>
    /// <exception cref="InvalidOperationException"> Thrown if <see langword="null"/> where to be returned as the child transaction. </exception>
    protected TTransaction CreateTransaction ()
    {
      WxeTransactionBase<TTransaction> parentTransaction = GetParentTransaction ();
      TTransaction transaction;

      bool isParentTransactionNull = parentTransaction == null || parentTransaction.Transaction == null;
      bool useParentTransaction = !_forceRoot && !isParentTransactionNull;
      if (useParentTransaction)
      {
        bool hasCurrentTransaction = CurrentTransaction != null;
        if (hasCurrentTransaction && parentTransaction.Transaction != CurrentTransaction)
          throw new InvalidOperationException ("The parent transaction does not match the current transaction.");

        OnTransactionCreating ();
        transaction = CreateChildTransaction (parentTransaction.Transaction);
        s_log.Debug ("Created child " + this.GetType ().Name + ".");
      }
      else
      {
        OnTransactionCreating ();
        transaction = CreateRootTransaction ();
        if (transaction == null)
          throw new InvalidOperationException (string.Format ("{0}.CreateRootTransaction() evaluated and returned null.", GetType ().Name));
        s_log.Debug ("Created root " + this.GetType ().Name + ".");
      }

      OnTransactionCreated (transaction);
      return transaction;
    }

    /// <summary> Called before the <see cref="Transaction"/> is created. </summary>
    /// <remarks> Raises the <see cref="TransactionCreating"/> event. </remarks>
    protected virtual void OnTransactionCreating ()
    {
      if (TransactionCreating != null)
        TransactionCreating (this, EventArgs.Empty);
    }
    
    /// <summary> Called after the <see cref="Transaction"/> has been created. </summary>
    /// <param name="transaction"> The <typeparamref name="TTransaction"/> that has been created. </param>
    /// <remarks> Raises the <see cref="TransactionCreated"/> event. </remarks>
    protected virtual void OnTransactionCreated (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      
      if (TransactionCreated != null)
        TransactionCreated (this, new WxeTransactionEventArgs<TTransaction> (transaction));
    }

    /// <summary> Creates a new root transaction. </summary>
    /// <returns> A new instance of type <typeparamref name="TTransaction"/>. </returns>
    protected abstract TTransaction CreateRootTransaction ();

    /// <summary> Creates a new <typeparamref name="TTransaction"/> using the <paramref name="parentTransaction"/> as parent. </summary>
    /// <param name="parentTransaction"> The <typeparamref name="TTransaction"/> to be used as parent. </param>
    /// <returns> A new instance of a type implementing <typeparamref name="TTransaction"/>. </returns>
    /// <remarks> 
    ///   The created transaction will be created as a root transaction if the <typeparamref name="TTransaction"/> 
    ///   implementation of the <paramref name="parentTransaction"/> does not support child transactions.
    /// </remarks>
    /// <exception cref="InvalidOperationException"> 
    ///   Thrown if <see langword="null"/> where to be returned as the child transaction.
    /// </exception>
    protected TTransaction CreateChildTransaction (TTransaction parentTransaction)
    {
      ArgumentUtility.CheckNotNull ("parentTransaction", parentTransaction);

      TTransaction transaction;
      if (parentTransaction.CanCreateChild)
      {
        transaction = (TTransaction) parentTransaction.CreateChild ();
        if (transaction == null)
        {
          throw new InvalidOperationException (string.Format (
              "{0}.CreateChild() evaluated and returned null.", parentTransaction.GetType ().Name));
        }
      }
      else
      {
        transaction = CreateRootTransaction ();
        if (transaction == null)
        {
          throw new InvalidOperationException (string.Format (
              "{0}.CreateRootTransaction() evaluated and returned null.", GetType ().Name));
        }
      }
      return transaction;
    }

    /// <summary> Gets the first parent of type <see cref="WxeTransactionBase{TTransaction}"/>. </summary>
    /// <value> 
    ///   A <see cref="WxeTransactionBase{TTransaction}"/> object or <see langword="null"/> if the current transaction is the 
    ///   topmost transaction.
    /// </value>
    protected WxeTransactionBase<TTransaction> GetParentTransaction()
    {
      return (WxeTransactionBase<TTransaction>) WxeStep.GetStepByType (ParentStep, typeof (WxeTransactionBase<TTransaction>));
    }

    /// <summary> Gets the underlying <typeparamref name="TTransaction"/>. </summary>
    public TTransaction Transaction
    {
      get { return _transaction; }
    }

    public override void Execute (WxeContext context)
    {
      if (!ExecutionStarted)
      {
        s_log.Debug ("Initializing execution of " + this.GetType ().FullName + ".");
        if (_transaction == null)
          _transaction = CreateTransaction ();
      }
      else
      {
        s_log.Debug (string.Format ("Resuming execution of " + this.GetType ().FullName + "."));
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

        RollbackAndReleaseTransaction ();
        RestorePreviousCurrentTransaction ();
        s_log.Debug ("Aborted execution of " + this.GetType ().Name + " because of exception: \"" + e.Message + "\" (" + e.GetType ().FullName + ").");

        throw;
      }

      if (_autoCommit)
        CommitAndReleaseTransaction ();
      else
        RollbackAndReleaseTransaction ();
      RestorePreviousCurrentTransaction ();

      s_log.Debug ("Ending execution of " + this.GetType ().Name);
    }

    protected override void AbortRecursive ()
    {
      s_log.Debug ("Aborting " + this.GetType ().Name);
      base.AbortRecursive ();
      RollbackAndReleaseTransaction ();
      RestorePreviousCurrentTransaction ();
    }

    /// <summary> Commits encasulated <typeparamref name="TTransaction"/> and releases it afterwards. </summary>
    protected void CommitAndReleaseTransaction ()
    {
      if (_transaction != null)
      {
        s_log.Debug ("Committing " + _transaction.GetType ().Name + ".");
        TTransaction previousTransaction = CurrentTransaction;
        SetCurrentTransaction (_transaction);
        try
        {
          CommitTransaction (_transaction);
        }
        finally
        {
          RestorePreviousTransaction ();
          Assertion.Assert (CurrentTransaction == previousTransaction);
        }
        _transaction.Release ();
        _transaction = null;
      }
    }

    /// <summary> Commits the <paramref name="transaction"/>. </summary>
    /// <param name="transaction"> The <typeparamref name="TTransaction"/> to be committed. </param>
    /// <remarks> 
    ///   Calls the <see cref="OnTransactionCommitting"/> method before committing the transaction
    ///   and the <see cref="OnTransactionCommitted"/> method after the transaction has been committed.
    /// </remarks>
    protected void CommitTransaction (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      OnTransactionCommitting ();
      transaction.Commit ();
      OnTransactionCommitted ();
    }

    /// <summary> Called before the <see cref="Transaction"/> is committed. </summary>
    /// <remarks> Raises the <see cref="TransactionCommitting"/> event. </remarks>
    protected virtual void OnTransactionCommitting ()
    {
      if (TransactionCommitting != null)
        TransactionCommitting (this, EventArgs.Empty);
    }

    /// <summary> Called after the <see cref="Transaction"/> has been committed. </summary>
    /// <remarks> Raises the <see cref="TransactionCommitted"/> event. </remarks>
    protected virtual void OnTransactionCommitted ()
    {
      if (TransactionCommitted != null)
        TransactionCommitted (this, EventArgs.Empty);
    }

    /// <summary> Rolls the encasulated <typeparamref name="TTransaction"/> back and relases it afterwards. </summary>
    protected void RollbackAndReleaseTransaction ()
    {
      if (_transaction != null)
      {
        s_log.Debug ("Rolling back " + _transaction.GetType ().Name + ".");
        TTransaction previousTransaction = CurrentTransaction;
        SetCurrentTransaction (_transaction);
        try
        {
          RollbackTransaction (_transaction);
        }
        finally
        {
          RestorePreviousTransaction ();
          Assertion.Assert (CurrentTransaction == previousTransaction);
        }
        _transaction.Release ();
        _transaction = null;
      }
    }

    /// <summary> Rolls the <paramref name="transaction"/> back. </summary>
    /// <param name="transaction"> The <typeparamref name="TTransaction"/> to be rolled back. </param>
    /// <remarks> 
    ///   Calls the <see cref="OnTransactionRollingBack"/> method before rolling back the transaction 
    ///   and the <see cref="OnTransactionRolledBack"/> method after the transaction has been rolled back.
    /// </remarks>
    protected void RollbackTransaction (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      OnTransactionRollingBack ();
      transaction.Rollback ();
      OnTransactionRolledBack ();
    }

    /// <summary> Called before the <see cref="Transaction"/> is rolled back. </summary>
    /// <remarks> Raises the <see cref="TransactionRollingBack"/> event. </remarks>
    protected virtual void OnTransactionRollingBack ()
    {
      if (TransactionRollingBack != null)
        TransactionRollingBack (this, EventArgs.Empty);
    }

    /// <summary> Called after the <see cref="Transaction"/> has been rolled back. </summary>
    /// <remarks> Raises the <see cref="TransactionRolledBack"/> event. </remarks>
    protected virtual void OnTransactionRolledBack ()
    {
      if (TransactionRolledBack != null)
        TransactionRolledBack (this, EventArgs.Empty);
    }

    /// <summary> Sets the backed up transaction as the old and new current transaction. </summary>
    protected void RestorePreviousCurrentTransaction ()
    {
      Assertion.Assert (ExecutionStarted);

      if (!_isPreviousCurrentTransactionRestored)
      {
        RestorePreviousTransaction ();
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

    /// <summary> Is fired before the <see cref="Transaction"/> is created. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCreating;

    /// <summary> Is fired after the <see cref="Transaction"/> has been created. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler<WxeTransactionEventArgs<TTransaction>> TransactionCreated;

    /// <summary> Is fired before the <see cref="Transaction"/> is committed. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCommitting;

    /// <summary> Is fired after the <see cref="Transaction"/> has been committed. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCommitted;

    /// <summary> Is fired before the <see cref="Transaction"/> is rolled back. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionRollingBack;

    /// <summary> Is fired after the <see cref="Transaction"/> has been rolled back. </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionRolledBack;
  }

}
