using System;
using System.Threading.Tasks;

namespace IteratorYield
{
  public static class TaskExtensions
  {
    public class TaskAwaitable<TIteratorValue>
    {
      private readonly AsyncExecutionIterator<TIteratorValue> _iterator;
      private readonly Task _task;

      public TaskAwaitable (AsyncExecutionIterator<TIteratorValue> iterator, Task task)
      {
        _iterator = iterator;
        _task = task;
      }

      public AsyncExecutionIterator<TIteratorValue>.Awaiter GetAwaiter()
      {
        return _iterator.CreateAwaiter(_task);
      }
    }

    public class TaskAwaitable<TTaskResult, TIteratorValue>
    {
      private readonly AsyncExecutionIterator<TIteratorValue> _iterator;
      private readonly Task<TTaskResult> _task;

      public TaskAwaitable(AsyncExecutionIterator<TIteratorValue> iterator, Task<TTaskResult> task)
      {
        _iterator = iterator;
        _task = task;
      }

      public AsyncExecutionIterator<TIteratorValue>.Awaiter<TTaskResult> GetAwaiter()
      {
        return _iterator.CreateAwaiter<TTaskResult>(_task);
      }
    }

    public static TaskAwaitable<TIteratorValue> ConfigureAwait<TIteratorValue> (this Task task, AsyncExecutionIterator<TIteratorValue> iterator)
    {
      return new TaskAwaitable<TIteratorValue> (iterator, task);
    }

    public static TaskAwaitable<TTaskResult, TIteratorValue> ConfigureAwait<TTaskResult, TIteratorValue>(this Task<TTaskResult> task, AsyncExecutionIterator<TIteratorValue> iterator)
    {
      return new TaskAwaitable<TTaskResult, TIteratorValue>(iterator, task);
    } 
  }
}