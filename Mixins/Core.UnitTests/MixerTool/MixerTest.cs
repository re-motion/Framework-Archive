using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using Rubicon.Collections;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.MixerTool;
using Rubicon.Mixins.UnitTests.SampleTypes;
using System.Reflection;
using NUnit.Framework.SyntaxHelpers;

namespace Rubicon.Mixins.UnitTests.MixerTool
{
  [Serializable]
  [TestFixture]
  public class MixerTest : MixerToolBaseTest
  {
    [Test]
    public void MixerLeavesCurrentTypeBuilderUnchanged ()
    {
      MockRepository repository = new MockRepository ();
      ConcreteTypeBuilder builder = ConcreteTypeBuilder.Current;
      IModuleManager scopeMock = repository.CreateMock<IModuleManager> ();
      builder.Scope = scopeMock;

      // expect no calls on scope

      repository.ReplayAll ();

      new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory).Execute ();

      repository.VerifyAll ();

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
      using (MixinConfiguration.ScopedExtend (typeof (object)))
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
    public void MixerToolGeneratesTypesForDefaultMixinConfiguration ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            mixer.Execute();
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            
            Set<ClassContext> contextsFromTypes = new Set<ClassContext> ();
            foreach (Type concreteType in theAssembly.GetTypes ())
            {
              ClassContext context = Mixin.GetMixinConfigurationFromConcreteType (concreteType);
              if (context != null)
                contextsFromTypes.Add (context);
            }

            Set<ClassContext> contextsFromConfig = new Set<ClassContext> ();
            foreach (ClassContext context in MixinConfiguration.ActiveContext.ClassContexts)
            {
              if (!context.Type.IsGenericTypeDefinition)
                contextsFromConfig.Add (context);
            }

            Assert.That (contextsFromTypes, Is.EquivalentTo (contextsFromConfig));
          });
    }

    [Test]
    public void MixerToolGeneratesTypesForActiveMixinConfiguration ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.ScopedEmpty())
            {
              using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (BT1Mixin1)))
              {
                mixer.Execute();
              }
            }
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            Assert.AreEqual (2, theAssembly.GetTypes().Length); // one for the base type, one for the base call proxy
          });
    }

    [Test]
    public void MixerToolIgnoresGenericTypes ()
    {
      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);
            using (MixinConfiguration.ScopedEmpty ())
            {
              using (MixinConfiguration.ScopedExtend (typeof (List<>), typeof (NullMixin)))
              {
                mixer.Execute ();
              }
            }
          });
      Assert.IsFalse (File.Exists (SignedAssemblyPath));
    }

    [Test]
    public void MixerToolGeneratesMixedTypes ()
    {
      AppDomainRunner.Run (
          delegate
          {
            Mixer mixer = new Mixer (Parameters.SignedAssemblyName, Parameters.UnsignedAssemblyName, Parameters.AssemblyOutputDirectory);

            using (MixinConfiguration.ScopedEmpty())
            {
              using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (BT1Mixin1)))
              {
                mixer.Execute();
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            Assert.AreEqual (2, theAssembly.GetTypes().Length);
            Type generatedType = theAssembly.GetTypes()[0];
            if (!generatedType.IsDefined (typeof (ConcreteMixedTypeAttribute), false))
              generatedType = theAssembly.GetTypes ()[1];

            Assert.IsNotNull (Mixin.GetMixinConfigurationFromConcreteType (generatedType));
            Assert.AreEqual (
                MixinConfiguration.ActiveContext.GetClassContext (typeof (BaseType1)),
                Mixin.GetMixinConfigurationFromConcreteType (generatedType));

            object instance = Activator.CreateInstance (generatedType);
            Assert.IsTrue (generatedType.IsInstanceOfType (instance));
            Assert.IsNotNull (Mixin.Get<BT1Mixin1> (instance));
          }
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
            using (MixinConfiguration.ScopedEmpty())
            {
              using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (BT1Mixin1)))
              {
                mixer.Execute();
              }
            }
          });

      AppDomainRunner.Run (
          delegate
          {
            Assembly theAssembly = Assembly.LoadFile (UnsignedAssemblyPath);
            ConcreteTypeBuilder.Current.LoadScopeIntoCache (theAssembly);
            using (MixinConfiguration.ScopedEmpty())
            {
              using (MixinConfiguration.ScopedExtend (typeof (BaseType1), typeof (BT1Mixin1)))
              {
                Type generatedType = TypeFactory.GetConcreteType (typeof (BaseType1));
                Assert.Contains (generatedType, theAssembly.GetTypes());
              }
            }
          });
    }
  }
}