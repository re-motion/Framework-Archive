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

    private IClassReflector _classReflector;
    
    // construction and disposing

    public AssemblyReflector () : this (new ClassReflector ())
    {
    }

    public AssemblyReflector (IClassReflector classReflector)
    {
      ArgumentUtility.CheckNotNull ("classReflector", classReflector);
      _classReflector = classReflector;
    }

    // methods and properties

    public IClassReflector ClassReflector
    {
      get { return _classReflector; }
    }

    public void GetMetadata (Assembly assembly, MetadataCache cache)
    {
      ArgumentUtility.CheckNotNull ("assembly", assembly);
      ArgumentUtility.CheckNotNull ("cache", cache);

      foreach (Type type in assembly.GetTypes ())
      {
        if (typeof (ISecurableType).IsAssignableFrom (type))
          _classReflector.GetMetadata (type, cache);
      }
    }
  }
}