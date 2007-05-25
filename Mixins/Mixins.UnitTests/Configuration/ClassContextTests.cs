using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Mixins.Context;
using Mixins.UnitTests.SampleTypes;
using Rubicon.Development.UnitTesting;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ClassContextTests
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
      Assert.AreEqual (2, contextForBaseType1.MixinCount);

      Assert.IsTrue (contextForBaseType1.ContainsMixin(typeof (BT1Mixin1)));
      Assert.IsTrue (contextForBaseType1.ContainsMixin(typeof (BT1Mixin2)));
    }

    [Test][ExpectedException (typeof (InvalidOperationException))]
    public void ThrowsOnDuplicateContexts ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      context.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void MultipleMixinTypeOccurrencesOnSameTargetTypeAreIgnored ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      DefaultContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly (), context);

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      Assert.AreEqual (2, classContext.MixinCount);

      Assert.IsTrue (classContext.ContainsMixin(typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.ContainsMixin(typeof (BT1Mixin2)));
     
    }

    [Test]
    public void MixinsOnInterface ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly ());
      context.GetOrAddClassContext (typeof (IBaseType2)).AddMixin (typeof (BT2Mixin1));

      ClassContext classContext = context.GetClassContext (typeof (IBaseType2));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT2Mixin1)));
    }

    [Test]
    public void AttributeOnTargetClass ()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly(Assembly.GetExecutingAssembly());

      ClassContext classContext = context.GetClassContext (typeof (BaseType3));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT3Mixin5)));
    }

    [Test]
    [Ignore ("TODO: Value equality for class contexts")]
    public void ClassContextHasValueEquality ()
    {
      ClassContext cc1 = new ClassContext (typeof (BaseType1));
      cc1.AddMixin (typeof (BT1Mixin1));

      ClassContext cc2 = new ClassContext (typeof (BaseType1));
      cc2.AddMixin (typeof (BT1Mixin1));

      Assert.AreEqual (cc1, cc2);
      Assert.AreEqual (cc1.GetHashCode(), cc2.GetHashCode());

      ClassContext cc3 = new ClassContext (typeof (BaseType2));
      cc3.AddMixin (typeof (BT1Mixin1));

      Assert.AreNotEqual (cc1, cc3);

      ClassContext cc4 = new ClassContext (typeof (BaseType2));
      cc3.AddMixin (typeof (BT1Mixin1));

      Assert.AreEqual (cc4, cc3);
      Assert.AreEqual (cc4.GetHashCode (), cc3.GetHashCode ());

      ClassContext cc5 = new ClassContext (typeof (BaseType2));
      cc3.AddMixin (typeof (BT1Mixin2));

      Assert.AreNotEqual (cc4, cc5);
    }

    [Test]
    [Ignore ("TODO: Value equality for class contexts")]
    public void ClassContextIsSerializable()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixin (typeof (BT1Mixin1));

      ClassContext cc2 = Serializer.SerializeAndDeserialize (cc);
      Assert.AreNotSame (cc2, cc);
      Assert.AreEqual (cc2, cc);
    }
  }
}
