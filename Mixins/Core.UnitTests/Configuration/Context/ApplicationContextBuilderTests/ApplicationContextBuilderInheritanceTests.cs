using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ApplicationContextBuilderTests
{
  [TestFixture]
  public class ApplicationContextBuilderInheritanceTests
  {
    [Uses (typeof (NullMixin))]
    public class Base { }

    public class Derived : Base { }

    [Uses (typeof (NullMixin2))]
    public class DerivedWithOwnMixin : Base { }

    public class DerivedDerived : Derived { }

    [Uses (typeof (NullMixin2))]
    public class DerivedDerivedWithOwnMixin : Derived { }

    [Uses (typeof (DerivedNullMixin))]
    public class DerivedWithOverride : Base { }

    [Uses (typeof (DerivedNullMixin))]
    public class DerivedDerivedWithOwnMixinAndOverride : DerivedDerivedWithOwnMixin { }

    public class DerivedNullMixin : NullMixin { }


    [Test]
    public void BaseContext ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly());
      ClassContext classContext = context.GetClassContext (typeof (Base));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.MixinCount);
    }

    [Test]
    public void DerivedContext ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (Derived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.MixinCount);
    }

    [Test]
    public void DerivedContextWithOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedContext ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order1 ()
    {
      ApplicationContextBuilder builder = new ApplicationContextBuilder (null);
      builder.AddType (typeof (Base));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));

      ApplicationContext context = builder.BuildContext();
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order2 ()
    {
      ApplicationContextBuilder builder = new ApplicationContextBuilder (null);
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (Base));

      ApplicationContext context = builder.BuildContext ();
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.MixinCount);
    }


    [Test]
    public void DerivedContextWithOverride ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedWithOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (DerivedNullMixin)));
      Assert.AreEqual (1, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedContextWithOverrideAndOverride ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixinAndOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (DerivedNullMixin)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.MixinCount);
    }
  }
}