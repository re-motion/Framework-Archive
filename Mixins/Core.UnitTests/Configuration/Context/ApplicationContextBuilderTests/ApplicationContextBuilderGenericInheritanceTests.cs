using System;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Mixins.Context;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ApplicationContextBuilderTests
{
  [TestFixture]
  public class ApplicationContextBuilderGenericInheritanceTests
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
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (GenericClass<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.AreEqual (1, classContext.MixinCount);
    }

    [Test]
    public void ClosedGenericClassContext_Closed_NoOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (GenericClass<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.AreEqual (1, classContext.MixinCount);
    }

    [Test]
    public void ClosedGenericClassContext_Closed_WithOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (GenericClass<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Open ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromOpen<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromOpen<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromOpen<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForClosedGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedClosedGeneric)));
      Assert.AreEqual (4, classContext.MixinCount);
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Open ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromClosed<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedGenericClassFromClosedContext_Closed_NoOwnMixins ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedGenericClassFromClosed<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Open ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedGenericClassFromOpen<>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_NoOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedGenericClassFromOpen<string>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedOpenGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }

    [Test]
    public void DerivedDerivedGenericClassFromOpenContext_Closed_WithOwnMixin ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedDerivedGenericClassFromOpen<int>));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForClosedGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedClosedGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedDerivedClosedGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForDerivedDerivedClosedGeneric)));
      Assert.AreEqual (5, classContext.MixinCount);
    }

    [Test]
    public void DerivedClassFromClosedContext ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());
      ClassContext classContext = context.GetClassContext (typeof (DerivedClassFromClosed));
      Assert.IsNotNull (classContext);
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForOpenGeneric)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinForClosedGeneric)));
      Assert.AreEqual (2, classContext.MixinCount);
    }
  }
}