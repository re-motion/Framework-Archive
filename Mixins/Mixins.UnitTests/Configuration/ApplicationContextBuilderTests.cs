using System;
using System.Collections.Generic;
using System.Reflection;
using Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Mixins.Context;

namespace Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ApplicationContextBuilderTests
  {
    [Test]
    public void BuildFromClassContexts()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildFromClasses (null, new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac.ContainsClassContext (typeof (BaseType2)));

      ApplicationContext ac2 = ApplicationContextBuilder.BuildFromClasses (ac, new ClassContext (typeof (BaseType2)), new ClassContext (typeof (BaseType3)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType2)));
      Assert.AreEqual (0, ac2.GetClassContext (typeof (BaseType2)).MixinCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType3)));

      ApplicationContext ac3 = ApplicationContextBuilder.BuildFromClasses (ac2, new ClassContext (typeof (BaseType2), typeof (BT2Mixin1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType2)));
      Assert.AreEqual (1, ac3.GetClassContext (typeof (BaseType2)).MixinCount);
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType3)));
    }

    [Test]
    public void BuildFromAssemblies()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildFromClasses (null, new ClassContext(typeof (object)));
      ApplicationContext ac2 = ApplicationContextBuilder.BuildFromAssemblies (AppDomain.CurrentDomain.GetAssemblies());
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac2.ContainsClassContext (typeof (object)));

      ApplicationContext ac3 = ApplicationContextBuilder.BuildFromAssemblies (ac, AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (object)));

      ApplicationContext ac4 = ApplicationContextBuilder.BuildFromAssemblies (ac, (IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac4.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac4.ContainsClassContext (typeof (object)));
    }

    [Test]
    public void AnalyzeAssemblyIntoContext()
    {
      ApplicationContext ac = new ApplicationContext();
      Assert.IsFalse (ac.ContainsClassContext (typeof (BaseType1)));
      ApplicationContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly(), ac);
      Assert.IsTrue (ac.ContainsClassContext (typeof (BaseType1)));

      ApplicationContext ac2 = ApplicationContextBuilder.BuildFromClasses (null, new ClassContext (typeof (BaseType1), typeof (string)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (string)));
      Assert.IsFalse (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
      ApplicationContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly(), ac2);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (string)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
      ApplicationContextBuilder.AnalyzeAssemblyIntoContext (Assembly.GetExecutingAssembly (), ac2);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (string)));
      Assert.IsTrue (ac2.GetClassContext (typeof (BaseType1)).ContainsMixin (typeof (BT1Mixin1)));
    }

    [Test]
    public void BuildDefault()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildDefault();
      Assert.IsNotNull (ac);
      Assert.AreNotEqual (0, ac.ClassContextCount);
    }
  }
}
