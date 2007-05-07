using System;
using Mixins.Definitions;

namespace Mixins.CodeGeneration
{
  public struct CurrentTypeFactoryScope: IDisposable
  {
    private bool _disposed;
    private TypeFactory _oldValue;

    public CurrentTypeFactoryScope (TypeFactory newValue)
    {
      _disposed = false;
      if (TypeFactory.HasCurrent)
        _oldValue = TypeFactory.Current;
      else
        _oldValue = null;
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