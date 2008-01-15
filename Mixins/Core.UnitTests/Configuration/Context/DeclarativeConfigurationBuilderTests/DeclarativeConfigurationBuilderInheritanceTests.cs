using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.UnitTests.SampleTypes;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.DeclarativeConfigurationBuilderTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderInheritanceTests
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

    [Test]
    public void BaseContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly());
      ClassContext classContext = context.GetClassContext (typeof (Base));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (Derived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedContextWithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerived));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order1 ()
    {
      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (Base));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));

      MixinConfiguration context = builder.BuildConfiguration();
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOwnMixin_Order2 ()
    {
      DeclarativeConfigurationBuilder builder = new DeclarativeConfigurationBuilder (null);
      builder.AddType (typeof (DerivedDerivedWithOwnMixin));
      builder.AddType (typeof (Derived));
      builder.AddType (typeof (Base));

      MixinConfiguration context = builder.BuildConfiguration ();
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixin));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }


    [Test]
    public void DerivedContextWithOverride ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedWithOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedContextWithOverrideAndOverride ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedWithOwnMixinAndOverride));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (DerivedNullMixin)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (NullMixin2)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }
  }
}