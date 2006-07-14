using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security
{
  public class FunctionalSecurityContextFactory : ISecurityContextFactory
  {
    // types

    // static members

    // member fields

    private SecurityContext _context;

    // construction and disposing

    public FunctionalSecurityContextFactory (SecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("context", context);
      _context = context;
    }

    public FunctionalSecurityContextFactory (Type classType)
    {
      _context = new SecurityContext (classType);
    }
    
    // methods and properties

    public SecurityContext GetSecurityContext ()
    {
      return _context;
    }
  }
}