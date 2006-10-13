using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Utilities;

namespace Rubicon.Security.UnitTests.Configuration
{

  public class SecurityServiceMock : ISecurityService
  {
    // types

    // static members

    // member fields

    // construction and disposing

    public SecurityServiceMock ()
    {
    }

    // methods and properties

    public AccessType[] GetAccess (SecurityContext context, System.Security.Principal.IPrincipal user)
    {
      return new AccessType[0];
    }

    public int GetRevision ()
    {
      return 0;
    }
  }
}