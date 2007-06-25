using System;
using System.Diagnostics;
using System.IO;
using Mixins.CodeGeneration;
using NUnit.Framework;
using Mixins.Definitions;
using Mixins.Context;
using System.Reflection;
using Mixins.Definitions.Building;

namespace Mixins.UnitTests.Mixins
{
  public abstract class MixinTestBase
  {
    public const string PEVerifyPath = @"C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\PEVerify.exe";

    private IDisposable currentConfiguration = null;

    [SetUp]
    public virtual void SetUp()
    {
      ConcreteTypeBuilder.SetCurrent (null);
      currentConfiguration = MixinConfiguration.ScopedExtend (Assembly.GetExecutingAssembly ());
    }

    [TearDown]
    public virtual void TearDown()
    {
      if (currentConfiguration != null)
        currentConfiguration.Dispose();

      string path;
      try
      {
        path = ConcreteTypeBuilder.Current.Scope.SaveAssembly ();
      }
      catch (Exception ex)
      {
        Assert.Fail ("Error when saving assembly: {0}", ex);
        return;
      }

      ConcreteTypeBuilder.SetCurrent (null);

      if (path != null || !File.Exists(path))
      {
        VerifyPEFile (path);
      }
      else
      {
        Console.WriteLine ("Assembly file was not saved.");
      }
    }

    public Type CreateMixedType (Type targetType, params Type[] mixinTypes)
    {
      using (MixinConfiguration.ScopedExtend(targetType, mixinTypes))
        return TypeFactory.GetConcreteType (targetType);
    }

    public InvokeWithWrapper<T> CreateMixedObject<T> (params Type[] mixinTypes)
    {
      using (MixinConfiguration.ScopedExtend(typeof (T), mixinTypes))
        return ObjectFactory.Create<T>();
    }

    private void VerifyPEFile (string assemblyPath)
    {
      Process process = new Process ();

      string verifierPath = PEVerifyPath;
      if (!File.Exists (verifierPath))
      {
        Assert.Ignore("Warning: PEVerify could not be found (search path: {0}).", verifierPath);
      }
      else
      {
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.FileName = verifierPath;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory;
        process.StartInfo.Arguments = assemblyPath;
        process.Start ();
        process.WaitForExit ();

        string result = string.Format ("PEVerify returned {0}\n{1}", process.ExitCode, process.StandardOutput.ReadToEnd ());
        Console.WriteLine ("PEVerify: " + process.ExitCode);
        if (process.ExitCode != 0)
        {
          Assert.Fail (result);
        }
      }
    }
  }
}
