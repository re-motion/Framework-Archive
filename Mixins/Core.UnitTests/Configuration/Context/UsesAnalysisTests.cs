using System;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.Configuration.Context
{
  [TestFixture]
  public class UsesAnalysisTests
  {
    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class UserWithoutDependencies
    {
    }

    [Uses (typeof (NullMixin), AdditionalDependencies = new Type[] { typeof (string) })]
    [IgnoreForMixinConfiguration]
    public class UserWithDependencies
    {
    }

    [Test]
    public void AdditionalDependencies ()
    {
      MixinConfiguration context =
          new DeclarativeConfigurationBuilder (null).AddType (typeof (UserWithoutDependencies)).AddType (typeof (UserWithDependencies)).BuildConfiguration();
      Assert.AreEqual (0, context.GetClassContext (typeof (UserWithoutDependencies)).GetMixinContext (typeof (NullMixin))
          .ExplicitDependencies.Count);
      Assert.AreEqual (1, context.GetClassContext (typeof (UserWithDependencies)).GetMixinContext (typeof (NullMixin))
          .ExplicitDependencies.Count);
      Assert.IsTrue (context.GetClassContext (typeof (UserWithDependencies)).GetMixinContext (typeof (NullMixin))
          .ExplicitDependencies.ContainsKey (typeof (string)));
    }

    [Uses (typeof (NullMixin), AdditionalDependencies = new Type[] { typeof (object) })]
    [IgnoreForMixinConfiguration]
    public class BaseWithUses { }

    public class DerivedWithoutUses : BaseWithUses { }

    [Test]
    public void UsesAttributeIsInherited ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithoutUses)).BuildConfiguration ();
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithoutUses)).ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithoutUses)).GetMixinContext (typeof (NullMixin))
          .ExplicitDependencies.ContainsKey (typeof (object)));
      Assert.AreEqual (1, context.GetClassContext (typeof (DerivedWithoutUses)).MixinCount);
    }

    public class DedicatedMixin {}

    [Uses( typeof (DedicatedMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithOwnUses : BaseWithUses { }

    [Test]
    public void UsesAttributeIsInherited_AndAugmentedWithOwn ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOwnUses)).BuildConfiguration ();
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithOwnUses)).ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithOwnUses)).ContainsMixin (typeof (DedicatedMixin)));

      Type[] mixinTypes = EnumerableUtility.SelectToArray<MixinContext, Type> (
        context.GetClassContext (typeof (DerivedWithOwnUses)).Mixins, delegate (MixinContext mixin) { return mixin.MixinType; });
      
      Assert.That (mixinTypes, Is.EquivalentTo (new Type[] {typeof (NullMixin), typeof (DedicatedMixin)}));
      Assert.AreEqual (2, context.GetClassContext (typeof (DerivedWithOwnUses)).MixinCount);
    }

    [Uses (typeof (NullMixin))]
    public class GenericBaseWithMixin<T>
    {
    }

    public class GenericDerivedWithInheritedMixin<T> : GenericBaseWithMixin<T>
    {
    }

    [Test]
    public void UsesAttributeIsInheritedOnOpenGenericTypes ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (GenericDerivedWithInheritedMixin<>)).BuildConfiguration ();
      Assert.IsTrue (context.GetClassContext (typeof (GenericDerivedWithInheritedMixin<>)).ContainsMixin (typeof (NullMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (GenericDerivedWithInheritedMixin<>)).MixinCount);
    }

    public class NonGenericDerivedWithInheritedMixinFromGeneric : GenericBaseWithMixin<int>
    {
    }

    [Test]
    public void UsesAttributeIsInheritedOnNonGenericTypesInheritingFromGeneric ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).BuildConfiguration ();
      Assert.IsTrue (context.GetClassContext (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).ContainsMixin (typeof (NullMixin)));
      Assert.AreEqual (1, context.GetClassContext (typeof (NonGenericDerivedWithInheritedMixinFromGeneric)).MixinCount);
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesDuplicateIsIgnored ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithUses)).BuildConfiguration ();
      Assert.IsTrue (context.GetClassContext (typeof (DerivedWithUses)).ContainsMixin (typeof (NullMixin)));
      Assert.IsFalse (context.GetClassContext (typeof (DerivedWithUses)).GetMixinContext (typeof (NullMixin))
          .ExplicitDependencies.ContainsKey (typeof (object)));
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
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithMoreSpecificUses)).BuildConfiguration ();
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
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOpenOverridingOpen)).BuildConfiguration ().GetClassContext (typeof (DerivedWithOpenOverridingOpen));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<,>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsOpenClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithOpenOverridingClosed)).BuildConfiguration ().GetClassContext (typeof (DerivedWithOpenOverridingClosed));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<,>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithClosedOverridingOpen)).BuildConfiguration ().GetClassContext (typeof (DerivedWithClosedOverridingOpen));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<object, object>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsClosedClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithClosedOverridingClosed)).BuildConfiguration ().GetClassContext (typeof (DerivedWithClosedOverridingClosed));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedGenericMixin<object, object>)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<BaseWithClosedGeneric, object>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedOpen ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingOpen)).BuildConfiguration ().GetClassContext (typeof (DerivedWithRealClosedOverridingOpen));
      Assert.IsTrue (ctx.ContainsMixin (typeof (DerivedClosedMixin)));
      Assert.IsFalse (ctx.ContainsMixin (typeof (BaseGenericMixin<,>)));
      Assert.AreEqual (1, ctx.MixinCount);
    }

    [Test]
    public void OverrideAlsoWorksForGenericsRealClosedClosed ()
    {
      ClassContext ctx = new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithRealClosedOverridingClosed)).BuildConfiguration ().GetClassContext (typeof (DerivedWithRealClosedOverridingClosed));
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
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DerivedWithDuplicateUses)).BuildConfiguration ();
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
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics1)).BuildConfiguration ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
                                                                           + "type .*DuplicateWithGenerics2.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics2 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics2)).BuildConfiguration ();
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin .*BaseGenericMixin`2 are configured for target "
                                                                           + "type .*DuplicateWithGenerics3.", MatchType = MessageMatch.Regex)]
    public void DuplicateDetectionAlsoWorksForGenerics3 ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (DuplicateWithGenerics3)).BuildConfiguration ();
    }

    [Uses (typeof (NullMixin), SuppressedMixins = new Type[] { typeof (SuppressedExtender) })]
    [IgnoreForMixinConfiguration]
    public class SuppressingUser { }

    [Extends (typeof (SuppressingUser))]
    public class SuppressedExtender
    {
    }

    [Test]
    public void SuppressedMixins ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null)
          .AddType (typeof (SuppressingUser))
          .AddType (typeof (SuppressedExtender))
          .BuildConfiguration ();
      ClassContext classContext = context.GetClassContext (typeof (SuppressingUser));
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.IsFalse (classContext.ContainsMixin (typeof (SuppressedExtender)));
    }

    [Uses (typeof (NullMixin), SuppressedMixins = new Type[] { typeof (NullMixin) })]
    [IgnoreForMixinConfiguration]
    public class SelfSuppressingUser { }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Mixin type Rubicon.Mixins.UnitTests.SampleTypes.NullMixin applied to "
        + "target class .*SelfSuppressingUser suppresses itself.", MatchType = MessageMatch.Regex)]
    public void SelfSuppresser ()
    {
      new DeclarativeConfigurationBuilder (null).AddType (typeof (SelfSuppressingUser)).BuildConfiguration ();
    }
  }
}