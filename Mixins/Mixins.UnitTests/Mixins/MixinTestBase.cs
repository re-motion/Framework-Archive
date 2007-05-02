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
    protected ApplicationDefinition Configuration;
    protected TypeFactory TypeFactory;
    protected ObjectFactory ObjectFactory;

    public const string PEVerifyPath = @"C:\Program Files\Microsoft Visual Studio 8\SDK\v2.0\Bin\PEVerify.exe";

    [SetUp]
    public virtual void SetUp()
    {
      ConcreteTypeBuilder.Scope.SetCurrent (null);

      Configuration = CreateConfiguration();
      TypeFactory = new TypeFactory (Configuration);
      ObjectFactory = new ObjectFactory (TypeFactory);
    }

    protected virtual ApplicationDefinition CreateConfiguration()
    {
      ApplicationContext context = DefaultContextBuilder.BuildContextFromAssembly (Assembly.GetExecutingAssembly());
      return DefinitionBuilder.CreateApplicationDefinition (context);
    }

    [TearDown]
    public virtual void TearDown()
    {
      string path;
      try
      {
        path = ConcreteTypeBuilder.Scope.Current.SaveAssembly ();
      }
      catch (Exception ex)
      {
        Assert.Fail ("Error when saving assembly: {0}", ex);
        return;
      }

      Configuration = null;
      TypeFactory = null;
      ObjectFactory = null;
      ConcreteTypeBuilder.Scope.SetCurrent (null);

      if (path != null || !File.Exists(path))
      {
        VerifyPEFile (path);
      }
      else
      {
        Console.WriteLine ("Assembly file was not saved.");
      }
    }

    public ObjectFactory NewObjectFactory(ApplicationDefinition configuration)
    {
      return new ObjectFactory (NewTypeFactory (configuration));
    }

    public TypeFactory NewTypeFactory (ApplicationDefinition configuration)
    {
      return new TypeFactory (configuration);
    }

    public Type CreateMixedType (Type targetType, params Type[] mixinTypes)
    {
      return NewTypeFactory (DefBuilder.Build (targetType, mixinTypes)).GetConcreteType (targetType);
    }

    public InvokeWithWrapper<T> CreateMixedObject<T> (params Type[] mixinTypes)
    {
      return NewObjectFactory (DefBuilder.Build (typeof (T), mixinTypes)).Create<T>();
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
