using System;

namespace Rubicon.Data.DomainObjects.Web.ExecutionEngine
{
/// <summary>
/// Indicates the behavior of a <see cref="WxeTransactedFunction"/>.
/// </summary>
public enum WxeTransactionMode
{
  /// <summary>Create a new transaction.</summary>
  CreateRoot,
  /// <summary>Never create a transaction.</summary>
  None
}
}
