using System;
using System.Runtime.Remoting.Messaging;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Manages a thread's <see cref="CurrentTransaction"/> and provides a mechanism for temporarily setting it to a given
  /// <see cref="ClientTransaction"/> instance in a scoped way. Optionally, the scope can also automatically rollback a transaction at the end
  /// of the scope.
  /// </summary>
  /// <remarks>
  /// <para>
  /// The constructor of this class temporarily sets <see cref="CurrentTransaction"/> to the given transaction intance, remembering
  /// its previous value. The <see cref="Leave"/> method resets <see cref="CurrentTransaction"/> to the remembered value (executing the scope's
  /// <see cref="AutoRollbackBehavior"/> as applicable). Employ a <c>using</c> block to set a new <see cref="CurrentTransaction"/> for the current
  /// thread and to restore the previous transaction (and execute the <see cref="AutoRollbackBehavior"/>) in a scoped way. If
  /// <see cref="Leave"/> is not called, the previous transaction is not automatically restored and the <see cref="AutoRollbackBehavior"/> is not
  /// executed, but other than that no resource leaks or problems are to be expected.
  /// </para>
  /// </remarks>
  public class ClientTransactionScope : IDisposable
  {
    /// <summary>
    /// Gets a value indicating if a <see cref="ClientTransaction"/> is currently set as <see cref="CurrentTransaction"/>. 
    /// </summary>
    /// <remarks>
    /// Even if the value returned by <b>HasCurrentTransaction</b> is false, <see cref="CurrentTransaction"/> will return a
    /// <see cref="ClientTransaction"/>. See <see cref="CurrentTransaction"/> for further information.
    /// </remarks>
    public static bool HasCurrentTransaction
    {
      get { return GetCurrentTransactionInternal () != null; }
    }

    /// <summary>
    /// Gets the default <b>ClientTransaction</b> of the current thread. 
    /// </summary>
    /// <remarks>If there is no <see cref="ClientTransaction"/> associated with the current thread, a new <see cref="ClientTransaction"/> is
    /// created.</remarks>
    public static ClientTransaction CurrentTransaction
    {
      get
      {
        if (!HasCurrentTransaction)
          SetCurrentTransaction (new ClientTransaction ());

        return GetCurrentTransactionInternal ();
      }
    }

    public static ClientTransactionScope ActiveScope
    {
      get { return (ClientTransactionScope) CallContext.GetData (c_callContextScopeKey);}
    }

    private static ClientTransaction GetCurrentTransactionInternal ()
    {
      return (ClientTransaction) CallContext.GetData (c_callContextKey);
    }

    private static void SetCurrentTransaction (ClientTransaction clientTransaction)
    {
      CallContext.SetData (c_callContextKey, clientTransaction);
    }

    private static void SetActiveScope (ClientTransactionScope scope)
    {
      CallContext.SetData (c_callContextScopeKey, scope);
    }

    private const string c_callContextKey = "Rubicon.Data.DomainObjects.ClientTransactionScope.CurrentTransaction";
    private const string c_callContextScopeKey = "Rubicon.Data.DomainObjects.ClientTransactionScope.ActiveScope";

    private ClientTransactionScope _previousScope;
    private ClientTransaction _previousTransaction;
    private ClientTransaction _scopedTransaction;
    private bool _hasBeenLeft = false;
    private bool _autoEnlistDomainObjects = false;
    private AutoRollbackBehavior _autoRollbackBehavior;

    /// <summary>
    /// Creates a new <see cref="ClientTransaction"/> and assigns it to the <see cref="CurrentTransaction"/> property.
    /// </summary>
    /// <remarks>By default, any changes made to the scope's transaction which are not committed are automatically rolled back at the end of the
    /// scope. See also <see cref="AutoRollbackBehavior"/>.</remarks>
    public ClientTransactionScope ()
      : this (AutoRollbackBehavior.Rollback)
    {
    }

    /// <summary>
    /// Creates a new <see cref="ClientTransaction"/> and assigns it to the <see cref="CurrentTransaction"/> property.
    /// </summary>
    /// <param name="autoRollbackBehavior">Specifies the automatic rollback behavior to be exhibited by this scope.</param>
    public ClientTransactionScope (AutoRollbackBehavior autoRollbackBehavior)
      : this (new ClientTransaction (), autoRollbackBehavior)
    {
    }

    /// <summary>
    /// Temporarily sets <see cref="CurrentTransaction"/>.
    /// </summary>
    /// <param name="scopedCurrentTransaction">The <see cref="ClientTransaction"/> object temporarily used as the current transaction.</param>
    /// <remarks>By default, no changes made within the scope are automatically rolled back. See also <see cref="AutoRollbackBehavior"/>.</remarks>
    public ClientTransactionScope (ClientTransaction scopedCurrentTransaction)
        : this (scopedCurrentTransaction, AutoRollbackBehavior.None)
    {
    }

    /// <summary>
    /// Temporarily sets <see cref="CurrentTransaction"/>.
    /// </summary>
    /// <param name="scopedCurrentTransaction">The <see cref="ClientTransaction"/> object temporarily used as the current transaction.</param>
    /// <param name="autoRollbackBehavior">Specifies the automatic rollback behavior to be exhibited by this scope.</param>
    public ClientTransactionScope (ClientTransaction scopedCurrentTransaction, AutoRollbackBehavior autoRollbackBehavior)
    {
      _autoRollbackBehavior = autoRollbackBehavior;

      _previousScope = ClientTransactionScope.ActiveScope;

      if (ClientTransactionScope.HasCurrentTransaction)
        _previousTransaction = ClientTransactionScope.CurrentTransaction;

      ClientTransactionScope.SetCurrentTransaction (scopedCurrentTransaction);
      ClientTransactionScope.SetActiveScope (this);
      _scopedTransaction = scopedCurrentTransaction;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this scope will automatically call <see cref="ClientTransaction.Rollback"/> on a transaction
    /// with uncommitted changed objects when the scope's <see cref="Leave"/> method is invoked.
    /// </summary>
    /// <value>An <see cref="AutoRollbackBehavior"/> value indicating how the scope should behave when it is disposed and its transaction's changes
    /// have not been committed.</value>
    public AutoRollbackBehavior AutoRollbackBehavior
    {
      get { return _autoRollbackBehavior; }
      set { _autoRollbackBehavior = value; }
    }

    /// <summary>
    /// Gets the transaction this scope was created for.
    /// </summary>
    /// <value>The transaction passed to the scope's constructor or automatically created by the scope.</value>
    public ClientTransaction ScopedTransaction
    {
      get { return _scopedTransaction; }
    }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="DomainObject"/> instances are automatically enlisted in the current transaction within
    /// this scope.
    /// </summary>
    /// <value>True if <see cref="DomainObject"/> instances are automatically enlisted in the current transaction when accessed;
    /// otherwise, false.</value>
    public bool AutoEnlistDomainObjects
    {
      get { return _autoEnlistDomainObjects; }
      set { _autoEnlistDomainObjects = value; }
    }

    /// <summary>
    /// Resets <see cref="CurrentTransaction"/> to the value it had before this scope was instantiated and performs the
    /// <see cref="AutoRollbackBehavior"/>. This method is ignored when executed more than once.
    /// </summary>
    public void Leave ()
    {
      if (_hasBeenLeft)
        throw new InvalidOperationException ("The ClientTransactionScope has already been left.");

      ExecuteAutoRollbackBehavior ();
      ClientTransactionScope.SetCurrentTransaction (_previousTransaction);
      ClientTransactionScope.SetActiveScope (_previousScope);
      _hasBeenLeft = true;
    }

    void IDisposable.Dispose ()
    {
      Leave ();
    }

    private void ExecuteAutoRollbackBehavior ()
    {
      if (AutoRollbackBehavior == AutoRollbackBehavior.Rollback && ScopedTransaction.HasChanged())
        Rollback ();
    }

    /// <summary>
    /// Commits the transaction scoped by this object. This is equivalent to <c>ScopedTransaction.Commt()</c>.
    /// </summary>
    /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
    /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the datasource.</exception>
    public void Commit ()
    {
      if (ScopedTransaction != null)
        ScopedTransaction.Commit ();
    }

    /// <summary>
    /// Performs a rollback on the transaction scoped by this object. This is equivalent to <c>ScopedTransaction.Rollback()</c>.
    /// </summary>
    public void Rollback ()
    {
      if (ScopedTransaction != null)
        ScopedTransaction.Rollback ();
    }
  }
}
