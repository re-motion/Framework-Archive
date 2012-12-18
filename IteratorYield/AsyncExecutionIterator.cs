using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace IteratorYield
{
  public class AsyncExecutionIterator<T> : IEnumerable<T>
  {
    private readonly List<Continuation> _continuationQueue = new List<Continuation>();

    public AsyncExecutionIterator (Func<AsyncExecutionIterator<T>, Task> function)
    {
      EnqueueContinuation (new Continuation (() => function (this), () => true));
    }

    public IEnumerator<T> GetEnumerator ()
    {
      while (_continuationQueue.Count > 0)
      {
        var continuation = GetContinuation();
        if (continuation is YieldingContinuation<T>)
          yield return ((YieldingContinuation<T>) continuation).GetValue();

        continuation.ContinuationAction();
      }
    }

    public ValueYieldingAwaitable<T> Yield (T value)
    {
      return new ValueYieldingAwaitable<T> (this, () => value);
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return GetEnumerator();
    }

    public Awaiter CreateAwaiter (Task task, Func<T> valueAccessor = null)
    {
      return new Awaiter (this, task, valueAccessor);
    }

    public Awaiter<TTaskResult> CreateAwaiter<TTaskResult> (Task<TTaskResult> task)
    {
      return new Awaiter<TTaskResult> (this, task);
    }

    public void EnqueueContinuation (Continuation continuation)
    {
      _continuationQueue.Add (continuation);
    }

    private Continuation GetContinuation ()
    {
      for (int i = 0; i < _continuationQueue.Count; i++)
      {
        var continuation = _continuationQueue[i];

        if (continuation.IsReady())
        {
          _continuationQueue.RemoveAt (i);
          return continuation;
        }
      }
      throw new InvalidOperationException ("this should never happen");
    }

    public class Awaiter : INotifyCompletion
    {
      private readonly Task _task;
      private readonly Func<T> _valueAccessor;
      private readonly AsyncExecutionIterator<T> _executionIterator;

      public Awaiter (AsyncExecutionIterator<T> executionIterator, Task task, Func<T> valueAccessor)
      {
        _executionIterator = executionIterator;
        _task = task;
        _valueAccessor = valueAccessor;
      }

      public bool IsCompleted
      {
        get { return false; }
      }

      public void OnCompleted (Action continuation)
      {
        Func<bool> isReadyAccessor = () => _task == null || _task.IsCompleted;

        Continuation continuationObject;
        if (_valueAccessor != null)
          continuationObject = new YieldingContinuation<T> (continuation, isReadyAccessor, _valueAccessor);
        else
          continuationObject = new Continuation (continuation, isReadyAccessor);

        _executionIterator.EnqueueContinuation (continuationObject);
      }

      public void GetResult ()
      {
      }
    }

    public class Awaiter<TTaskResult> : INotifyCompletion
    {
      private readonly AsyncExecutionIterator<T> _executionIterator;
      private readonly Task<TTaskResult> _task;

      internal Awaiter (AsyncExecutionIterator<T> executionIterator, Task<TTaskResult> task)
      {
        _executionIterator = executionIterator;
        _task = task;
      }

      public bool IsCompleted
      {
        get { return false; }
      }

      public void OnCompleted (Action continuation)
      {
        _executionIterator.EnqueueContinuation (new Continuation(continuation, () => _task == null || _task.IsCompleted));
      }

      public TTaskResult GetResult ()
      {
        if (!_task.IsCompleted)
          throw new InvalidOperationException ("result is not available.");

        return _task.Result;
      }
    }
  }
}