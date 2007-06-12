using System;

namespace Mixins.Utilities.DependencySort
{
  public class DependencySortException : Exception
  {
    public DependencySortException (string message)
        : base (message)
    {
    }

    public DependencySortException (string message, Exception inner)
        : base (message, inner)
    {
    }
  }
}