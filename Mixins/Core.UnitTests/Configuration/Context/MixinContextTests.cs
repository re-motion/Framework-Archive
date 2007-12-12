using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration.Context
{
  [TestFixture]
  public class MixinContextTests
  {
    [Test]
    public void ExplicitInterfaceDependencies ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (BaseType7));
      MixinContext mixinContext = classContext.GetOrAddMixinContext (typeof (BT7Mixin1));

      Assert.AreEqual (0, mixinContext.ExplicitDependencyCount);
      Assert.IsFalse (mixinContext.ContainsExplicitDependency (typeof (IBaseType2)));

      List<Type> deps = new List<Type> (mixinContext.ExplicitDependencies);
      Assert.AreEqual (0, deps.Count);

      Assert.IsFalse (mixinContext.RemoveExplicitDependency (typeof (IBaseType2)));

      mixinContext.AddExplicitDependency (typeof (IBaseType2));

      Assert.AreEqual (1, mixinContext.ExplicitDependencyCount);
      Assert.IsTrue (mixinContext.ContainsExplicitDependency (typeof (IBaseType2)));

      deps = new List<Type> (mixinContext.ExplicitDependencies);
      Assert.AreEqual (1, deps.Count);
      Assert.IsTrue (deps.Contains (typeof (IBaseType2)));

      Assert.IsTrue (mixinContext.RemoveExplicitDependency (typeof (IBaseType2)));

      Assert.AreEqual (0, mixinContext.ExplicitDependencyCount);
      Assert.IsFalse (mixinContext.ContainsExplicitDependency (typeof (IBaseType2)));

      deps = new List<Type> (mixinContext.ExplicitDependencies);
      Assert.AreEqual (0, deps.Count);

      Assert.IsFalse (mixinContext.RemoveExplicitDependency (typeof (IBaseType2)));
    }

    [Test]
    public void ExplicitMixinDependencies ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (BaseType7));
      MixinContext mixinContext = classContext.GetOrAddMixinContext (typeof (BT7Mixin1));

      Assert.AreEqual (0, mixinContext.ExplicitDependencyCount);
      Assert.IsFalse (mixinContext.ContainsExplicitDependency (typeof (BT7Mixin2)));

      List<Type> deps = new List<Type> (mixinContext.ExplicitDependencies);
      Assert.AreEqual (0, deps.Count);

      mixinContext.AddExplicitDependency (typeof (BT7Mixin2));

      Assert.AreEqual (1, mixinContext.ExplicitDependencyCount);
      Assert.IsTrue (mixinContext.ContainsExplicitDependency (typeof (BT7Mixin2)));
    }

    [Test]
    public void CannotCastExplicitDependenciesToICollection ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      MixinContext mc = cc.GetOrAddMixinContext (typeof (BT1Mixin1));
      Assert.IsTrue (mc.ExplicitDependencies is IEnumerable<Type>);
      Assert.IsFalse (mc.ExplicitDependencies is List<Type>);
      Assert.IsFalse (mc.ExplicitDependencies is IList<Type>);
      Assert.IsFalse (mc.ExplicitDependencies is ICollection<Type>);
      Assert.IsFalse (mc.ExplicitDependencies is ICollection);
      Assert.IsFalse (mc.ExplicitDependencies is IList);
    }
  }
}