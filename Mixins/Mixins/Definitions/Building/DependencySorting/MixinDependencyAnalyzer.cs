using System;
using System.Collections.Generic;
using Mixins.Utilities;
using Mixins.Utilities.DependencySort;
using Rubicon.Text;

namespace Mixins.Definitions.Building.DependencySorting
{
  public class MixinDependencyAnalyzer : IDependencyAnalyzer<MixinDefinition>
  {
    public DependencyKind AnalyzeDirectDependency (MixinDefinition first, MixinDefinition second)
    {
      foreach (BaseDependencyDefinition baseDependency in first.BaseDependencies)
      {
        if (baseDependency.GetImplementer () == second)
          return DependencyKind.FirstOnSecond;
      }
      foreach (BaseDependencyDefinition baseDependency in second.BaseDependencies)
      {
        if (baseDependency.GetImplementer () == first)
          return DependencyKind.SecondOnFirst;
      }
      return DependencyKind.None;
    }

    public MixinDefinition ResolveEqualRoots (IEnumerable<MixinDefinition> equalRoots)
    {
      string message = string.Format ("The following mixins are applied to the same base class and require a clear base call ordering, but do not "
          + "provide enough dependency information: {0}.{1}Please add base call dependencies to the mixin definitions or adjust the mixin configuration "
          + "accordingly.", CollectionStringBuilder.BuildCollectionString (equalRoots, ", ", delegate (MixinDefinition m) { return m.FullName; }),
          Environment.NewLine);
      throw new ConfigurationException (message);
    }
  }
}
