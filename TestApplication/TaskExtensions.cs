using System;
using System.Threading.Tasks;

namespace TestApplication
{
  public static class TaskExtensions
  {
    public class Awaitable
    {
      private readonly AsyncExecutionIterator _iterator;
      private readonly Task _task;

      public Awaitable (AsyncExecutionIterator iterator, Task task)
      {
        _iterator = iterator;
        _task = task;
      }

      public AsyncExecutionIterator.Awaiter GetAwaiter()
      {
        return _iterator.CreateAwaiter(null, _task);
      }
    }

    public class Awaitable<T>
    {
      private readonly AsyncExecutionIterator _iterator;
      private readonly Task<T> _task;

      public Awaitable(AsyncExecutionIterator iterator, Task<T> task)
      {
        _iterator = iterator;
        _task = task;
      }

      public AsyncExecutionIterator.Awaiter<T> GetAwaiter()
      {
        return _iterator.CreateAwaiter<T>(null, _task);
      }
    }

    public static Awaitable ConfigureAwait (this Task task, AsyncExecutionIterator iterator)
    {
      return new Awaitable (iterator, task);
    }

    public static Awaitable<T> ConfigureAwait<T>(this Task<T> task, AsyncExecutionIterator iterator)
    {
      return new Awaitable<T>(iterator, task);
    } 
  }
}