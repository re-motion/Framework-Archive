using System;
using Rubicon.Mixins.Context;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ExtendsAnalysisTests
  {
    [Extends (typeof (object))]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithoutDependencies
    {
    }

    [Extends (typeof (object), AdditionalDependencies = new Type[] { typeof (string) })]
    [IgnoreForMixinConfiguration]
    public class ExtenderWithDependencies
    {
    }

    [Test]
    public void AdditionalDependencies ()
    {
      ApplicationContext context =
          new ApplicationContextBuilder (null).AddType (typeof (ExtenderWithDependencies)).AddType (typeof (ExtenderWithoutDependencies)).BuildContext ();
      Assert.AreEqual (0, context.GetClassContext (typeof (object)).GetOrAddMixinContext (typeof (ExtenderWithoutDependencies))
          .ExplicitDependencyCount);
      Assert.AreEqual (1, context.GetClassContext (typeof (object)).GetOrAddMixinContext (typeof (ExtenderWithDependencies))
          .ExplicitDependencyCount);
      Assert.IsTrue (context.GetClassContext (typeof (object)).GetOrAddMixinContext (typeof (ExtenderWithDependencies))
          .ContainsExplicitDependency (typeof (string)));
    }

    public class ExtendsTargetBase { }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetDerivedWithExtends))]
    [IgnoreForMixinConfiguration]
    public class ExtendingMixin { }

    public class ExtendsTargetDerivedWithoutExtends : ExtendsTargetBase { }

    [Test]
    public void ExtendsAttributeAppliesToInheritanceChain ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin))
          .AddType (typeof (ExtendsTargetDerivedWithoutExtends)).BuildContext ();
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).MixinCount);
    }

    public class ExtendsTargetDerivedWithExtends : ExtendsTargetBase { }

    [Test]
    public void InheritedDuplicateExtensionIsIgnored ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin))
          .AddType (typeof (ExtendsTargetDerivedWithExtends)).BuildContext ();
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithExtends)).MixinCount);
    }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin : ExtendingMixin { }

    public class ExtendsTargetDerivedWithDerivedExtends : ExtendsTargetBase { }

    [Test]
    public void SubclassExtensionOverridesBaseExtends ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (DerivedExtendingMixin))
          .BuildContext();

      Assert.IsFalse (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).MixinCount);
    }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin2 : DerivedExtendingMixin { }

    [Test]
    public void ExplicitApplicationOfBaseAndDerivedMixinToSameClass()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (DerivedExtendingMixin))
          .AddType (typeof (DerivedExtendingMixin2)).BuildContext ();

      Assert.IsFalse (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin2)));
      Assert.AreEqual (2, context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).MixinCount);
    }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetBase))]
    [IgnoreForMixinConfiguration]
    public class DoubleExtendingMixin { }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*DoubleExtendingMixin are "
        + "configured for target type .*ExtendsTargetBase.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnDuplicateExtendsForSameClass ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (DoubleExtendingMixin)).BuildContext ();
    }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetDerivedWithoutExtends))]
    [IgnoreForMixinConfiguration]
    public class MixinExtendingBaseAndDerived { }

    [Test]
    public void DuplicateExtendsForSameClassInInheritanceHierarchyIsIgnored ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (MixinExtendingBaseAndDerived)).BuildContext ();
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetBase)).ContainsMixin (typeof (MixinExtendingBaseAndDerived)));
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).ContainsMixin (typeof (MixinExtendingBaseAndDerived)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).MixinCount);
    }
  }
}
