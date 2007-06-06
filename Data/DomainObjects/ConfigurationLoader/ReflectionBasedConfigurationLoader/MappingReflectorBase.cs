using System;
using System.Collections;
using System.Collections.Generic;
using Rubicon.Data.DomainObjects.Mapping;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class MappingReflectorBase: IMappingLoader
  {
    protected MappingReflectorBase()
    {
    }

    protected abstract Type[] GetDomainObjectTypes();

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
        classReflector.GetRelationDefinitions (classDefinitions, relationDefinitions);

      return relationDefinitions;
    }

    private List<ClassReflector> CreateClassReflectors()
    {
      List<ClassReflector> classReflectors = new List<ClassReflector>();
      foreach (Type domainObjectClass in GetDomainObjectTypesSorted ())
        classReflectors.Add (ClassReflector.CreateClassReflector (domainObjectClass));
      
      return classReflectors;
    }

    private Type[] GetDomainObjectTypesSorted ()
    {
      Type[] domainObjectTypes = GetDomainObjectTypes ();

      Array.Sort (
          domainObjectTypes,
          delegate (Type left, Type right) { return string.Compare (left.FullName, right.FullName, StringComparison.Ordinal); });

      return domainObjectTypes;
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }
  }
}