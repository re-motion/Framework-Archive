using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader
{
  public interface IMappingLoader
  {
    ClassDefinitionCollection GetClassDefinitions();

    RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions);

    bool ResolveTypes { get; }
  }
}