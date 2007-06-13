using System.Collections.Generic;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Mixins.Utilities.DependencySort
{
  public class DependentObjectSorterAlgorithm<T>
  {
    private IDependencyAnalyzer<T> _analyzer;
    private Set<T> _objects;

    public DependentObjectSorterAlgorithm (IDependencyAnalyzer<T> analyzer, IEnumerable<T> objects)
    {
      ArgumentUtility.CheckNotNull ("objects", objects);
      _analyzer = analyzer;
      _objects = new Set<T> (objects);
    }

    public IEnumerable<T> Execute ()
    {
      while (_objects.Count > 0)
      {
        T root = GetRoot ();
        yield return root;
        _objects.Remove (root);
      }
    }

    private T GetRoot ()
    {
      Set<T> rootCandidates = new Set<T> (_objects);
      foreach (T first in _objects)
      {
        foreach (T second in _objects)
        {
          switch (_analyzer.AnalyzeDirectDependency (first, second))
          {
            case DependencyKind.FirstOnSecond:
              rootCandidates.Remove (second);
              break;
            case DependencyKind.SecondOnFirst:
              rootCandidates.Remove (first);
              goto nextOuter;
          }
        }
      nextOuter:
        ;
      }
      if (rootCandidates.Count == 0)
        throw new DependencySortException ("The object graph contains circular dependencies, no root object can be found.");
      else if (rootCandidates.Count == 1)
      {
        IEnumerator<T> enumerator = rootCandidates.GetEnumerator ();
        enumerator.MoveNext ();
        return enumerator.Current;
      }
      else
      {
        Assertion.Assert (rootCandidates.Count > 1);
        return _analyzer.ResolveEqualRoots (rootCandidates);
      }
    }
  }
}