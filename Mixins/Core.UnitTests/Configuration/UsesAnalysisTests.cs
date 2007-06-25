using System;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class UsesAnalysisTests
  {
    [Uses (typeof (NullMixin), AdditionalDependencies = new Type[] { typeof (object) })]
    [IgnoreForMixinConfiguration]
    public class BaseWithUses { }

    public class DerivedWithoutUses : BaseWithUses { }

    [Test]
    public void UsesAttributeIsInherited ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithoutUses)).BuildContext ();
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithoutUses)).ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithoutUses)).GetOrAddMixinContext (typeof (NullMixin)).ContainsExplicitDependency (typeof (object)));
      Assert.AreEqual (1, context.GetClassContext (typeof (DerivedWithoutUses)).MixinCount);
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesDuplicateIsIgnored ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithUses)).BuildContext ();
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithUses)).ContainsMixin (typeof (NullMixin)));
      Assert.IsFalse (context.GetClassContext (typeof (DerivedWithUses)).GetOrAddMixinContext (typeof (NullMixin)).ContainsExplicitDependency (typeof (object)));
      Assert.AreEqual (1, context.GetClassContext (typeof (DerivedWithUses)).MixinCount);
    }

    public class DerivedNullMixin : NullMixin { }

    [Uses (typeof (DerivedNullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithMoreSpecificUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesCanBeOverridden ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithMoreSpecificUses)).BuildContext ();
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithMoreSpecificUses)).ContainsMixin (typeof (DerivedNullMixin)));
      Assert.IsFalse (context.GetClassContext (typeof (DerivedWithMoreSpecificUses)).ContainsMixin (typeof (NullMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (DerivedWithMoreSpecificUses)).MixinCount);
    }

    public class BaseGenericMixin<TThis, TBase> : Mixin<TThis, TBase>
      where TThis : class
      where TBase : class
    { }

    public class DerivedGenericMixin<TThis, TBase> : BaseGenericMixin<TThis, TBase>
      where TThis : class
      where TBase : class
    {
    }

    public class DerivedClosedMixin : BaseGenericMixin<object, object> { }

    [Uses (typeof (BaseGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class BaseWithOpenGeneric
    {
    }

    [Uses (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>))]
    [IgnoreForMixinConfiguration]
    public class BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOpenOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithClosedOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOpenOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithClosedOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Uses (typeof (DerivedClosedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithRealClosedOverridingOpen : BaseWithOpenGeneric
    {
    }

    [Uses (typeof (DerivedClosedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithRealClosedOverridingClosed : BaseWithClosedGeneric
    {
    }

    [Test]
    public void OverrideAlsoWorksForGenericsOpenOpen ()
    {
      ClassContext ctx = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithOpenOverridingOpen)).BuildContext ().GetClassContext (typeof (DerivedWithOpenOverridingOpen));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<,>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsOpenClosed ()
    {
      ClassContext ctx = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithOpenOverridingClosed)).BuildContext ().GetClassContext (typeof (DerivedWithOpenOverridingClosed));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<,>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedOpen ()
    {
      ClassContext ctx = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithClosedOverridingOpen)).BuildContext ().GetClassContext (typeof (DerivedWithClosedOverridingOpen));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<object, object>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedClosed ()
    {
      ClassContext ctx = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithClosedOverridingClosed)).BuildContext ().GetClassContext (typeof (DerivedWithClosedOverridingClosed));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<object, object>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedOpen ()
    {
      ClassContext ctx = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingOpen)).BuildContext ().GetClassContext (typeof (DerivedWithRealClosedOverridingOpen));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedClosedMixin)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedClosed ()
    {
      ClassContext ctx = new ApplicationContextBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingClosed)).BuildContext ().GetClassContext (typeof (DerivedWithRealClosedOverridingClosed));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedClosedMixin)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Uses (typeof (NullMixin))]
    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithDuplicateUses : BaseWithUses
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*NullMixin are configured for target type "
        + ".*DerivedWithDuplicateUses.", MatchType = MessageMatch.Regex)]
    public void ThrowsOnUsesDuplicateOnSameClass ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (DerivedWithDuplicateUses)).BuildContext ();
    }

    [Uses (typeof (BaseGenericMixin<,>))]
    [Uses (typeof (BaseGenericMixin<,>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics1
    {
    }

    [Uses (typeof (BaseGenericMixin<,>))]
    [Uses (typeof (BaseGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics2
    {
    }

    [Uses (typeof (BaseGenericMixin<DuplicateWithGenerics3, object>))]
    [Uses (typeof (BaseGenericMixin<object, object>))]
    [IgnoreForMixinConfiguration]
    public class DuplicateWithGenerics3
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics1.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics1 ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (DuplicateWithGenerics1)).BuildContext ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics2.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics2 ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (DuplicateWithGenerics2)).BuildContext ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
        + "type .*DuplicateWithGenerics3.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics3 ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (DuplicateWithGenerics3)).BuildContext ();
    }
  }
}
