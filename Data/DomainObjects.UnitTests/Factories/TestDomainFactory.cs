using System;
using System.Reflection;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;
using System.IO;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomainSimple = Compile (
        @"Configuration\Mapping\TestDomain\Simple",
        @"Configuration.Dlls\Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Simple.dll");

    public static readonly Assembly ConfigurationMappingTestDomainEmpty = Compile (
        @"Configuration\Mapping\TestDomain\Empty",
        @"Configuration.Dlls\Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Empty.dll");

    public static readonly Assembly ConfigurationMappingTestDomainErrors = Compile (
        @"Configuration\Mapping\TestDomain\Errors",
        @"Configuration.Dlls\Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.dll");

    private static Assembly Compile (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      string outputAssemblyDirectory = Path.GetDirectoryName (Path.GetFullPath (outputAssembly));
      if (!Directory.Exists (outputAssemblyDirectory))
        Directory.CreateDirectory (outputAssemblyDirectory);
      
      AssemblyCompiler compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssembly,
          ArrayUtility.Combine (new string[] { "Rubicon.Core.dll", "Rubicon.Data.DomainObjects.dll", "Rubicon.Mixins.dll" }, referencedAssemblies));

      compiler.Compile ();
      return compiler.CompiledAssembly;
    }
  }
}