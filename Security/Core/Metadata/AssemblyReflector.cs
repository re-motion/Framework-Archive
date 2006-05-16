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
    private IAbstractRoleReflector _abstractRoleReflector;
    
    // construction and disposing

    public AssemblyReflector () : this (new ClassReflector (), new AbstractRoleReflector ())
    {
    }

    public AssemblyReflector (IClassReflector classReflector, IAbstractRoleReflector abstractRoleReflector)
    {
      ArgumentUtility.CheckNotNull ("classReflector", classReflector);
      ArgumentUtility.CheckNotNull ("abstractRoleReflector", abstractRoleReflector);
      
      _classReflector = classReflector;
      _abstractRoleReflector = abstractRoleReflector;
    }

    // methods and properties

    public IClassReflector ClassReflector
    {
      get { return _classReflector; }
    }

    public IAbstractRoleReflector AbstractRoleReflector
    {
      get { return _abstractRoleReflector; }
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

      _abstractRoleReflector.GetAbstractRoles (assembly, cache);
    }
  }
}