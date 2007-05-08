using System;
using Mixins;
using Mixins.Definitions;
using NUnit.Framework;
using Mixins.CodeGeneration;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class CustomConfigurationTests : MixinTestBase
  {
    [Test]
    public void CurrentTypeFactoryScope()
    {
      Assert.IsTrue (TypeFactory.HasCurrent);
      TypeFactory old = TypeFactory.Current;
      TypeFactory new1 = new TypeFactory (DefBuilder.Build(typeof (CustomConfigurationTests)));
      
      Assert.AreSame (old, TypeFactory.Current);
      using (new CurrentTypeFactoryScope(new1))
      {
        Assert.AreNotSame (old, TypeFactory.Current);
        Assert.AreSame (new1, TypeFactory.Current);

        TypeFactory new2 = new TypeFactory (DefBuilder.Build (typeof (CustomConfigurationTests)));
        Assert.AreNotSame (new2, TypeFactory.Current);
        using (new CurrentTypeFactoryScope(new2))
        {
          Assert.AreNotSame (old, TypeFactory.Current);
          Assert.AreNotSame (new1, TypeFactory.Current);
          Assert.AreSame (new2, TypeFactory.Current);
        }

        Assert.AreNotSame (old, TypeFactory.Current);
        Assert.AreSame (new1, TypeFactory.Current);
        Assert.AreNotSame (new2, TypeFactory.Current);
      }

      Assert.AreSame (old, TypeFactory.Current);
      Assert.AreNotSame (new1, TypeFactory.Current);

      TypeFactory.SetCurrent (null);

      Assert.IsFalse (TypeFactory.HasCurrent);
      using (new CurrentTypeFactoryScope(TypeFactory.DefaultInstance))
      {
        Assert.IsTrue (TypeFactory.HasCurrent);
        Assert.AreSame (TypeFactory.DefaultInstance, TypeFactory.Current);
      }
      Assert.IsFalse (TypeFactory.HasCurrent);
    }

    [Test]
    public void PartialInstantiation()
    {
      BT1Mixin1 m1 = new BT1Mixin1 ();
      BaseType1 bt1 = ObjectFactory.CreateWithMixinInstances<BaseType1> (m1).With();

      Assert.IsNotNull (Mixin.Get<BT1Mixin1> ((object) bt1));
      Assert.AreSame (m1, Mixin.Get<BT1Mixin1> ((object) bt1));
      Assert.IsNotNull (Mixin.Get<BT1Mixin2> ((object) bt1));
      Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> ((object) bt1));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException))]
    public void ThrowsOnWrongInstantiation ()
    {
      BT2Mixin1 m1 = new BT2Mixin1 ();
      BaseType3 bt3 = ObjectFactory.CreateWithMixinInstances<BaseType3> (m1).With ();
    }
  }
}
