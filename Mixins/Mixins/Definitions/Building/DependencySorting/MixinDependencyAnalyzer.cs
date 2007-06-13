using System;
using System.Collections.Generic;
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
      SeparatedStringBuilder sb = new SeparatedStringBuilder (", ");
      foreach  (MixinDefinition mixin in equalRoots)
      {
        sb.Append (mixin.FullName);
      }
      string message = string.Format ("The following mixins are applied to the same base class, but their order cannot be determined: {0}.", sb);
      throw new ConfigurationException (message);
    }
  }
}
