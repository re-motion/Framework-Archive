using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Configuration.Provider;

namespace Rubicon.Configuration
{
  /// <summary>Base class for all providers.</summary>
  /// <remarks>
  /// <see cref="ExtendedProviderBase"/> changes the protocoll for initializing a configuration provider from using a default constructor
  /// followed by a call to <see cref="Initialize"/> to initialize the provider during construction.
  /// </remarks>
  public abstract class ExtendedProviderBase: ProviderBase
  {
    /// <summary>Initializes a new instance of the <see cref="ExtendedProviderBase"/>.</summary>
    /// <param name="name">The friendly name of the provider. Must not be <see langword="null" /> or empty.</param>
    /// <param name="config">
    /// A collection of the name/value pairs representing the provider-specific attributes specified in the configuration for this provider.
    /// Must not be <see langword="null" />.
    /// </param>
    protected ExtendedProviderBase (string name, NameValueCollection config)
    {
      Initialize (name, config);
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    public override sealed void Initialize (string name, NameValueCollection config)
    {
      base.Initialize (name, config);
    }
  }
}