using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration.MixinConfigurationTests
{
  [TestFixture]
  public class MixinConfigurationGeneralTests
  {
    [Test]
    public void NewMixinConfigurationDoesNotKnowAnyClasses ()
    {
      MixinConfiguration context = new MixinConfiguration();
      Assert.AreEqual (0, context.ClassContextCount);
      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.AreEqual (0, classContexts.Count);
      Assert.IsFalse (context.ContainsClassContext (typeof (BaseType1)));
    }

    [Test]
    public void BuildFromTestAssembly ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      CheckContext (context);
    }

    [Test]
    public void BuildFromTestAssemblies ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (null, AppDomain.CurrentDomain.GetAssemblies ());
      CheckContext (context);
    }

    private static void CheckContext (MixinConfiguration context)
    {
      Assert.IsTrue (context.ContainsClassContext (typeof (BaseType1)));

      List<ClassContext> classContexts = new List<ClassContext> (context.ClassContexts);
      Assert.IsTrue (classContexts.Count > 0);

      ClassContext contextForBaseType1 = context.GetClassContext (typeof (BaseType1));
      Assert.AreEqual (2, contextForBaseType1.MixinCount);

      Assert.IsTrue (contextForBaseType1.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (contextForBaseType1.ContainsMixin (typeof (BT1Mixin2)));
    }


    [Test]
    public void GeneralFunctionality()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      Assert.AreEqual (0, ac.ClassContextCount);
      Assert.IsFalse (ac.ContainsClassContext (typeof (BaseType1)));
      Assert.IsNull (ac.GetClassContext (typeof (BaseType1)));
      Assert.IsEmpty (new List<ClassContext> (ac.ClassContexts));

      ClassContext newContext1 = new ClassContext (typeof (BaseType1));
      ac.AddClassContext (newContext1);
      Assert.AreEqual (1, ac.ClassContextCount);
      Assert.IsTrue (ac.ContainsClassContext (typeof (BaseType1)));
      Assert.IsNotNull (ac.GetClassContext (typeof (BaseType1)));
      Assert.AreSame (newContext1, ac.GetClassContext (typeof (BaseType1)));
      Assert.Contains (newContext1, new List<ClassContext> (ac.ClassContexts));
      Assert.AreEqual (1, new List<ClassContext> (ac.ClassContexts).Count);
      Assert.AreSame (newContext1, ac.GetOrAddClassContext(typeof (BaseType1)));

      Assert.IsTrue (ac.RemoveClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac.RemoveClassContext (typeof (BaseType1)));

      ClassContext newContext2 = ac.GetOrAddClassContext (typeof (BaseType2));
      Assert.IsNotNull (newContext2);
      Assert.AreNotSame (newContext1, newContext2);

      ClassContext newContext3 = new ClassContext (typeof (BaseType2));
      Assert.AreSame (newContext2, ac.GetClassContext (typeof (BaseType2)));
      ac.AddOrReplaceClassContext (newContext3);
      Assert.AreNotSame (newContext2, ac.GetClassContext (typeof (BaseType2)));
      Assert.AreSame (newContext3, ac.GetClassContext (typeof (BaseType2)));

      ClassContext newContext4 = new ClassContext (typeof (BaseType3));
      Assert.IsFalse (ac.ContainsClassContext (newContext4.Type));
      ac.AddOrReplaceClassContext (newContext4);
      Assert.IsTrue (ac.ContainsClassContext (newContext4.Type));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "already a class context", MatchType = MessageMatch.Contains)]
    public void ThrowsOnDoubleAdd ()
    {
      MixinConfiguration ac = new MixinConfiguration ();
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ThrowsOnDoubleAdd_ViaConstructor ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      context.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void CopyTo ()
    {
      MixinConfiguration parent = new MixinConfiguration();
      parent.GetOrAddClassContext (typeof (BaseType2)).GetOrAddMixinContext (typeof (BT2Mixin1)).AddExplicitDependency (typeof (IBaseType33));
      parent.RegisterInterface (typeof (IBaseType2), typeof (BaseType2));

      MixinConfiguration source = new MixinConfiguration (parent);
      source.GetOrAddClassContext (typeof (BaseType1)).GetOrAddMixinContext (typeof (BT1Mixin1)).AddExplicitDependency (typeof (IBaseType34));
      source.GetOrAddClassContext (typeof (BaseType1)).AddCompleteInterface (typeof (IBaseType33));

      Assert.IsTrue (source.ContainsClassContext (typeof (BaseType2)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType2)).ContainsMixin (typeof (BT2Mixin1)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType2)).GetOrAddMixinContext (typeof (BT2Mixin1)).ContainsExplicitDependency (typeof (IBaseType33)));

      Assert.AreSame (source.GetClassContext (typeof (BaseType2)), source.ResolveInterface (typeof (IBaseType2)));

      Assert.IsTrue (source.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (source.GetOrAddClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (source.GetOrAddClassContext (typeof (BaseType1)).GetOrAddMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType34)));

      MixinConfiguration destination = new MixinConfiguration ();
      destination.AddClassContext (new ClassContext (typeof (BaseType2)));
      destination.AddClassContext (new ClassContext (typeof (BaseType4)));
      destination.RegisterInterface (typeof (IBaseType2), typeof (BaseType4));

      Assert.IsTrue (destination.ContainsClassContext (typeof (BaseType2)));
      Assert.IsTrue (destination.ContainsClassContext (typeof (BaseType4)));
      Assert.IsFalse (destination.ContainsClassContext (typeof (BaseType1)));

      Assert.AreSame (destination.GetClassContext (typeof (BaseType4)), destination.ResolveInterface (typeof (IBaseType2)));
      
      source.CopyTo (destination);

      Assert.IsTrue (destination.ContainsClassContext (typeof (BaseType4)));

      Assert.IsTrue (destination.ContainsClassContext (typeof (BaseType2)));
      Assert.IsTrue (destination.GetClassContext (typeof (BaseType2)).ContainsMixin (typeof (BT2Mixin1)));
      Assert.IsTrue (destination.GetClassContext (typeof (BaseType2)).GetOrAddMixinContext (typeof (BT2Mixin1)).ContainsExplicitDependency (typeof (IBaseType33)));

      Assert.AreSame (destination.GetClassContext (typeof (BaseType2)), destination.ResolveInterface (typeof (IBaseType2)));

      Assert.IsTrue (destination.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (destination.GetOrAddClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (destination.GetOrAddClassContext (typeof (BaseType1)).GetOrAddMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType34)));

      source.GetClassContext (typeof (BaseType2)).RemoveMixin (typeof (BT2Mixin1));
      Assert.IsFalse (source.GetClassContext (typeof (BaseType2)).ContainsMixin (typeof (BT2Mixin1)));
      Assert.IsTrue (destination.GetClassContext (typeof (BaseType2)).ContainsMixin (typeof (BT2Mixin1)));
    }

    [Test]
    public void GetContextNonRecursive ()
    {
      using (MixinConfiguration.BuildFromActive().ForClass<NullTarget> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        ClassContext context = MixinConfiguration.ActiveConfiguration.GetClassContextNonRecursive (typeof (DerivedNullTarget));
        Assert.IsNull (context);
      }
    }

    [Test]
    public void GenericTypesNotTransparentlyConvertedToTypeDefinitions ()
    {
      MixinConfiguration context = new MixinConfiguration ();
      context.AddClassContext (new ClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsFalse (context.ContainsClassContext (typeof (List<>)));

      context.AddClassContext (new ClassContext (typeof (List<string>)));

      Assert.AreEqual (2, context.ClassContextCount);
      context.AddOrReplaceClassContext (new ClassContext (typeof (List<double>)));
      Assert.AreEqual (3, context.ClassContextCount);

      ClassContext classContext1 = context.GetClassContext (typeof (List<int>));
      ClassContext classContext2 = context.GetClassContextNonRecursive (typeof (List<string>));
      Assert.AreNotSame (classContext1, classContext2);

      ClassContext classContext3 = context.GetClassContext (typeof (List<List<int>>));
      Assert.IsNull (classContext3);

      Assert.IsFalse (context.RemoveClassContext (typeof (List<bool>)));
      Assert.AreEqual (3, context.ClassContextCount);
      Assert.IsTrue (context.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (2, context.ClassContextCount);
      Assert.IsFalse (context.RemoveClassContext (typeof (List<int>)));
    }

    [Test]
    public void AddContextForGenericSpecialization ()
    {
      MixinConfiguration context = new MixinConfiguration ();
      context.AddClassContext (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, context.ClassContextCount);
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (context.GetClassContext (typeof (List<>)), context.GetClassContext (typeof (List<int>)));

      context.AddClassContext (new ClassContext (typeof (List<int>)));

      Assert.AreEqual (2, context.ClassContextCount);
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (context.GetClassContext (typeof (List<>)), context.GetClassContext (typeof (List<int>)));
    }

    [Test]
    public void AddOrReplaceContextForGenericSpecialization ()
    {
      MixinConfiguration context = new MixinConfiguration ();
      context.AddClassContext (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, context.ClassContextCount);
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (context.GetClassContext (typeof (List<>)), context.GetClassContext (typeof (List<int>)));

      ClassContext listIntContext = new ClassContext (typeof (List<int>));
      context.AddOrReplaceClassContext (listIntContext);

      Assert.AreEqual (2, context.ClassContextCount);
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (context.GetClassContext (typeof (List<>)), context.GetClassContext (typeof (List<int>)));
      Assert.AreSame (listIntContext, context.GetClassContext (typeof (List<int>)));

      ClassContext newListIntContext = new ClassContext (typeof (List<int>));
      context.AddOrReplaceClassContext (newListIntContext);
      Assert.AreEqual (2, context.ClassContextCount);

      Assert.AreSame (newListIntContext, context.GetClassContext (typeof (List<int>)));
    }

    [Test]
    public void GetContextForGenericTypeDefinitions ()
    {
      MixinConfiguration context = new MixinConfiguration ();
      context.AddClassContext (new ClassContext (typeof (List<>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      ClassContext classContext1 = context.GetClassContext (typeof (List<int>));
      ClassContext classContext2 = context.GetClassContextNonRecursive (typeof (List<>));
      Assert.AreNotSame (classContext1, classContext2);
    }

    [Test]
    public void GetOrAddContextForGenericTypeDefinitions ()
    {
      MixinConfiguration context = new MixinConfiguration ();
      ClassContext genericListContext = new ClassContext (typeof (List<>));
      context.AddClassContext (genericListContext);

      ClassContext listIntContext = context.GetClassContext (typeof (List<int>));
      ClassContext listListContext = context.GetOrAddClassContext (typeof (List<List<int>>));
      Assert.AreNotSame (listIntContext, listListContext);

      ClassContext listListContext2 = context.GetOrAddClassContext (typeof (List<List<int>>));
      Assert.AreSame (listListContext, listListContext2);

      ClassContext genericListContext2 = context.GetOrAddClassContext (typeof (List<>));
      Assert.AreSame (genericListContext, genericListContext2);
    }

    [Test]
    public void RemoveClassContextForGenericTypeDefinitions ()
    {
      MixinConfiguration context = new MixinConfiguration ();
      context.AddClassContext (new ClassContext (typeof (List<>)));
      Assert.IsFalse (context.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (1, context.ClassContextCount);
      
      context.AddClassContext (new ClassContext (typeof (List<int>)));
      Assert.AreEqual (2, context.ClassContextCount);
      
      Assert.IsTrue (context.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (1, context.ClassContextCount);
      Assert.IsFalse (context.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (1, context.ClassContextCount);

      Assert.IsTrue (context.RemoveClassContext (typeof (List<>)));
      Assert.AreEqual (0, context.ClassContextCount);
      Assert.IsFalse (context.RemoveClassContext (typeof (List<>)));
      Assert.AreEqual (0, context.ClassContextCount);
    }
  }
}