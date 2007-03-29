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
      List<Assembly> assemblies = new List<Assembly>();
      foreach (Assembly assembly in _rootAssemblies)
        assemblies.AddRange (FindAssemblies (assembly));

      List<Type> domainObjectClasses = new List<Type>();
      foreach (Assembly assembly in assemblies)
      {
        foreach (Type type in assembly.GetTypes ())
        {
          if (type.IsSubclassOf (typeof (DomainObject)))
            domainObjectClasses.Add (type);
        }
      }

      ClassDefinitionCollection classDefinitions = new ClassDefinitionCollection();
      List<RelationReflector> relations = new List<RelationReflector>();
      foreach (Type domainObjectClass in domainObjectClasses)
      {
        ClassReflector classReflector = new ClassReflector (domainObjectClass, classDefinitions, relations);
        classReflector.GetMetadata();
      }

      return classDefinitions;
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