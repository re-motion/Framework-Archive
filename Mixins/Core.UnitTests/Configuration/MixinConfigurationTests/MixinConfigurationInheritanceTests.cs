using System;
using System.Collections.Generic;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationInheritanceTests
  {
    [Test]
    public void InheritingMixinConfigurationKnowsClassesFromBasePlusOwn ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      Assert.AreEqual (0, ac.ClassContextCount);
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
      ac.AddClassContext (new ClassContext (typeof (BaseType2)));
      Assert.AreEqual (2, ac.ClassContextCount);

      MixinConfiguration ac2 = new MixinConfiguration (ac);
      Assert.AreEqual (2, ac2.ClassContextCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType2)));
      Assert.IsFalse (ac2.ContainsClassContext (typeof (BaseType3)));

      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType1)));
      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType2)));
      Assert.IsNull (ac2.GetClassContext (typeof (BaseType3)));

      ac2.AddClassContext (new ClassContext (typeof (BaseType3)));
      Assert.AreEqual (3, ac2.ClassContextCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType2)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType3)));

      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType1)));
      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType2)));
      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType3)));

      List<ClassContext> contexts = new List<ClassContext> (ac2.ClassContexts);
      Assert.AreEqual (3, contexts.Count);
      Assert.Contains (ac.GetClassContext (typeof (BaseType1)), contexts);
      Assert.Contains (ac.GetClassContext (typeof (BaseType2)), contexts);
      Assert.Contains (ac2.GetClassContext (typeof (BaseType3)), contexts);
    }

    [Test]
    public void OverridingClassContextsFromParent ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      Assert.AreEqual (0, ac.ClassContextCount);
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
      ac.AddClassContext (new ClassContext (typeof (BaseType2)));
      Assert.AreEqual (2, ac.ClassContextCount);

      MixinConfiguration ac2 = new MixinConfiguration (ac);
      Assert.AreEqual (2, ac2.ClassContextCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.AreEqual (ac.GetClassContext (typeof (BaseType1)), ac2.GetClassContext (typeof (BaseType1)));

      ClassContext newContext = new ClassContext (typeof (BaseType1));
      ac2.AddOrReplaceClassContext (newContext);
      Assert.AreEqual (2, ac2.ClassContextCount);

      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.AreNotSame (ac.GetClassContext (typeof (BaseType1)), ac2.GetClassContext (typeof (BaseType1)));
    }

    [Test]
    public void GetContextWorksRecursively ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (DerivedNullTarget));
        Assert.IsNotNull (context);
        Assert.AreEqual (typeof (DerivedNullTarget), context.Type);
        Assert.IsTrue (context.ContainsMixin (typeof (NullMixin)));
      }
    }

    [Test]
    public void GetContextWorksRecursively_OverGenericDefinition ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (GenericTargetClass<>)).Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (GenericTargetClass<object>));
        Assert.IsNotNull (context);
        Assert.AreEqual (typeof (GenericTargetClass<object>), context.Type);
        Assert.IsTrue (context.ContainsMixin (typeof (NullMixin)));
      }
    }

    [Test]
    public void GetContextWorksRecursively_OverGenericDefinitionAndBase ()
    {
      using (MixinConfiguration.BuildFromActive ()
          .ForClass (typeof (GenericTargetClass<>)).Clear ().AddMixins (typeof (NullMixin))
          .ForClass (typeof (GenericTargetClass<object>)).Clear ().AddMixins (typeof (NullMixin2))
          .EnterScope ())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.GetClassContext (typeof (DerivedGenericTargetClass<object>));
        Assert.IsNotNull (context);
        Assert.AreEqual (typeof (DerivedGenericTargetClass<object>), context.Type);
        Assert.IsTrue (context.ContainsMixin (typeof (NullMixin)));
        Assert.IsTrue (context.ContainsMixin (typeof (NullMixin2)));
      }
    }

    [Test]
    public void ContainsContextWorksRecursively ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass<NullTarget> ().Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (DerivedNullTarget)));
      }
    }

    [Test]
    public void ContainsContextWorksRecursively_OverGenericDefinition ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (GenericTargetClass<>)).Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        Assert.IsTrue (MixinConfiguration.ActiveConfiguration.ContainsClassContext (typeof (GenericTargetClass<object>)));
      }
    }
  }
}