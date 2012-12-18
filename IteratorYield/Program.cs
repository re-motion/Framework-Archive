using System;
using System.Threading.Tasks;

namespace IteratorYield
{
  internal class Program
  {
    private static void Main (string[] args)
    {
      var iterator = new AsyncExecutionIterator<string> (Startup);
      foreach (string s in iterator)
        Console.WriteLine (s);
    }

    public static async Task Startup (AsyncExecutionIterator<string> iterator)
    {
      await iterator.Yield ("begin Startup()");
      int res = await Sub1 (iterator).ConfigureAwait (iterator);
      await iterator.Yield ("Sub1() returned " + res);
      await iterator.Yield ("end Startup()");
    }

    public static async Task<int> Sub1 (AsyncExecutionIterator<string> iterator)
    {
      await iterator.Yield ("Sub1");
      return 1;
    }
  }
}