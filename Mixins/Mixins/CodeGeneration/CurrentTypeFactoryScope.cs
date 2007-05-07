using System;
using Mixins.Definitions;

namespace Mixins.CodeGeneration
{
  public struct CurrentTypeFactoryScope : IDisposable
  {
    private bool _disposed;
    private TypeFactory _oldValue;

    public CurrentTypeFactoryScope (TypeFactory newValue)
    {
      _disposed = false;
      _oldValue = TypeFactory.Current;
      TypeFactory.SetCurrent (newValue);
    }

    public CurrentTypeFactoryScope (ApplicationDefinition configuration)
        : this (new TypeFactory (configuration))
    {
    }

    public void Dispose()
    {
      if (!_disposed)
      {
        TypeFactory.SetCurrent (_oldValue);
        _disposed = true;
      }
    }
  }
}
