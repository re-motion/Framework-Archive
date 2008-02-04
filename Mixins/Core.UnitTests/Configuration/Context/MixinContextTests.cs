using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
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

      Assert.AreEqual (0, mixinContext.ExplicitDependencies.Count);
      Assert.IsFalse (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBaseType2)));

      Assert.That (mixinContext.ExplicitDependencies, Is.Empty);
    }

    [Test]
    public void ExplicitInterfaceDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT6Mixin1), new Type[] {typeof (IBT6Mixin2), typeof (IBT6Mixin3)});

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (IBT6Mixin3)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new Type[] { typeof(IBT6Mixin2), typeof(IBT6Mixin3) }));
    }

    [Test]
    public void ExplicitMixinDependencies_NonEmpty ()
    {
      MixinContext mixinContext = new MixinContext (typeof (BT6Mixin1), new Type[] { typeof (BT6Mixin2), typeof (BT6Mixin3<>) });

      Assert.AreEqual (2, mixinContext.ExplicitDependencies.Count);
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin2)));
      Assert.IsTrue (mixinContext.ExplicitDependencies.ContainsKey (typeof (BT6Mixin3<>)));

      Assert.That(mixinContext.ExplicitDependencies, Is.EquivalentTo(new Type[] { typeof(BT6Mixin2), typeof(BT6Mixin3<>) }));
    }
  }
}