using System;

namespace IteratorYield
{
  public class YieldingContinuation<T> : Continuation
  {
    private readonly Func<T> _valueAccessor;

    public YieldingContinuation (Action continuation, Func<bool> isReady, Func<T> valueAccessor)
        : base(continuation, isReady)
    {
      _valueAccessor = valueAccessor;
    }

    public T GetValue ()
    {
      return _valueAccessor();
    }
  }
}