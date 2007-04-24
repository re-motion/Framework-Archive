using System;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Design;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.Design;

namespace Rubicon.Data.DomainObjects.UnitTests.Configuration.Mapping
{
  [DesignModeMappingLoader(typeof (FakeDesignModeMappingLoader))]
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