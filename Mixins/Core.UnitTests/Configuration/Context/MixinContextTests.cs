using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Mixins.UnitTests.Configuration.Context
{
  [TestFixture]
  public class MixinContextTests
  {
    [Test]
    public void ExplicitDependencies_Empty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT7Mixin1), new Type[0]);

      Assert.AreEqual (0, mixinContext.ExplicitDependencyCount);
      Assert.IsFalse (mixinContext.ContainsExplicitDependency (typeof (IBaseType2)));

      List<Type> deps = new List<Type> (mixinContext.ExplicitDependencies);
      Assert.That (deps, Is.Empty);
    }

    [Test]
    public void ExplicitInterfaceDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT6Mixin1), new Type[] {typeof (IBT6Mixin2), typeof (IBT6Mixin3)});

      Assert.AreEqual (2, mixinContext.ExplicitDependencyCount);
      Assert.IsTrue (mixinContext.ContainsExplicitDependency (typeof (IBT6Mixin2)));
      Assert.IsTrue (mixinContext.ContainsExplicitDependency (typeof (IBT6Mixin3)));

      List<Type> deps = new List<Type> (mixinContext.ExplicitDependencies);
      Assert.That (deps, Is.EqualTo (new Type[] {typeof (IBT6Mixin2), typeof (IBT6Mixin3)}));
    }

    [Test]
    public void ExplicitMixinDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT6Mixin1), new Type[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });

      Assert.AreEqual (2, mixinContext.ExplicitDependencyCount);
      Assert.IsTrue (mixinContext.ContainsExplicitDependency (typeof (BT6Mixin2)));
      Assert.IsTrue (mixinContext.ContainsExplicitDependency (typeof (BT6Mixin3<>)));

      List<Type> deps = new List<Type> (mixinContext.ExplicitDependencies);
      Assert.That (deps, Is.EqualTo (new Type[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) }));
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