using System;
using System.Collections.Generic;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.Context;

namespace Mixins.UnitTests.Configuration
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
      Assert.IsFalse (ac.HasClassContext (typeof (BaseType1)));
      Assert.IsNull (ac.GetClassContext (typeof (BaseType1)));
      Assert.IsEmpty (new List<ClassContext> (ac.ClassContexts));

      ClassContext newContext1 = new ClassContext (typeof (BaseType1));
      ac.AddClassContext (newContext1);
      Assert.AreEqual (1, ac.ClassContextCount);
      Assert.IsTrue (ac.HasClassContext (typeof (BaseType1)));
      Assert.IsNotNull (ac.GetClassContext (typeof (BaseType1)));
      Assert.AreSame (newContext1, ac.GetClassContext (typeof (BaseType1)));
      Assert.Contains (newContext1, new List<ClassContext> (ac.ClassContexts));
      Assert.AreEqual (1, new List<ClassContext> (ac.ClassContexts).Count);
      Assert.AreSame (newContext1, ac.GetOrAddClassContext(typeof (BaseType1)));

      Assert.IsTrue (ac.RemoveClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac.HasClassContext (typeof (BaseType1)));
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
      Assert.IsFalse (ac.HasClassContext (newContext4.Type));
      ac.AddOrReplaceClassContext (newContext4);
      Assert.IsTrue (ac.HasClassContext (newContext4.Type));
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
      Assert.IsTrue (ac2.HasClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.HasClassContext (typeof (BaseType2)));
      Assert.IsFalse (ac2.HasClassContext (typeof (BaseType3)));

      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType1)));
      Assert.IsNotNull (ac2.GetClassContext (typeof (BaseType2)));
      Assert.IsNull (ac2.GetClassContext (typeof (BaseType3)));

      ac2.AddClassContext (new ClassContext (typeof (BaseType3)));
      Assert.AreEqual (3, ac2.ClassContextCount);
      Assert.IsTrue (ac2.HasClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.HasClassContext (typeof (BaseType2)));
      Assert.IsTrue (ac2.HasClassContext (typeof (BaseType3)));

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
      Assert.IsTrue (ac2.HasClassContext (typeof (BaseType1)));
      Assert.AreSame (ac.GetClassContext (typeof (BaseType1)), ac2.GetClassContext (typeof (BaseType1)));

      ClassContext newContext = new ClassContext (typeof (BaseType1));
      ac2.AddOrReplaceClassContext (newContext);
      Assert.AreEqual (2, ac2.ClassContextCount);

      Assert.IsTrue (ac2.HasClassContext (typeof (BaseType1)));
      Assert.AreNotSame (ac.GetClassContext (typeof (BaseType1)), ac2.GetClassContext (typeof (BaseType1)));
    }
  }
}
