using Rubicon.Reflection;

namespace Rubicon.Mixins.CodeGeneration
{
  public struct InvokeWithWrapper<T>
  {
    private readonly IInvokeWith<T> _invokeWith;

    public InvokeWithWrapper(IInvokeWith<T> invokeWith)
    {
      _invokeWith = invokeWith;
    }

    public T With()
    {
      return _invokeWith.With();
    }

    public T With<A1> (A1 a1)
    {
      return _invokeWith.With (a1);
    }

    public T With<A1, A2> (A1 a1, A2 a2)
    {
      return _invokeWith.With (a1, a2);
    }
  }
}