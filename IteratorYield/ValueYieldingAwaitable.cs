using System;

namespace IteratorYield
{
    public class ValueYieldingAwaitable<TIteratorValue>
    {
      private readonly AsyncExecutionIterator<TIteratorValue> _iterator;
      private readonly Func<TIteratorValue> _valueAccessor;

      public ValueYieldingAwaitable (AsyncExecutionIterator<TIteratorValue> iterator, Func<TIteratorValue> valueAccessor)
      {
        _iterator = iterator;
        _valueAccessor = valueAccessor;
      }

      public AsyncExecutionIterator<TIteratorValue>.Awaiter GetAwaiter()
      {
        return _iterator.CreateAwaiter(null, _valueAccessor);
      }
    }
}