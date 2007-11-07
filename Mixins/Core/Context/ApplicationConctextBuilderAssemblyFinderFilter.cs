using System.Reflection;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Reflection;

namespace Rubicon.Mixins.Context
{
  public class ApplicationConctextBuilderAssemblyFinderFilter : IAssemblyFinderFilter
  {
    private readonly ApplicationAssemblyFinderFilter _assemblyFinderFilter;

    public ApplicationConctextBuilderAssemblyFinderFilter ()
    {
      // Note: ApplicationAssemblyFinderFilter automatically excludes the generated assemblies because they have the NonApplicationAssemblyAttribute
      // applied to them. No additional logic is required.
      _assemblyFinderFilter = ApplicationAssemblyFinderFilter.Instance;
    }

    public bool ShouldConsiderAssembly (AssemblyName assemblyName)
    {
      return _assemblyFinderFilter.ShouldConsiderAssembly (assemblyName);
    }

    public bool ShouldIncludeAssembly (Assembly assembly)
    {
      return _assemblyFinderFilter.ShouldIncludeAssembly (assembly);
    }
  }
}