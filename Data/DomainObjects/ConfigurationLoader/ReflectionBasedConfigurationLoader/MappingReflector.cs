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
      List<Assembly> assemblies = new List<Assembly> (AppDomain.CurrentDomain.GetAssemblies());
      LoadAssemblies(assemblies, Directory.GetFiles (AppDomain.CurrentDomain.BaseDirectory, "*.dll"));
      LoadAssemblies (assemblies, Directory.GetFiles (AppDomain.CurrentDomain.BaseDirectory, "*.exe"));

      _rootAssemblies =
          assemblies.FindAll (delegate (Assembly assembly) { return assembly.IsDefined (typeof (MappingAssemblyAttribute), false); }).ToArray();
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

    private void LoadAssemblies (List<Assembly> assemblies, string[] paths)
    {
      foreach (string path in paths)
      {
        Assembly assembly = TryLoadAssembly (path);
        if (assembly != null && !assemblies.Contains (assembly))
          assemblies.Add (assembly);
      }
    }

    private Assembly TryLoadAssembly (string path)
    {
      try
      {
        return Assembly.LoadFile (path);
      }
      catch (BadImageFormatException)
      {
        return null;
      }
    }
  }
}