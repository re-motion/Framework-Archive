using System;
using System.Collections.Generic;
using Mixins.Definitions.Building;
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
    public void MixinDefinitionsAreSortedCorrectlySmall()
    {
      BaseClassDefinition bt7 = TypeFactory.GetActiveConfiguration (typeof (BaseType7));
      Assert.AreEqual (7, bt7.Mixins.Count);
      // group 1
      Assert.AreEqual (0, bt7.Mixins[typeof (BT7Mixin0)].MixinIndex);

      Assert.AreEqual (1, bt7.Mixins[typeof (BT7Mixin2)].MixinIndex);
      Assert.AreEqual (2, bt7.Mixins[typeof (BT7Mixin3)].MixinIndex);
      Assert.AreEqual (3, bt7.Mixins[typeof (BT7Mixin1)].MixinIndex);

      // group 2
      Assert.AreEqual (4, bt7.Mixins[typeof (BT7Mixin10)].MixinIndex);
      Assert.AreEqual (5, bt7.Mixins[typeof (BT7Mixin9)].MixinIndex);

      // group 3
      Assert.AreEqual (6, bt7.Mixins[typeof (BT7Mixin5)].MixinIndex);
    }

    [Test]
    [Ignore ("TODO: API for excplicitly adding dependencies to mixin configuration")]
    public void MixinDefinitionsAreSortedCorrectlyGrand ()
    {
      using (new MixinConfiguration (typeof (BaseType7), typeof (BT7Mixin0), typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3),
          typeof (BT7Mixin4), typeof (BT7Mixin5), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin8), typeof (BT7Mixin9), typeof (BT7Mixin10)))
      {
        CheckGrandOrdering();
      }
      using (new MixinConfiguration (typeof (BaseType7), typeof (BT7Mixin10), typeof (BT7Mixin9), typeof (BT7Mixin8), typeof (BT7Mixin7),
          typeof (BT7Mixin6), typeof (BT7Mixin5), typeof (BT7Mixin4), typeof (BT7Mixin3), typeof (BT7Mixin2), typeof (BT7Mixin1), typeof (BT7Mixin0)))
      {
        CheckGrandOrdering ();
      }
      using (new MixinConfiguration (typeof (BaseType7), typeof (BT7Mixin5), typeof (BT7Mixin8), typeof (BT7Mixin9), typeof (BT7Mixin2),
          typeof (BT7Mixin1), typeof (BT7Mixin10), typeof (BT7Mixin4), typeof (BT7Mixin0), typeof (BT7Mixin6), typeof (BT7Mixin3), typeof (BT7Mixin7)))
      {
        CheckGrandOrdering ();
      }
    }

    private void CheckGrandOrdering ()
    {
      //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin0), typeof (BT7Mixin7));
      //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin7), typeof (BT7Mixin4));
      //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin4), typeof (BT7Mixin6));

      BaseClassDefinition bt7 = TypeFactory.GetActiveConfiguration (typeof (BaseType7));
      Assert.AreEqual (11, bt7.Mixins.Count);
      // group 1
      Assert.AreEqual (5, bt7.Mixins[typeof (BT7Mixin0)].MixinIndex); // u
      Assert.AreEqual (6, bt7.Mixins[typeof (BT7Mixin7)].MixinIndex); // u
      Assert.AreEqual (1, bt7.Mixins[typeof (BT7Mixin4)].MixinIndex); // u
      Assert.AreEqual (2, bt7.Mixins[typeof (BT7Mixin6)].MixinIndex); // u

      Assert.AreEqual (4, bt7.Mixins[typeof (BT7Mixin2)].MixinIndex);
      Assert.AreEqual (3, bt7.Mixins[typeof (BT7Mixin3)].MixinIndex);
      Assert.AreEqual (0, bt7.Mixins[typeof (BT7Mixin1)].MixinIndex);

      // group 2
      Assert.AreEqual (10, bt7.Mixins[typeof (BT7Mixin10)].MixinIndex);
      Assert.AreEqual (9, bt7.Mixins[typeof (BT7Mixin9)].MixinIndex); // u
      Assert.AreEqual (8, bt7.Mixins[typeof (BT7Mixin8)].MixinIndex); // u

      // group 3
      Assert.AreEqual (7, bt7.Mixins[typeof (BT7Mixin5)].MixinIndex);
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = @"The following mixins are applied to the same base class and require a "
       + @"clear base call ordering, but do not provide enough dependency information: ((.*BT7Mixin0)|(.*BT7Mixin4)|(.*BT7Mixin6)|(.*BT7Mixin7)){4}\.",
      MatchType = MessageMatch.Regex)]
    public void ThrowsIfConnectedMixinsCannotBeSorted()
    {
      using (new MixinConfiguration (typeof (BaseType7), typeof (BT7Mixin0), typeof (BT7Mixin4), typeof (BT7Mixin6), typeof (BT7Mixin7),
          typeof (BT7Mixin2), typeof (BT7Mixin5)))
      {
        TypeFactory.GetActiveConfiguration (typeof (BaseType7));
      }
    }

    public interface ICircular1 { }

    public class Circular1 : Mixin<object, ICircular2>, ICircular1
    {
    }

    public interface ICircular2 { }

    public class Circular2 : Mixin<object, ICircular1>, ICircular2
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "circular dependencies.*Circular[12].*Circular[12]",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnCircularDependencies()
    {
      using (new MixinConfiguration (typeof (object), typeof (Circular1), typeof (Circular2)))
      {
        TypeFactory.GetActiveConfiguration (typeof (object));
      }
    }

    interface ISelfDependency { }

    class SelfDependency : Mixin<object, ISelfDependency>, ISelfDependency
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "circular dependencies.*SelfDependency",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnSelfDependencies ()
    {
      using (new MixinConfiguration (typeof (object), typeof (SelfDependency)))
      {
        TypeFactory.GetActiveConfiguration (typeof (object));
      }
    }

    [Test]
    [Ignore ("TODO: API for excplicitly adding dependencies to mixin configuration")]
    public void DependencyAnalyzer ()
    {
      using (new MixinConfiguration (typeof (BaseType7), typeof (BT7Mixin0), typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3),
          typeof (BT7Mixin4), typeof (BT7Mixin5), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin8), typeof (BT7Mixin9), typeof (BT7Mixin10)))
      {
        //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin0), typeof (BT7Mixin7));
        //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin7), typeof (BT7Mixin4));
        //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin4), typeof (BT7Mixin6));

        BaseClassDefinition bt7 = TypeFactory.GetActiveConfiguration (typeof (BaseType7));
        MixinDependencyAnalyzer analyzer = new MixinDependencyAnalyzer();

        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin0)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin1)]));

        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin2)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin1)]));

        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin0)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin1)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin2)]));
        Assert.AreEqual (DependencyKind.None, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin4)], bt7.Mixins[typeof (BT7Mixin3)]));

        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin0)], bt7.Mixins[typeof (BT7Mixin2)]));

        Assert.AreEqual (
            DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin1)], bt7.Mixins[typeof (BT7Mixin3)]));
        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin1)]));

        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin2)], bt7.Mixins[typeof (BT7Mixin3)]));
        Assert.AreEqual (
            DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]));

        Assert.AreEqual (
            DependencyKind.SecondOnFirst, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin3)], bt7.Mixins[typeof (BT7Mixin2)]));

        Assert.AreEqual (
            DependencyKind.FirstOnSecond, analyzer.AnalyzeDirectDependency (bt7.Mixins[typeof (BT7Mixin7)], bt7.Mixins[typeof (BT7Mixin2)]));
      }
    }
    
    [Test]
    [Ignore ("TODO: API for excplicitly adding dependencies to mixin configuration")]
    public void MixinGroupBuilder ()
    {
      using (new MixinConfiguration (typeof (BaseType7), typeof (BT7Mixin0), typeof (BT7Mixin1), typeof (BT7Mixin2), typeof (BT7Mixin3),
          typeof (BT7Mixin4), typeof (BT7Mixin5), typeof (BT7Mixin6), typeof (BT7Mixin7), typeof (BT7Mixin8), typeof (BT7Mixin9), typeof (BT7Mixin10)))
      {
        //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin0), typeof (BT7Mixin7));
        //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin7), typeof (BT7Mixin4));
        //MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType7)).AddMixinDependency (typeof (BT7Mixin4), typeof (BT7Mixin6));

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
}
