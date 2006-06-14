using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{

  public static class AccessTypes
  {
    public static readonly EnumValueInfo Read = new EnumValueInfo ("Read", 0, "Security.GeneralAccessType, Rubicon.Core");
    public static readonly EnumValueInfo Write = new EnumValueInfo ("Write", 1, "Security.Security.GeneralAccessType, Rubicon.Core");
    public static readonly EnumValueInfo Journalize = new EnumValueInfo ("Journalize", 0, "Rubicon.Security.UnitTests.TestDomain.DomainAccessType, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo Archive = new EnumValueInfo ("Archive", 1, "Rubicon.Security.UnitTests.TestDomain.DomainAccessType, Rubicon.Security.UnitTests.TestDomain");
  }
}