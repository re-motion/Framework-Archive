using System;
using System.Collections.Generic;
using Mixins.Definitions.Building.DependencySorting;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.Definitions;
using Mixins.Utilities.DependencySort;
using Rubicon.Collections;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class MixinDependencySortTests
  {
    [Test]
    [Ignore ("TODO: Sort mixins")]
    public void MixinDefinitionsAreSortedCorrectly()
    {
      BaseClassDefinition bt7 = TypeFactory.GetActiveConfiguration (typeof (BaseType7));
      Assert.AreEqual (11, bt7.Mixins.Count);
      Assert.AreEqual (0, bt7.Mixins[typeof (BT7Mixin1)].MixinIndex);
      Assert.AreEqual (1, bt7.Mixins[typeof (BT7Mixin3)].MixinIndex);
      Assert.AreEqual (2, bt7.Mixins[typeof (BT7Mixin2)].MixinIndex);
      Assert.AreEqual (3, bt7.Mixins[typeof (BT7Mixin0)].MixinIndex);
      Assert.AreEqual (4, bt7.Mixins[typeof (BT7Mixin4)].MixinIndex);
    }

    [Test]
    public void DependencyAnalyzer()
    {
      BaseClassDefinition bt7 = TypeFactory.GetActiveConfiguration (typeof (BaseType7));
      MixinDependencyAnalyzer analyzer = new MixinDependencyAnalyzer();

      Assert.AreEqual (DependencyKind.None,  analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof(BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin0)]));
      Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin1)]));

      Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin2)]));
      Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin1)]));

      Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin0)]));
      Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin1)]));
      Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin2)]));
      Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin3)]));

      Assert.AreEqual (DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin2)]));

      Assert.AreEqual (DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin3)]));
      Assert.AreEqual (DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin1)]));

      Assert.AreEqual (DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin3)]));
      Assert.AreEqual (DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]));

      Assert.AreEqual (DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]));

      Assert.AreEqual (DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin7)], bt7.Mixins[typeof (BT7Mixin2)]));
    }

    [Test]
    public void MixinGroupBuilder()
    {
      BaseClassDefinition bt7 = TypeFactory.GetActiveConfiguration (typeof (BaseType7));
      DependentMixinGrouper grouper = new DependentMixinGrouper();
      List<Set<MixinDefinition>> mixinGroups = new List<Set<MixinDefinition>> (grouper.GroupMixins (bt7.Mixins));
      Assert.AreEqual (3, mixinGroups.Count);

      mixinGroups.Sort (delegate (Set<MixinDefinition> one, Set<MixinDefinition> two) { return one.Count.CompareTo (two.Count); });

      Set<MixinDefinition> smaller = mixinGroups[0];
      Set<MixinDefinition> medium = mixinGroups[1];
      Set<MixinDefinition> larger = mixinGroups[2];

      Assert.Contains (bt7.Mixins[typeof (BT7Mixin0)], larger, "dependency+method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin1)], larger, "dependency+method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin2)], larger, "dependency+method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin3)], larger, "dependency+method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin4)], larger, "method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin6)], larger, "method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin7)], larger, "dependency");
      Assert.AreEqual (7, larger.Count);

      Assert.Contains (bt7.Mixins[typeof (BT7Mixin8)], medium, "method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin9)], medium, "dependency+method");
      Assert.Contains (bt7.Mixins[typeof (BT7Mixin10)], medium, "dependency");
      Assert.AreEqual (3, medium.Count);

      Assert.Contains (bt7.Mixins[typeof (BT7Mixin5)], smaller, "nothing");
      Assert.AreEqual (1, smaller.Count);
    }
  }
}
