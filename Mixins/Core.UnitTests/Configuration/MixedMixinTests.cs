using System;
using NUnit.Framework;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class MixedMixinTests
  {
    public class TargetClass
    {
    }

    [Extends (typeof (TargetClass))]
    public class One
    {
    }

    [Extends (typeof (One))]
    public class Two
    {
    }

    [Test]
    public void MixinOnMixinDoesNotInfluenceTargetClass ()
    {
      ClassContext c1;
      ClassContext c2;

      using (MixinConfiguration.ScopedExtend (typeof (TargetClass), typeof (One)))
      {
        c1 = MixinConfiguration.ActiveContext.GetClassContext (typeof (TargetClass));
        using (MixinConfiguration.ScopedExtend (typeof (One), typeof (Two)))
        {
          c2 = MixinConfiguration.ActiveContext.GetClassContext (typeof (TargetClass));
        }
      }

      Assert.AreEqual (c1, c2);

      TargetClassDefinition targetClass = TypeFactory.GetActiveConfiguration (typeof (TargetClass));
      Assert.AreEqual (typeof (TargetClass), targetClass.Type);
      Assert.AreEqual (1, targetClass.Mixins.Count);
      Assert.AreEqual (typeof (One), targetClass.Mixins[0].Type);
    }

    [Test]
    public void MixinOnMixinYieldsTargetClassDefinitionForMixin ()
    {
      TargetClassDefinition targetClassForMixin = TypeFactory.GetActiveConfiguration (typeof (One));
      Assert.AreEqual (typeof (One), targetClassForMixin.Type);
      Assert.AreEqual (1, targetClassForMixin.Mixins.Count);
      Assert.AreEqual (typeof (Two), targetClassForMixin.Mixins[0].Type);
    }
  }
}