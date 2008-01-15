using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.DeclarativeConfigurationBuilderTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderGenericInheritanceTests
  {
    public class GenericClass<T> { }
    public class DerivedGenericClassFromOpen<T> : GenericClass<T> { }
    public class DerivedGenericClassFromClosed<T> : GenericClass<int> { }
    public class DerivedClassFromClosed : GenericClass<int> { }
    public class DerivedDerivedGenericClassFromOpen<T> : DerivedGenericClassFromOpen<T> { }

    [Extends (typeof (GenericClass<>))]
    public class MixinForOpenGeneric { }

    [Extends (typeof (GenericClass<int>))]
    public class MixinForClosedGeneric { }

    [Extends (typeof (DerivedGenericClassFromOpen<>))]
    public class MixinForDerivedOpenGeneric { }

    [Extends (typeof (DerivedGenericClassFromOpen<int>))]
    public class MixinForDerivedClosedGeneric { }

    [Extends (typeof (DerivedDerivedGenericClassFromOpen<int>))]
    public class MixinForDerivedDerivedClosedGeneric { }

    [Test]
    public void OpenGenericClassContext_Open()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (GenericClass<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void ClosedGenericClassContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (GenericClass<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.AreEqual (1, classContext.Mixins.Count);
    }

    [Test]
    public void ClosedGenericClassContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (GenericClass<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Open ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromOpen<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromOpen<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromOpen<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedClosedGeneric)));
      Assert.AreEqual (4, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Open ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromClosed<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Closed_NoOwnMixins ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromClosed<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Open ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedGenericClassFromOpen<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedGenericClassFromOpen<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedGenericClassFromOpen<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedDerivedClosedGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForDerivedDerivedClosedGeneric)));
      Assert.AreEqual (5, classContext.Mixins.Count);
    }

    [Test]
    public void DerivedClassFromClosedContext ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedClassFromClosed));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.Mixins.Count);
    }
  }
}