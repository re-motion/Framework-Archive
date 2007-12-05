using System;
using System.Collections.Generic;
using Rubicon.Mixins.Context;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.Configuration.Context
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

    [Extends (typeof (ExtendsTargetBase), MixinTypeArguments = new Type[] { typeof (List<int>), typeof (IList<int>) })]
    [IgnoreForMixinConfiguration]
    public class GenericMixinWithSpecialization<TThis, TBase> : Mixin<TThis, TBase>
        where TThis : class
        where TBase : class
    {
    }

    [Test]
    public void ExtendsCanSpecializeGenericMixin ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (GenericMixinWithSpecialization<,>)).BuildContext ();
      MixinContext mixinContext = new List<MixinContext> (context.GetClassContext (typeof (ExtendsTargetBase)).Mixins)[0];
      Assert.IsTrue (ReflectionUtility.CanAscribe (mixinContext.MixinType, typeof (GenericMixinWithSpecialization<,>)));
      Assert.IsFalse (mixinContext.MixinType.IsGenericTypeDefinition);
      Assert.IsFalse (mixinContext.MixinType.ContainsGenericParameters);
      Assert.AreEqual (new Type[] {typeof (List<int>), typeof (IList<int>)}, mixinContext.MixinType.GetGenericArguments());
    }

    [Extends (typeof (ExtendsTargetBase), MixinTypeArguments = new Type[] { typeof (List<int>) })]
    [IgnoreForMixinConfiguration]
    public class InvalidGenericMixin<TThis, TBase> : Mixin<TThis, TBase>
        where TThis : class
        where TBase : class
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "The ExtendsAttribute for target class "
        + ".*ExtendsTargetBase applied to mixin type "
        + ".*InvalidGenericMixin`2 specified invalid generic type arguments.", MatchType = MessageMatch.Regex)]
    public void InvalidTypeParametersThrowConfigurationException ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (InvalidGenericMixin<,>)).BuildContext ();
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (ExtendingMixin) })]
    [IgnoreForMixinConfiguration]
    public class SuppressingExtender { }

    [Test]
    public void SuppressedMixins ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (ExtendingMixin)).AddType (typeof (SuppressingExtender))
          .BuildContext ();
      ClassContext classContext = context.GetClassContext (typeof (ExtendsTargetBase));
      Assert.IsTrue (classContext.ContainsMixin (typeof (SuppressingExtender)));
      Assert.IsFalse (classContext.ContainsMixin (typeof (ExtendingMixin)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (CircularSuppressingExtender2) })]
    [IgnoreForMixinConfiguration]
    public class CircularSuppressingExtender1 { }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (CircularSuppressingExtender1) })]
    [IgnoreForMixinConfiguration]
    public class CircularSuppressingExtender2 { }

    [Test]
    public void CircularSuppressingMixins ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (CircularSuppressingExtender1))
          .AddType (typeof (CircularSuppressingExtender2)).BuildContext ();
      ClassContext classContext = context.GetClassContext (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.ContainsMixin (typeof (CircularSuppressingExtender1)));
      Assert.IsFalse (classContext.ContainsMixin (typeof (CircularSuppressingExtender2)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (SelfSuppressingExtender2) })]
    [IgnoreForMixinConfiguration]
    public class SelfSuppressingExtender2 { }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Mixin type "
        + ".*SelfSuppressingExtender2 applied to target class .*ExtendsTargetBase suppresses itself.", MatchType = MessageMatch.Regex)]
    public void SelfSuppressingMixin ()
    {
      new ApplicationContextBuilder (null).AddType (typeof (SelfSuppressingExtender2)).BuildContext ();
    }

    [Extends (typeof (ExtendsTargetBase))]
    [IgnoreForMixinConfiguration]
    public class GenericMixinWithoutSpecialization<TThis, TBase> : Mixin<TThis, TBase>
        where TThis : class
        where TBase : class
    {
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (GenericMixinWithoutSpecialization<,>),
        typeof (GenericMixinWithSpecialization<,>) })]
    [IgnoreForMixinConfiguration]
    public class GenericSuppressingExtender { }

    [Test]
    public void GenericSuppressingMixinWithSpecialization ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (GenericSuppressingExtender))
          .BuildContext ();
      ClassContext classContext = context.GetClassContext (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.ContainsMixin (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Test]
    public void GenericSuppressingMixinWithoutSpecialization ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null)
          .AddType (typeof (GenericMixinWithoutSpecialization<,>))
          .AddType (typeof (GenericSuppressingExtender))
          .BuildContext ();
      ClassContext classContext = context.GetClassContext (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.ContainsMixin (typeof (GenericMixinWithoutSpecialization<,>)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (GenericMixinWithSpecialization<List<int>, IList<int>>) })]
    [IgnoreForMixinConfiguration]
    public class ClosedGenericSuppressingExtender { }

    [Test]
    public void ClosedGenericSuppressingMixin ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (ClosedGenericSuppressingExtender)).BuildContext ();
      ClassContext classContext = context.GetClassContext (typeof (ExtendsTargetBase));
      Assert.IsFalse (classContext.ContainsMixin (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Extends (typeof (ExtendsTargetBase), SuppressedMixins = new Type[] { typeof (GenericMixinWithSpecialization<object, string>) })]
    [IgnoreForMixinConfiguration]
    public class ClosedGenericSuppressingExtender_WithWrongParameterTypes { }

    [Test]
    public void ClosedGenericSuppressingMixin_WithWrongParameterTypes ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null)
          .AddType (typeof (GenericMixinWithSpecialization<,>))
          .AddType (typeof (ClosedGenericSuppressingExtender_WithWrongParameterTypes)).BuildContext ();
      ClassContext classContext = context.GetClassContext (typeof (ExtendsTargetBase));
      Assert.IsTrue (classContext.ContainsMixin (typeof (GenericMixinWithSpecialization<List<int>, IList<int>>)));
    }

    [Test]
    public void ExtendsAppliedToSpecificGenericClass ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (MixinExtendingSpecificGenericClass)).BuildContext();
      Assert.IsTrue (context.ContainsClassContext (typeof (GenericClassExtendedByMixin<int>)));
      Assert.IsFalse (context.ContainsClassContext (typeof (GenericClassExtendedByMixin<string>)));
      Assert.IsFalse (context.ContainsClassContext (typeof (GenericClassExtendedByMixin<>)));

      Assert.IsNotNull (context.GetClassContext (typeof (GenericClassExtendedByMixin<int>)));
      Assert.IsNull (context.GetClassContext (typeof (GenericClassExtendedByMixin<string>)));
      Assert.IsNull (context.GetClassContext (typeof (GenericClassExtendedByMixin<>)));
    }
  }
}