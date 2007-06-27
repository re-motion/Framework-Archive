using System;
using System.Runtime.Remoting.Messaging;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Provides a mechanism for temporarily setting <see cref="CurrentTransaction"/> to a given <see cref="ClientTransaction"/> instance.
  /// </summary>
  /// <remarks>The constructor of this class temporarily sets <see cref="CurrentTransaction"/> to the given transaction intance, remembering
  /// its previous value. The <see cref="Dispose"/> method resets <see cref="CurrentTransaction"/> to the remembered value.</remarks>
  public class ClientTransactionScope : IDisposable
  {
    /// <summary>
    /// Gets a value indicating if a <see cref="ClientTransaction"/> is currently set as <see cref="CurrentTransaction"/>. 
    /// </summary>
    /// <remarks>
    /// Even if the value returned by <b>HasCurrentTransaction</b> is false, <see cref="CurrentTransaction"/> will return a <see cref="ClientTransaction"/>. See <see cref="CurrentTransaction"/> for further de.
    /// </remarks>
    public static bool HasCurrentTransaction
    {
      get { return GetCurrentTransactionInternal () != null; }
    }

    /// <summary>
    /// Gets the default <b>ClientTransaction</b> of the current thread. 
    /// </summary>
    /// <remarks>If there is no <see cref="ClientTransaction"/> associated with the current thread, a new <see cref="ClientTransaction"/> is created.</remarks>
    public static ClientTransaction CurrentTransaction
    {
      get
      {
        if (!HasCurrentTransaction)
          SetCurrentTransaction (new ClientTransaction ());

        return GetCurrentTransactionInternal ();
      }
    }

    private static ClientTransaction GetCurrentTransactionInternal ()
    {
      return (ClientTransaction) CallContext.GetData (c_callContextKey);
    }

   /// <summary>
    /// Sets the default <b>ClientTransaction</b> of the current thread.
    /// </summary>
    /// <param name="clientTransaction">The <b>ClientTransaction</b> to which the current <b>ClientTransaction</b> is set.</param>
    public static void SetCurrentTransaction (ClientTransaction clientTransaction)
    {
      CallContext.SetData (c_callContextKey, clientTransaction);
    }

    private const string c_callContextKey = "Rubicon.Data.DomainObjects.ClientTransactionScope.CurrentTransaction";

    private ClientTransaction _previousTransaction;
    private ClientTransaction _scopedTransaction;
    private bool _wasDisposed = false;
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

      if (ClientTransactionScope.HasCurrentTransaction)
        _previousTransaction = ClientTransactionScope.CurrentTransaction;

      ClientTransactionScope.SetCurrentTransaction (scopedCurrentTransaction);
      _scopedTransaction = scopedCurrentTransaction;
    }

    /// <summary>
    /// Gets or sets a value indicating whether this scope will automatically call <see cref="ClientTransaction.Rollback"/> on a transaction
    /// with uncommitted changed objects when the scope's <see cref="Dispose"/> method is invoked.
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
    /// Resets <see cref="CurrentTransaction"/> to the value it had before this scope was instantiated and performs the
    /// <see cref="AutoRollbackBehavior"/>. This method is ignored when executed more than once.
    /// </summary>
    public void Dispose ()
    {
      if (!_wasDisposed)
      {
        ExecuteAutoRollbackBehavior ();
        ClientTransactionScope.SetCurrentTransaction (_previousTransaction);
        _wasDisposed = true;
      }
    }

    private void ExecuteAutoRollbackBehavior ()
    {
      if (AutoRollbackBehavior == AutoRollbackBehavior.Rollback && ScopedTransaction.HasChanged())
      {
        Rollback ();
      }
    }

    /// <summary>
    /// Commits the transaction scoped by this object. This is equivalent to <c>ScopedTransaction.Commt()</c>.
    /// </summary>
    /// <exception cref="Persistence.PersistenceException">Changes to objects from multiple storage providers were made.</exception>
    /// <exception cref="Persistence.StorageProviderException">An error occurred while committing the changes to the datasource.</exception>
    public void Commit ()
    {
      ScopedTransaction.Commit ();
    }

    /// <summary>
    /// Performs a rollback on the transaction scoped by this object. This is equivalent to <c>ScopedTransaction.Rollback()</c>.
    /// </summary>
    public void Rollback ()
    {
      ScopedTransaction.Rollback ();
    }
  }
}
