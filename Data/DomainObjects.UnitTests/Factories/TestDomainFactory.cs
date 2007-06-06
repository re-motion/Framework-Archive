using System;
using System.Reflection;
using Rubicon.Development.UnitTesting;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomainSimple = Compile (
        @"Configuration\Mapping\TestDomain\Simple",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Simple.dll");

    public static readonly Assembly ConfigurationMappingTestDomainEmpty = Compile (
        @"Configuration\Mapping\TestDomain\Empty",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Empty.dll");

    public static readonly Assembly ConfigurationMappingTestDomainErrors = Compile (
        @"Configuration\Mapping\TestDomain\Errors",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.Errors.dll");

    private static Assembly Compile (string sourceDirectory, string outputAssembly, params string[] referencedAssemblies)
    {
      AssemblyCompiler compiler = new AssemblyCompiler (
          sourceDirectory,
          outputAssembly,
          ArrayUtility.Combine (new string[] { "Rubicon.Core.dll", "Rubicon.Data.DomainObjects.dll" }, referencedAssemblies));

      compiler.Compile ();
      return compiler.CompiledAssembly;
    }
  }
}