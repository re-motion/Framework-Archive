using System;
namespace Remotion.Security
{
  [Obsolete ("Use ISecurityAdapter instead. (Version: 1.7.41)")]
  public interface ISecurityProviderObsolete : ISecurityAdapter
  {
  }

  /// <summary>
  /// Marker interface, used as type parameter for the <see cref="SecurityAdapterRegistry.SetAdapter"/> and 
  /// <see cref="SecurityAdapterRegistry.GetAdapter"/> methods of <see cref="SecurityAdapterRegistry"/>.
  /// </summary>
  public interface ISecurityAdapter
  {
  }
}
