using System;
using System.Runtime.Serialization;
using Rubicon.Web.ExecutionEngine;

using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
//TODO Doc: autocommit = true or false? 
//TODO Doc: Never pass DomainObjects between WXE functions! Use only ObjectIDs, because the other function could use another ClientTransaction.
//TODO Doc: Provide general namespace information for Rubicon.Data.DomainObjects.Web.ExecutionEngine and integrate it into "Rubicon Commons.chm" start page.
//TODO Doc: Provide information on all undocumented Rubicon.Data.DomainObjects namespaces that they are not intended to be used directly from applications.
 
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
    if (_transactionMode == TransactionMode.CreateRoot)
      return new WxeTransaction (AutoCommit, true);

    return null;
  }

  // TODO Doc:
  protected virtual bool AutoCommit
  {
    get { return true; }
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

