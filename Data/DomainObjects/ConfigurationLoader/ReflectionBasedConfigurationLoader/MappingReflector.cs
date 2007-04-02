using System;
using System.Collections.Generic;
using System.Reflection;
using Rubicon.Data.DomainObjects.Mapping;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public class MappingReflector: IMappingLoader
  {
    private readonly Assembly[] _rootAssemblies;

    public MappingReflector ()
    {
    }

    public MappingReflector (params Assembly[] rootAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("rootAssemblies", rootAssemblies);
      
      _rootAssemblies = rootAssemblies;
    }

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
      foreach (ClassReflector classReflector in CreateClassReflectors ())
      {
        foreach (RelationDefinition relationDefinition in classReflector.GetRelationDefinitions (classDefinitions))
          relationDefinitions.Add (relationDefinition);
      }

      return relationDefinitions;
    }

    public List<Assembly> FindAssemblies (Assembly assembly)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);

      List<Assembly> referencesAssemblies = new List<Assembly>();
      referencesAssemblies.Add (assembly);

      foreach (AssemblyName assemblyName in assembly.GetReferencedAssemblies())
      {
        Assembly referencedAssembly = Assembly.Load (assemblyName);
        if (Attribute.IsDefined (referencedAssembly, typeof (MappingAssemblyAttribute)))
          referencesAssemblies.AddRange (FindAssemblies (referencedAssembly));
      }

      return referencesAssemblies;
    }

    bool IMappingLoader.ResolveTypes
    {
      get { return true; }
    }

    private List<ClassReflector> CreateClassReflectors ()
    {
      List<Assembly> assemblies = new List<Assembly> ();
      foreach (Assembly assembly in _rootAssemblies)
        assemblies.AddRange (FindAssemblies (assembly));

      List<Type> domainObjectClasses = new List<Type> ();
      foreach (Assembly assembly in assemblies)
      {
        foreach (Type type in assembly.GetTypes ())
        {
          if (type.IsSubclassOf (typeof (DomainObject)))
            domainObjectClasses.Add (type);
        }
      }

      List<ClassReflector> classReflectors = new List<ClassReflector> ();
      foreach (Type domainObjectClass in domainObjectClasses)
        classReflectors.Add (ClassReflector.CreateClassReflector (domainObjectClass));
      return classReflectors;
    }
  }
}