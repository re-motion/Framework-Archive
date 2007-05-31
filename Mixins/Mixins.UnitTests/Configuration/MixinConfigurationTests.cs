using System;
using NUnit.Framework;
using Mixins.Context;
using System.Threading;
using Mixins.UnitTests.SampleTypes;
using System.Reflection;

namespace Mixins.UnitTests.Configurationw
{
  [TestFixture]
  public class MixinConfigurationTests
  {
    [SetUp]
    public void SetUp()
    {
      MixinConfiguration.SetActiveContext (null);
    }

    [TearDown]
    public void TearDown ()
    {
      MixinConfiguration.SetActiveContext (null);
    }

    [Test]
    public void InitialConfiguration()
    {
      Assert.IsFalse (MixinConfiguration.HasActiveContext);
      ApplicationContext context = MixinConfiguration.ActiveContext;
      Assert.IsNotNull (context);
      Assert.IsTrue (MixinConfiguration.HasActiveContext);
    }

    [Test (Description = "Checks whether this test fixture correctly resets the mixin configuration before running each tests.")]
    public void InitialConfiguration2 ()
    {
      Assert.IsFalse (MixinConfiguration.HasActiveContext);
      ApplicationContext context = MixinConfiguration.ActiveContext;
      Assert.IsNotNull (context);
      Assert.IsTrue (MixinConfiguration.HasActiveContext);
    }

    [Test]
    public void DefaultContext()
    {
      ApplicationContext context = MixinConfiguration.ActiveContext;
      Assert.AreEqual (0, context.ClassContextCount);
    }

    [Test]
    public void SetContext()
    {
      ApplicationContext context = MixinConfiguration.ActiveContext;
      Assert.AreSame (context, MixinConfiguration.ActiveContext);
      ApplicationContext newContext = new ApplicationContext();
      MixinConfiguration.SetActiveContext (newContext);
      Assert.AreNotSame (context, MixinConfiguration.ActiveContext);
      Assert.AreSame (newContext, MixinConfiguration.ActiveContext);
    }

    [Test]
    public void ActiveContextIsThreadSpecific()
    {
      ApplicationContext context = MixinConfiguration.ActiveContext;
      ApplicationContext newContext = new ApplicationContext ();
      MixinConfiguration.SetActiveContext (newContext);

      ApplicationContext context2 = null;
      ApplicationContext newContext2 = new ApplicationContext();

      Thread.MemoryBarrier();

      Thread secondThread = new Thread ((ThreadStart) delegate
      {
        context2 = MixinConfiguration.ActiveContext;
        MixinConfiguration.SetActiveContext (newContext2);
        Assert.AreSame (newContext2, MixinConfiguration.ActiveContext);
      });
      secondThread.Start();
      secondThread.Join();

      Thread.MemoryBarrier();

      Assert.IsNotNull (context2);
      Assert.AreNotSame (context, context2);
      Assert.AreNotSame (newContext, context2);
      Assert.AreNotSame (newContext2, MixinConfiguration.ActiveContext);
      Assert.AreSame (newContext, MixinConfiguration.ActiveContext);
    }

    [Test]
    public void ScopedMixinConfiguration()
    {
      ApplicationContext context = new ApplicationContext ();
      ApplicationContext context2 = new ApplicationContext ();
      Assert.IsFalse (MixinConfiguration.HasActiveContext);
      using (new MixinConfiguration (context))
      {
        Assert.IsTrue (MixinConfiguration.HasActiveContext);
        Assert.AreSame (context, MixinConfiguration.ActiveContext);
        using (new MixinConfiguration (context2))
        {
          Assert.AreNotSame (context, MixinConfiguration.ActiveContext);
          Assert.AreSame (context2, MixinConfiguration.ActiveContext);
        }
        Assert.IsTrue (MixinConfiguration.HasActiveContext);
        Assert.AreNotSame (context2, MixinConfiguration.ActiveContext);
        Assert.AreSame (context, MixinConfiguration.ActiveContext);
      }
      Assert.IsFalse (MixinConfiguration.HasActiveContext);
    }

    [Test]
    public void MixinConfigurationOverloads()
    {
      Assert.IsFalse (MixinConfiguration.HasActiveContext);
      using (new MixinConfiguration (typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin2)))
      {
        Assert.IsTrue (MixinConfiguration.HasActiveContext);
        Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
        Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
        Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin2)));
        Assert.IsFalse (MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT2Mixin1)));
        Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));

        using (new MixinConfiguration (new ClassContext(typeof (BaseType3), typeof (BT1Mixin1)), new ClassContext (typeof (object))))
        {
          Assert.IsTrue (MixinConfiguration.HasActiveContext);
          Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
          Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType3)));
          Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType3)).ContainsMixin (typeof (BT1Mixin1)));
          Assert.IsFalse (MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType3)).ContainsMixin (typeof (BT3Mixin1)));
          Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (object)));

          using (new MixinConfiguration (Assembly.GetExecutingAssembly()))
          {
            Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (object)));
            Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType1)));
            Assert.IsFalse (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType2)));
            Assert.IsTrue (MixinConfiguration.ActiveContext.ContainsClassContext (typeof (BaseType3)));
            Assert.IsFalse (MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType3)).ContainsMixin (typeof (BT1Mixin1)));
            Assert.IsTrue (MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType3)).ContainsMixin (typeof (BT3Mixin1)));
          }
        }
      }
      Assert.IsFalse (MixinConfiguration.HasActiveContext);
    }
  }
}
