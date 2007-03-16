using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomain = Compile (
        @"Configuration\Mapping\TestDomain",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.dll");

    private static Assembly Compile (string directory, string outputAssembly)
    {
      CodeDomProvider provider = new CSharpCodeProvider ();

      string[] sourceFiles = Directory.GetFiles (directory);

      CompilerParameters cp = new CompilerParameters ();
      cp.GenerateExecutable = false;
      cp.OutputAssembly = outputAssembly;
      cp.GenerateInMemory = false;
      cp.TreatWarningsAsErrors = false;
      cp.ReferencedAssemblies.Add ("Rubicon.Core.dll");
      cp.ReferencedAssemblies.Add ("Rubicon.Data.DomainObjects.dll");

      CompilerResults compilerResults = provider.CompileAssemblyFromFile (cp, sourceFiles);

      if (compilerResults.Errors.Count > 0)
      {
        StringBuilder errorBuilder = new StringBuilder ();
        errorBuilder.AppendFormat ("Errors building {0} into {1}", directory, compilerResults.PathToAssembly).AppendLine ();
        foreach (CompilerError compilerError in compilerResults.Errors)
          errorBuilder.AppendFormat ("  ").AppendLine (compilerError.ToString ());
        Assert.Fail (errorBuilder.ToString ());
      }

      return compilerResults.CompiledAssembly;
    }
  }
}