using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{

  public static class AbstractRoles
  {
    public static readonly EnumValueInfo Clerk = new EnumValueInfo (0, "Clerk");
    public static readonly EnumValueInfo Secretary = new EnumValueInfo (1, "Secretary");
    public static readonly EnumValueInfo Administrator = new EnumValueInfo (0, "Administrator");
  }
}