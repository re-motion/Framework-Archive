using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{

  public static class AbstractRoles
  {
    public static readonly EnumValueInfo Clerk = new EnumValueInfo ("Clerk", 0, "Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo Secretary = new EnumValueInfo ("Secretary", 1, "Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo Administrator = new EnumValueInfo ("Administrator", 0, "Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRole, Rubicon.Security.UnitTests.TestDomain");
  }
}