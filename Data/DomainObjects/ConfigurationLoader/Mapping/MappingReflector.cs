using System;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.Mapping
{
  public class MappingReflector: IMappingLoader
  {
    // constants

    // types

    // static members

    // member fields

    // construction and disposing

    public MappingReflector()
    {
    }

    // methods and properties
    
    public ClassDefinitionCollection GetClassDefinitions()
    {
      throw new NotImplementedException();
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      throw new NotImplementedException();
    }

    public bool ResolveTypes
    {
      get { return true; }
    }
  }
}