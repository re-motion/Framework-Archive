using System;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects;
using Rubicon.Web.ExecutionEngine;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
  /// <summary>
  /// Creates a scope for a <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/>.
  /// </summary>
  /// <remarks>Derived classes can provide specific implementations of <see cref="ClientTransaction"/>s by overriding <see cref="GetRootTransactionFromFunction"/>,
  /// but in most cases. In many cases it will however be more convenient to override <see cref="WxeTransactedFunction.CreateRootTransaction"/>.</remarks>
  [Serializable]
  public class WxeTransaction : WxeTransactionBase<ClientTransaction>
  {
    [NonSerialized]
    private Stack<ClientTransactionScope> _scopeStack;

    /// <summary>Creates a new instance with an empty step list and autoCommit enabled that uses the existing transaction, if one exists.</summary>
    public WxeTransaction ()
        : this (null, true, true)
    {
    }

    /// <summary>Creates a new instance with an empty step list that uses the existing transaction, if one exists.</summary>
    /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
    public WxeTransaction (bool autoCommit)
        : this (null, autoCommit, true)
    {
    }

    /// <summary>Creates a new instance with an empty step list.</summary>
    /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
    /// <param name="forceRoot">If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists.</param>
    public WxeTransaction (bool autoCommit, bool forceRoot)
        : this (null, autoCommit, forceRoot)
    {
    }

    /// <summary>Creates a new instance.</summary>
    /// <param name="steps">Initial step list. Can be <see langword="null"/>.</param>
    /// <param name="autoCommit">If <see langword="true"/>, the transaction is committed after execution, otherwise it is rolled back.</param>
    /// <param name="forceRoot">If <see langword="true"/>, a new root transaction will be created even if a parent transaction exists.</param>
    public WxeTransaction (WxeStepList steps, bool autoCommit, bool forceRoot)
        : base (steps, autoCommit, forceRoot)
    {
    }

    /// <summary>
    /// Gets the current <see cref="Rubicon.Data.DomainObjects.ClientTransaction"/> or <see langword="null"/> if none is set.
    /// </summary>
    /// <remarks>
    /// As opposed to <see cref="ClientTransactionScope.CurrentTransaction"/> this property returns <see langword="null"/>, 
    /// if <see cref="ClientTransactionScope.HasCurrentTransaction"/> is false.
    /// </remarks>
    protected override ClientTransaction CurrentTransaction
    {
      get
      {
        if (ClientTransactionScope.HasCurrentTransaction)
          return ClientTransactionScope.CurrentTransaction;
        else
          return null;
      }
    }

    private Stack<ClientTransactionScope> ScopeStack
    {
      get
      {
        if (_scopeStack == null)
          _scopeStack = new Stack<ClientTransactionScope>();
        return _scopeStack;
      }
    }

    /// <summary>
    /// Sets a new current transaction.
    /// </summary>
    /// <param name="transaction">The new transaction.</param>
    protected override void SetCurrentTransaction (ClientTransaction transaction)
    {
      ScopeStack.Push (ClientTransactionScope.ActiveScope);
      ClientTransactionScope newScope = new ClientTransactionScope (transaction); // set new scope and store old one
      Assertion.IsTrue (ClientTransactionScope.ActiveScope == newScope);
    }

    /// <summary>
    /// Resets the current <see cref="ClientTransaction"/> to the transaction previously replaced via
    /// <see cref="SetCurrentTransaction"/>.
    /// </summary>
    /// <param name="previousTransaction">The transaction previously replaced by <see cref="SetCurrentTransaction"/>.</param>
    protected override void SetPreviousCurrentTransaction (ClientTransaction previousTransaction)
    {
      Assertion.IsTrue (ScopeStack.Count != 0, "RestorePreviousTransaction is never called more often than SetCurrentTransaction.");

      if (ClientTransactionScope.ActiveScope == null)
        throw new InconsistentClientTransactionScopeException ("Somebody else has removed ClientTransactionScope.ActiveScope.");
      else
        ClientTransactionScope.ActiveScope.Leave();

      if (ClientTransactionScope.ActiveScope != ScopeStack.Pop())
        throw new InconsistentClientTransactionScopeException ("ClientTransactionScope.ActiveScope does not contain the expected transaction scope.");

      if (previousTransaction != null && ClientTransactionScope.ActiveScope == null)
      {
        // there was a Thread transition during execution of this function, we need to restore the transaction that was active when this whole thing
        // started
        // we cannot restore the same scope we had on the other thread, but we can restore the transaction
        new ClientTransactionScope (previousTransaction);
      }

      Assertion.IsTrue ((previousTransaction == null && !ClientTransactionScope.HasCurrentTransaction)
        || (previousTransaction != null && ClientTransactionScope.HasCurrentTransaction &&
            ClientTransactionScope.CurrentTransaction == previousTransaction));
    }
  }
}