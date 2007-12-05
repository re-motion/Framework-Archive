using System;
using System.Collections.Generic;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ApplicationContextTests
{
  [TestFixture]
  public class ApplicationContextInheritanceTests
  {
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
  }
}