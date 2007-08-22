using System.Reflection;
using Rubicon.Mixins.CodeGeneration;
using Rubicon.Reflection;

namespace Rubicon.Mixins.Context
{
  public class ApplicationConctextBuilderAssemblyFinderFilter : IAssemblyFinderFilter
  {
    private readonly RegexAssemblyFinderFilter _generatedAssemblyFinderFilter;
    private readonly NonSystemAssemblyFinderFilter _nonSystemAssemblyFinderFilter;

    public ApplicationConctextBuilderAssemblyFinderFilter ()
    {
      string matchExpression =
          string.Format (
              @"^({0})|({1})$", ConcreteTypeBuilder.Current.Scope.SignedAssemblyName, ConcreteTypeBuilder.Current.Scope.UnsignedAssemblyName);
      _generatedAssemblyFinderFilter = new RegexAssemblyFinderFilter (matchExpression, RegexAssemblyFinderFilter.MatchTargetKind.SimpleName);
      _nonSystemAssemblyFinderFilter = new NonSystemAssemblyFinderFilter ();
    }

    public bool ShouldConsiderAssembly (AssemblyName assemblyName)
    {
      return !_generatedAssemblyFinderFilter.ShouldConsiderAssembly (assemblyName)
             && _nonSystemAssemblyFinderFilter.ShouldConsiderAssembly (assemblyName);
    }

    public bool ShouldIncludeAssembly (Assembly assembly)
    {
      return _nonSystemAssemblyFinderFilter.ShouldIncludeAssembly (assembly);
    }
  }
}