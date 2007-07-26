using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
  /// <summary>
  /// Thrown when a <see cref="WxeTransaction"/> cannot perform correct <see cref="ClientTransaction"/> handling, because one of its execution
  /// steps left an incorrect <see cref="ClientTransactionScope"/> state.
  /// </summary>
  public class InconsistentClientTransactionScopeException : Exception
  {
    public InconsistentClientTransactionScopeException (string message)
      : base (message)
    {
    }
  }
}
