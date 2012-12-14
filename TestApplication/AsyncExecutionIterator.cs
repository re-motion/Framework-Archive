using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TestApplication
{
  public class AsyncExecutionIterator : IEnumerable<Action>
  {
    // make invisible
    public class Awaiter : INotifyCompletion
    {
      private readonly AsyncExecutionIterator _executionIterator;
      private readonly Action _action;

      internal Awaiter(AsyncExecutionIterator executionIterator, Action action)
      {
        _executionIterator = executionIterator;
        _action = action;
      }

      public bool IsCompleted
      {
        get { return false; }
      }

      public void GetResult()
      {
      }

      public void OnCompleted(Action continuation)
      {
        _executionIterator.SetReentryAction(_action);
        _executionIterator.SetContinuation(continuation);
      }
    }

    public Awaiter CreateAwaiter(Action action)
    {
      return new Awaiter (this, action);
    }

    private readonly Queue<Action> _continuationQueue = new Queue<Action>();
    private readonly Queue<Action> _reentryQueue = new Queue<Action>();

    public AsyncExecutionIterator(Func<Task> function)
    {
      SetContinuation(() => function());
    }

    private Action GetContinuation()
    {
      return _reentryQueue.Count > 0 ? _reentryQueue.Dequeue() : _continuationQueue.Dequeue();
    }

    public void SetContinuation(Action continuation)
    {
      _continuationQueue.Clear();
      _continuationQueue.Enqueue(continuation);
    }

    public void SetReentryAction(Action executePageStep)
    {
      _reentryQueue.Clear();
      _reentryQueue.Enqueue(executePageStep);
    }

    public void ResetReentryQueue()
    {
      _reentryQueue.Clear();
    }

    public IEnumerator<Action> GetEnumerator()
    {
      while (_reentryQueue.Count > 0 || _continuationQueue.Count > 0)
      {
        yield return GetContinuation();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}