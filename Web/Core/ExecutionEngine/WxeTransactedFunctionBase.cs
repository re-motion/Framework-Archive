using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Data;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

  /// <summary> A <see cref="WxeFunction"/> with an excapsulated <see cref="WxeTransactionBase{TTransaction}"/>. </summary>
  /// <typeparam name="TTransaction">
  ///   The <see cref="ITransaction"/> implementation wrapped by this <see cref="WxeTransactedFunctionBase{TTransaction}"/>. 
  /// </typeparam>
  [Serializable]
  public abstract class WxeTransactedFunctionBase<TTransaction> : WxeFunction, IDeserializationCallback
    where TTransaction : class, ITransaction
  {
    private WxeTransactionBase<TTransaction> _wxeTransaction = null;

    /// <summary> Creates a new instance. </summary>
    /// <param name="actualParameters"> The actual function parameters. </param>
    public WxeTransactedFunctionBase (params object[] actualParameters)
      : base (actualParameters)
    {
    }

    /// <summary> Creates the <see cref="WxeTransactionBase{TTransaction}"/> to be encapsulated. </summary>
    /// <returns>
    ///   The <see cref="WxeTransactionBase{TTransaction}"/> instance to be encapsulated in this 
    ///   <see cref="WxeTransactedFunctionBase{TTransaction}"/> or <see langword="null"/> if the 
    ///   <see cref="WxeTransactedFunctionBase{TTransaction}"/> does not have its own transaction.
    /// </returns>
    /// <remarks>
    ///   Called during the first invokation of <see cref="Execute"/>
    ///   <note type="inotes">
    ///     Override this method to initialize your <see cref="WxeTransactionBase{TTransaction}"/> implementation.
    ///   </note>
    /// </remarks>
    protected abstract WxeTransactionBase<TTransaction> CreateWxeTransaction ();

    /// <summary> Gets the underlying <typeparamref name="TTransaction"/>. </summary>
    protected TTransaction Transaction
    {
      get
      {
        if (_wxeTransaction == null)
          return null;
        else
          return _wxeTransaction.Transaction;
      }
    }

    public override void Execute (WxeContext context)
    {
      if (!ExecutionStarted)
      {
        _wxeTransaction = CreateWxeTransaction ();

        if (_wxeTransaction != null)
        {
          Encapsulate (_wxeTransaction);
          InitializeEvents (_wxeTransaction);
        }
      }

      base.Execute (context);
    }

    protected void InitializeEvents (WxeTransactionBase<TTransaction> wxeTransaction)
    {
      ArgumentUtility.CheckNotNull ("wxeTransaction", wxeTransaction);

      wxeTransaction.TransactionCommitting += new EventHandler (WxeTransaction_TransactionCommitting);
      wxeTransaction.TransactionCommitted += new EventHandler (WxeTransaction_TransactionCommitted);
      wxeTransaction.TransactionRollingBack += new EventHandler (WxeTransaction_TransactionRollingBack);
      wxeTransaction.TransactionRolledBack += new EventHandler (WxeTransaction_TransactionRolledBack);
    }

    void IDeserializationCallback.OnDeserialization (Object sender)
    {
      if (_wxeTransaction != null)
        InitializeEvents (_wxeTransaction);
    }

    /// <summary> 
    ///   Handles the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.TransactionCommitting"/> event.
    /// </summary>
    private void WxeTransaction_TransactionCommitting (object sender, EventArgs e)
    {
      OnCommitting ();
    }

    /// <summary> 
    ///   Handles the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.TransactionCommitted"/> event.
    /// </summary>
    private void WxeTransaction_TransactionCommitted (object sender, EventArgs e)
    {
      OnCommitted ();
    }

    /// <summary> 
    ///   Handles the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.TransactionRollingBack"/> event.
    /// </summary>
    private void WxeTransaction_TransactionRollingBack (object sender, EventArgs e)
    {
      OnRollingBack ();
    }

    /// <summary> 
    ///   Handles the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.TransactionRolledBack"/> event.
    /// </summary>
    private void WxeTransaction_TransactionRolledBack (object sender, EventArgs e)
    {
      OnRolledBack ();
    }

    /// <summary> Called before committing the <see cref="WxeTransactionBase{TTransaction}"/>. </summary>
    protected virtual void OnCommitting ()
    {
      if (Committing != null)
        Committing (this, EventArgs.Empty);
    }

    /// <summary> Called after the <see cref="WxeTransactionBase{TTransaction}"/> has been committed. </summary>
    protected virtual void OnCommitted ()
    {
      if (Committed != null)
        Committed (this, EventArgs.Empty);
    }

    /// <summary> Called before rolling the <see cref="WxeTransactionBase{TTransaction}"/> back. </summary>
    protected virtual void OnRollingBack ()
    {
      if (RollingBack != null)
        RollingBack (this, EventArgs.Empty);
    }

    /// <summary> Called after the <see cref="WxeTransactionBase{TTransaction}"/> has been rolled back. </summary>
    protected virtual void OnRolledBack ()
    {
      if (RolledBack != null)
        RolledBack (this, EventArgs.Empty);
    }

    /// <summary> Is raises before the<see cref="WxeTransactionBase{TTransaction}"/> is committed. </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> 
    ///     has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler Committing;

    /// <summary> Is raised after the <see cref="WxeTransactionBase{TTransaction}"/> has been committed. </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> 
    ///     has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler Committed;

    /// <summary> Is raised before the <see cref="WxeTransactionBase{TTransaction}"/> is rolled back. </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> 
    ///     has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler RollingBack;

    /// <summary> Is raised after the <see cref="WxeTransactionBase{TTransaction}"/> has been rolled back. </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> 
    ///     has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler RolledBack;
  }

}

