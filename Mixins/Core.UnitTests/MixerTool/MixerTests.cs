using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Collections;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.Definitions;
using Rubicon.Mixins.MixerTool;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;
using NUnit.Framework.SyntaxHelpers;
using Rubicon.Reflection;

namespace Rubicon.Mixins.UnitTests.MixerTool
{
  [Serializable]
  [TestFixture]
  public class MixerTests : MixerToolBaseTests
  {
    [Test]
    public void Execute_SaveFalse ()
    {
      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      Assert.IsFalse (File.Exists (SignedAssemblyPath));
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute (false);
    }

   [Test]
    public void Execute_SaveTrue_Unsigned ()
    {
      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      using (MixinConfiguration.BuildNew ().ForClass (typeof (MixerTests)).AddMixin (typeof (object)).EnterScope ())
      {
        Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.Execute (true);
      }
     Assert.IsTrue (File.Exists (UnsignedAssemblyPath));
    }

    [Test]
    public void Execute_SaveTrue_Signed ()
    {
      Assert.IsFalse (File.Exists (SignedAssemblyPath));
      using (MixinConfiguration.BuildNew().ForClass (typeof (List<List<List<int>>>)).AddMixin(typeof (object)).EnterScope())
      {
        Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.Execute(true);
      }
      Assert.IsTrue (File.Exists (SignedAssemblyPath));
    }

    [Test]
    public void GeneratesFileInRightDirectory ()
    {
      string outputDirectory = Path.Combine (Environment.CurrentDirectory, "MixinTool.Output");
      if (Directory.Exists (outputDirectory))
        Directory.Delete (outputDirectory, true);

      string outputPath = Path.Combine (outputDirectory, Parameters.UnsignedAssemblyName + ".dll");
      Assert.IsFalse (File.Exists (outputPath));
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, outputDirectory);
      mixer.Execute(true);
      Assert.IsTrue (File.Exists (outputPath));
    }

    [Test]
    public void MixerCanBeExecutedTwice ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute (true);
      mixer.Execute (true);
    }

    [Test]
    public void MixerToolCanBeRunTwice_InIsolation ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute (true);
          });
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute (true);
          });
    }

    [Test]
    public void DefaultConfigurationIsProcessed ()
    {
        Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.Execute (false);

        Set<ClassContext> contextsFromConfig = GetExpectedDefaultContexts ();
        Assert.That (mixer.ProcessedContexts.Values, Is.EquivalentTo (contextsFromConfig));
    }

    [Test]
    public void TypesAreGeneratedForProcessedContexts ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute(true);
            
            Set<ClassContext> contextsFromTypes = GetContextsFromGeneratedTypes (Assembly.LoadFile (UnsignedAssemblyPath));
            contextsFromTypes.AddRange (GetContextsFromGeneratedTypes (Assembly.LoadFile (SignedAssemblyPath)));
            
            Set<ClassContext> contextsFromConfig = GetExpectedDefaultContexts();
            Assert.That (contextsFromTypes, Is.EquivalentTo (contextsFromConfig));
          });
    }

    [Test]
    public void NoErrorsInDefaultConfiguration ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute (false);
      Assert.IsEmpty (mixer.Errors);
    }

    [Test]
    public void GenericTypeDefinitionsAreIgnored ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute (false);
      Assert.IsTrue (
          new List<ClassContext> (mixer.ProcessedContexts.Values).TrueForAll (delegate (ClassContext c) { return !c.Type.IsGenericTypeDefinition; }));
    }

    [Test]
    public void InterfacesAreIgnored ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute (false);
      Assert.IsTrue (new List<ClassContext> (mixer.ProcessedContexts.Values).TrueForAll (delegate (ClassContext c) { return !c.Type.IsInterface; }));
    }

    [Test]
    public void ActiveMixinConfigurationIsProcessed ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        mixer.Execute (false);
        Assert.AreEqual (1, mixer.ProcessedContexts.Count);
        Assert.That(mixer.ProcessedContexts.Values, Is.EquivalentTo (MixinConfiguration.ActiveConfiguration.ClassContexts));
      }
    }

    [Test]
    public void ProcessesSubclassesOfTargetTypes ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      
      using (MixinConfiguration.BuildNew().ForClass<NullTarget>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
      {
        Assert.IsTrue (TypeUtility.HasMixin (typeof (DerivedNullTarget), typeof (NullMixin)));
        mixer.Execute (false);
      }

      Assert.IsTrue (mixer.ProcessedContexts.ContainsKey (typeof (DerivedNullTarget)));
    }

    [Test]
    public void HandlesClosedGenericTypes ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew ().ForClass (typeof (List<List<List<int>>>)).Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
      {
        mixer.Execute (false);
        Assert.IsEmpty (mixer.Errors);
      }

      Assert.IsTrue (mixer.FinishedTypes.ContainsKey (typeof (List<List<List<int>>>)));
    }

    [Test]
    public void MixerLeavesCurrentTypeBuilderUnchanged ()
    {
      try
      {
        MockRepository repository = new MockRepository ();
        ConcreteTypeBuilder builder = ConcreteTypeBuilder.Current;
        IModuleManager scopeMock = repository.CreateMock<IModuleManager> ();
        builder.Scope = scopeMock;

        // expect no calls on scope

        repository.ReplayAll ();

        new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory).Execute (true);

        repository.VerifyAll ();

        Assert.AreSame (builder, ConcreteTypeBuilder.Current);
        Assert.AreSame (scopeMock, ConcreteTypeBuilder.Current.Scope);
      }
      finally
      {
        ConcreteTypeBuilder.SetCurrent (null);
      }
    }
    
    [Test]
    public void MixerIgnoresInvalidTypes ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        using (MixinConfiguration.BuildNew ()
            .ForClass<BaseType1> ().Clear ().AddMixins (typeof (BT1Mixin1))  // valid
            .ForClass<BaseType2>().Clear().AddMixins (typeof (BT1Mixin1))  // invalid
            .EnterScope())
        {
          Console.WriteLine ("The following error is expected:");
          mixer.Execute (false);
          Assert.AreEqual (1, mixer.Errors.Count);
          Assert.AreEqual (MixinConfiguration.ActiveConfiguration.ClassContexts.GetExact (typeof (BaseType2)), mixer.Errors[0].A);
          Assert.That (mixer.FinishedTypes.Keys, Is.EquivalentTo (new object[] { typeof (BaseType1) }));
          Console.WriteLine ("This error was expected.");
        }
      }
    }

    [Test]
    public void MixerRaisesEventForEachClassContextBeingProcessed ()
    {
      List<ClassContext> classContextsBeingProcessed = new List<ClassContext>();

      // only use this assembly for this test case
      using (DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (typeof (MixerTests).Assembly).EnterScope())
      {
        Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.ClassContextBeingProcessed +=
            delegate (object sender, ClassContextEventArgs args) { classContextsBeingProcessed.Add (args.ClassContext); };
        mixer.Execute(false);
        Assert.That (classContextsBeingProcessed, Is.EquivalentTo (mixer.ProcessedContexts.Values));
      }
    }

    private class FooNameProvider : INameProvider
    {
      public string GetNewTypeName (ClassDefinitionBase configuration)
      {
        return "Foo";
      }
    }

    [Test]
    public void UsesGivenNameProvider ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.NameProvider = new FooNameProvider();
      using (MixinConfiguration.BuildNew().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
      {
        mixer.Execute (false);
        Assert.AreEqual ("Foo", mixer.FinishedTypes[typeof (BaseType1)].FullName);
      }
    }

    [Test]
    public void SameInputAndOutputDirectory ()
    {
      using (MixinConfiguration.BuildNew ()
          .ForClass (typeof (List<List<List<int>>>)).AddMixin (typeof (object))
          .ForClass (typeof (MixerTests)).AddMixin (typeof (object))
          .EnterScope ())
      {
        Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
        Assert.IsFalse (File.Exists (SignedAssemblyPath));

        Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, AppDomain.CurrentDomain.BaseDirectory);
        mixer.Execute (true);

        Assert.IsTrue (File.Exists (UnsignedAssemblyPath));
        Assert.IsTrue (File.Exists (SignedAssemblyPath));

        ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (null); // trigger reloading of assemblies

        // trigger reanalysis of the default mixin configuration
        MixinConfiguration.SetActiveConfiguration (null);
        MixinConfiguration.ResetMasterConfiguration();

        mixer.Execute (true); // this should _not_ load/lock the generated files

        File.Delete (UnsignedAssemblyPath);
        File.Delete (SignedAssemblyPath);
      }
    }

    private Set<ClassContext> GetExpectedDefaultContexts ()
    {
      Set<ClassContext> contextsFromConfig = new Set<ClassContext> ();
      foreach (ClassContext context in MixinConfiguration.ActiveConfiguration.ClassContexts)
      {
        if (!context.Type.IsGenericTypeDefinition && !context.Type.IsInterface)
          contextsFromConfig.Add (context);
      }

      foreach (Type t in ContextAwareTypeDiscoveryService.GetInstance ().GetTypes (null, false))
      {
        if (!t.IsGenericTypeDefinition && !t.IsInterface)
        {
          ClassContext context = MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (t);
          if (context != null)
            contextsFromConfig.Add (context);
        }
      }
      return contextsFromConfig;
    }
  }
}