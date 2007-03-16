using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  public class FakeMappingLoader: IMappingLoader
  {
    public FakeMappingLoader()
    {
    }

    public ClassDefinitionCollection GetClassDefinitions()
    {
      return new ClassDefinitionCollection();
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinitionCollection();
    }

    public bool ResolveTypes
    {
      get { return false; }
    }
  }
}