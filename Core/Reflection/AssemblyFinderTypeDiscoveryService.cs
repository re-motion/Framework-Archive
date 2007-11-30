using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using Rubicon.Utilities;

namespace Rubicon.Reflection
{
  public class AssemblyFinderTypeDiscoveryService : ITypeDiscoveryService
  {
    private readonly AssemblyFinder _assemblyFinder;

    private Assembly[] _assemblyCache;

    public AssemblyFinderTypeDiscoveryService (AssemblyFinder assemblyFinder)
    {
      ArgumentUtility.CheckNotNull ("assemblyFinder", assemblyFinder);
      _assemblyFinder = assemblyFinder;
    }

    public ICollection GetTypes (Type baseType, bool excludeGlobalTypes)
    {
      List<Type> types = new List<Type>();
      foreach (Assembly assembly in GetAssemblies (excludeGlobalTypes))
        types.AddRange (GetTypes (assembly, baseType));

      return types;
    }

    private IEnumerable<Type> GetTypes (Assembly assembly, Type baseType)
    {
      Type[] allTypesInAssembly = assembly.GetTypes ();
      if (baseType == null)
        return allTypesInAssembly;
      else
        return GetFilteredTypes (allTypesInAssembly, baseType);
    }

    private IEnumerable<Type> GetFilteredTypes (IEnumerable<Type> types, Type baseType)
    {
      foreach (Type type in types)
      {
        if (baseType.IsAssignableFrom (type))
          yield return type;
      }
    }

    private IEnumerable<Assembly> GetAssemblies (bool excludeGlobalTypes)
    {
      if (_assemblyCache == null)
        _assemblyCache = _assemblyFinder.FindAssemblies();

      foreach (Assembly assembly in _assemblyCache)
      {
        if (!excludeGlobalTypes || !assembly.GlobalAssemblyCache)
          yield return assembly;
      }
    }
  }
}