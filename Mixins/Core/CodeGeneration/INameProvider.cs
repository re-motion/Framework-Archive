using Rubicon.Mixins.Definitions;

namespace Rubicon.Mixins.CodeGeneration
{
  public interface INameProvider
  {
    string GetNewTypeName (ClassDefinition configuration);
  }
}