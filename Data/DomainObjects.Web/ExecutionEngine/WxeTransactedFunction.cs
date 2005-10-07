using System;
using System.Runtime.Serialization;
using Rubicon.Web.ExecutionEngine;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
/// <summary>
/// A <see cref="WxeFunction"/> that will always have a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
/// </summary>
/// <remarks>
/// <para>A <b>WxeTransactedFunction</b> always creates a new <see cref="ClientTransaction"/>, unless <see cref="TransactionMode"/>.<b>None</b> 
/// is passed to the constructor.<br />
/// Therefore you should not pass <see cref="DomainObject"/>s to a <b>WxeTransactedFunction</b> as parameters since it is not allowed to use a 
/// <see cref="DomainObject"/> from one <see cref="ClientTransaction"/> in another <see cref="ClientTransaction"/>.<br />
/// Pass the corresponding <see cref="ObjectID"/>s instead and use the <see cref="DomainObject.GetObject"/> or <see cref="ClientTransaction.GetObject"/> method.</para>
/// <para>A <b>WxeTransactedFunction</b> has <see cref="AutoCommit"/> set to <b>true</b> by default. <br />
/// To change this behavior for a function you can overwrite this property.</para>
/// </remarks>
[Serializable]
public class WxeTransactedFunction: WxeTransactedFunctionBase
{
  private TransactionMode _transactionMode;

  /// <summary>
  /// Creates a new <b>WxeTransactedFunction</b> that has a new <see cref="ClientTransaction"/>.
  /// </summary>
  /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
  public WxeTransactedFunction (params object[] actualParameters) : this (TransactionMode.CreateRoot, actualParameters)
  {
  }

  /// <summary>
  /// Creates a new <b>WxeTransactedFunction</b>
  /// </summary>
  /// <param name="transactionMode">A value indicating the behavior of the WxeTransactedFunction.</param>
  /// <param name="actualParameters">Parameters that are passed to the <see cref="WxeFunction"/>.</param>
  public WxeTransactedFunction (TransactionMode transactionMode, params object[] actualParameters) : base (actualParameters)
  {
    ArgumentUtility.CheckValidEnumValue (transactionMode, "transactionMode");

    _transactionMode = transactionMode;
  }

  /// <summary>
  /// Creates a new <see cref="WxeTransaction"/> depending on the value of <see cref="TransactionMode"/>.
  /// </summary>
  /// <returns>A new WxeTransaction, if <see cref="TransactionMode"/> has a value of <b>CreateRoot</b>; otherwise <see langword="null"/>.</returns>
  protected override WxeTransactionBase CreateWxeTransaction ()
  {
    if (_transactionMode == TransactionMode.CreateRoot)
      return new WxeTransaction (AutoCommit, true);

    return null;
  }

  /// <summary>
  /// Gets a value indicating if the function performs an automatic commit on the <see cref="ClientTransaction"/>.
  /// </summary>
  /// <note type="inheritinfo">Overwrite this property to change the behavior of the function.</note>
  protected virtual bool AutoCommit
  {
    get { return true; }
  }

  /// <summary>
  /// Gets or sets the <see cref="TransactionMode"/> of the <see cref="WxeTransactedFunction"/>.
  /// </summary>
  /// <remarks>The property must not be set after execution of the function has started.</remarks>
  /// <exception cref="System.InvalidOperationException">An attempt to set the property is performed after execution of the function has started.</exception>
  public TransactionMode TransactionMode
  {
    get 
    { 
      return _transactionMode; 
    }
    set 
    {
      ArgumentUtility.CheckValidEnumValue (value, "transactionMode");

      if (ExecutionStarted)
        throw new InvalidOperationException ("CreateTransactionMode must not be set after execution of this function has started.");

      _transactionMode = value; 
    }
  }
}
}

