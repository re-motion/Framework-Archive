using System;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Core.Metadata
{

  public static class AbstractRoles
  {
    public static readonly EnumValueInfo Clerk = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain", "Clerk", 0);
    public static readonly EnumValueInfo Secretary = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.DomainAbstractRoles, Rubicon.Security.UnitTests.TestDomain", "Secretary", 1);
    public static readonly EnumValueInfo Administrator = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.SpecialAbstractRoles, Rubicon.Security.UnitTests.TestDomain", "Administrator", 0);
  }
}