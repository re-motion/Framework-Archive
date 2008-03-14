using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Rhino.Mocks;
using Rubicon.Design;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.UnitTests.SampleTypes;
using NUnit.Framework;
using Rubicon.Mixins.Context;
using Rubicon.Reflection;
using Rubicon.Utilities;
using ReflectionUtility=Rubicon.Mixins.Utilities.ReflectionUtility;
using System.IO;

namespace Rubicon.Mixins.UnitTests.Context.DeclarativeConfigurationBuilder_IntegrationTests
{
  [TestFixture]
  public class DeclarativeConfigurationBuilderGeneralTests
  {
    [Test]
    public void BuildFromClassContexts()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (null, new ClassContext (typeof (BaseType1)));
      Assert.IsTrue (ac.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsFalse (ac.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));

      MixinConfiguration ac2 = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (ac, new ClassContext (typeof (BaseType2)), new ClassContext (typeof (BaseType3)));
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
      Assert.AreEqual (0, ac2.ClassContexts.GetWithInheritance (typeof (BaseType2)).Mixins.Count);
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));

      MixinConfiguration ac3 = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (ac2, new ClassContext (typeof (BaseType2), typeof (BT2Mixin1)));
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType2)));
      Assert.AreEqual (1, ac3.ClassContexts.GetWithInheritance (typeof (BaseType2)).Mixins.Count);
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType3)));
    }

    [Test]
    public void BuildFromAssemblies()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildConfigurationFromClasses (null, new ClassContext(typeof (object)));
      MixinConfiguration ac2 = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (AppDomain.CurrentDomain.GetAssemblies());
      Assert.IsTrue (ac2.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsFalse (ac2.ClassContexts.ContainsWithInheritance (typeof (object)));

      MixinConfiguration ac3 = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (ac, AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac3.ClassContexts.ContainsWithInheritance (typeof (object)));
      Assert.IsTrue (ac3.ClassContexts.GetWithInheritance (typeof (BaseType6)).CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin1)));
      Assert.AreSame (ac3.ClassContexts.GetWithInheritance (typeof (BaseType6)), ac3.ResolveInterface (typeof (ICBT6Mixin1)));

      MixinConfiguration ac4 = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (ac, (IEnumerable<Assembly>) AppDomain.CurrentDomain.GetAssemblies ());
      Assert.IsTrue (ac4.ClassContexts.ContainsWithInheritance (typeof (BaseType1)));
      Assert.IsTrue (ac4.ClassContexts.ContainsWithInheritance (typeof (object)));
    }

    [Test]
    public void DoubleAssembliesAreIgnored ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly (), Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType1));
      Assert.AreEqual (2, classContext.Mixins.Count);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void DoubleTypesAreIgnored ()
    {
      MixinConfiguration context = new DeclarativeConfigurationBuilder (null).AddType (typeof (BaseType1)).AddType (typeof (BaseType1))
          .AddType (typeof (BT1Mixin1)).AddType (typeof (BT1Mixin1)).AddType (typeof (BT1Mixin2)).AddType (typeof (BT1Mixin2)).BuildConfiguration ();

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType1));
      Assert.AreEqual (2, classContext.Mixins.Count);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin2)));
    }

    [Test]
    public void BuildDefault()
    {
      MixinConfiguration ac = DeclarativeConfigurationBuilder.BuildDefaultConfiguration();
      Assert.IsNotNull (ac);
      Assert.AreNotEqual (0, ac.ClassContexts.Count);
    }

    [Test]
    public void BuildDefault_DoesNotLockPersistedFile ()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      TypeFactory.GetConcreteType (typeof (object), GenerationPolicy.ForceGeneration);
      string[] paths = ConcreteTypeBuilder.Current.SaveAndResetDynamicScope();
      try
      {
        Assert.AreEqual (1, paths.Length);
        ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (null);
        DeclarativeConfigurationBuilder.BuildDefaultConfiguration ();
      }
      finally
      {
        File.Delete (paths[0]);
      }
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
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (object).Assembly.GetName ()));
      Assert.IsFalse (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (Uri).Assembly.GetName ()));
    }

    [Test]
    public void FilterExcludesGeneratedAssemblies ()
    {
      AssemblyFinderTypeDiscoveryService service =
          (AssemblyFinderTypeDiscoveryService)
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
      
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
              PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");

      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (DeclarativeConfigurationBuilderGeneralTests).Assembly.GetName ()));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (typeof (DeclarativeConfigurationBuilder).Assembly.GetName ()));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldConsiderAssembly (new AssemblyName ("whatever")));

      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (DeclarativeConfigurationBuilderGeneralTests).Assembly));
      Assert.IsTrue (service.AssemblyFinder.Filter.ShouldIncludeAssembly (typeof (DeclarativeConfigurationBuilder).Assembly));
    }

    [Test]
    public void DesignModeIsDetected ()
    {
      ITypeDiscoveryService service =
          (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");
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
        service = (ITypeDiscoveryService) PrivateInvoke.InvokeNonPublicStaticMethod (typeof (DeclarativeConfigurationBuilder), "GetTypeDiscoveryService");

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
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (TargetClassWithAdditionalDependencies));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (MixinWithAdditionalClassDependency)));
      Assert.IsTrue (classContext.Mixins[typeof (MixinWithAdditionalClassDependency)].ExplicitDependencies.ContainsKey (typeof (MixinWithNoAdditionalDependency)));
    }

    [Test]
    public void MixinAttributeOnMixinClass ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType1));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.Mixins.ContainsKey (typeof (BT1Mixin1)));
    }

    [Test]
    public void CompleteInterfaceConfiguredViaAttribute ()
    {
      MixinConfiguration context = DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (Assembly.GetExecutingAssembly ());

      ClassContext classContext = context.ClassContexts.GetWithInheritance (typeof (BaseType6));
      Assert.IsNotNull (classContext);

      Assert.IsTrue (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin1)));
      Assert.IsTrue (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin2)));
      Assert.IsTrue (classContext.CompleteInterfaces.ContainsKey (typeof (ICBT6Mixin3)));
    }
  }
}