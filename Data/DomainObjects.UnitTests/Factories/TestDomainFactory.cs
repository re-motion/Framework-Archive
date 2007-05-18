using System;
using System.Reflection;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomain = Compile (
        @"Configuration\Mapping\TestDomain",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.dll",
        CompileInSeparateAppDomain (
            @"Configuration\Mapping\ReferencedTestDomain",
            "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain.dll"));

    public static readonly Assembly ConfigurationMappingTestDomainEmpty = Compile (
        @"Configuration\Mapping\TestDomainEmpty",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainEmpty.dll");

    public static readonly Assembly ConfigurationMappingTestDomainWithErrors = Compile (
        @"Configuration\Mapping\TestDomainWithErrors",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.dll");

    private static Assembly Compile (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      AssemblyCompiler compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssembly,
          ArrayUtility.Combine (new string[] { "Rubicon.Core.dll", "Rubicon.Data.DomainObjects.dll" }, referencedAssemblies));

      compiler.Compile ();
      return compiler.CompiledAssembly;
    }

    private static string CompileInSeparateAppDomain (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      AssemblyCompiler compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssembly,
          ArrayUtility.Combine (new string[] { "Rubicon.Core.dll", "Rubicon.Data.DomainObjects.dll" }, referencedAssemblies));

      compiler.CompileInSeparateAppDomain ();
      return compiler.OutputAssembly;
    }

  }
}