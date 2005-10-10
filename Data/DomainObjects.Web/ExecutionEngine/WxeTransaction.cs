using System;
using Rubicon.Data;
using Rubicon.Web.ExecutionEngine;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{

/// <summary>
/// Creates a scope for a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
/// </summary>
[Serializable]
public class WxeTransaction : WxeTransactionBase
{
  /// <summary>Creates a new instance with an empty step list and autoCommit enabled that uses the existing transaction, if one exists.</summary>
  public WxeTransaction () : this (null, true, true)
  {
  }

  /// <summary>Creates a new instance with an empty step list that uses the existing transaction, if one exists.</summary>
  /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
  public WxeTransaction (bool autoCommit) : this (null, autoCommit, true)
  {
  }

  /// <summary>Creates a new instance with an empty step list.</summary>
  /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
  /// <param name="forceRoot">If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists.</param>
  public WxeTransaction (bool autoCommit, bool forceRoot) : this (null, autoCommit, forceRoot)
  {
  }

  /// <summary>Creates a new instance.</summary>
  /// <param name="steps">Initial step list. Can be <see langword="null"/>.</param>
  /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
  /// <param name="forceRoot">If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists.</param>
  public WxeTransaction (WxeStepList steps, bool autoCommit, bool forceRoot) : base (steps, autoCommit, forceRoot)
  {
  }

  /// <summary>
  /// Gets the current <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> or <see langword="null"/> if none is set.
  /// </summary>
  /// <remarks>
  /// As opposed to <see cref="Rubicon.Data.DomainObjects.ClientTransaction.Current"/> this property returns <see langword="null"/>, 
  /// if <see cref="Rubicon.Data.DomainObjects.ClientTransaction.HasCurrent"/> is false.
  /// </remarks>
  protected override ITransaction CurrentTransaction
  {
    get 
    { 
      if (ClientTransaction.HasCurrent)
        return ClientTransaction.Current;
      else
        return null;
    }
  }

  /// <summary>
  /// Sets a new current transaction.
  /// </summary>
  /// <param name="transaction">The new transaction. This must be a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> or derived type.</param>
  /// <exception cref="System.InvalidCastException"><paramref name="transaction"/> cannot be casted to <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.</exception>
  protected override void SetCurrentTransaction (ITransaction transaction)
  {
    ClientTransaction.SetCurrent ((ClientTransaction) transaction);
  }

  /// <summary>
  /// Creates a new <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> object.
  /// </summary>
  /// <returns>A new <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.</returns>
  protected override ITransaction CreateRootTransaction ()
  {
    return new ClientTransaction ();
  }
}
}