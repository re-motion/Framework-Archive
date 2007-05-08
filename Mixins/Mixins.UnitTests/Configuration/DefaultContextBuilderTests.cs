using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Mixins.Context;
using Mixins.UnitTests.SampleTypes;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class DefaultContextBuilderTests
  {
    [Test]
    public void NewContextHasNoDefinitions ()
    {
      ApplicationContext context = new ApplicationContext ();
      Assert.IsFalse (context.HasClassContext (typeof (BaseType1)));
      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.AreEqual (0, classContexts.Count);
    }

    [Test]
    public void BuildFromTestAssembly ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      CheckContext(context);
    }

    [Test]
    public void BuildFromTestAssemblies ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssemblies (AppDomain.CurrentDomain.GetAssemblies ());
      CheckContext (context);
    }

    private static void CheckContext(ApplicationContext context)
    {
      Assert.IsTrue (context.HasClassContext (typeof (BaseType1)));
      
      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.IsTrue (classContexts.Count > 0);

      ClassContext contextForBaseType1 = context.GetClassContext (typeof (BaseType1));
      List<MixinContext> mixinContexts = new List<MixinContext> (contextForBaseType1.MixinContexts);
      Assert.AreEqual (2, mixinContexts.Count);

      MixinContext def1 =
          mixinContexts.Find (delegate (MixinContext mixinContext) { return mixinContext.MixinType == typeof (BT1Mixin1); });
      Assert.IsNotNull (def1);

      Assert.AreEqual (typeof (BT1Mixin1), def1.MixinType);
      Assert.AreEqual (typeof (BaseType1), def1.TargetType);
      Assert.AreEqual (typeof (BaseType1), def1.TargetType);

      MixinContext def2 =
          mixinContexts.Find (delegate (MixinContext mixinContext) { return mixinContext.MixinType == typeof (BT1Mixin2); });
      Assert.IsNotNull (def2);

      Assert.AreEqual (typeof (BT1Mixin2), def2.MixinType);
      Assert.AreEqual (typeof (BaseType1), def2.TargetType);
      Assert.AreEqual (typeof (BaseType1), def2.TargetType);
    }

    [Test][ExpectedException (typeof (InvalidOperationException))]
    public void ThrowsOnDuplicateContexts ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      context.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void MultipleMixinTypeOccurrencesOnSameTargetType ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      DefaultContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly (), context);

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      List<MixinContext> mixinContexts = new List<MixinContext> (classContext.MixinContexts);
      Assert.AreEqual (4, mixinContexts.Count);

      List<MixinContext> defs1 =
          mixinContexts.FindAll (delegate (MixinContext mixinContext) { return mixinContext.MixinType == typeof (BT1Mixin1); });
      Assert.AreEqual (2, defs1.Count);

      List<MixinContext> defs2 =
          mixinContexts.FindAll (delegate (MixinContext mixinContext) { return mixinContext.MixinType == typeof (BT1Mixin2); });
      Assert.AreEqual (2, defs2.Count);
     
    }

    [Test]
    public void MixinsOnInterface ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      context.GetOrAddClassContext (typeof (IBaseType2)).AddMixinContext (new MixinContext (typeof (IBaseType2), typeof (BT2Mixin1)));

      ClassContext classContext = context.GetClassContext (typeof (IBaseType2));
      Assert.IsNotNull (classContext);

      List<MixinContext> mixinContexts = new List<MixinContext> (classContext.MixinContexts);
      Assert.AreEqual (1, mixinContexts.Count);

      MixinContext definition =
          new List<MixinContext> (classContext.MixinContexts).Find (
              delegate (MixinContext mixinContext) { return mixinContext.MixinType == typeof (BT2Mixin1); });

      Assert.IsNotNull (definition);
    }

    [Test]
    public void AttributeOnTargetClass ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly(Assembly.GetExecutingAssembly());

      ClassContext classContext = context.GetClassContext (typeof (BaseType3));
      Assert.IsNotNull (classContext);

      MixinContext definition =
          new List<MixinContext> (classContext.MixinContexts).Find (
              delegate (MixinContext mixinContext) { return mixinContext.MixinType == typeof (BT3Mixin5); });
      Assert.IsNotNull (definition);
    }

    [Test]
    [ExpectedException(typeof (ArgumentException), ExpectedMessage = "Cannot add mixin definition for different type Mixins.UnitTests.SampleTypes."
        + "BaseType3 to context of class Mixins.UnitTests.SampleTypes.BaseType1.\r\nParameter name: mixinContext")]
    public void ThrowsOnMixinContextForDifferentClass ()
    {
      MixinContext mc = new MixinContext (typeof (BaseType3), typeof(BT3Mixin1));
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixinContext (mc);
    }
  }
}
