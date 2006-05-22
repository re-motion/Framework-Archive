using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Rubicon.Utilities;

namespace Rubicon.Security.Metadata
{
  public class MetadataExtractor
  {
    private IMetadataConverter _converter;
    private List<Assembly> _assemblies;

    public MetadataExtractor (IMetadataConverter converter)
    {
      ArgumentUtility.CheckNotNull ("converter", converter);

      _assemblies = new List<Assembly> ();
      _converter = converter;
    }

    public void AddAssembly (Assembly assembly)
    {
      _assemblies.Add (assembly);
    }

    public void AddAssembly (string assemblyPath)
    {
      if (!assemblyPath.EndsWith (".dll"))
        assemblyPath = assemblyPath + ".dll";

      Assembly assembly = Assembly.LoadFrom (assemblyPath);
      AddAssembly (assembly);
    }

    public void Save (string filename)
    {
      MetadataCache metadata = new MetadataCache ();
      AssemblyReflector reflector = new AssemblyReflector ();

      foreach (Assembly assembly in _assemblies)
        reflector.GetMetadata (assembly, metadata);

      _converter.ConvertAndSave (metadata, filename);
    }
  }
}
