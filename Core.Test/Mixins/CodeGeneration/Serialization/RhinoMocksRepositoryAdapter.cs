using System;
using Remotion.Development.UnitTesting;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.UnitTests.Mixins.CodeGeneration.Serialization
{
  public class RhinoMocksRepositoryAdapter : IMockRepository
  {
    private readonly MockRepository _mockRepository;

    public RhinoMocksRepositoryAdapter ()
        : this (new MockRepository())
    {
    }

    public RhinoMocksRepositoryAdapter (MockRepository mockRepository)
    {
      ArgumentUtility.CheckNotNull ("mockRepository", mockRepository);
      _mockRepository = mockRepository;
    }

    public T CreateMock<T> (params object[] argumentsForConstructor)
    {
      return _mockRepository.CreateMock<T> (argumentsForConstructor);
    }

    public void ReplayAll ()
    {
      _mockRepository.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mockRepository.VerifyAll ();
    }

    public IDisposable Ordered ()
    {
      return _mockRepository.Ordered ();
    }

    public void LastCall_IgnoreArguments ()
    {
      LastCall.IgnoreArguments ();
    }
  }
}