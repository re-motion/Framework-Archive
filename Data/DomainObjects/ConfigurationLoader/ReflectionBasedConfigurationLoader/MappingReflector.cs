using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Rubicon.Data.DomainObjects.Design;
using Rubicon.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  [DesignModeMappingLoader (typeof (DesignModeMappingReflector))]
  public class MappingReflector : MappingReflectorBase
  {
    private readonly AssemblyFinder _assemblyFinder;

    //TODO: Test
    public MappingReflector ()
    {
      _assemblyFinder = new AssemblyFinder (new ApplicationAssemblyFinderFilter());
    }

    public MappingReflector (params Assembly[] rootAssemblies)
    {
      ArgumentUtility.CheckNotNullOrEmptyOrItemsNull ("rootAssemblies", rootAssemblies);

      _assemblyFinder = new AssemblyFinder (new ApplicationAssemblyFinderFilter (), rootAssemblies);
    }

    protected override Type[] GetDomainObjectTypes ()
    {
      List<Type> domainObjectClasses = new List<Type>();
      foreach (Assembly assembly in _assemblyFinder.FindAssemblies ())
      {
        foreach (Type type in assembly.GetTypes())
        {
          if (typeof (DomainObject).IsAssignableFrom (type) && !domainObjectClasses.Contains (type))
            domainObjectClasses.Add (type);
        }
      }

      return domainObjectClasses.ToArray();
    }
  }
}