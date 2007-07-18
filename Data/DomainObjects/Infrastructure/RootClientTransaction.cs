using System;
using System.Collections.Generic;

namespace Rubicon.Data.DomainObjects.Infrastructure
{
  /// <summary>
  /// Represents a top-level <see cref="ClientTransaction"/>, which does not have a parent transaction.
  /// </summary>
  [Serializable]
  public class RootClientTransaction : ClientTransaction
  {
    /// <summary>
    /// Initializes a new instance of the <b>RootClientTransaction</b> class.
    /// </summary>
    public RootClientTransaction ()
      : base (new Dictionary<Enum, object>(), new ClientTransactionExtensionCollection ())
    {
    }

    public override ClientTransaction ParentTransaction
    {
      get { return null; }
    }

    public override ClientTransaction RootTransaction
    {
      get { return this; }
    }

    public override bool ReturnToParentTransaction ()
    {
      return false;
    }
  }
}