using System;
using System.Collections.Generic;
using System.Reflection;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.Context;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ApplicationContextBuilderTests
  {
    [Test]
    public void BuildFromClassContexts()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildFromClasses (null, new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac.ContainsClassContext (typeof (BaseType2)));

      ApplicationContext ac2 = ApplicationContextBuilder.BuildFromClasses (ac, new ClassContext (typeof (BaseType2)), new ClassContext (typeof (BaseType3)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType2)));
      Assert.AreEqual (0, ac2.GetClassContext (typeof (BaseType2)).MixinCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType3)));

      ApplicationContext ac3 = ApplicationContextBuilder.BuildFromClasses (ac2, new ClassContext (typeof (BaseType2), typeof (BT2Mixin1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType2)));
      Assert.AreEqual (1, ac3.GetClassContext (typeof (BaseType2)).MixinCount);
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType3)));
    }

    [Test]
    public void BuildFromAssemblies()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildFromClasses (null, new ClassContext(typeof (object)));
      ApplicationContext ac2 = ApplicationContextBuilder.BuildFromAssemblies (AppDomain.CurrentDomain.GetAssemblies());
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac2.ContainsClassContext (typeof (object)));

      ApplicationContext ac3 = ApplicationContextBuilder.BuildFromAssemblies (ac, AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (object)));

      ApplicationContext ac4 = ApplicationContextBuilder.BuildFromAssemblies (ac, (IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac4.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac4.ContainsClassContext (typeof (object)));
    }

    [Test]
    public void AnalyzeAssemblyIntoContext()
    {
      ApplicationContext ac = new ApplicationContext();
      Assert.IsFalse (ac.ContainsClassContext (typeof (BaseType1)));
      ApplicationContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly(), ac);
      Assert.IsTrue (ac.ContainsClassContext (typeof (BaseType1)));

      ApplicationContext ac2 = ApplicationContextBuilder.BuildFromClasses (null, new ClassContext (typeof (BaseType1), typeof (string)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (string)));
      Assert.IsFalse (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
      ApplicationContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly(), ac2);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (string)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
      ApplicationContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly (), ac2);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (string)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
    }

    [Test]
    public void BuildDefault()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildDefault();
      Assert.IsNotNull (ac);
      Assert.AreNotEqual (0, ac.ClassContextCount);
    }

    [Extends (typeof (BaseType1))]
    [IgnoreForMixinConfiguration]
    public class Foo { }

    [Test]
    public void IgnoreForMixinConfiguration()
    {
      Assert.IsFalse (TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins.HasItem (typeof (Foo)));
    }

    [Uses (typeof (NullMixin), AdditionalDependencies = new Type[] {typeof (object)})]
    [IgnoreForMixinConfiguration]
    public class BaseWithUses { }

    public class DerivedWithoutUses : BaseWithUses { }
    

    [Test]
    public void UsesAttributeIsInherited ()
    {
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (DerivedWithoutUses), MixinConfiguration.ActiveContext);

      Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (DerivedWithoutUses)).ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (DerivedWithoutUses)).GetOrAddMixinContext (typeof (NullMixin)).ContainsExplicitDependency (typeof (object)));
      Assert.AreEqual (1, MixinConfiguration.ActiveContext.GetClassContext (typeof (DerivedWithoutUses)).MixinCount);
    }

    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithUses : BaseWithUses
    {
    }

    [Test]
    public void InheritedUsesDuplicateIsIgnored ()
    {
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (DerivedWithUses), MixinConfiguration.ActiveContext);

      Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (DerivedWithUses)).ContainsMixin (typeof (NullMixin)));
      Assert.IsFalse (MixinConfiguration.ActiveContext.GetClassContext (typeof (DerivedWithUses)).GetOrAddMixinContext (typeof (NullMixin)).ContainsExplicitDependency (typeof (object)));
      Assert.AreEqual (1, MixinConfiguration.ActiveContext.GetClassContext (typeof (DerivedWithUses)).MixinCount);
    }

    [Uses (typeof (NullMixin))]
    [Uses (typeof (NullMixin))]
    [IgnoreForMixinConfiguration]
    public class DerivedWithDuplicateUses : BaseWithUses
    {
    }

    [Test]
    [ExpectedException (typeof (ConfigurationException), ExpectedMessage = "Two instances of mixin Mixins.UnitTests.SampleTypes.NullMixin are "
        + "configured for target type Mixins.UnitTests.Configuration.ApplicationContextBuilderTests+DerivedWithDuplicateUses.")]
    public void ThrowsOnUsesDuplicateOnSameClass ()
    {
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (DerivedWithDuplicateUses), MixinConfiguration.ActiveContext);
    }

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
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (ExtendingMixin), MixinConfiguration.ActiveContext);

      Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.AreEqual (1, MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithoutExtends)).MixinCount);
    }

    public class ExtendsTargetDerivedWithExtends : ExtendsTargetBase { }

    [Test]
    [Ignore ("TODO: Implement Extends inheritance")]
    public void InheritedDuplicateExtensionIsIgnored ()
    {
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (ExtendingMixin), MixinConfiguration.ActiveContext);

      Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.AreEqual (1, MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithExtends)).MixinCount);
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
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (ExtendingMixin), MixinConfiguration.ActiveContext);
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (DerivedExtendingMixin), MixinConfiguration.ActiveContext);

      Assert.IsFalse (MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.IsTrue(MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin)));
      Assert.AreEqual (1, MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).MixinCount);

      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (DerivedExtendingMixin2), MixinConfiguration.ActiveContext);
      Assert.IsFalse (MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (ExtendingMixin)));
      Assert.IsFalse (MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin)));
      Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).ContainsMixin (typeof (DerivedExtendingMixin2)));
      Assert.AreEqual (1, MixinConfiguration.ActiveContext.GetClassContext (typeof (ExtendsTargetDerivedWithDerivedExtends)).MixinCount);
    }

    [Extends (typeof (ExtendsTargetBase))]
    [Extends (typeof (ExtendsTargetBase))]
    [IgnoreForMixinConfiguration]
    public class DoubleExtendingMixin { }

    [Test]
    [Ignore ("TODO: Implement Extends inheritance")]
    public void ThrowsOnDuplicateExtendsForSameClass ()
    {
      ApplicationContextBuilder.AnalyzeTypeIntoContext (typeof (DoubleExtendingMixin), MixinConfiguration.ActiveContext);
      Assert.Fail ("Exception expected");
    }
  }
}
