using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Causes all domain objects instantiated by DomainObject.Create to be created with the <see cref="DomainObjectFactory"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Domain objects instantiated by <see cref="DomainObject.Create"/> (e.g. when they are loaded from a data store) are usually created 
  /// via an ordinary constructor invocation. They are only created with the <see cref="DomainObjectFactory"/> if the mapping says so (TODO)
  /// or if their class is annotated with a FactoryInstantiatedAttribute. You can use this class in a using-scope in order to force 
  /// <see cref="DomainObject.Create"/> to create all domain objects via the factory while within the scope.
  /// </para>
  /// <para>
  /// It is vital to call the scope's <see cref="Dispose"/> method, either from a using block or explicitly from the finally part of a try/finally
  /// block. If <see cref="Dispose"/> is not called, the factory will continued to be used, which might break legacy code.
  /// </para>
  /// </remarks>
  public class FactoryInstantiationScope : IDisposable
  {
    [ThreadStatic]
    private static int _scopeCount;

    private bool _disposed;

    public FactoryInstantiationScope ()
    {
      ++_scopeCount;
    }

    public void Dispose ()
    {
      if (!_disposed)
      {
        _disposed = true;
        --FactoryInstantiationScope._scopeCount;
        Debug.Assert (_scopeCount >= 0);
      }
    }

    public static bool WithinScope
    {
      get
      {
        return _scopeCount > 0;
      }
    }
  }
}
