using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using Rubicon.Utilities;
using Rubicon.Data;

namespace Rubicon.Security.Metadata
{

  public class AssemblyReflector
  {
    // types

    // static members

    // member fields

    private ITypeReflector _typeReflector;
    
    // construction and disposing

    public AssemblyReflector () : this (new TypeReflector ())
    {
    }

    public AssemblyReflector (ITypeReflector typeReflector)
    {
      ArgumentUtility.CheckNotNull ("typeReflector", typeReflector);
      _typeReflector = typeReflector;
    }

    // methods and properties

    public ITypeReflector TypeReflector
    {
      get { return _typeReflector; }
    }

    public void GetMetadata (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);

      foreach (Type type in assembly.GetTypes ())
      {
        if (typeof (ISecurableType).IsAssignableFrom (type))
          _typeReflector.GetMetadata (type, cache);
      }
    }
  }
}