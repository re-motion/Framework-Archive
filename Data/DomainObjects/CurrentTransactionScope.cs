using System;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Provides a mechanism for temporarily setting <see cref="ClientTransaction.Current"/> to a given <see cref="ClientTransaction"/> instance.
  /// </summary>
  /// <remarks>The constructor of this class temporarily sets <see cref="ClientTransaction.Current"/> to the given transaction intance, remembering
  /// its previous value. The <see cref="Dispose"/> method resets <see cref="ClientTransaction.Current"/> to the remembered value.</remarks>
  public class CurrentTransactionScope : IDisposable
  {
    private ClientTransaction _previousValue;
    private bool _wasDisposed = false;

    /// <summary>
    /// Temporarily sets <see cref="ClientTransaction.Current"/>.
    /// </summary>
    /// <param name="temporaryCurrentTransaction">The <see cref="ClientTransaction"/> object temporarily used as the current transaction.</param>
    public CurrentTransactionScope (ClientTransaction temporaryCurrentTransaction)
    {
      if (ClientTransaction.HasCurrent)
      {
        _previousValue = ClientTransaction.Current;
      }
      ClientTransaction.SetCurrent (temporaryCurrentTransaction);
    }

    /// <summary>
    /// Resets <see cref="ClientTransaction.Current"/> to the value it had before this scope was instantiated.
    /// </summary>
    public void Dispose ()
    {
      if (!_wasDisposed)
      {
        ClientTransaction.SetCurrent (_previousValue);
        _wasDisposed = true;
      }
    }
  }
}
