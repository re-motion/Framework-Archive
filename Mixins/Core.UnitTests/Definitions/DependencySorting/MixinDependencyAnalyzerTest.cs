using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Definitions.Building.DependencySorting;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Mixins.Utilities.DependencySort;

namespace Rubicon.Mixins.UnitTests.Definitions.DependencySorting
{
  [TestFixture]
  public class MixinDependencyAnalyzerTest
  {
    private MixinDependencyAnalyzer _analyzer;
    private MixinDefinition _independent1;
    private MixinDefinition _independent2;
    private MixinDefinition _dependentSecond;
    private MixinDefinition _dependentThird;
    private MixinDefinition _alphabeticAccepter;
    private MixinDefinition _alphabeticAccepter2;

    [SetUp]
    public void SetUp ()
    {
      _analyzer = new MixinDependencyAnalyzer ();
      using (MixinConfiguration.BuildNew().ForClass (typeof (NullTarget))
          .AddMixin (typeof (NullMixin))
          .AddMixin (typeof (NullMixin2))
          .AddMixin (typeof (NullMixin3)).WithDependency (typeof (NullMixin2))
          .AddMixin (typeof (NullMixin4)).WithDependency (typeof (NullMixin3))
          .AddMixin (typeof (MixinAcceptingAlphabeticOrdering1))
          .AddMixin (typeof (MixinAcceptingAlphabeticOrdering2))
          .EnterScope())
      {
        _independent1 = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin)];
        _independent2 = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin2)];
        _dependentSecond = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin3)];
        _dependentThird = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (NullMixin4)];
        _alphabeticAccepter = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinAcceptingAlphabeticOrdering1)];
        _alphabeticAccepter2 = TypeFactory.GetActiveConfiguration (typeof (NullTarget)).Mixins[typeof (MixinAcceptingAlphabeticOrdering2)];
      }
    }

    [Test]
    public void AnalyzeDirectDependency_Independent ()
    {
      Assert.AreEqual (DependencyKind.None, _analyzer.AnalyzeDirectDependency (_independent1, _independent1));
    }

    [Test]
    public void AnalyzeDirectDependency_DirectDependent_SecondOnFirst ()
    {
      Assert.AreEqual (DependencyKind.SecondOnFirst, _analyzer.AnalyzeDirectDependency (_independent2, _dependentSecond));
      Assert.AreEqual (DependencyKind.SecondOnFirst, _analyzer.AnalyzeDirectDependency (_dependentSecond, _dependentThird));
    }

    [Test]
    public void AnalyzeDirectDependency_DirectDependent_FirstOnSecond ()
    {
      Assert.AreEqual (DependencyKind.FirstOnSecond, _analyzer.AnalyzeDirectDependency (_dependentSecond, _independent2));
      Assert.AreEqual (DependencyKind.FirstOnSecond, _analyzer.AnalyzeDirectDependency (_dependentThird, _dependentSecond));
    }

    [Test]
    public void AnalyzeDirectDependency_IndirectDependent ()
    {
      Assert.AreEqual (DependencyKind.None, _analyzer.AnalyzeDirectDependency (_independent2, _dependentThird));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The following mixins are applied to the same base class "
        + "Rubicon.Mixins.UnitTests.SampleTypes.NullTarget and require a clear base call ordering, but do not provide enough dependency information: "
        + "Rubicon.Mixins.UnitTests.SampleTypes.NullMixin, Rubicon.Mixins.UnitTests.SampleTypes.NullMixin2, "
        + "Rubicon.Mixins.UnitTests.SampleTypes.NullMixin4.\r\nPlease supply additional dependencies to the mixin definitions, use the "
        + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.")]
    public void ResolveEqualRoots_Throws ()
    {
      _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _independent2, _dependentThird });
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering ()
    {
      Assert.AreSame (_alphabeticAccepter, _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _alphabeticAccepter }));
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoAccepters ()
    {
      Assert.AreSame (_alphabeticAccepter, 
          _analyzer.ResolveEqualRoots (new MixinDefinition[] {_independent1, _alphabeticAccepter, _alphabeticAccepter2}));
    }

    [Test]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoAccepters_OtherOrder ()
    {
      Assert.AreSame (_alphabeticAccepter,
          _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _alphabeticAccepter2, _alphabeticAccepter }));
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The following mixins are applied to the same base class "
        + "Rubicon.Mixins.UnitTests.SampleTypes.NullTarget and require a clear base call ordering, but do not provide enough dependency information: "
        + "Rubicon.Mixins.UnitTests.SampleTypes.NullMixin, Rubicon.Mixins.UnitTests.SampleTypes.NullMixin2, Rubicon.Mixins.UnitTests.SampleTypes."
       + "MixinAcceptingAlphabeticOrdering1.\r\nPlease supply additional dependencies to the mixin definitions, use the "
       + "AcceptsAlphabeticOrderingAttribute, or adjust the mixin configuration accordingly.")]
    public void ResolveEqualRoots_WithEnabledAlphabeticOrdering_TwoNonAccepters ()
    {
      _analyzer.ResolveEqualRoots (new MixinDefinition[] { _independent1, _independent2, _alphabeticAccepter });
      Assert.Fail();
    }
  }
}