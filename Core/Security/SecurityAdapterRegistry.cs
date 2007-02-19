using System;
using System.Collections.Generic;

namespace Rubicon.Security
{
  [Obsolete ("Use SecurityAdapterRegistry instead. (Version: 1.7.41)", true)]
  public abstract class SecurityProviderRegistry
  {
    public static SecurityProviderRegistry Instance
    {
      get { throw new NotImplementedException ("Use SecurityAdapterRegistry.Instance instead."); }
    }

    [Obsolete ("Use SetAdapter<T>(...) instead. (Version: 1.7.41)", true)]
    public abstract void SetProvider<T> (T value) where T : class, ISecurityProviderObsolete;

    [Obsolete ("Use GetAdapter<T>() instead. (Version: 1.7.41)", true)]
    public abstract T GetProvider<T>() where T : class, ISecurityProviderObsolete;
  }

  /// <summary>Used to register <see cref="ISecurityAdapter"/> instances.</summary>
  /// <remarks>Used by those modules of the framework that do not have binary depedencies on the security module to access security information.</remarks>
  public class SecurityAdapterRegistry
  {
    // types

    // static members

    private static SecurityAdapterRegistry s_instance = new SecurityAdapterRegistry();

    public static SecurityAdapterRegistry Instance
    {
      get { return s_instance; }
    }

    // member fields

    private Dictionary<Type, ISecurityAdapter> _registry = new Dictionary<Type, ISecurityAdapter>();

    // construction and disposing

    protected SecurityAdapterRegistry()
    {
    }

    // methods and properties

    public void SetAdapter<T> (T value) where T : class, ISecurityAdapter
    {
      _registry[typeof (T)] = value;
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