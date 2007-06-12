using System;
using System.Collections.Generic;
using Mixins.Utilities.DependencySort;
using NUnit.Framework;
using Rubicon.Collections;
using Rubicon.Utilities;

namespace Mixins.UnitTests.Utilities
{
  [TestFixture]
  public class DependencySortTest
  {
    private class SimpleDependencyAnalyzer<T> : IDependencyAnalyzer<T>
    {
      private Dictionary<T, Set<T>> _dependencies = new Dictionary<T, Set<T>> ();

      public void AddDependency (T first, T second)
      {
        GetDependencies (first).Add (second);
      }

      private Set<T> GetDependencies (T first)
      {
        if (!_dependencies.ContainsKey (first))
          _dependencies.Add (first, new Set<T>());
        return _dependencies[first];
      }

      public DependencyKind AnalyzeDirectDependency (T first, T second)
      {
        if (GetDependencies (first).Contains (second))
          return DependencyKind.FirstOnSecond;
        else if (GetDependencies (second).Contains (first))
          return DependencyKind.SecondOnFirst;
        else
          return DependencyKind.None;
      }

      public T ResolveEqualRoots (IEnumerable<T> equalRoots)
      {
        List<T> roots = new List<T> (equalRoots);
        roots.Sort();
        return roots[0];
      }
    }

    private IEnumerable<T> AnalyzeDependencies<T> (IEnumerable<T> objectsEnumerable, IDependencyAnalyzer<T> analyzer)
    {
      ArgumentUtility.CheckNotNull ("objectsEnumerable", objectsEnumerable);
      ArgumentUtility.CheckNotNull ("analyzer", analyzer);

      DependentObjectSorter<T> sorter = new DependentObjectSorter<T>(analyzer);
      return sorter.SortDependencies (objectsEnumerable);
    }

    private void AssertListsEqualInOrder<T> (IEnumerable<T> expected, IEnumerable<T> actual)
    {
      List<T> expectedList = new List<T> (expected);
      List<T> actualList = new List<T> (actual);
      Assert.AreEqual (expectedList.Count, actualList.Count);
      for (int i = 0; i < expectedList.Count; ++i)
        Assert.AreEqual (expectedList[i], actualList[i]);
    }

    [Test]
    public void SortDependentObjectsList ()
    {
      string[] dependentObjects = new string[] {"h", "g", "f", "e", "d", "c", "b", "a"};
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string>();
      analyzer.AddDependency ("a", "b");
      analyzer.AddDependency ("b", "c");
      analyzer.AddDependency ("c", "d");
      analyzer.AddDependency ("d", "e");
      analyzer.AddDependency ("e", "f");
      analyzer.AddDependency ("f", "g");
      analyzer.AddDependency ("g", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "c", "d", "e", "f", "g", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraph1 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "b");
      analyzer.AddDependency ("b", "c");
      analyzer.AddDependency ("c", "d");
      analyzer.AddDependency ("c", "e");
      analyzer.AddDependency ("c", "h");
      analyzer.AddDependency ("d", "f");
      analyzer.AddDependency ("e", "g");
      analyzer.AddDependency ("g", "d");
      analyzer.AddDependency ("g", "d");
      analyzer.AddDependency ("f", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "c", "e", "g", "d", "f", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraph2 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "b");
      analyzer.AddDependency ("a", "c");
      analyzer.AddDependency ("a", "d");
      analyzer.AddDependency ("b", "e");
      analyzer.AddDependency ("c", "b");
      analyzer.AddDependency ("c", "f");
      analyzer.AddDependency ("d", "f");
      analyzer.AddDependency ("e", "d");
      analyzer.AddDependency ("e", "g");
      analyzer.AddDependency ("f", "g");
      analyzer.AddDependency ("g", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "c", "b", "e", "d", "f", "g", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraphWithMultiRoots1 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "d");
      analyzer.AddDependency ("b", "e");
      analyzer.AddDependency ("c", "f");
      analyzer.AddDependency ("d", "f");
      analyzer.AddDependency ("e", "d");
      analyzer.AddDependency ("f", "g");
      analyzer.AddDependency ("f", "h");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "c", "e", "d", "f", "g", "h" }, sortedObjects);
    }

    [Test]
    public void SortDependentObjectsGraphWithMultiRoots2 ()
    {
      string[] dependentObjects = new string[] { "h", "g", "f", "e", "d", "c", "b", "a" };
      SimpleDependencyAnalyzer<string> analyzer = new SimpleDependencyAnalyzer<string> ();
      analyzer.AddDependency ("a", "c");
      analyzer.AddDependency ("b", "d");
      analyzer.AddDependency ("c", "g");
      analyzer.AddDependency ("d", "c");
      analyzer.AddDependency ("f", "e");

      IEnumerable<string> sortedObjects = AnalyzeDependencies (dependentObjects, analyzer);
      AssertListsEqualInOrder (new string[] { "a", "b", "d", "c", "f", "e", "g", "h" }, sortedObjects);
    }
  }
}