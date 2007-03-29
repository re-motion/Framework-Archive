using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  [Serializable]
  public class TestDomainCompiler
  {
    public static Assembly Compile (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      CodeDomProvider provider = new CSharpCodeProvider ();

      string[] sourceFiles = Directory.GetFiles (sourceDirectory);

      CompilerParameters compilerParameters = new CompilerParameters ();
      compilerParameters.GenerateExecutable = false;
      compilerParameters.OutputAssembly = outputAssembly;
      compilerParameters.GenerateInMemory = false;
      compilerParameters.TreatWarningsAsErrors = false;
      compilerParameters.ReferencedAssemblies.Add ("Rubicon.Core.dll");
      compilerParameters.ReferencedAssemblies.Add ("Rubicon.Data.DomainObjects.dll");
      compilerParameters.ReferencedAssemblies.AddRange (referencedAssemblies);

      CompilerResults compilerResults = provider.CompileAssemblyFromFile (compilerParameters, sourceFiles);

      if (compilerResults.Errors.Count > 0)
      {
        StringBuilder errorBuilder = new StringBuilder ();
        errorBuilder.AppendFormat ("Errors building {0} into {1}", sourceDirectory, compilerResults.PathToAssembly).AppendLine ();
        foreach (CompilerError compilerError in compilerResults.Errors)
          errorBuilder.AppendFormat ("  ").AppendLine (compilerError.ToString ());
        Assert.Fail (errorBuilder.ToString ());
      }

      return compilerResults.CompiledAssembly;
    }

    public static string CompileInSeparateAppDomain (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      AppDomain appDomain = null;
      try
      {
        appDomain = AppDomain.CreateDomain (
            "CompilerAppDomain",
            null,
            AppDomain.CurrentDomain.BaseDirectory,
            AppDomain.CurrentDomain.RelativeSearchPath,
            AppDomain.CurrentDomain.ShadowCopyFiles);

        TestDomainCompiler testDomainCompiler = new TestDomainCompiler (sourceDirectory, outputAssembly, referencedAssemblies);
        appDomain.DoCallBack (testDomainCompiler.Compile);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload (appDomain);
      }

      return outputAssembly;
    }

    private readonly string _sourceDirectory;
    private readonly string _outputAssembly;
    private readonly string[] _referencedAssemblies;

    private TestDomainCompiler (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      _sourceDirectory = sourceDirectory;
      _outputAssembly = outputAssembly;
      _referencedAssemblies = referencedAssemblies;
    }

    private void Compile ()
    {
      Compile (_sourceDirectory, _outputAssembly, _referencedAssemblies);
    }
  }
}