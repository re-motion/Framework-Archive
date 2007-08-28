using System;
using NUnit.Framework;
using Rubicon.Development.UnitTesting;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Mixins.Context;
using Rubicon.Mixins.MixerTool;
using System.IO;
using System.Reflection;

namespace Rubicon.Mixins.UnitTests.MixerTool
{
  [Serializable]
  [TestFixture]
  public class MixerRunnerTest : MixerToolBaseTest
  {
    [Test]
    public void ParameterDefaults ()
    {
      Assert.AreEqual (Environment.CurrentDirectory, Parameters.AssemblyOutputDirectory);
      Assert.AreEqual (Environment.CurrentDirectory, Parameters.BaseDirectory);
      Assert.AreEqual ("", Parameters.ConfigFile);
      Assert.AreEqual (true, Parameters.Verbose);
      Assert.AreEqual ("Rubicon.Mixins.Generated.Signed", Parameters.SignedAssemblyName);
      Assert.AreEqual ("Rubicon.Mixins.Generated.Unsigned", Parameters.UnsignedAssemblyName);
    }

    [Test]
    public void RunDefault ()
    {
      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      MixerRunner runner = new MixerRunner (Parameters);
      runner.Run ();
      Assert.IsTrue (File.Exists (UnsignedAssemblyPath));
    }

    [Test]
    public void RunWithDifferentBaseDirectory ()
    {
      string basePath = Path.Combine (Environment.CurrentDirectory, "TempBasePath");
      string localSampleTypesPath = Path.Combine (Environment.CurrentDirectory, "SampleTypes.dll");
      string localGeneratedPath = Path.Combine (Environment.CurrentDirectory, "Rubicon.Mixins.Generated.Unsigned.dll");
      string remoteSampleTypesPath = Path.Combine (basePath, "SampleTypes.dll");

      if (Directory.Exists (basePath))
        Directory.Delete (basePath, true);

      Directory.CreateDirectory (basePath);

      if (File.Exists (localSampleTypesPath))
        File.Delete (localSampleTypesPath);

      if (File.Exists (localGeneratedPath))
        File.Delete (localGeneratedPath);

      File.Copy (typeof (ExtendsAttribute).Assembly.Location, Path.Combine (basePath, "Rubicon.Mixins.dll"));

      AssemblyCompiler compiler = new AssemblyCompiler ("MixerTool\\SampleAssembly", remoteSampleTypesPath,
          Path.Combine (basePath, "Rubicon.Mixins.dll"));
      compiler.CompileInSeparateAppDomain ();

      Parameters.BaseDirectory = basePath;

      Assert.IsFalse (File.Exists (UnsignedAssemblyPath));
      MixerRunner runner = new MixerRunner (Parameters);
      runner.Run ();
      Assert.IsTrue (File.Exists (UnsignedAssemblyPath));

      File.Move (remoteSampleTypesPath, localSampleTypesPath);
      File.Move (UnsignedAssemblyPath, localGeneratedPath);

      AppDomainRunner.Run (
          delegate (object[] args)
          {
            string path = (string) args[0];
            Assembly assembly = Assembly.LoadFile (path);
            Assert.AreEqual (2, assembly.GetTypes().Length); // concrete type + base call proxy
            Type generatedType = assembly.GetTypes ()[0];
            if (!generatedType.IsDefined (typeof (ConcreteMixedTypeAttribute), false))
              generatedType = assembly.GetTypes()[1];
            Assert.AreEqual ("BaseType", generatedType.BaseType.Name);
          },  localGeneratedPath);

      File.Delete (localSampleTypesPath);
      File.Delete (localGeneratedPath);
    }
  }
}