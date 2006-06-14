using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{

  public static class AbstractRoles
  {
    public static readonly EnumValueInfo Clerk = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain", "Clerk", 0);
    public static readonly EnumValueInfo Secretary = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.DomainAbstractRole, Rubicon.Security.UnitTests.TestDomain", "Secretary", 1);
    public static readonly EnumValueInfo Administrator = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRole, Rubicon.Security.UnitTests.TestDomain", "Administrator", 0);
  }
}