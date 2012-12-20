using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Remotion.Web.ExecutionEngine
{
  public class Continuation
  {
    private readonly Action _continuation;
    private readonly Func<bool> _isReadyAccessor;

    public Continuation (Action continuation, Func<bool> isReadyAccessor)
    {
      _continuation = continuation;
      _isReadyAccessor = isReadyAccessor;
    }

    public Action ContinuationAction
    {
      get { return _continuation; }
    }

    public Func<bool> IsReadyAccessor
    {
      get { return _isReadyAccessor; }
    }
  }

  public class AsyncExecutionIterator : IEnumerable<Action>
  {
    private readonly List<Continuation> _continuationQueue = new List<Continuation>();
    private readonly Queue<Action> _reentryQueue = new Queue<Action>();

    public AsyncExecutionIterator (Func<Task> function)
    {
      EnqueueContinuation (new Continuation(() => function(), () => true));
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

    public Awaiter CreateAwaiter (Action action, Task task)
    {
      return new Awaiter (this, action, task);
    }

    public Awaiter<T> CreateAwaiter<T> (Action action, Task<T> task)
    {
      return new Awaiter<T> (this, action, task);
    }

    public void EnqueueContinuation (Continuation continuation)
    {
      _continuationQueue.Add (continuation);
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
      if (_reentryQueue.Count > 0)
        return _reentryQueue.Dequeue();
      else
      {
        for (int i = 0; i < _continuationQueue.Count; i++)
        {
          var continuation = _continuationQueue[i];

          if (continuation.IsReadyAccessor())
          {
            _continuationQueue.RemoveAt (i);
            return continuation.ContinuationAction;
          }
        }
        throw new InvalidOperationException ("this should never happen");
      }
    }

    public class Awaiter : INotifyCompletion
    {
      private readonly Action _action;
      private readonly Task _task;
      private readonly AsyncExecutionIterator _executionIterator;

      public Awaiter (AsyncExecutionIterator executionIterator, Action action, Task task)
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
        _executionIterator.EnqueueContinuation (new Continuation(continuation, () => _task == null || _task.IsCompleted));
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
        _executionIterator.EnqueueContinuation (new Continuation(continuation, () => _task == null || _task.IsCompleted));
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