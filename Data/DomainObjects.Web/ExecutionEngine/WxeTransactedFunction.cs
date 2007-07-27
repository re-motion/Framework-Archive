using System;
using Rubicon.Utilities;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
  /// <summary>
  /// A <see cref="WxeFunction"/> that will always have a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
  /// </summary>
  /// <remarks>
  /// <para>A <b>WxeTransactedFunction</b> always creates a new <see cref="ClientTransaction"/>, unless <see cref="WxeTransactionMode"/>.<b>None</b> 
  /// is passed to the constructor.<br />
  /// Therefore you should not pass <see cref="DomainObject"/>s to a <b>WxeTransactedFunction</b> as parameters since it is not allowed to use a 
  /// <see cref="DomainObject"/> from one <see cref="ClientTransaction"/> in another <see cref="ClientTransaction"/>.<br />
  /// Pass the corresponding <see cref="ObjectID"/>s instead and use the <see cref="DomainObject.GetObject"/> or <see cref="ClientTransaction.GetObject"/> method.</para>
  /// <para>A <b>WxeTransactedFunction</b> has <see cref="AutoCommit"/> set to <see langword="true"/> by default. <br />
  /// To change this behavior for a function you can overwrite this property.</para>
  /// </remarks>
  [Serializable]
  public class WxeTransactedFunction : WxeTransactedFunctionBase<ClientTransaction>
  {
    private WxeTransactionMode _transactionMode;

    /// <summary>
    /// Creates a new <b>WxeTransactedFunction</b> that has a new <see cref="ClientTransaction"/>.
    /// </summary>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeTransactedFunction (params object[] actualParameters)
        : this (WxeTransactionMode.CreateRoot, actualParameters)
    {
    }

    /// <summary>
    /// Creates a new <b>WxeTransactedFunction</b>
    /// </summary>
    /// <param name="transactionMode">A value indicating the behavior of the WxeTransactedFunction.</param>
    /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
    public WxeTransactedFunction (WxeTransactionMode transactionMode, params object[] actualParameters)
        : base (actualParameters)
    {
      ArgumentUtility.CheckValidEnumValue ("transactionMode", transactionMode);

      _transactionMode = transactionMode;
    }

    /// <summary>
    /// Gets a reference to the current <see cref="ClientTransaction"/>.
    /// </summary>
    public new ClientTransaction Transaction
    {
      get { return base.OwnTransaction; }
    }

    /// <summary>
    /// Gets or sets the <see cref="WxeTransactionMode"/> of the <see cref="WxeTransactedFunction"/>.
    /// </summary>
    /// <remarks>The property must not be set after execution of the function has started.</remarks>
    /// <exception cref="System.InvalidOperationException">An attempt to set the property is performed after execution of the function has started.</exception>
    public WxeTransactionMode TransactionMode
    {
      get { return _transactionMode; }
      set
      {
        ArgumentUtility.CheckValidEnumValue ("transactionMode", value);

        if (ExecutionStarted)
          throw new InvalidOperationException ("CreateTransactionMode must not be set after execution of this function has started.");

        _transactionMode = value;
      }
    }

    /// <summary>
    /// Creates a new <see cref="WxeTransaction"/> depending on the value of <see cref="WxeTransactionMode"/>. 
    /// </summary>
    /// <returns>A new WxeTransaction, if <see cref="WxeTransactionMode"/> has a value of <b>CreateRoot</b>; otherwise <see langword="null"/>.</returns>
    protected override sealed WxeTransactionBase<ClientTransaction> CreateWxeTransaction ()
    {
      switch (_transactionMode)
      {
        case WxeTransactionMode.CreateRoot:
          return CreateWxeTransaction (AutoCommit, true);
        case WxeTransactionMode.CreateChildIfParent:
          return CreateWxeTransaction (AutoCommit, false);
        default:
          return null;
      }
    }

    /// <summary>
    /// Creates a new <see cref="WxeTransaction"/>. Derived classes should override this method to use their own <see cref="WxeTransaction"/>.
    /// </summary>
    /// <param name="autoCommit"><b>autoCommit</b> is forwarded to <see cref="WxeTransaction"/>'s constructor. For further information see <see cref="WxeTransaction"/>.</param>
    /// <param name="forceRoot"><b>forceRoot</b> is forwarded to <see cref="WxeTransaction"/>'s constructor. For further information see <see cref="WxeTransaction"/>.</param>
    /// <returns>A new WxeTransaction.</returns>
    protected virtual WxeTransaction CreateWxeTransaction (bool autoCommit, bool forceRoot)
    {
      return new WxeTransaction (autoCommit, forceRoot);
    }

    /// <summary>
    /// Gets a value indicating if the function performs an automatic commit on the <see cref="ClientTransaction"/>.
    /// </summary>
    /// <note type="inheritinfo">Overwrite this property to change the behavior of the function.</note>
    protected virtual bool AutoCommit
    {
      get { return true; }
    }
  }
}