using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Rubicon.Data.DomainObjects.Design;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  [DesignModeMappingLoader (typeof (DesignModeMappingReflector))]
  public class MappingReflector : MappingReflectorBase
  {
    private readonly Assembly[] _rootAssemblies;

    //TODO: Test
    public MappingReflector ()
    {
      _rootAssemblies = Array.FindAll (
          AppDomain.CurrentDomain.GetAssemblies (),
          delegate (Assembly assembly) { return assembly.IsDefined (typeof (MappingAssemblyAttribute), false); });
    }

    public MappingReflector (params Assembly[] rootAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("rootAssemblies", rootAssemblies);

      _rootAssemblies = rootAssemblies;
    }

    protected override Type[] GetDomainObjectTypes ()
    {
      List<Assembly> assemblies = new List<Assembly>();
      foreach (Assembly assembly in _rootAssemblies)
        assemblies.AddRange (FindAssemblies (assembly));

      List<Type> domainObjectClasses = new List<Type>();
      foreach (Assembly assembly in assemblies)
      {
        foreach (Type type in assembly.GetTypes())
        {
          if (typeof (DomainObject).IsAssignableFrom (type) && !domainObjectClasses.Contains (type))
            domainObjectClasses.Add (type);
        }
      }

      return domainObjectClasses.ToArray();
    }

    private List<Assembly> FindAssemblies (Assembly assembly)
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
  }
}