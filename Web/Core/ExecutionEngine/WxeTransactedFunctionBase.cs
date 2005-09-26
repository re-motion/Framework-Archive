using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Web;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;

namespace Rubicon.Web.ExecutionEngine
{

[Serializable]
public abstract class WxeTransactedFunctionBase: WxeFunction//, IDeserializationCallback
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
  public abstract WxeTransactionBase CreateWxeTransaction();

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

  protected void InitializeEvents (WxeTransactionBase _wxeTransaction)
  {
    _wxeTransaction.TransactionCommitting += new EventHandler(WxeTransaction_TransactionCommitting);
    _wxeTransaction.TransactionCommitted += new EventHandler(WxeTransaction_TransactionCommitted);
    _wxeTransaction.TransactionRollingBack += new EventHandler(WxeTransaction_TransactionRollingBack);
    _wxeTransaction.TransactionRolledBack += new EventHandler(WxeTransaction_TransactionRolledBack);
  }
//
//  void IDeserializationCallback.OnDeserialization(Object sender) 
//  {
//    InitializeEvents (_wxeTransaction);
//  }

  /// <summary> 
  ///   Do not invoke this method. Must be public because of serialization and will become private with .net 2.0.
  /// </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void WxeTransaction_TransactionCommitting(object sender, EventArgs e)
  {
    OnCommitting();
  }

  /// <summary> 
  ///   Do not invoke this method. Must be public because of serialization and will become private with .net 2.0.
  /// </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void WxeTransaction_TransactionCommitted(object sender, EventArgs e)
  {
    OnCommitted();
  }

  /// <summary> 
  ///   Do not invoke this method. Must be public because of serialization and will become private with .net 2.0.
  /// </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void WxeTransaction_TransactionRollingBack(object sender, EventArgs e)
  {
    OnRollingBack();
  }

  /// <summary> 
  ///   Do not invoke this method. Must be public because of serialization and will become private with .net 2.0.
  /// </summary>
  [EditorBrowsable (EditorBrowsableState.Never)]
  public void WxeTransaction_TransactionRolledBack(object sender, EventArgs e)
  {
    OnRolledBack();
  }

  /// <summary> Called before committing the <see cref="WxeTransaction"/>. </summary>
  protected virtual void OnCommitting()
  {
  }

  /// <summary> Called after the <see cref="WxeTransaction"/> has been committed. </summary>
  protected virtual void OnCommitted()
  {
  }

  /// <summary> Called before rolling the <see cref="WxeTransaction"/> back. </summary>
  protected virtual void OnRollingBack()
  {
  }

  /// <summary> Called after the <see cref="WxeTransaction"/> has been rolled back. </summary>
  protected virtual void OnRolledBack()
  {
  }
}

}

