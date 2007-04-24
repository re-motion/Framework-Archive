using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class MappingReflectorBase: IMappingLoader
  {
    protected abstract ICollection GetDomainObjectClasses();

    public ClassDefinitionCollection GetClassDefinitions()
    {
      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectors())
        classReflector.GetClassDefinition (classDefinitions);

      return classDefinitions;
    }

    public RelationDefinitionCollection GetRelationDefinitions (ClassDefinitionCollection classDefinitions)
    {
      RelationDefinitionCollection relationDefinitions = new RelationDefinitionCollection();
      foreach (ClassReflector classReflector in CreateClassReflectors())
      {
        foreach (RelationDefinition relationDefinition in classReflector.GetRelationDefinitions (classDefinitions))
          relationDefinitions.Add (relationDefinition);
      }

      return relationDefinitions;
    }

    private List<ClassReflector> CreateClassReflectors()
    {
      List<ClassReflector> classReflectors = new List<ClassReflector>();
      foreach (Type domainObjectClass in GetDomainObjectClasses())
        classReflectors.Add (ClassReflector.CreateClassReflector (domainObjectClass));
      
      return classReflectors;
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }
  }
}