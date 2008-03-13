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
using Rubicon.Utilities;

namespace Rubicon.Mixins.UnitTests.MixerTool
{
  [Serializable]
  [TestFixture]
  public class MixerTest : MixerToolBaseTest
  {
    [Test]
    public void MixerLeavesCurrentTypeBuilderUnchanged ()
    {
      MockRepository repository = new MockRepository();
      ConcreteTypeBuilder builder = ConcreteTypeBuilder.Current;
      IModuleManager scopeMock = repository.CreateMock<IModuleManager>();
      builder.Scope = scopeMock;

      // expect no calls on scope

      repository.ReplayAll();

      new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory).Execute();

      repository.VerifyAll();

      Assert.AreSame (builder, ConcreteTypeBuilder.Current);
      Assert.AreSame (scopeMock, ConcreteTypeBuilder.Current.Scope);
    }

    [Test]
    public void MixerToolGeneratesUnsignedAssemblyFiles ()
    {
      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
      mixer.Execute();
      Assert.IsTrue (File.Exists (UnsignedAssemblyPath));
    }

    [Test]
    public void MixerToolGeneratesSignedAssemblyFiles ()
    {
      Assert.IsFalse (File.Exists (SignedAssemblyPath));
      using (MixinConfiguration.BuildFromActive().ForClass (typeof (NullTarget)).Clear().EnterScope())
      {
        Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.Execute();
      }
      Assert.IsTrue (File.Exists (SignedAssemblyPath));
    }

    [Test]
    public void MixerToolGeneratesFileInRightDirectory ()
    {
      string outputDirectory = Path.Combine (Environment.CurrentDirectory, "MixinTool.Output");
      if (Directory.Exists (outputDirectory))
        Directory.Delete (outputDirectory, true);

      string outputPath = Path.Combine (outputDirectory, Parameters.UnsignedAssemblyName + ".dll");
      Assert.IsFalse (File.Exists (outputPath));
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, outputDirectory);
      mixer.Execute();
      Assert.IsTrue (File.Exists (outputPath));
    }

    [Test]
    public void MixerCanBeExecutedTwice ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
            mixer.Execute ();
          });
    }

    [Test]
    public void MixerToolCanBeRunTwice ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
          });
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
          });
    }

    [Test]
    public void DefaultConfigurationIsProcessed ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();

            Set<ClassContext> contextsFromConfig = GetExpectedDefaultContexts ();
            Assert.That (mixer.ProcessedContexts.Values, Is.EquivalentTo (contextsFromConfig));
          });
    }

    [Test]
    public void TypesAreGeneratedForProcessedContexts ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute();
            
            Set<ClassContext> contextsFromTypes = GetContextsFromGeneratedTypes (Assembly.LoadFile (UnsignedAssemblyPath));
            contextsFromTypes.AddRange (GetContextsFromGeneratedTypes (Assembly.LoadFile (SignedAssemblyPath)));
            
            Set<ClassContext> contextsFromConfig = GetExpectedDefaultContexts();
            Assert.That (contextsFromTypes, Is.EquivalentTo (contextsFromConfig));
          });
    }

    private Set<ClassContext> GetExpectedDefaultContexts ()
    {
      Set<ClassContext> contextsFromConfig = new Set<ClassContext>();
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

    [Test]
    public void NoErrorsInDefaultConfiguration ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
            Assert.IsEmpty (mixer.Errors);
          });
    }

    [Test]
    public void GenericTypeDefinitionsAreIgnored ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
            Assert.IsTrue (new List<ClassContext> (mixer.ProcessedContexts.Values)
                .TrueForAll (delegate (ClassContext c) { return !c.Type.IsGenericTypeDefinition; }));
          });
    }

    [Test]
    public void InterfacesAreIgnored ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute ();
            Assert.IsTrue (new List<ClassContext> (mixer.ProcessedContexts.Values)
                .TrueForAll (delegate (ClassContext c) { return !c.Type.IsInterface; }));
          });
    }

    [Test]
    public void ActiveMixinConfigurationIsProcessed ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.BuildNew().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
            {
              mixer.Execute();
              Assert.AreEqual (1, mixer.ProcessedContexts.Count);
              Assert.That(mixer.ProcessedContexts.Values, Is.EquivalentTo (MixinConfiguration.ActiveConfiguration.ClassContexts));
            }
          });
    }

    [Test]
    public void MixerToolProcessesSubclassesOfTargetTypes ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            
            using (MixinConfiguration.BuildNew().ForClass<NullTarget>().Clear().AddMixins (typeof (NullMixin)).EnterScope())
            {
              Assert.IsTrue (TypeUtility.HasMixin (typeof (DerivedNullTarget), typeof (NullMixin)));
              mixer.Execute();
            }

            Assert.IsTrue (mixer.ProcessedContexts.ContainsKey (typeof (DerivedNullTarget)));
          });
    }

    [Test]
    public void MixerToolHandlesClosedGenericTypes ()
    {
      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.BuildNew ().ForClass (typeof (List<int>)).Clear ().AddMixins (typeof (NullMixin)).EnterScope ())
            {
              mixer.Execute ();
            }

            Assert.IsTrue (mixer.ProcessedContexts.ContainsKey (typeof (List<int>)));
          });
    }

    [Test]
    public void MixerToolGeneratesMixedTypes ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);

            using (MixinConfiguration.BuildNew().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
            {
              mixer.Execute();

              Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
              Assert.AreEqual (2, theAssembly.GetTypes().Length);
              Type generatedType = GetFirstMixedType (theAssembly);

              Assert.IsNotNull (Mixin.GetMixinConfigurationFromConcreteType (generatedType));
              Assert.AreEqual (
                  MixinConfiguration.ActiveConfiguration.ClassContexts.GetWithInheritance (typeof (BaseType1)),
                  Mixin.GetMixinConfigurationFromConcreteType (generatedType));

              object instance = Activator.CreateInstance (generatedType);
              Assert.IsTrue (generatedType.IsInstanceOfType (instance));
              Assert.IsNotNull (Mixin.Get<BT1Mixin1> (instance));
            }
          });
    }

    [Test]
    public void AssemblyGeneratedByMixerToolCanBeLoadedIntoTypeBuilder ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.BuildNew().EnterScope())
            {
              using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
              {
                mixer.Execute();
              }
            }
          });

      AppDomainRunner.Run (
          delegate
          {
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            ConcreteTypeBuilder.Current.LoadAssemblyIntoCache (theAssembly);
            using (MixinConfiguration.BuildNew().EnterScope())
            {
              using (MixinConfiguration.BuildFromActive().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
              {
                Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
                Assert.Contains (generatedType, theAssembly.GetTypes());
              }
            }
          });
    }

    [Test]
    public void AssemblyGeneratedByMixerToolHasNonApplicationAssemblyAttribute ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.BuildNew().ForClass<BaseType1>().Clear().AddMixins (typeof (BT1Mixin1)).EnterScope())
            {
              mixer.Execute();
            }
          });

      AppDomainRunner.Run (
          delegate
          {
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            Assert.IsTrue (theAssembly.IsDefined (typeof (NonApplicationAssemblyAttribute), false));
          });
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
          mixer.Execute();
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
      using (DeclarativeConfigurationBuilder.BuildConfigurationFromAssemblies (typeof (MixerTest).Assembly).EnterScope())
      {
        Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
        mixer.ClassContextBeingProcessed +=
            delegate (object sender, ClassContextEventArgs args) { classContextsBeingProcessed.Add (args.ClassContext); };
        mixer.Execute();
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
        mixer.Execute();
        Assert.AreEqual ("Foo", mixer.FinishedTypes[typeof (BaseType1)].FullName);
      }
    }

    [Test]
    [Explicit ("This test can only be executed in isolation because it requires that the assemblies are not locked.")]
    public void SameInputAndOutputDirectory ()
    {
      Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, AppDomain.CurrentDomain.BaseDirectory);
      mixer.Execute();
      ContextAwareTypeDiscoveryService.DefaultService.SetCurrent (null); // trigger reloading of assemblies
      
      // trigger reanalysis of the default mixin configuration
      MixinConfiguration.SetActiveConfiguration (null);
      MixinConfiguration.ResetMasterConfiguration();
      
      mixer.Execute(); // this should _not_ load the generated files

      File.Delete (Parameters.UnsignedAssemblyName);
      File.Delete (Parameters.SignedAssemblyName);
    }
  }
}