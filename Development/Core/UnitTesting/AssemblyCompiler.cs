using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using Rubicon.Utilities;

namespace Rubicon.Development.UnitTesting
{
  [Serializable]
  public class AssemblyCompiler
  {
    private readonly string _sourceDirectory;
    private readonly string _outputAssembly;
    private readonly string[] _referencedAssemblies;
    private Assembly _compiledAssembly;

    public AssemblyCompiler (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("sourceDirectory", sourceDirectory);
      ArgumentUtility.CheckNotNullOrEmpty ("outputAssembly", outputAssembly);
      ArgumentUtility.CheckNotNullOrItemsNull ("referencedAssemblies", referencedAssemblies);

      _sourceDirectory = sourceDirectory;
      _outputAssembly = outputAssembly;
      _referencedAssemblies = referencedAssemblies;
    }

    public Assembly CompiledAssembly
    {
      get { return _compiledAssembly; }
    }

    public string OutputAssembly
    {
      get { return _outputAssembly; }
    }

    public string SourceDirectory
    {
      get { return _sourceDirectory; }
    }

    public string[] ReferencedAssemblies
    {
      get { return _referencedAssemblies; }
    }

    public void Compile ()
    {
      _compiledAssembly = null;
      CodeDomProvider provider = new CSharpCodeProvider ();

      string[] sourceFiles = Directory.GetFiles (_sourceDirectory);

      CompilerParameters compilerParameters = new CompilerParameters ();
      compilerParameters.GenerateExecutable = false;
      compilerParameters.OutputAssembly = _outputAssembly;
      compilerParameters.GenerateInMemory = false;
      compilerParameters.TreatWarningsAsErrors = false;
      compilerParameters.ReferencedAssemblies.AddRange (_referencedAssemblies);

      CompilerResults compilerResults = provider.CompileAssemblyFromFile (compilerParameters, sourceFiles);

      if (compilerResults.Errors.Count > 0)
      {
        StringBuilder errorBuilder = new StringBuilder ();
        errorBuilder.AppendFormat ("Errors building {0} into {1}", _sourceDirectory, compilerResults.PathToAssembly).AppendLine ();
        foreach (CompilerError compilerError in compilerResults.Errors)
          errorBuilder.AppendFormat ("  ").AppendLine (compilerError.ToString ());

        throw new AssemblyCompilationException (errorBuilder.ToString ());
      }

      _compiledAssembly = compilerResults.CompiledAssembly;
    }

    public string CompileInSeparateAppDomain ()
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

        appDomain.DoCallBack (Compile);
      }
      finally
      {
        if (appDomain != null)
          AppDomain.Unload (appDomain);
      }

      return OutputAssembly;
    }
  }
}