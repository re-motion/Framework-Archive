using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomSynchronizationContext
{
  public class AsyncIterator<T> : IEnumerable<T>, IEnumerator<T>
  {
    T _current;
    bool _done = false;
    Func<AsyncIterator<T>, Task> _action;

    public AsyncIterator (Func<AsyncIterator<T>, Task> action)
    {
      _action = action;
    }

    public Task Yield(T value) // Task or anything else that can be awaited
    {
      _current = value;
      // interrupt execution and flush entire call stack, remember continuation(s) for MoveNext
      throw new NotImplementedException();
    }

    public T Current
    {
      get { return _current; }
    }

    public bool MoveNext()
    {
      if (_done)
        throw new InvalidOperationException();
      // execute until next Yield() 
      return ! _done;
    }

    #region uninteresting stuff
    public IEnumerator<T> GetEnumerator()
    {
      return this;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    object System.Collections.IEnumerator.Current
    {
      get { return Current; }
    }

    public void Reset()
    {
      throw new NotImplementedException();
    }
    #endregion
  }
}