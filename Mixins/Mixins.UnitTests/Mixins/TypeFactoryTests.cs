using System;
using Mixins;
using Mixins.Context;
using Mixins.Definitions;
using NUnit.Framework;
using Mixins.CodeGeneration;
using Mixins.UnitTests.SampleTypes;
using System.Reflection;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class TypeFactoryTests
  {
    [Test]
    public void GetActiveConfiguration()
    {
      Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));
      Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
      Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
      Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));

      using (new MixinConfiguration (typeof (BaseType1)))
      {
        Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));
        Assert.AreSame (BaseClassDefinitionCache.Current.GetBaseClassDefinition (new ClassContext (typeof (BaseType1))),
          TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
        Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
        Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
        Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));

        using (new MixinConfiguration (typeof (BaseType2)))
        {
          Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
          Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));

          Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
          Assert.AreSame (BaseClassDefinitionCache.Current.GetBaseClassDefinition (new ClassContext (typeof (BaseType1))),
            TypeFactory.GetActiveConfiguration (typeof (BaseType1)));
          Assert.IsNotNull (TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
          Assert.AreSame (BaseClassDefinitionCache.Current.GetBaseClassDefinition (new ClassContext (typeof (BaseType2))),
            TypeFactory.GetActiveConfiguration (typeof (BaseType2)));
        }
      }
    }

    [Test]
    public void PartialInstantiation()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        BT1Mixin1 m1 = new BT1Mixin1();
        BaseType1 bt1 = ObjectFactory.CreateWithMixinInstances<BaseType1> (m1).With();

        Assert.IsNotNull (Mixin.Get<BT1Mixin1> (bt1));
        Assert.AreSame (m1, Mixin.Get<BT1Mixin1> (bt1));
        Assert.IsNotNull (Mixin.Get<BT1Mixin2> (bt1));
        Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> (bt1));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void ThrowsOnWrongInstantiation ()
    {
      using (new MixinConfiguration (Assembly.GetExecutingAssembly ()))
      {
        BT2Mixin1 m1 = new BT2Mixin1();
        ObjectFactory.CreateWithMixinInstances<BaseType3> (m1).With();
      }
    }

    [Test]
    public void DefinitionGeneratedEvenIfNoConfig()
    {
      Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (object)));
      Type t = TypeFactory.GetConcreteType (typeof (object));
      Assert.IsTrue (typeof (IMixinTarget).IsAssignableFrom (t));
    }
  }
}
