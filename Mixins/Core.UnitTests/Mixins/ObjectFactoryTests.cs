using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Mixins
{
  [TestFixture]
  public class ObjectFactoryTests : MixinTestBase
  {
    [Test]
    public void MixedObjectsCanBeCreated ()
    {
      object o = ObjectFactory.Create<BaseType3> ().With();
      Assert.IsNotNull (o);
      Assert.IsTrue (o is BaseType3);
      Assert.IsTrue (o is IMixinTarget);

      Assert.IsNotNull (((IMixinTarget) o).Mixins[0]);
    }

    [Test]
    public void MixedObjectsCanBeCreatedFromType ()
    {
      object o = ObjectFactory.Create (typeof (BaseType3)).With ();
      Assert.IsNotNull (o);
      Assert.IsTrue (o is IMixinTarget);
      Assert.IsNotNull (((IMixinTarget) o).Mixins[0]);
    }

    [Test]
    public void MixedObjectsCanBeCreatedWithMixinInstances ()
    {
      using (MixinConfiguration.ScopedExtend (Assembly.GetExecutingAssembly ()))
      {
        BT1Mixin1 m1 = new BT1Mixin1 ();
        BaseType1 bt1 = ObjectFactory.CreateWithMixinInstances<BaseType1> (m1).With ();

        Assert.IsNotNull (Mixin.Get<BT1Mixin1> (bt1));
        Assert.AreSame (m1, Mixin.Get<BT1Mixin1> (bt1));
        Assert.IsNotNull (Mixin.Get<BT1Mixin2> (bt1));
        Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> (bt1));
      }
    }

    [Test]
    public void MixedObjectsWithMixinInstancesCanBeCreatedFromType ()
    {
      using (MixinConfiguration.ScopedExtend (Assembly.GetExecutingAssembly ()))
      {
        BT1Mixin1 m1 = new BT1Mixin1 ();
        BaseType1 bt1 = (BaseType1) ObjectFactory.CreateWithMixinInstances (typeof (BaseType1), m1).With ();

        Assert.IsNotNull (Mixin.Get<BT1Mixin1> (bt1));
        Assert.AreSame (m1, Mixin.Get<BT1Mixin1> (bt1));
        Assert.IsNotNull (Mixin.Get<BT1Mixin2> (bt1));
        Assert.AreNotSame (m1, Mixin.Get<BT1Mixin2> (bt1));
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type .* is not valid in the current configuration.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnWrongMixinInstances ()
    {
      using (MixinConfiguration.ScopedExtend (Assembly.GetExecutingAssembly ()))
      {
        BT2Mixin1 m1 = new BT2Mixin1 ();
        ObjectFactory.CreateWithMixinInstances<BaseType3> (m1).With ();
      }
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The supplied mixin of type .* is not valid in the current configuration.",
        MatchType = MessageMatch.Regex)]
    public void ThrowsOnWrongMixinInstancesWithType ()
    {
      using (MixinConfiguration.ScopedExtend (Assembly.GetExecutingAssembly ()))
      {
        BT2Mixin1 m1 = new BT2Mixin1 ();
        ObjectFactory.CreateWithMixinInstances (typeof (BaseType3), m1).With ();
      }
    }

    [Test]
    public void MixinsAreInitializedWithTarget ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin2)))
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        BT3Mixin2 mixin = Mixin.Get<BT3Mixin2> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
      }
    }

    [Test]
    public void MixinsAreInitializedWithBase ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin1)))
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
        Assert.IsNotNull (mixin.Base);
        Assert.AreNotSame (bt3, mixin.Base);
      }
    }

    [Test]
    public void MixinsAreInitializedWithConfiguration ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin1)))
      {
        BaseType3 bt3 = ObjectFactory.Create<BaseType3> ().With ();
        BT3Mixin1 mixin = Mixin.Get<BT3Mixin1> (bt3);
        Assert.IsNotNull (mixin);
        Assert.AreSame (bt3, mixin.This);
        Assert.IsNotNull (mixin.Base);
        Assert.AreNotSame (bt3, mixin.Base);
        Assert.AreSame (((IMixinTarget) bt3).Configuration.Mixins[typeof (BT3Mixin1)], mixin.Configuration);
      }
    }

    [Test]
    public void CompleteFaceInterfacesAddedByMixins ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (BaseType3), typeof (BT3Mixin7Face), typeof (BT3Mixin4)))
      {
        ICBaseType3BT3Mixin4 complete = ObjectFactory.Create<BaseType3>().With() as ICBaseType3BT3Mixin4;

        Assert.IsNotNull (complete);
        Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType33) complete).IfcMethod());
        Assert.AreEqual ("BaseType3.IfcMethod", ((IBaseType34) complete).IfcMethod());
        Assert.AreEqual ("BaseType3.IfcMethod2", complete.IfcMethod2());
        Assert.AreEqual ("BaseType3.IfcMethod-BT3Mixin4.Foo", Mixin.Get<BT3Mixin7Face> (complete).InvokeThisMethods());
      }
    }

    [Test]
    public void CompleteFaceInterfacesAddedExplicitly ()
    {
      object complete = ObjectFactory.Create<BaseType6> ().With ();

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgument ()
    {
      ICBT6Mixin1 complete = ObjectFactory.Create<ICBT6Mixin1> ().With ();

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    public interface IEmptyInterface { }

    [Test]
    public void CompleteFaceInterfaceAddedImperativelyAsTypeArgument ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        MixinConfiguration.ActiveContext.GetOrAddClassContext (typeof (BaseType6)).AddCompleteInterface (typeof (IEmptyInterface));
        MixinConfiguration.ActiveContext.RegisterInterface (typeof (IEmptyInterface), typeof (BaseType6));

        IEmptyInterface complete = ObjectFactory.Create<IEmptyInterface> ().With ();

        Assert.IsNotNull (complete);
        Assert.IsTrue (complete is BaseType6);
        Assert.IsTrue (complete is IEmptyInterface);
      }
    }

    [Test]
    public void CompleteFaceInterfaceAsTypeArgumentWithMixins ()
    {
      ICBT6Mixin1 complete = ObjectFactory.CreateWithMixinInstances<ICBT6Mixin1> (new BT6Mixin1 ()).With ();

      Assert.IsNotNull (complete);
      Assert.IsTrue (complete is BaseType6);
      Assert.IsTrue (complete is ICBT6Mixin1);
      Assert.IsTrue (complete is ICBT6Mixin2);
      Assert.IsTrue (complete is ICBT6Mixin3);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not been registered",
        MatchType = MessageMatch.Contains)]
    public void InterfaceAsTypeArgumentWithoutCompleteness ()
    {
      ObjectFactory.Create<IBaseType2> ().With ();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "not been registered",
        MatchType = MessageMatch.Regex)]
    public void InterfaceAsTypeArgumentWithoutCompletenessWithMixins ()
    {
      ObjectFactory.CreateWithMixinInstances<IBaseType2> ().With ();
    }
  }
}
