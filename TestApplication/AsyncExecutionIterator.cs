using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace TestApplication
{
  public class AsyncExecutionIterator : IEnumerable<Action>
  {
    private readonly Queue<Action> _continuationQueue = new Queue<Action>();
    private readonly Queue<Action> _reentryQueue = new Queue<Action>();

    public AsyncExecutionIterator (Func<Task> function)
    {
      EnqueueContinuation (() => function());
    }

    public IEnumerator<Action> GetEnumerator ()
    {
      while (_reentryQueue.Count > 0 || _continuationQueue.Count > 0)
      {
        yield return GetContinuation();
      }
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public Awaiter CreateAwaiter (Action action)
    {
      return new Awaiter (this, action);
    }

    public Awaiter<T> CreateAwaiter<T> (Action action, Task<T> task)
    {
      return new Awaiter<T> (this, action, task);
    }

    public void EnqueueContinuation (Action continuation)
    {
      _continuationQueue.Enqueue (continuation);
    }

    public void SetReentryAction (Action executePageStep)
    {
      _reentryQueue.Clear();
      _reentryQueue.Enqueue (executePageStep);
    }

    public void ResetReentryAction ()
    {
      _reentryQueue.Clear();
    }

    private Action GetContinuation ()
    {
      return _reentryQueue.Count > 0 ? _reentryQueue.Dequeue() : _continuationQueue.Dequeue();
    }

    public class Awaiter : INotifyCompletion
    {
      private readonly Action _action;
      private readonly AsyncExecutionIterator _executionIterator;

      public Awaiter (AsyncExecutionIterator executionIterator, Action action)
      {
        _executionIterator = executionIterator;
        _action = action;
      }

      public bool IsCompleted
      {
        get { return false; }
      }

      public void OnCompleted (Action continuation)
      {
        if (_action != null)
          _executionIterator.SetReentryAction (_action);
        _executionIterator.EnqueueContinuation (continuation);
      }

      public void GetResult ()
      {
      }
    }

    public class Awaiter<T> : INotifyCompletion
    {
      private readonly Action _action;
      private readonly AsyncExecutionIterator _executionIterator;
      private readonly Task<T> _task;

      internal Awaiter (AsyncExecutionIterator executionIterator, Action action, Task<T> task)
      {
        _executionIterator = executionIterator;
        _action = action;
        _task = task;
      }

      public bool IsCompleted
      {
        get { return false; }
      }

      public void OnCompleted (Action continuation)
      {
        if (_action != null)
          _executionIterator.SetReentryAction (_action);
        _executionIterator.EnqueueContinuation (continuation);
      }

      public T GetResult ()
      {
        if (!_task.IsCompleted)
          throw new InvalidOperationException ("result is not available.");

        return _task.Result;
      }
    }
  }
}