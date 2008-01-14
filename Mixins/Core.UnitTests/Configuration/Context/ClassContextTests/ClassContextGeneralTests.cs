using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Context.FluentBuilders;
using Rubicon.Mixins.UnitTests.SampleTypes;
using Rubicon.Development.UnitTesting;
using NUnit.Framework.SyntaxHelpers;

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
    public void GetMixinContext ()
    {
      ClassContext classContext = new ClassContext (typeof (BaseType7));

      Assert.IsFalse (classContext.ContainsMixin (typeof (BT7Mixin1)));
      MixinContext mixinContext = classContext.GetMixinContext (typeof (BT7Mixin1));
      Assert.IsNull (mixinContext);

      classContext = new ClassContext (typeof (BaseType7), typeof (BT7Mixin1));
      Assert.IsTrue (classContext.ContainsMixin (typeof (BT7Mixin1)));
      mixinContext = classContext.GetMixinContext (typeof (BT7Mixin1));
      Assert.AreSame (mixinContext, classContext.GetMixinContext (typeof (BT7Mixin1)));
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
      MixinConfiguration context = MixinConfiguration.BuildFromActive()
          .ForClass <IBaseType2>().AddMixin<BT2Mixin1>().BuildConfiguration();

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
    public void CompleteInterfaces_Empty()
    {
      ClassContext context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[0]);
      Assert.AreEqual (0, context.CompleteInterfaceCount);
      Assert.IsEmpty (new List<Type> (context.CompleteInterfaces));
      Assert.IsFalse (context.ContainsCompleteInterface (typeof (IBT5MixinC1)));
    }

    [Test]
    public void CompleteInterfaces_NonEmpty ()
    {
      ClassContext context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[] { typeof (IBT5MixinC1) });
      Assert.AreEqual (1, context.CompleteInterfaceCount);
      Assert.Contains (typeof (IBT5MixinC1), new List<Type> (context.CompleteInterfaces));
      Assert.IsTrue (context.ContainsCompleteInterface (typeof (IBT5MixinC1)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The mixin type System.Object was tried to be added twice.\r\n"
        + "Parameter name: mixinTypes")]
    public void ConstructorThrowsOnDuplicateMixinContextsInAdd ()
    {
      new ClassContext (typeof (string), typeof (object), typeof (object));
    }

    [Test]
    public void DuplicateCompleteInterfacesAreIgnored ()
    {
      ClassContext context = new ClassContext (typeof (BaseType5), new MixinContext[0], new Type[] { typeof (IBT5MixinC1), typeof (IBT5MixinC1) });
      Assert.AreEqual (1, context.CompleteInterfaceCount);
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
      ClassContext cc1 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext();

      ClassContext cc2 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext ();

      Assert.AreEqual (cc1, cc2);
      Assert.AreEqual (cc1.GetHashCode (), cc2.GetHashCode ());

      ClassContext cc3 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext();

      Assert.AreNotEqual (cc1, cc3);

      ClassContext cc4 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext();

      Assert.AreEqual (cc4, cc3);
      Assert.AreEqual (cc4.GetHashCode (), cc3.GetHashCode ());

      ClassContext cc5 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin2)).AddCompleteInterface (typeof (IBT5MixinC1))
          .BuildClassContext ();

      Assert.AreNotEqual (cc4, cc5);

      ClassContext cc6 = new ClassContextBuilder (typeof (BaseType2)).AddMixin (typeof (BT1Mixin1)).AddCompleteInterface (typeof (IBT5MixinC2))
          .BuildClassContext();

      Assert.AreNotEqual (cc4, cc6);

      ClassContext cc7 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1))
          .BuildClassContext ();

      ClassContext cc8 = new ClassContextBuilder (typeof (BaseType1)).AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType2))
          .BuildClassContext ();
      
      Assert.AreEqual (cc7, cc7);
      Assert.AreNotEqual (cc7, cc8);
    }

    [Test]
    public void ClassContextIsSerializable ()
    {
      ClassContext cc = new ClassContextBuilder (typeof (BaseType1))
          .AddCompleteInterface (typeof (IBT5MixinC1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType2))
          .BuildClassContext ();

      ClassContext cc2 = Serializer.SerializeAndDeserialize (cc);
      Assert.AreNotSame (cc2, cc);
      Assert.AreEqual (cc2, cc);
      Assert.IsTrue (cc2.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (cc2.GetMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType2)));
    }

    [Test]
    public void SpecializeWithTypeArguments ()
    {
      ClassContext original = new ClassContextBuilder (typeof (List<>)).AddMixin<BT1Mixin1>().WithDependency<IBaseType2>().BuildClassContext();

      ClassContext specialized = original.SpecializeWithTypeArguments (new Type[] { typeof (int) });
      Assert.IsNotNull (specialized);
      Assert.AreEqual (typeof (List<int>), specialized.Type);
      Assert.IsTrue (specialized.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (specialized.GetMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType2)));
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
      ClassContext context = new ClassContext (typeof (object), typeof (IList<int>));

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

    [Test]
    public void CloneForSpecificType ()
    {
      MixinContext[] mixins = new MixinContext[] {
          new MixinContext (typeof (BT1Mixin1), new Type[] { typeof (IBT1Mixin1) }),
          new MixinContext (typeof (BT1Mixin2))
      };
      Type[] interfaces = new Type[] { typeof (ICBT6Mixin1), typeof (ICBT6Mixin2)};
      ClassContext source = new ClassContext (typeof (BaseType1), mixins, interfaces);
      ClassContext clone = source.CloneForSpecificType (typeof (BaseType2));
      Assert.AreNotEqual (source, clone);
      Assert.That (new List<MixinContext> (clone.Mixins), Is.EqualTo (mixins));
      Assert.That (new List<Type> (clone.CompleteInterfaces), Is.EqualTo (interfaces));
      Assert.AreEqual (typeof (BaseType2), clone.Type);
      Assert.AreEqual (typeof (BaseType1), source.Type);
    }
  }
}