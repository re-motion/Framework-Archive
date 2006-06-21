using System;
using System.Collections.Generic;
using System.Text;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// An value indicating the <see cref="IClientTransactionExtension"/> which data is being retrieved from a <see cref="DomainObject"/>.
  /// </summary>
  public enum RetrievalType
  {
    /// <summary>
    /// The original value is being retrieved. See <see cref="PropertyValue.OriginalValue"/>, <see cref="DomainObject.GetOriginalRelatedObject"/> and 
    /// <see cref="DomainObject.GetOriginalRelatedObjects"/> for further information.
    /// </summary>
    OriginalValue = 0,

    /// <summary>
    /// The current value is being retrieved. See <see cref="DataContainer.GetValue"/>, <see cref="DomainObject.GetRelatedObject"/> and 
    /// <see cref="DomainObject.GetRelatedObjects"/> for further information.
    /// </summary>
    CurrentValue = 1
  }
}
