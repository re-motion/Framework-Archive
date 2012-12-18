using System;

namespace IteratorYield
{
  public class Continuation
  {
    private readonly Action _continuation;
    private readonly Func<bool> _isReady;

    public Continuation (Action continuation, Func<bool> isReady)
    {
      _continuation = continuation;
      _isReady = isReady;
    }

    public Action ContinuationAction
    {
      get { return _continuation; }
    }

    public Func<bool> IsReady
    {
      get { return _isReady; }
    }
  }
}