using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mixins.Validation;
using NUnit.Framework;
using Mixins.Context;
using Mixins.UnitTests.SampleTypes;
using Rubicon.Development.UnitTesting;
using Mixins.Definitions;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ClassContextTests
  {
    [Test]
    public void NewContextHasNoDefinitions ()
    {
      ApplicationContext context = new ApplicationContext ();
      Assert.IsFalse (context.ContainsClassContext (typeof (BaseType1)));
      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.AreEqual (0, classContexts.Count);
    }

    [Test]
    public void BuildFromTestAssembly ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());
      CheckContext(context);
    }

    [Test]
    public void BuildFromTestAssemblies ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (null, AppDomain.CurrentDomain.GetAssemblies ());
      CheckContext (context);
    }

    private static void CheckContext(ApplicationContext context)
    {
      Assert.IsTrue (context.ContainsClassContext (typeof (BaseType1)));
      
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
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());
      context.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void MultipleMixinTypeOccurrencesOnSameTargetTypeAreIgnored ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());
      ApplicationContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly (), context);

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      Assert.AreEqual (2, classContext.MixinCount);

      Assert.IsTrue (classContext.ContainsMixin(typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.ContainsMixin(typeof (BT1Mixin2)));
     
    }

    [Test]
    public void MixinsOnInterface ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());
      context.GetOrAddClassContext (typeof (IBaseType2)).AddMixin (typeof (BT2Mixin1));

      ClassContext classContext = context.GetClassContext (typeof (IBaseType2));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT2Mixin1)));
    }

    [Test]
    public void MixinAttributeOnTargetClass ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.GetClassContext (typeof (BaseType3));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT3Mixin5)));
    }

    [Test]
    public void MixinAttributeOnMixinClass ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin1)));
    }

    [Test]
    public void ClassContextHasValueEquality ()
    {
      ClassContext cc1 = new ClassContext (typeof (BaseType1));
      cc1.AddMixin (typeof (BT1Mixin1));
      cc1.AddCompleteInterface (typeof (IBT5MixinC1));

      ClassContext cc2 = new ClassContext (typeof (BaseType1));
      cc2.AddMixin (typeof (BT1Mixin1));
      cc2.AddCompleteInterface (typeof (IBT5MixinC1));

      Assert.AreEqual (cc1, cc2);
      Assert.AreEqual (cc1.GetHashCode(), cc2.GetHashCode());

      ClassContext cc3 = new ClassContext (typeof (BaseType2));
      cc3.AddMixin (typeof (BT1Mixin1));
      cc3.AddCompleteInterface (typeof (IBT5MixinC1));

      Assert.AreNotEqual (cc1, cc3);

      ClassContext cc4 = new ClassContext (typeof (BaseType2));
      cc4.AddMixin (typeof (BT1Mixin1));
      cc4.AddCompleteInterface (typeof (IBT5MixinC1));

      Assert.AreEqual (cc4, cc3);
      Assert.AreEqual (cc4.GetHashCode (), cc3.GetHashCode ());

      ClassContext cc5 = new ClassContext (typeof (BaseType2));
      cc5.AddMixin (typeof (BT1Mixin2));
      cc5.AddCompleteInterface (typeof (IBT5MixinC1));

      Assert.AreNotEqual (cc4, cc5);

      ClassContext cc6 = new ClassContext (typeof (BaseType2));
      cc5.AddMixin (typeof (BT1Mixin1));
      cc5.AddCompleteInterface (typeof (IBT5MixinC2));

      Assert.AreNotEqual (cc4, cc5);
    }

    [Test]
    public void ClassContextIsSerializable()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixin (typeof (BT1Mixin1));
      cc.AddCompleteInterface (typeof (IBT5MixinC1));

      ClassContext cc2 = Serializer.SerializeAndDeserialize (cc);
      Assert.AreNotSame (cc2, cc);
      Assert.AreEqual (cc2, cc);
    }

    [Test]
    public void ClassContextFrozen()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc.IsFrozen);
      cc.Freeze();
      Assert.IsTrue (cc.IsFrozen);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "is frozen", MatchType = MessageMatch.Contains)]
    public void ThrowsOnAddWMixinhenFrozen()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.Freeze();
      Assert.IsTrue (cc.IsFrozen);
      cc.AddMixin (typeof (BT1Mixin1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "is frozen", MatchType = MessageMatch.Contains)]
    public void ThrowsOnRemoveWMixinWhenFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.Freeze();
      Assert.IsTrue (cc.IsFrozen);
      cc.RemoveMixin (typeof (BT1Mixin1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "is frozen", MatchType = MessageMatch.Contains)]
    public void ThrowsOnAddCompleteInterfaceWhenFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);
      cc.AddCompleteInterface (typeof (IBT5MixinC1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "is frozen", MatchType = MessageMatch.Contains)]
    public void ThrowsOnRemoveCompleteInterfaceWhenFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);
      cc.RemoveCompleteInterface (typeof (IBT5MixinC1));
    }


    [Test]
    public void NonchangingMethodsAndFreezeCanBeExecutedWhenFrozen()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      int hc = cc.GetHashCode();

      cc.Freeze();
      Assert.IsTrue (cc.IsFrozen);
      cc.Freeze();
      Assert.IsTrue (cc.IsFrozen);
      Assert.IsFalse (cc.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsFalse (cc.Equals (null));
      Assert.IsTrue (cc.Equals (cc));
      Assert.AreEqual (hc, cc.GetHashCode());
      Assert.AreEqual (0, cc.MixinCount);
      Assert.IsNotNull (cc.Mixins);
      Assert.AreEqual (typeof (BaseType1), cc.Type);
    }

    [Test]
    public void FrozenContextCanBeClonedUnfrozen()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixin (typeof (BT1Mixin1));
      cc.AddMixin (typeof (BT1Mixin2));
      cc.AddCompleteInterface (typeof (IBT5MixinC1));
      cc.AddCompleteInterface (typeof (IBT5MixinC2));
      cc.Freeze();
      Assert.IsTrue (cc.IsFrozen);

      ClassContext cc2 = cc.Clone();
      Assert.IsNotNull (cc2);
      Assert.AreNotSame (cc, cc2);
      Assert.IsFalse (cc2.IsFrozen);

      Assert.IsTrue (cc2.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (cc2.ContainsMixin (typeof (BT1Mixin2)));
      Assert.IsTrue (cc2.ContainsCompleteInterface (typeof (IBT5MixinC1)));
      Assert.IsTrue (cc2.ContainsCompleteInterface (typeof (IBT5MixinC2)));

      Assert.AreEqual (cc, cc2);
      Assert.AreEqual (cc.GetHashCode(), cc2.GetHashCode());

      Assert.AreEqual (2, cc.MixinCount);
      Assert.IsNotNull (cc2.Mixins);

      List<Type> mixinTypes = new List<Type> (cc2.Mixins);
      Assert.AreEqual (2, mixinTypes.Count);
      Assert.AreEqual (typeof (BT1Mixin1), mixinTypes[0]);
      Assert.AreEqual (typeof (BT1Mixin2), mixinTypes[1]);

      Assert.AreEqual (2, cc.CompleteInterfaceCount);
      Assert.IsNotNull (cc2.CompleteInterfaces);

      List<Type> interfaceTypes = new List<Type> (cc2.CompleteInterfaces);
      Assert.AreEqual (2, interfaceTypes.Count);
      Assert.AreEqual (typeof (IBT5MixinC1), interfaceTypes[0]);
      Assert.AreEqual (typeof (IBT5MixinC2), interfaceTypes[1]);

      Assert.AreEqual (typeof (BaseType1), cc.Type);
    }

    [Test]
    public void AdaptingClonedContext()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixin (typeof (BT1Mixin1));
      cc.AddMixin (typeof (BT1Mixin2));
      Assert.IsFalse (cc.ContainsMixin (typeof (BT3Mixin1)));
      cc.AddCompleteInterface (typeof (IBT5MixinC1));
      Assert.IsFalse (cc.ContainsCompleteInterface (typeof (IBT5MixinC2)));

      ClassContext cc2 = cc.Clone();

      Assert.AreEqual (2, cc2.MixinCount);
      Assert.IsFalse (cc2.RemoveMixin (typeof (BT2Mixin1)));
      Assert.AreEqual (2, cc2.MixinCount);
      Assert.IsTrue (cc2.RemoveMixin (typeof (BT1Mixin2)));
      Assert.AreEqual (1, cc2.MixinCount);
      Assert.IsFalse (cc2.RemoveMixin (typeof (BT1Mixin2)));
      Assert.AreEqual (1, cc2.MixinCount);
      cc2.AddMixin (typeof (BT3Mixin1));
      Assert.AreEqual (2, cc2.MixinCount);

      Assert.IsTrue (cc2.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsFalse (cc2.ContainsMixin (typeof (BT1Mixin2)));
      Assert.IsFalse (cc2.ContainsMixin (typeof (BT2Mixin1)));
      Assert.IsTrue (cc2.ContainsMixin (typeof (BT3Mixin1)));

      Assert.IsFalse (cc.ContainsMixin (typeof (BT3Mixin1)));

      cc2.RemoveCompleteInterface (typeof (IBT5MixinC2));
      cc2.RemoveCompleteInterface (typeof (IBT5MixinC1));
      cc2.AddCompleteInterface (typeof (IBT5MixinC1));
      cc2.AddCompleteInterface (typeof (IBT5MixinC2));

      Assert.IsTrue (cc2.ContainsCompleteInterface (typeof (IBT5MixinC1)));
      Assert.IsTrue (cc2.ContainsCompleteInterface (typeof (IBT5MixinC2)));

      Assert.IsTrue (cc.ContainsCompleteInterface (typeof (IBT5MixinC1)));
      Assert.IsFalse (cc.ContainsCompleteInterface (typeof (IBT5MixinC2)));
    }

    [Test]
    public void CannotCastMixinsToICollection()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsTrue (cc.Mixins is IEnumerable<Type>);
      Assert.IsFalse (cc.Mixins is List<Type>);
      Assert.IsFalse (cc.Mixins is IList<Type>);
      Assert.IsFalse (cc.Mixins is ICollection<Type>);
      Assert.IsFalse (cc.Mixins is ICollection);
      Assert.IsFalse (cc.Mixins is IList);
    }

    [Test]
    public void ClassContextWithMixinParameters()
    {
      ClassContext context = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1), typeof (BT1Mixin2));
      Assert.AreEqual (2, context.MixinCount);
      Assert.IsTrue (context.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (context.ContainsMixin (typeof (BT1Mixin2)));
      Assert.IsFalse (context.ContainsMixin (typeof (BT2Mixin1)));
    }

    [Test]
    public void CompleteInterfaces()
    {
      ClassContext context = new ClassContext (typeof (BaseType5));
      Assert.AreEqual (0, context.CompleteInterfaceCount);
      context.AddCompleteInterface (typeof (IBT5MixinC1));
      Assert.AreEqual (1, context.CompleteInterfaceCount);
      Assert.Contains (typeof (IBT5MixinC1), new List<Type> (context.CompleteInterfaces));
      Assert.IsTrue (context.ContainsCompleteInterface (typeof (IBT5MixinC1)));
      Assert.IsTrue (context.RemoveCompleteInterface (typeof (IBT5MixinC1)));
      Assert.IsFalse (context.ContainsCompleteInterface (typeof (IBT5MixinC1)));
      Assert.AreEqual (0, context.CompleteInterfaceCount);
      Assert.IsFalse (context.RemoveCompleteInterface (typeof (IBT5MixinC1)));
      Assert.AreEqual (0, context.CompleteInterfaceCount);
    }

    [Test]
    public void DuplicateCompleteInterfacesAreIgnored ()
    {
      ClassContext context = new ClassContext (typeof (BaseType5));
      Assert.AreEqual (0, context.CompleteInterfaceCount);
      context.AddCompleteInterface (typeof (IBT5MixinC1));
      Assert.AreEqual (1, context.CompleteInterfaceCount);
      Assert.Contains (typeof (IBT5MixinC1), new List<Type> (context.CompleteInterfaces));
      context.AddCompleteInterface (typeof (IBT5MixinC1));
      Assert.AreEqual (1, context.CompleteInterfaceCount);
      Assert.Contains (typeof (IBT5MixinC1), new List<Type> (context.CompleteInterfaces));
    }

    [Test]
    public void CannotCastCompleteInterfacesToICollection()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsTrue (cc.CompleteInterfaces is IEnumerable<Type>);
      Assert.IsFalse (cc.CompleteInterfaces is List<Type>);
      Assert.IsFalse (cc.CompleteInterfaces is IList<Type>);
      Assert.IsFalse (cc.CompleteInterfaces is ICollection<Type>);
      Assert.IsFalse (cc.CompleteInterfaces is ICollection);
      Assert.IsFalse (cc.CompleteInterfaces is IList);
    }

    [Test]
    public void CompleteInterfaceConfiguredViaAttribute ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.GetClassContext (typeof (BaseType6));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsCompleteInterface (typeof (ICBT6Mixin1)));
      Assert.IsTrue (classContext.ContainsCompleteInterface (typeof (ICBT6Mixin2)));
      Assert.IsTrue (classContext.ContainsCompleteInterface (typeof (ICBT6Mixin3)));
    }
  }
}
