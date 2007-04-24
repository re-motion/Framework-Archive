using System;
using System.ComponentModel;
using Rubicon.Data.DomainObjects.ConfigurationLoader;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Data.DomainObjects.UnitTests.TestDomain;

namespace Rubicon.Data.DomainObjects.UnitTests.Design
{
  public class FakeDesignModeMappingLoader : IMappingLoader
  {
    private readonly ISite _site;

    public FakeDesignModeMappingLoader (ISite site)
    {
      _site = site;
    }

    public ISite Site
    {
      get { return _site; }
    }

    public ClassDefinitionCollection GetClassDefinitions()
    {
      ClassDefinitionCollection classDefinitionCollection = new ClassDefinitionCollection();
      classDefinitionCollection.Add (
          new ReflectionBasedClassDefinition ("Fake", "Fake", "Fake", typeof (Company), false));

      return classDefinitionCollection;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      return new RelationDefinitionCollection();
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }
  }
}