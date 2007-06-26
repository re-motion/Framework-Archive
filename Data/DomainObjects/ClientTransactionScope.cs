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
    private const string c_callContextKey = "Rubicon.Data.DomainObjects.ClientTransactionScope.CurrentTransaction";

    private ClientTransaction _previousValue;
    private bool _wasDisposed = false;

    /// <summary>
    /// Temporarily sets <see cref="CurrentTransaction"/>.
    /// </summary>
    /// <param name="temporaryCurrentTransaction">The <see cref="ClientTransaction"/> object temporarily used as the current transaction.</param>
    public ClientTransactionScope (ClientTransaction temporaryCurrentTransaction)
    {
      if (ClientTransactionScope.HasCurrentTransaction)
        _previousValue = ClientTransactionScope.CurrentTransaction;

      ClientTransactionScope.SetCurrentTransaction (temporaryCurrentTransaction);
    }

    /// <summary>
    /// Gets a value indicating if a <see cref="ClientTransaction"/> is currently set as <see cref="CurrentTransaction"/>. 
    /// </summary>
    /// <remarks>
    /// Even if the value returned by <b>HasCurrentTransaction</b> is false, <see cref="CurrentTransaction"/> will return a <see cref="ClientTransaction"/>. See <see cref="CurrentTransaction"/> for further de.
    /// </remarks>
    public static bool HasCurrentTransaction
    {
      get { return GetCurrentTransactionInternal () != null ; }
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

    /// <summary>
    /// Resets <see cref="CurrentTransaction"/> to the value it had before this scope was instantiated.
    /// </summary>
    public void Dispose ()
    {
      if (!_wasDisposed)
      {
        ClientTransactionScope.SetCurrentTransaction (_previousValue);
        _wasDisposed = true;
      }
    }

    /// <summary>
    /// Sets the default <b>ClientTransaction</b> of the current thread.
    /// </summary>
    /// <param name="clientTransaction">The <b>ClientTransaction</b> to which the current <b>ClientTransaction</b> is set.</param>
    public static void SetCurrentTransaction (ClientTransaction clientTransaction)
    {
      CallContext.SetData (c_callContextKey, clientTransaction);
    }

    private static ClientTransaction GetCurrentTransactionInternal ()
    {
      return (ClientTransaction) CallContext.GetData (c_callContextKey);
    }
  }
}
