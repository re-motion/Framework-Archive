using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Development.UnitTesting;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ClassContextTests
{
  [TestFixture]
  public class ClassContextGeneralTests
  {
    [Test]
    public void NewMixinConfigurationDoesNotKnowAnyClasses ()
    {
      MixinConfiguration context = new MixinConfiguration ();
      Assert.AreEqual (0, context.ClassContextCount);
      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.AreEqual (0, classContexts.Count);
      Assert.IsFalse (context.ContainsClassContext (typeof (BaseType1)));
    }

    [Test]
    public void GetOrAddMixinContext ()
    {
      ClassContext classContext = new ClassContext (typeof (BaseType7));

      Assert.IsFalse (classContext.ContainsMixin (typeof (BT7Mixin1)));
      MixinContext mixinContext = classContext.GetOrAddMixinContext (typeof (BT7Mixin1));
      Assert.IsNotNull (mixinContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (BT7Mixin1)));
      Assert.AreSame (mixinContext, classContext.GetOrAddMixinContext (typeof (BT7Mixin1)));

      MixinContext mixinContext2 = classContext.AddMixin (typeof (BT7Mixin2));
      Assert.AreSame (mixinContext2, classContext.GetOrAddMixinContext (typeof (BT7Mixin2)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "Object was tried to be added twice", MatchType = MessageMatch.Contains)]
    public void ConstructorThrowsOnDuplicateMixinContexts ()
    {
      new ClassContext (typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void MixinsOnInterface ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      context.GetOrAddClassContext (typeof (IBaseType2)).AddMixin (typeof (BT2Mixin1));

      ClassContext classContext = context.GetClassContext (typeof (IBaseType2));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT2Mixin1)));
    }

    [Test]
    public void CannotCastMixinsToICollection()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsTrue (cc.Mixins is IEnumerable<MixinContext>);
      Assert.IsFalse (cc.Mixins is List<MixinContext>);
      Assert.IsFalse (cc.Mixins is IList<MixinContext>);
      Assert.IsFalse (cc.Mixins is ICollection<MixinContext>);
      Assert.IsFalse (cc.Mixins is ICollection);
      Assert.IsFalse (cc.Mixins is IList);
    }

    [Test]
    public void ConstructorWithMixinParameters()
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
    [ExpectedException (typeof (InvalidOperationException))]
    public void AddMixinThrowsOnDuplicateMixinContextsInAdd ()
    {
      ClassContext context = new ClassContext (typeof (string));
      context.AddMixin (typeof (object));
      context.AddMixin (typeof (object));
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
    public void ClassContextHasValueEquality ()
    {
      ClassContext cc1 = new ClassContext (typeof (BaseType1));
      cc1.AddMixin (typeof (BT1Mixin1));
      cc1.AddCompleteInterface (typeof (IBT5MixinC1));

      ClassContext cc2 = new ClassContext (typeof (BaseType1));
      cc2.AddMixin (typeof (BT1Mixin1));
      cc2.AddCompleteInterface (typeof (IBT5MixinC1));

      Assert.AreEqual (cc1, cc2);
      Assert.AreEqual (cc1.GetHashCode (), cc2.GetHashCode ());

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

      ClassContext cc7 = new ClassContext (typeof (BaseType1));
      cc7.AddMixin (typeof (BT1Mixin1));

      ClassContext cc8 = cc7.Clone ();
      cc8.RemoveMixin (typeof (BT1Mixin1));
      cc8.AddMixinContext (new MixinContext (typeof (BT1Mixin1), new Type[] {typeof (IBaseType2)}));

      Assert.AreEqual (cc7, cc7);
      Assert.AreEqual (cc7, cc7.Clone ());
      Assert.AreNotEqual (cc7, cc8);
    }

    [Test]
    public void ClassContextIsSerializable ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddCompleteInterface (typeof (IBT5MixinC1));
      cc.AddMixinContext (new MixinContext (typeof (BT1Mixin1), new Type[] {typeof (IBaseType2)}));

      ClassContext cc2 = Serializer.SerializeAndDeserialize (cc);
      Assert.AreNotSame (cc2, cc);
      Assert.AreEqual (cc2, cc);
      Assert.IsTrue (cc2.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (cc2.GetOrAddMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType2)));
    }

    [Test]
    public void ClassContextFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      Assert.IsFalse (cc.IsFrozen);
      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "is frozen", MatchType = MessageMatch.Contains)]
    public void ThrowsOnAddMixinWhenFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);
      cc.AddMixin (typeof (BT1Mixin1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "is frozen", MatchType = MessageMatch.Contains)]
    public void ThrowsOnGetOrAddMixinContextWhenFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);
      cc.GetOrAddMixinContext (typeof (BT1Mixin1));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "is frozen", MatchType = MessageMatch.Contains)]
    public void ThrowsOnRemoveMixinWhenFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.Freeze ();
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
    public void NonchangingMethodsAndFreezeCanBeExecutedWhenFrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixinContext (new MixinContext (typeof (BT1Mixin2), new Type[] {typeof (IBaseType2)}));
      int hc = cc.GetHashCode ();

      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);
      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);
      Assert.IsFalse (cc.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsFalse (cc.Equals (null));
      Assert.IsTrue (cc.Equals (cc));
      Assert.AreEqual (hc, cc.GetHashCode ());
      Assert.AreEqual (1, cc.MixinCount);
      Assert.IsNotNull (cc.Mixins);
      Assert.AreEqual (typeof (BaseType1), cc.Type);
      Assert.IsNotNull (cc.GetOrAddMixinContext (typeof (BT1Mixin2)));
      Assert.AreEqual (1, cc.GetOrAddMixinContext (typeof (BT1Mixin2)).ExplicitDependencyCount);
      Dev.Null = cc.GetOrAddMixinContext (typeof (BT1Mixin2)).ExplicitDependencies;
    }

    [Test]
    public void FrozenContextCanBeClonedUnfrozen ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixinContext (new MixinContext (typeof (BT1Mixin1), new Type[] {typeof (IBaseType2)}));
      cc.AddMixin (typeof (BT1Mixin2));
      cc.AddCompleteInterface (typeof (IBT5MixinC1));
      cc.AddCompleteInterface (typeof (IBT5MixinC2));
      cc.Freeze ();
      Assert.IsTrue (cc.IsFrozen);

      ClassContext cc2 = cc.Clone ();
      Assert.IsNotNull (cc2);
      Assert.AreNotSame (cc, cc2);
      Assert.IsFalse (cc2.IsFrozen);

      Assert.IsTrue (cc2.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (cc2.GetOrAddMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType2)));
      Assert.IsTrue (cc2.ContainsMixin (typeof (BT1Mixin2)));
      Assert.IsFalse (cc2.GetOrAddMixinContext (typeof (BT1Mixin2)).ContainsExplicitDependency (typeof (IBaseType2)));
      Assert.IsTrue (cc2.ContainsCompleteInterface (typeof (IBT5MixinC1)));
      Assert.IsTrue (cc2.ContainsCompleteInterface (typeof (IBT5MixinC2)));

      Assert.AreEqual (cc, cc2);
      Assert.AreEqual (cc.GetHashCode (), cc2.GetHashCode ());

      Assert.AreEqual (2, cc.MixinCount);
      Assert.IsNotNull (cc2.Mixins);

      List<MixinContext> mixinContexts = new List<MixinContext> (cc2.Mixins);
      Assert.AreEqual (2, mixinContexts.Count);
      Assert.AreEqual (typeof (BT1Mixin1), mixinContexts[0].MixinType);
      Assert.AreEqual (typeof (BT1Mixin2), mixinContexts[1].MixinType);

      Assert.AreEqual (2, cc.CompleteInterfaceCount);
      Assert.IsNotNull (cc2.CompleteInterfaces);

      List<Type> interfaceTypes = new List<Type> (cc2.CompleteInterfaces);
      Assert.AreEqual (2, interfaceTypes.Count);
      Assert.AreEqual (typeof (IBT5MixinC1), interfaceTypes[0]);
      Assert.AreEqual (typeof (IBT5MixinC2), interfaceTypes[1]);

      Assert.AreEqual (typeof (BaseType1), cc.Type);
    }

    [Test]
    public void AdaptingClonedContext ()
    {
      ClassContext cc = new ClassContext (typeof (BaseType1));
      cc.AddMixin (typeof (BT1Mixin1));
      cc.AddMixin (typeof (BT1Mixin2));
      Assert.IsFalse (cc.ContainsMixin (typeof (BT3Mixin1)));
      cc.AddCompleteInterface (typeof (IBT5MixinC1));
      Assert.IsFalse (cc.ContainsCompleteInterface (typeof (IBT5MixinC2)));

      ClassContext cc2 = cc.Clone ();

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
    public void SpecializeWithTypeArguments ()
    {
      ClassContext original = new ClassContext (typeof (List<>));
      original.AddMixinContext (new MixinContext (typeof (BT1Mixin1), new Type[] {typeof (IBaseType2)}));

      ClassContext specialized = original.SpecializeWithTypeArguments (new Type[] { typeof (int) });
      Assert.IsNotNull (specialized);
      Assert.AreEqual (typeof (List<int>), specialized.Type);
      Assert.IsTrue (specialized.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (specialized.GetOrAddMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType2)));
    }

    [Test]
    public void GenericTypesNotTransparentlyConvertedToTypeDefinitions ()
    {
      ClassContext context = new ClassContext (typeof (List<int>));
      Assert.AreEqual (typeof (List<int>), context.Type);
    }

    [Test]
    public void ContainsAssignableMixin ()
    {
      ClassContext context = new ClassContext (typeof (object));
      context.AddMixin (typeof (IList<int>));

      Assert.IsTrue (context.ContainsMixin (typeof (IList<int>)));
      Assert.IsTrue (context.ContainsAssignableMixin (typeof (IList<int>)));

      Assert.IsFalse (context.ContainsMixin (typeof (ICollection<int>)));
      Assert.IsTrue (context.ContainsAssignableMixin (typeof (ICollection<int>)));

      Assert.IsFalse (context.ContainsMixin (typeof (object)));
      Assert.IsTrue (context.ContainsAssignableMixin (typeof (object)));

      Assert.IsFalse (context.ContainsMixin (typeof (List<int>)));
      Assert.IsFalse (context.ContainsAssignableMixin (typeof (List<int>)));

      Assert.IsFalse (context.ContainsMixin (typeof (IList<>)));
      Assert.IsFalse (context.ContainsAssignableMixin (typeof (List<>)));
    }
  }
}