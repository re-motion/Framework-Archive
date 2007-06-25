using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration
{
  [TestFixture]
  public class ApplicationContextBuilderTests
  {
    [Test]
    public void BuildFromClassContexts()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildContextFromClasses (null, new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac.ContainsClassContext (typeof (BaseType2)));

      ApplicationContext ac2 = ApplicationContextBuilder.BuildContextFromClasses (ac, new ClassContext (typeof (BaseType2)), new ClassContext (typeof (BaseType3)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType2)));
      Assert.AreEqual (0, ac2.GetClassContext (typeof (BaseType2)).MixinCount);
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType3)));

      ApplicationContext ac3 = ApplicationContextBuilder.BuildContextFromClasses (ac2, new ClassContext (typeof (BaseType2), typeof (BT2Mixin1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType2)));
      Assert.AreEqual (1, ac3.GetClassContext (typeof (BaseType2)).MixinCount);
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType3)));
    }

    [Test]
    public void BuildFromAssemblies()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildContextFromClasses (null, new ClassContext(typeof (object)));
      ApplicationContext ac2 = ApplicationContextBuilder.BuildContextFromAssemblies (AppDomain.CurrentDomain.GetAssemblies());
      Assert.IsTrue (ac2.ContainsClassContext (typeof (BaseType1)));
      Assert.IsFalse (ac2.ContainsClassContext (typeof (object)));

      ApplicationContext ac3 = ApplicationContextBuilder.BuildContextFromAssemblies (ac, AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac3.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac3.ContainsClassContext (typeof (object)));
      Assert.IsTrue (ac3.GetClassContext (typeof (BaseType6)).ContainsCompleteInterface (typeof (ICBT6Mixin1)));
      Assert.AreSame (ac3.GetClassContext (typeof (BaseType6)), ac3.ResolveInterface (typeof (ICBT6Mixin1)));

      ApplicationContext ac4 = ApplicationContextBuilder.BuildContextFromAssemblies (ac, (IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac4.ContainsClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac4.ContainsClassContext (typeof (object)));
    }

    [Test]
    public void BuildDefault()
    {
      ApplicationContext ac = ApplicationContextBuilder.BuildDefaultContext();
      Assert.IsNotNull (ac);
      Assert.AreNotEqual (0, ac.ClassContextCount);
    }

    [Extends (typeof (BaseType1))]
    [IgnoreForMixinConfiguration]
    public class Foo { }

    [Test]
    public void IgnoreForMixinConfiguration()
    {
      Assert.IsFalse (TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins.HasItem (typeof (Foo)));
    }
  }
}
