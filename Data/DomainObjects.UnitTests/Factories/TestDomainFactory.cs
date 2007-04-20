using System;
using System.Reflection;

namespace Rubicon.Data.DomainObjects.UnitTests.Factories
{
  public static class TestDomainFactory
  {
    public static readonly Assembly ConfigurationMappingTestDomain = TestDomainCompiler.Compile (
        @"Configuration\Mapping\TestDomain",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomain.dll",
        TestDomainCompiler.CompileInSeparateAppDomain (
            @"Configuration\Mapping\ReferencedTestDomain",
            "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.ReferencedTestDomain.dll"));

    public static readonly Assembly ConfigurationMappingTestDomainEmpty = TestDomainCompiler.Compile (
        @"Configuration\Mapping\TestDomainEmpty",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainEmpty.dll");

    public static readonly Assembly ConfigurationMappingTestDomainWithErrors = TestDomainCompiler.Compile (
        @"Configuration\Mapping\TestDomainWithErrors",
        "Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping.TestDomainWithErrors.dll");
  }
}