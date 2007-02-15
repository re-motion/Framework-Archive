using System;
using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{

  public static class AccessTypes
  {
    public static readonly EnumValueInfo Read = new EnumValueInfo ("Security.GeneralAccessTypes, Rubicon.Core", "Read", 0);
    public static readonly EnumValueInfo Write = new EnumValueInfo ("Security.Security.GeneralAccessTypes, Rubicon.Core", "Write", 1);
    public static readonly EnumValueInfo Journalize = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain", "Journalize", 0);
    public static readonly EnumValueInfo Archive = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.DomainAccessTypes, Rubicon.Security.UnitTests.TestDomain", "Archive", 1);
  }
}