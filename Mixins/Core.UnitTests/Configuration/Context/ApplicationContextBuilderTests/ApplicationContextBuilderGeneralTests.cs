using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Rhino.Mocks;
using Rubicon.Design;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Reflection;
using Rubicon.Utilities;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;

namespace Rubicon.Mixins.UnitTests.Configuration.Context.ApplicationContextBuilderTests
{
  [TestFixture]
  public class ApplicationContextBuilderGeneralTests
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
    public void DoubleAssembliesAreIgnored ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly (), Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      Assert.AreEqual (2, classContext.MixinCount);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin2)));
    }

    [Test]
    public void DoubleTypesAreIgnored ()
    {
      ApplicationContext context = new ApplicationContextBuilder (null).AddType (typeof (BaseType1)).AddType (typeof (BaseType1))
          .AddType (typeof (BT1Mixin1)).AddType (typeof (BT1Mixin1)).AddType (typeof (BT1Mixin2)).AddType (typeof (BT1Mixin2)).BuildContext ();

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      Assert.AreEqual (2, classContext.MixinCount);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin2)));
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
      Assert.IsFalse (TypeFactory.GetActiveConfiguration (typeof (BaseType1)).Mixins.ContainsKey (typeof (Foo)));
    }

    [Test]
    public void FilterExcludesSystemAssemblies ()
    {
      AssemblyFinderTypeDiscoveryService service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ApplicationContextBuilder), "GetTypeDiscoveryService");
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName ()));
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (Uri).Assembly.GetName ()));
    }

    [Test]
    public void FilterExcludesGeneratedAssemblies ()
    {
      AssemblyFinderTypeDiscoveryService service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ApplicationContextBuilder), "GetTypeDiscoveryService");
      
      Assembly signedAssembly = TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration).Assembly;
      Assembly unsignedAssembly = TypeFactory.GetConcreteType (typeof (BaseType1), GenerationPolicy.ForceGeneration).Assembly;

      Assert.IsTrue (ReflectionUtility.IsAssemblySigned (signedAssembly));
      Assert.IsFalse (ReflectionUtility.IsAssemblySigned (unsignedAssembly));

      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldIncludeAssembly (signedAssembly));
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldIncludeAssembly (unsignedAssembly));
    }

    [Test]
    public void FilterIncludesAllNormalAssemblies ()
    {
      AssemblyFinderTypeDiscoveryService service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ApplicationContextBuilder), "GetTypeDiscoveryService");

      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (ApplicationContextBuilderGeneralTests).Assembly.GetName ()));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (ApplicationContextBuilder).Assembly.GetName ()));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (new AssemblyName ("whatever")));

      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (ApplicationContextBuilderGeneralTests).Assembly));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (ApplicationContextBuilder).Assembly));
    }

    [Test]
    public void DesignModeIsDetected ()
    {
      ITypeDiscoveryService service =
          (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ApplicationContextBuilder), "GetTypeDiscoveryService");
      Assert.IsInstanceOfType (typeof (AssemblyFinderTypeDiscoveryService), service);

      MockRepository repository = new MockRepository();
      IDesignModeHelper designModeHelperMock = repository.CreateMock<IDesignModeHelper> ();
      IDesignerHost designerHostMock = repository.CreateMock<IDesignerHost>();
      ITypeDiscoveryService designerServiceMock = repository.CreateMock<ITypeDiscoveryService> ();

      Expect.Call (designModeHelperMock.DesignerHost).Return (designerHostMock);
      Expect.Call (designerHostMock.GetService (typeof (ITypeDiscoveryService))).Return (designerServiceMock);

      repository.ReplayAll();
      
      DesignerUtility.SetDesignMode (designModeHelperMock);
      try
      {
        service = (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (ApplicationContextBuilder), "GetTypeDiscoveryService");

        Assert.IsNotInstanceOfType (typeof (AssemblyFinderTypeDiscoveryService), service);
        Assert.AreSame (designerServiceMock, service);
      }
      finally
      {
        DesignerUtility.ClearDesignMode();
      }

      repository.VerifyAll ();
    }

    [Test]
    public void MixinAttributeOnTargetClass ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.GetClassContext (typeof (TargetClassWithAdditionalDependencies));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (MixinWithAdditionalClassDependency)));
      Assert.IsTrue (
          classContext.GetOrAddMixinContext (typeof (MixinWithAdditionalClassDependency)).ContainsExplicitDependency (typeof (MixinWithNoAdditionalDependency)));
    }

    [Test]
    public void MixinAttributeOnMixinClass ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.GetClassContext (typeof (BaseType1));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsMixin (typeof (BT1Mixin1)));
    }

    [Test]
    public void CompleteInterfaceConfiguredViaAttribute ()
    {
      ApplicationContext context = ApplicationContextBuilder.BuildContextFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.GetClassContext (typeof (BaseType6));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.ContainsCompleteInterface (typeof (ICBT6Mixin1)));
      Assert.IsTrue (classContext.ContainsCompleteInterface (typeof (ICBT6Mixin2)));
      Assert.IsTrue (classContext.ContainsCompleteInterface (typeof (ICBT6Mixin3)));
    }
  }
}