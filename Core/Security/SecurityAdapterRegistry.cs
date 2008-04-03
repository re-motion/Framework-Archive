using System;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Security
{
  [Obsolete ("Use SecurityAdapterRegistry instead. (Version: 1.7.41)", true)]
  public abstract class SecurityProviderRegistry
  {
    public static SecurityProviderRegistry Instance
    {
      get { throw new NotImplementedException ("Use SecurityAdapterRegistry.Instance instead."); }
    }

    [Obsolete ("Use SetAdapter(...) instead. (Version: 1.7.41)", true)]
    public abstract void SetProvider<T> (T value) where T : class, ISecurityProviderObsolete;

    [Obsolete ("Use GetAdapter<T>() instead. (Version: 1.7.41)", true)]
    public abstract T GetProvider<T>() where T : class, ISecurityProviderObsolete;
  }

  /// <summary>Used to register <see cref="ISecurityAdapter"/> instances.</summary>
  /// <remarks>Used by those modules of the framework that do not have binary depedencies to the security module to access security information.</remarks>
  public class SecurityAdapterRegistry
  {
    private static readonly SecurityAdapterRegistry s_instance = new SecurityAdapterRegistry();

    public static SecurityAdapterRegistry Instance
    {
      get { return s_instance; }
    }

    private readonly Dictionary<Type, ISecurityAdapter> _registry = new Dictionary<Type, ISecurityAdapter>();

    protected SecurityAdapterRegistry()
    {
    }

    public void SetAdapter (Type adapterType, ISecurityAdapter value)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("adapterType", adapterType, typeof (ISecurityAdapter));
      ArgumentUtility.CheckType ("value", value, adapterType);

      _registry[adapterType] = value;
    }

    public T GetAdapter<T>() where T : class, ISecurityAdapter
    {
      if (_registry.ContainsKey (typeof (T)))
        return (T) _registry[typeof (T)];
      else
        return null;
    }
  }
}