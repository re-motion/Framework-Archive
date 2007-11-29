using System;
using System.Collections.Generic;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Validation;
using System.Runtime.Serialization;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ApplicationContextTests
  {
    [Test]
    public void CompletelyNewApplicationContextDoesNotKnowAnyClasses()
    {
      ApplicationContext ac = new ApplicationContext();
      Assert.AreEqual (0, ac.ClassContextCount);
    }

    [Test]
    public void GeneralFunctionality()
    {
      ApplicationContext ac = new ApplicationContext ();
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
      ApplicationContext ac = new ApplicationContext ();
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
    }

    [Test]
    public void InheritingApplicationContextKnowsClassesFromBasePlusOwn ()
    {
      ApplicationContext ac = new ApplicationContext ();
      Assert.AreEqual (0, ac.ClassContextCount);
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
      ac.AddClassContext (new ClassContext (typeof (BaseType2)));
      Assert.AreEqual (2, ac.ClassContextCount);

      ApplicationContext ac2 = new ApplicationContext (ac);
      Assert.AreEqual (2, ac2.ClassContextCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType2)));
      Assert.IsFalse (ac2.ContainsClassContext (typeof (BaseType3)));

      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType1)));
      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType2)));
      Assert.IsNull (ac2.GetClassContext (typeof (BaseType3)));

      ac2.AddClassContext (new ClassContext (typeof (BaseType3)));
      Assert.AreEqual (3, ac2.ClassContextCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType2)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType3)));

      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType1)));
      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType2)));
      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType3)));

      List<ClassContext> contexts = new List<ClassContext> (ac2.ClassContexts);
      Assert.AreEqual (3, contexts.Count);
      Assert.Contains (ac.GetClassContext (typeof (BaseType1)), contexts);
      Assert.Contains (ac.GetClassContext (typeof (BaseType2)), contexts);
      Assert.Contains (ac2.GetClassContext (typeof (BaseType3)), contexts);
    }

    [Test]
    public void OverridingClassContextsFromParent ()
    {
      ApplicationContext ac = new ApplicationContext ();
      Assert.AreEqual (0, ac.ClassContextCount);
      ac.AddClassContext (new ClassContext (typeof (BaseType1)));
      ac.AddClassContext (new ClassContext (typeof (BaseType2)));
      Assert.AreEqual (2, ac.ClassContextCount);
      
      ApplicationContext ac2 = new ApplicationContext (ac);
      Assert.AreEqual (2, ac2.ClassContextCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.AreEqual (ac.GetClassContext (typeof (BaseType1)), ac2.GetClassContext (typeof (BaseType1)));

      ClassContext newContext = new ClassContext (typeof (BaseType1));
      ac2.AddOrReplaceClassContext (newContext);
      Assert.AreEqual (2, ac2.ClassContextCount);

      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.AreNotSame (ac.GetClassContext (typeof (BaseType1)), ac2.GetClassContext (typeof (BaseType1)));
    }

    [Test]
    public void RegisterAndResolveCompleteInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      cc.AddCompleteInterface (typeof (IBaseType2));
      Assert.IsNull (ac.ResolveInterface (typeof (IBaseType2)));
      ac.RegisterInterface (typeof (IBaseType2), cc);
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));

      ac.GetOrAddClassContext (typeof (BaseType3));
      ac.RegisterInterface (typeof (IBaseType31), typeof (BaseType3));
      Assert.AreSame (ac.GetClassContext (typeof (BaseType3)), ac.ResolveInterface (typeof (IBaseType31)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The argument is not an interface.", MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnNonInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (BaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The class context hasn't been added to this application context.",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext1 ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.RegisterInterface (typeof (IBaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The class context hasn't been added to this application context.",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext2 ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (IBaseType2), new ClassContext (typeof (BaseType2)));
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "There is no class context for the given type",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnInvalidClassContext3 ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ac.RegisterInterface (typeof (IBaseType2), typeof (BaseType2));
    }

    [Test]
    public void RemoveClassContextCausesInterfaceRegistrationToBeRemoved ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
      Assert.IsNotNull (ac.ResolveInterface (typeof (IBaseType2)));
      Assert.AreSame (cc, ac.ResolveInterface (typeof (IBaseType2)));
      ac.RemoveClassContext (typeof (BaseType2));
      Assert.IsNull (ac.ResolveInterface (typeof (IBaseType2)));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage = "already been associated with a class context",
        MatchType = MessageMatch.Contains)]
    public void RegisterCompleteInterfaceThrowsOnDuplicateInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ClassContext cc = new ClassContext (typeof (BaseType2));
      ac.AddClassContext (cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
      ac.RegisterInterface (typeof (IBaseType2), cc);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage = "The argument is not an interface.", MatchType = MessageMatch.Contains)]
    public void ResolveCompleteInterfaceThrowsOnNonInterface ()
    {
      ApplicationContext ac = new ApplicationContext ();
      ac.ResolveInterface (typeof (BaseType2));
    }

    [Test]
    public void CopyTo ()
    {
      ApplicationContext parent = new ApplicationContext();
      parent.GetOrAddClassContext (typeof (BaseType2)).GetOrAddMixinContext (typeof (BT2Mixin1)).AddExplicitDependency (typeof (IBaseType33));
      parent.RegisterInterface (typeof (IBaseType2), typeof (BaseType2));

      ApplicationContext source = new ApplicationContext (parent);
      source.GetOrAddClassContext (typeof (BaseType1)).GetOrAddMixinContext (typeof (BT1Mixin1)).AddExplicitDependency (typeof (IBaseType34));
      source.GetOrAddClassContext (typeof (BaseType1)).AddCompleteInterface (typeof (IBaseType33));

      Assert.IsTrue (source.ContainsClassContext (typeof (BaseType2)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType2)).ContainsMixin (typeof (BT2Mixin1)));
      Assert.IsTrue (source.GetClassContext (typeof (BaseType2)).GetOrAddMixinContext (typeof (BT2Mixin1)).ContainsExplicitDependency (typeof (IBaseType33)));

      Assert.AreSame (source.GetClassContext (typeof (BaseType2)), source.ResolveInterface (typeof (IBaseType2)));

      Assert.IsTrue (source.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (source.GetOrAddClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (source.GetOrAddClassContext (typeof (BaseType1)).GetOrAddMixinContext (typeof (BT1Mixin1)).ContainsExplicitDependency (typeof (IBaseType34)));

      ApplicationContext destination = new ApplicationContext ();
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
    public void ValidateWithNoErrors ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (NullMixin)))
        {
          IValidationLog log = MixinConfiguration.ActiveContext.Validate();
          Assert.IsTrue (log.GetNumberOfSuccesses() > 0);
          Assert.AreEqual (0, log.GetNumberOfFailures());
        }
      }
    }

    [Test]
    public void ValidateWithErrors ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (int), typeof (NullMixin)))
        {
          IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
          Assert.IsTrue (log.GetNumberOfFailures () > 0);
        }
      }
    }

    [Test]
    public void ValidateWithGenerics ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (KeyValuePair<,>), typeof (NullMixin)))
        {
          IValidationLog log = MixinConfiguration.ActiveContext.Validate ();
          Assert.IsTrue (log.GetNumberOfFailures () > 0);
        }
      }
    }

    class UninstantiableGeneric<T>
        where T : ISerializable, IServiceProvider
    {
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "The ApplicationContext contains a ClassContext for the generic type "
        + "Rubicon.Mixins.UnitTests.Configuration.ApplicationContextTests+UninstantiableGeneric`1[T], of which it cannot make a closed type. "
        + "Because closed types are needed for validation, the ApplicationContext cannot be validated as a whole. The configuration might still "
        + "be correct, but validation must be deferred to TypeFactory.GetActiveConfiguration.")]
    public void ValidationThrowsWhenGenericsCannotBeSpecialized ()
    {
      using (MixinConfiguration.ScopedEmpty ())
      {
        using (MixinConfiguration.ScopedExtend (typeof (UninstantiableGeneric<>), typeof (NullMixin)))
        {
          MixinConfiguration.ActiveContext.Validate ();
        }
      }
    }

    [Test]
    public void GetContextWorksRecursively ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (NullMixin)))
      {
        ClassContext context = MixinConfiguration.ActiveContext.GetClassContext (typeof (DerivedNullTarget));
        Assert.IsNotNull (context);
        Assert.AreEqual (typeof (DerivedNullTarget), context.Type);
        Assert.IsTrue (context.ContainsMixin (typeof (NullMixin)));
      }
    }

    [Test]
    public void GetContextNonRecursive ()
    {
      using (MixinConfiguration.ScopedExtend (typeof (NullTarget), typeof (NullMixin)))
      {
        ClassContext context = MixinConfiguration.ActiveContext.GetClassContextNonRecursive (typeof (DerivedNullTarget));
        Assert.IsNull (context);
      }
    }

    [Test]
    public void GenericTypesNotTransparentlyConvertedToTypeDefinitions ()
    {
      ApplicationContext context = new ApplicationContext ();
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
      ApplicationContext context = new ApplicationContext ();
      context.AddClassContext (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, context.ClassContextCount);
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      Assert.AreSame (context.GetClassContext (typeof (List<>)), context.GetClassContext (typeof (List<int>)));

      context.AddClassContext (new ClassContext (typeof (List<int>)));

      Assert.AreEqual (2, context.ClassContextCount);
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      Assert.AreNotSame (context.GetClassContext (typeof (List<>)), context.GetClassContext (typeof (List<int>)));
    }

    [Test]
    public void AddOrReplaceContextForGenericSpecialization ()
    {
      ApplicationContext context = new ApplicationContext ();
      context.AddClassContext (new ClassContext (typeof (List<>)));

      Assert.AreEqual (1, context.ClassContextCount);
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      Assert.AreSame (context.GetClassContext (typeof (List<>)), context.GetClassContext (typeof (List<int>)));

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
      ApplicationContext context = new ApplicationContext ();
      context.AddClassContext (new ClassContext (typeof (List<>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<int>)));
      Assert.IsTrue (context.ContainsClassContext (typeof (List<>)));

      ClassContext classContext1 = context.GetClassContext (typeof (List<int>));
      ClassContext classContext2 = context.GetClassContextNonRecursive (typeof (List<>));
      Assert.AreSame (classContext1, classContext2);
    }

    [Test]
    public void GetOrAddContextForGenericTypeDefinitions ()
    {
      ApplicationContext context = new ApplicationContext ();
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
      ApplicationContext context = new ApplicationContext ();
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
