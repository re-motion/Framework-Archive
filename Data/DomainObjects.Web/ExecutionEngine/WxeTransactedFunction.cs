using System;
using System.Runtime.Serialization;
using Rubicon.Web.ExecutionEngine;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
//TODO Doc: autocommit = true or false?

/// <summary>
/// A <see cref="WxeFunction"/> that will always have a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
/// </summary>
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
  /// <returns>A new WxeTransaction, if <see cref="TransactionMode"/> has a value of <b>CreateRoot</b>; otherwise null.</returns>
  protected override WxeTransactionBase CreateWxeTransaction ()
  {
    // TODO: check this with ML: (true, true) is different to default constructor behavior of WxeTransaction
    if (_transactionMode == TransactionMode.CreateRoot)
      return new WxeTransaction (true, true);

    return null;
  }

  /// <summary>
  /// Gets or sets the <b>TransactionMode</b> of the <see cref="WxeTransactedFunction"/>.
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

