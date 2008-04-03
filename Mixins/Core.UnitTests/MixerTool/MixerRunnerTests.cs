using System;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Mixins.MixerTool;
using System.IO;
using System.Reflection;
using Remotion.Mixins.UnitTests.SampleTypes;

namespace Remotion.Mixins.UnitTests.MixerTool
{
  [TestFixture]
  [Serializable]
  public class MixerRunnerTests : MixerToolBaseTests
  {
    [Test]
    public void ParameterDefaults ()
    {
      Assert.AreEqual (Environment.CurrentDirectory, Parameters.AssemblyOutputDirectory);
      Assert.AreEqual (Environment.CurrentDirectory, Parameters.BaseDirectory);
      Assert.AreEqual ("", Parameters.ConfigFile);
      Assert.AreEqual ("Remotion.Mixins.Persistent.Signed", Parameters.SignedAssemblyName);
      Assert.AreEqual ("Remotion.Mixins.Persistent.Unsigned", Parameters.UnsignedAssemblyName);
      Assert.AreEqual (false, Parameters.KeepTypeNames);
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
      string localGeneratedPath = Path.Combine (Environment.CurrentDirectory, "Remotion.Mixins.Generated.Unsigned.dll");
      string remoteSampleTypesPath = Path.Combine (basePath, "SampleTypes.dll");

      if (Directory.Exists (basePath))
        Directory.Delete (basePath, true);

      Directory.CreateDirectory (basePath);

      if (File.Exists (localSampleTypesPath))
        File.Delete (localSampleTypesPath);

      if (File.Exists (localGeneratedPath))
        File.Delete (localGeneratedPath);

      File.Copy (typeof (ExtendsAttribute).Assembly.Location, Path.Combine (basePath, "Remotion.Mixins.dll"));

      AssemblyCompiler compiler = new AssemblyCompiler ("MixerTool\\SampleAssembly", remoteSampleTypesPath,
          Path.Combine (basePath, "Remotion.Mixins.dll"));
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
            Type generatedType = GetFirstMixedType (assembly);
            Assert.AreEqual ("BaseType", generatedType.BaseType.Name);
          },  localGeneratedPath);

      File.Delete (localSampleTypesPath);
      File.Delete (localGeneratedPath);
    }

    [Test]
    public void RunWithKeepTypeNames ()
    {
      Parameters.KeepTypeNames = true;
      using (MixinConfiguration.BuildNew().EnterScope())
      {
        using (MixinConfiguration.BuildFromActive().ForClass<BaseType1> ().Clear().AddMixins (typeof (NullMixin)).EnterScope())
        {
          MixerRunner runner = new MixerRunner (Parameters);
          runner.Run();
        }
      }

      AppDomainRunner.Run (
          delegate
          {
            Assembly assembly = Assembly.LoadFile (UnsignedAssemblyPath);
            Type generatedType = GetFirstMixedType (assembly);
            Assert.IsTrue (generatedType.Namespace.EndsWith ("MixedTypes"));
          });
    }
  }
}