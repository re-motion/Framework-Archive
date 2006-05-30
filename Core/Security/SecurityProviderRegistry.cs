using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class SecurityProviderRegistry
  {
    // types

    // static members

    private static SecurityProviderRegistry s_instance = new SecurityProviderRegistry ();

    public static SecurityProviderRegistry Instance
    {
      get { return s_instance; }
    }

    // member fields

    private Dictionary<Type, ISecurityProvider> _registry = new Dictionary<Type, ISecurityProvider> ();

    // construction and disposing

    protected SecurityProviderRegistry ()
    {
    }

    // methods and properties

    public void SetProvider<T> (T value) where T : class, ISecurityProvider
    {
      _registry[typeof (T)] = value;
    }

    public T GetProvider<T> () where T : class, ISecurityProvider
    {
      if (_registry.ContainsKey (typeof (T)))
        return (T) _registry[typeof (T)];
      else
        return null;
    }
  }
}