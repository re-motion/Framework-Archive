using System;
using Mixins.Context;
using NUnit.Framework;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ExtendsAnalysisTests
  {
    public class ExtendsTargetBase { }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetDerivedWithExtends))]
    [IgnoreForMixinConfiguration]
    public class ExtendingMixin { }

    public class ExtendsTargetDerivedWithoutExtends : ExtendsTargetBase { }

    [Test]
    [Ignore ("TODO: Implement Extends inheritance")]
    public void ExtendsAttributeAppliesToInheritanceChain ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin)).Analyze ();
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).MixinCount);
    }

    public class ExtendsTargetDerivedWithExtends : ExtendsTargetBase { }

    [Test]
    [Ignore ("TODO: Implement Extends inheritance")]
    public void InheritedDuplicateExtensionIsIgnored ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin)).Analyze ();
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithExtends)).MixinCount);
    }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin : ExtendingMixin { }

    [Extends (typeof (ExtendsTargetDerivedWithDerivedExtends))]
    [IgnoreForMixinConfiguration]
    public class DerivedExtendingMixin2 : DerivedExtendingMixin { }

    public class ExtendsTargetDerivedWithDerivedExtends : ExtendsTargetBase { }

    [Test]
    [Ignore ("TODO: Implement Extends inheritance")]
    public void SubclassExtensionOverridesBaseExtends ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (DerivedExtendingMixin)).Analyze ();

      Assert.IsFalse (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).MixinCount);

      context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (DerivedExtendingMixin)).AddType (typeof (DerivedExtendingMixin2)).Analyze ();
      Assert.IsFalse (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.IsFalse (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin2)));
      Assert.AreEqual (1, context.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).MixinCount);
    }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetBase))]
    [IgnoreForMixinConfiguration]
    public class DoubleExtendingMixin { }

    [Test]
    [Ignore ("TODO: Implement Extends inheritance")]
    public void ThrowsOnDuplicateExtendsForSameClass ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (DoubleExtendingMixin)).Analyze ();
      Assert.Fail ("Exception expected");
    }
  }
}
