using System;
using System.Runtime.Serialization;
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

    /// <summary> Gets the underlying <typeparamref name="TTransaction"/> owned by this <see cref="WxeTransactedFunctionBase{TTransaction}"/>.</summary>
    protected TTransaction MyTransaction
    {
      get
      {
        if (_wxeTransaction == null)
          return null;
        else
          return _wxeTransaction.Transaction;
      }
    }

    /// <summary> Gets the underlying <typeparamref name="TTransaction"/> used when this <see cref="WxeTransactedFunctionBase{TTransaction}"/>
    /// is executed. This is either <see cref="MyTransaction"/> or, when this function does not have an own transaction, the
    /// <see cref="Transaction"/> of this function's parent function.</summary>
    protected TTransaction Transaction
    {
      get
      {
        if (MyTransaction != null)
          return MyTransaction;
        else
        {
          WxeTransactedFunctionBase<TTransaction> parent = GetStepByType<WxeTransactedFunctionBase<TTransaction>> (ParentFunction);
          if (parent != null)
            return parent.Transaction;
          else
            return null;
        }
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

    private void InitializeEvents (WxeTransactionBase<TTransaction> wxeTransaction)
    {
      wxeTransaction.TransactionCreating += delegate { OnTransactionCreating (); };
      wxeTransaction.TransactionCreated += delegate (object sender, WxeTransactionEventArgs<TTransaction> args) 
          { 
            OnTransactionCreated (args.Transaction); 
          };
      wxeTransaction.TransactionCommitting += delegate { OnCommitting (); };
      wxeTransaction.TransactionCommitted += delegate { OnCommitted(); };
      wxeTransaction.TransactionRollingBack += delegate { OnRollingBack (); };
      wxeTransaction.TransactionRolledBack += delegate { OnRolledBack (); };
    }

    void IDeserializationCallback.OnDeserialization (Object sender)
    {
      OnDeserialization (sender);
    }

    protected virtual void OnDeserialization (Object sender)
    {
      if (_wxeTransaction != null)
        InitializeEvents (_wxeTransaction);
    }

    /// <summary> 
    ///   Called before creating the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/>. 
    /// </summary>
    protected virtual void OnTransactionCreating ()
    {
      if (TransactionCreating != null)
        TransactionCreating (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been created.
    /// </summary>
    protected virtual void OnTransactionCreated (TTransaction transaction)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);

      if (TransactionCreated != null)
        TransactionCreated (this, new WxeTransactedFunctionEventArgs<TTransaction> (transaction));
    }

    /// <summary> 
    ///   Called before committing the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/>. 
    /// </summary>
    protected virtual void OnCommitting ()
    {
      if (Committing != null)
        Committing (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been committed. 
    /// </summary>
    protected virtual void OnCommitted ()
    {
      if (Committed != null)
        Committed (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called before rolling the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> back. 
    /// </summary>
    protected virtual void OnRollingBack ()
    {
      if (RollingBack != null)
        RollingBack (this, EventArgs.Empty);
    }

    /// <summary> 
    ///   Called after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been rolled back. 
    /// </summary>
    protected virtual void OnRolledBack ()
    {
      if (RolledBack != null)
        RolledBack (this, EventArgs.Empty);
    }
    /// <summary> 
    ///   Is fired before the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is created. 
    /// </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler TransactionCreating;

    /// <summary> 
    ///   Is fired after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is created. 
    /// </summary>
    /// <remarks>
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler<WxeTransactedFunctionEventArgs<TTransaction>> TransactionCreated;

    /// <summary> 
    ///   Is fired before the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is committed. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler Committing;

    /// <summary> 
    ///   Is fired after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been committed. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler Committed;

    /// <summary> 
    ///   Is fired before the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> is rolled back. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler RollingBack;

    /// <summary> 
    ///   Is fired after the <see cref="WxeTransactionBase{TTransaction}"/>'s <see cref="WxeTransactionBase{TTransaction}.Transaction"/> has been rolled back. 
    /// </summary>
    /// <remarks> 
    ///   <note type="caution">
    ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase{TTransaction}"/> has been deserialized.
    ///   </note>
    /// </remarks>
    [field: NonSerialized]
    public event EventHandler RolledBack;
  }

}

