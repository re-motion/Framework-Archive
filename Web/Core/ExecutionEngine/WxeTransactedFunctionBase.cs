using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

[Serializable]
public abstract class WxeTransactedFunctionBase: WxeFunction, IDeserializationCallback
{
  private WxeTransactionBase _wxeTransaction = null;

  /// <summary> Creates a new instance. </summary>
  /// <param name="actualParameters"> The actual function parameters. </param>
  public WxeTransactedFunctionBase (params object[] actualParameters)
    : base (actualParameters)
  {
  }

  /// <summary> Creates the <see cref="WxeTransactionBase"/> to be encapsulated. </summary>
  /// <returns>
  ///   The <see cref="WxeTransactionBase"/> instance to be encapsulated in this 
  ///   <see cref="WxeTransactedFunctionBase"/> or <see langword="null"/> if the 
  ///   <see cref="WxeTransactedFunctionBase"/> does not have it's own transaction.
  /// </returns>
  /// <remarks>
  ///   Called during the first invokation of <see cref="Execute"/>
  ///   <note type="inheritinfo">
  ///     Override this method to initialize your <see cref="WxeTransactionBase"/> implementation.
  ///   </note>
  /// </remarks>
  protected abstract WxeTransactionBase CreateWxeTransaction();

  public override void Execute (WxeContext context)
  {
    if (! ExecutionStarted)
    {
      _wxeTransaction = CreateWxeTransaction();

      if (_wxeTransaction != null)
      {
        Encapsulate (_wxeTransaction);
        InitializeEvents (_wxeTransaction);
      }
    }

    base.Execute (context);
  }

  protected void InitializeEvents (WxeTransactionBase wxeTransaction)
  {
    ArgumentUtility.CheckNotNull ("wxeTransaction", wxeTransaction);

    wxeTransaction.TransactionCommitting += new EventHandler(WxeTransaction_TransactionCommitting);
    wxeTransaction.TransactionCommitted += new EventHandler(WxeTransaction_TransactionCommitted);
    wxeTransaction.TransactionRollingBack += new EventHandler(WxeTransaction_TransactionRollingBack);
    wxeTransaction.TransactionRolledBack += new EventHandler(WxeTransaction_TransactionRolledBack);
  }

  void IDeserializationCallback.OnDeserialization(Object sender) 
  {
    if (_wxeTransaction != null)
      InitializeEvents (_wxeTransaction);
  }

  /// <summary> 
  ///   Handles the <see cref="WxeTransactionBase"/>'s <see cref="WxeTransactionBase.TransactionCommitting"/> event.
  /// </summary>
  private void WxeTransaction_TransactionCommitting (object sender, EventArgs e)
  {
    OnCommitting();
  }

  /// <summary> 
  ///   Handles the <see cref="WxeTransactionBase"/>'s <see cref="WxeTransactionBase.TransactionCommitted"/> event.
  /// </summary>
  private void WxeTransaction_TransactionCommitted (object sender, EventArgs e)
  {
    OnCommitted();
  }

  /// <summary> 
  ///   Handles the <see cref="WxeTransactionBase"/>'s <see cref="WxeTransactionBase.TransactionRollingBack"/> event.
  /// </summary>
  private void WxeTransaction_TransactionRollingBack (object sender, EventArgs e)
  {
    OnRollingBack();
  }

  /// <summary> 
  ///   Handles the <see cref="WxeTransactionBase"/>'s <see cref="WxeTransactionBase.TransactionRolledBack"/> event.
  /// </summary>
  private void WxeTransaction_TransactionRolledBack (object sender, EventArgs e)
  {
    OnRolledBack();
  }

  /// <summary> Called before committing the <see cref="WxeTransactionBase"/>. </summary>
  protected virtual void OnCommitting()
  {
    if (Committing != null)
      Committing (this, EventArgs.Empty);
  }

  /// <summary> Called after the <see cref="WxeTransactionBase"/> has been committed. </summary>
  protected virtual void OnCommitted()
  {
    if (Committed != null)
      Committed (this, EventArgs.Empty);
  }

  /// <summary> Called before rolling the <see cref="WxeTransactionBase"/> back. </summary>
  protected virtual void OnRollingBack()
  {
    if (RollingBack != null)
      RollingBack (this, EventArgs.Empty);
  }

  /// <summary> Called after the <see cref="WxeTransactionBase"/> has been rolled back. </summary>
  protected virtual void OnRolledBack()
  {
    if (RolledBack != null)
      RolledBack (this, EventArgs.Empty);
  }

  /// <summary> Is raises before the<see cref="WxeTransactionBase"/> is committed. </summary>
  /// <remarks> 
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase"/> 
  ///     has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler Committing;
  
  /// <summary> Is raised after the <see cref="WxeTransactionBase"/> has been committed. </summary>
  /// <remarks> 
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase"/> 
  ///     has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler Committed;
  
  /// <summary> Is raised before the <see cref="WxeTransactionBase"/> is rolled back. </summary>
  /// <remarks> 
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase"/> 
  ///     has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler RollingBack;

  /// <summary> Is raised after the <see cref="WxeTransactionBase"/> has been rolled back. </summary>
  /// <remarks> 
  ///   <note type="caution">
  ///     The event handler must be reattached after the <see cref="WxeTransactedFunctionBase"/> 
  ///     has been deserialized.
  ///   </note>
  /// </remarks>
  [field:NonSerialized]
  public event EventHandler RolledBack;
}

}

