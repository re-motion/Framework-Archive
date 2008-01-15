using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Mixins.Context.FluentBuilders;
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
      MixinConfiguration configuration = new MixinConfiguration();
      Assert.AreEqual (0, configuration.ClassContextCount);
      List<ClassContext> classContexts = new List<ClassContext> (configuration.ClassContexts);
      Assert.AreEqual (0, classContexts.Count);
      Assert.IsFalse (configuration.ContainsClassContext (typeof (BaseType1)));
    }

    [Test]
    public void BuildFromTestAssembly ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      CheckContext (configuration);
    }

    [Test]
    public void BuildFromTestAssemblies ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (null, AppDomain.CurrentDomain.GetAssemblies ());
      CheckContext (configuration);
    }

    private static void CheckContext (MixinConfiguration configuration)
    {
      Assert.IsTrue (configuration.ContainsClassContext (typeof (BaseType1)));

      List<ClassContext> classContexts = new List<ClassContext> (configuration.ClassContexts);
      Assert.IsTrue (classContexts.Count > 0);

      ClassContext contextForBaseType1 = configuration.GetClassContext (typeof (BaseType1));
      Assert.AreEqual (2, contextForBaseType1.Mixins.Count);

      Assert.IsTrue (contextForBaseType1.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (contextForBaseType1.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void NewConfiguration ()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      Assert.AreEqual (0, configuration.ClassContextCount);
      Assert.IsFalse (configuration.ContainsClassContext (typeof (BaseType1)));
      Assert.IsNull (configuration.GetClassContext (typeof (BaseType1)));
      Assert.IsEmpty (new List<ClassContext> (configuration.ClassContexts));
    }

    [Test]
    public void AddClassContext ()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      ClassContext newContext1 = new ClassContext (typeof (BaseType1));
      configuration.AddClassContext (newContext1);
      Assert.AreEqual (1, configuration.ClassContextCount);
      Assert.IsTrue (configuration.ContainsClassContext (typeof (BaseType1)));
      Assert.AreSame (newContext1, configuration.GetClassContext (typeof (BaseType1)));
      Assert.Contains (newContext1, new List<ClassContext> (configuration.ClassContexts));
      Assert.AreEqual (1, new List<ClassContext> (configuration.ClassContexts).Count);

      ClassContext newContext2 = new ClassContext (typeof (BaseType2));
      configuration.AddClassContext (newContext2);
      Assert.IsNotNull (newContext2);
      Assert.AreNotSame (newContext1, newContext2);
    }

    [Test]
    public void RemoveClassContext()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      ClassContext newContext1 = new ClassContext (typeof (BaseType1));
      configuration.AddClassContext (newContext1);

      Assert.IsTrue (configuration.RemoveClassContext (typeof (BaseType1)));
      Assert.IsFalse (configuration.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (configuration.RemoveClassContext (typeof (BaseType1)));
    }

    [Test]
    public void AddOrReplaceClassContext()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      ClassContext existingContext = new ClassContext (typeof (BaseType2));
      configuration.AddClassContext (existingContext);

      ClassContext replacement = new ClassContext (typeof (BaseType2));
      Assert.AreSame (existingContext, configuration.GetClassContext (typeof (BaseType2)));
      
      configuration.AddOrReplaceClassContext (replacement);
      
      Assert.AreNotSame (existingContext, configuration.GetClassContext (typeof (BaseType2)));
      Assert.AreSame (replacement, configuration.GetClassContext (typeof (BaseType2)));

      ClassContext additionalContext = new ClassContext (typeof (BaseType3));
      Assert.IsFalse (configuration.ContainsClassContext (additionalContext.Type));
      configuration.AddOrReplaceClassContext (additionalContext);
      Assert.IsTrue (configuration.ContainsClassContext (additionalContext.Type));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "already a class context", MatchType = MessageMatch.Contains)]
    public void ThrowsOnDoubleAdd ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.AddClassContext (new ClassContext (typeof (BaseType1)));
      configuration.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException))]
    public void ThrowsOnDoubleAdd_ViaConstructor ()
    {
      MixinConfiguration configuration = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      configuration.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void Clear ()
    {
      MixinConfiguration configuration = new MixinConfiguration();
      ClassContext classContext = new ClassContext (typeof (object));
      configuration.AddClassContext (classContext);
      configuration.RegisterInterface (typeof (IServiceProvider), classContext);

      Assert.AreEqual (1, configuration.ClassContextCount);
      Assert.AreSame (classContext, configuration.ResolveInterface (typeof (IServiceProvider)));

      configuration.Clear();

      Assert.AreEqual (0, configuration.ClassContextCount);
      Assert.IsNull (configuration.ResolveInterface (typeof (IServiceProvider)));
    }

    [Test]
    public void CopyTo ()
    {
      MixinConfiguration parent = new MixinConfigurationBuilder (null)
          .ForClass (typeof (BaseType2))
          .AddMixin (typeof (BT2Mixin1)).WithDependency (typeof (IBaseType33))
          .AddCompleteInterface (typeof (IBaseType2))
          .BuildConfiguration();

      MixinConfiguration source = new MixinConfigurationBuilder (parent)
          .ForClass (typeof (BaseType1))
          .AddMixin (typeof (BT1Mixin1)).WithDependency (typeof (IBaseType34))
          .AddCompleteInterface (typeof (IBaseType33))
          .BuildConfiguration();

      Assert.IsTrue (source.ContainsClassContext (typeof (BaseType2)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType2)).Mixins.ContainsKey (typeof (BT2Mixin1)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType2)).Mixins[typeof (BT2Mixin1)]
          .ExplicitDependencies.ContainsKey (typeof (IBaseType33)));

      Assert.AreSame (source.GetClassContext (typeof (BaseType2)), source.ResolveInterface (typeof (IBaseType2)));

      Assert.IsTrue (source.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType1)).Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)]
          .ExplicitDependencies.ContainsKey (typeof (IBaseType34)));

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
      Assert.IsTrue (destination.GetClassContext (typeof (BaseType2)).Mixins.ContainsKey (typeof (BT2Mixin1)));
      Assert.IsTrue (destination.GetClassContext (typeof (BaseType2)).Mixins[typeof (BT2Mixin1)]
          .ExplicitDependencies.ContainsKey (typeof (IBaseType33)));

      Assert.AreSame (destination.GetClassContext (typeof (BaseType2)), destination.ResolveInterface (typeof (IBaseType2)));

      Assert.IsTrue (destination.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (destination.GetClassContext (typeof (BaseType1)).Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (destination.GetClassContext (typeof (BaseType1)).Mixins[typeof (BT1Mixin1)]
          .ExplicitDependencies.ContainsKey (typeof (IBaseType34)));
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
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.AddClassContext (new ClassContext (typeof (List<int>)));
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<int>)));
      Assert.IsFalse (configuration.ContainsClassContext (typeof (List<>)));

      configuration.AddClassContext (new ClassContext (typeof (List<string>)));

      Assert.AreEqual (2, configuration.ClassContextCount);
      configuration.AddOrReplaceClassContext (new ClassContext (typeof (List<double>)));
      Assert.AreEqual (3, configuration.ClassContextCount);

      ClassContext classContext1 = configuration.GetClassContext (typeof (List<int>));
      ClassContext classContext2 = configuration.GetClassContextNonRecursive (typeof (List<string>));
      Assert.AreNotSame (classContext1, classContext2);

      ClassContext classContext3 = configuration.GetClassContext (typeof (List<List<int>>));
      Assert.IsNull (classContext3);

      Assert.IsFalse (configuration.RemoveClassContext (typeof (List<bool>)));
      Assert.AreEqual (3, configuration.ClassContextCount);
      Assert.IsTrue (configuration.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (2, configuration.ClassContextCount);
      Assert.IsFalse (configuration.RemoveClassContext (typeof (List<int>)));
    }

    [Test]
    public void AddContextForGenericSpecialization ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.AddClassContext (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, configuration.ClassContextCount);
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (configuration.GetClassContext (typeof (List<>)), configuration.GetClassContext (typeof (List<int>)));

      configuration.AddClassContext (new ClassContext (typeof (List<int>)));

      Assert.AreEqual (2, configuration.ClassContextCount);
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (configuration.GetClassContext (typeof (List<>)), configuration.GetClassContext (typeof (List<int>)));
    }

    [Test]
    public void AddOrReplaceContextForGenericSpecialization ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.AddClassContext (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, configuration.ClassContextCount);
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (configuration.GetClassContext (typeof (List<>)), configuration.GetClassContext (typeof (List<int>)));

      ClassContext listIntContext = new ClassContext (typeof (List<int>));
      configuration.AddOrReplaceClassContext (listIntContext);

      Assert.AreEqual (2, configuration.ClassContextCount);
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (configuration.GetClassContext (typeof (List<>)), configuration.GetClassContext (typeof (List<int>)));
      Assert.AreSame (listIntContext, configuration.GetClassContext (typeof (List<int>)));

      ClassContext newListIntContext = new ClassContext (typeof (List<int>));
      configuration.AddOrReplaceClassContext (newListIntContext);
      Assert.AreEqual (2, configuration.ClassContextCount);

      Assert.AreSame (newListIntContext, configuration.GetClassContext (typeof (List<int>)));
    }

    [Test]
    public void GetContextForGenericTypeDefinitions ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.AddClassContext (new ClassContext (typeof (List<>)));
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (configuration.ContainsClassContext (typeof (List<>)));

      ClassContext classContext1 = configuration.GetClassContext (typeof (List<int>));
      ClassContext classContext2 = configuration.GetClassContextNonRecursive (typeof (List<>));
      Assert.AreNotSame (classContext1, classContext2);
    }

    [Test]
    public void GetOrAddContextForGenericTypeDefinitions ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      ClassContext genericListContext = new ClassContext (typeof (List<>));
      configuration.AddClassContext (genericListContext);

      ClassContext listIntContext = configuration.GetClassContext (typeof (List<int>));
      ClassContext listListContext = configuration.GetClassContext (typeof (List<List<int>>));
      Assert.AreNotSame (listIntContext, listListContext);
      Assert.AreNotEqual (listIntContext, listListContext);
      Assert.IsNotNull (listListContext);

      ClassContext listListContext2 = configuration.GetClassContext (typeof (List<List<int>>));
      Assert.AreNotSame (listListContext, listListContext2);
      Assert.AreEqual (listListContext, listListContext2);

      ClassContext genericListContext2 = configuration.GetClassContext (typeof (List<>));
      Assert.AreSame (genericListContext, genericListContext2);
    }

    [Test]
    public void RemoveClassContextForGenericTypeDefinitions ()
    {
      MixinConfiguration configuration = new MixinConfiguration ();
      configuration.AddClassContext (new ClassContext (typeof (List<>)));
      Assert.IsFalse (configuration.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (1, configuration.ClassContextCount);
      
      configuration.AddClassContext (new ClassContext (typeof (List<int>)));
      Assert.AreEqual (2, configuration.ClassContextCount);
      
      Assert.IsTrue (configuration.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (1, configuration.ClassContextCount);
      Assert.IsFalse (configuration.RemoveClassContext (typeof (List<int>)));
      Assert.AreEqual (1, configuration.ClassContextCount);

      Assert.IsTrue (configuration.RemoveClassContext (typeof (List<>)));
      Assert.AreEqual (0, configuration.ClassContextCount);
      Assert.IsFalse (configuration.RemoveClassContext (typeof (List<>)));
      Assert.AreEqual (0, configuration.ClassContextCount);
    }
  }
}