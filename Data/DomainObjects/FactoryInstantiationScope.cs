using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Rubicon.Data.DomainObjects.Infrastructure;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects
{
  /// <summary>
  /// Causes all domain objects instantiated by <see cref="DomainObject.Create"/> or <see cref="DomainObject.GetObject"/> to be created with
  /// the <see cref="DomainObjectFactory"/>.
  /// </summary>
  /// <remarks>
  /// <para>
  /// Usually, domain objects instantiated by <see cref="DomainObject.Create"/> or <see cref="DomainObject.GetObject"/> are allocated
  /// via an ordinary constructor invocation. They are only created with the <see cref="DomainObjectFactory"/> if the mapping is reflection based. 
  /// You can use this class in a using-scope in order to force <see cref="DomainObject"/> to create all domain objects via the factory while 
  /// within the scope.
  /// </para>
  /// <para>
  /// It is vital to call the scope's <see cref="Dispose"/> method, either from a using block or explicitly from the finally part of a try/finally
  /// block. If <see cref="Dispose"/> is not called, the factory will continued to be used, which might break legacy code.
  /// </para>
  /// </remarks>
  /// <example>
  /// <code>
  /// using (new FactoryInstantiationScope())
  /// {
  ///   // in here, every DomainObject instantiated via DomainObject.GetObject or DomainObject.Create is automatically created
  ///   // via the DomainObjectFactory
  /// }
  /// </code>
  /// </example>
  public class FactoryInstantiationScope : IDisposable
  {
    [ThreadStatic]
    private static int s_scopeCount;

    private bool _disposed;

    public FactoryInstantiationScope ()
    {
      ++s_scopeCount;
    }

    public void Dispose ()
    {
      if (!_disposed)
      {
        _disposed = true;
        --FactoryInstantiationScope.s_scopeCount;
        Assertion.Assert (s_scopeCount >= 0);
      }
    }

    public static bool WithinScope
    {
      get
      {
        return s_scopeCount > 0;
      }
    }
  }
}
