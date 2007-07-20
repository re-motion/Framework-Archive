using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Provides an enumeration to configure <see cref="ClientTransactionScope">ClientTransctionScope's</see> automatic rollback behavior.
  /// </summary>
  public enum AutoRollbackBehavior
  {
    /// <summary>
    /// Indicates that <see cref="ClientTransactionScope"/> should not perform any automatic operation.
    /// </summary>
    None,
    /// <summary>
    /// Indicates that <see cref="ClientTransactionScope"/> should automatically call <see cref="ClientTransaction.Rollback"/> at its end when
    /// its transaction holds uncommitted changed.
    /// </summary>
    Rollback,
    /// <summary>
    /// Indicates that <see cref="ClientTransactionScope"/> should automatically call <see cref="ClientTransaction.ReturnToParentTransaction"/> at
    /// its end.
    /// </summary>
    ReturnToParent
  }
}
