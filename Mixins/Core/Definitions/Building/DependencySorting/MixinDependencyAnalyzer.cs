using System;
using System.Collections.Generic;
using Rubicon.Mixins.Utilities.DependencySort;
using Rubicon.Text;
using Rubicon.Utilities;

namespace Rubicon.Mixins.Definitions.Building.DependencySorting
{
  public class MixinDependencyAnalyzer : IDependencyAnalyzer<MixinDefinition>
  {
    public DependencyKind AnalyzeDirectDependency (MixinDefinition first, MixinDefinition second)
    {
      foreach (DependencyDefinitionBase dependency in first.GetOrderRelevantDependencies ())
      {
        if (dependency.GetImplementer () == second)
          return DependencyKind.FirstOnSecond;
      }
      foreach (DependencyDefinitionBase dependency in second.GetOrderRelevantDependencies ())
      {
        if (dependency.GetImplementer () == first)
          return DependencyKind.SecondOnFirst;
      }
      return DependencyKind.None;
    }

    public MixinDefinition ResolveEqualRoots (IEnumerable<MixinDefinition> equalRoots)
    {
      IEnumerator<MixinDefinition> equalRootsEnumerator = equalRoots.GetEnumerator();
      bool hasFirst = equalRootsEnumerator.MoveNext ();
      Assertion.IsTrue (hasFirst);
      MixinDefinition first = equalRootsEnumerator.Current;
      string message = string.Format ("The following mixins are applied to the same base class {0} and require a clear base call ordering, but do not "
          + "provide enough dependency information: {1}.{2}Please supply additional dependencies to the mixin definitions or adjust the mixin configuration "
          + "accordingly.", first.TargetClass.FullName,
          SeparatedStringBuilder.Build (", ", equalRoots, delegate (MixinDefinition m) { return m.FullName; }),
          Environment.NewLine);
      throw new ConfigurationException (message);
    }
  }
}
