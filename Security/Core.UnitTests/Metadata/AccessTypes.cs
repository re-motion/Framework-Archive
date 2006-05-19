using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{

  public static class AccessTypes
  {
    public static readonly EnumValueInfo Read = new EnumValueInfo (0, "Read");
    public static readonly EnumValueInfo Write = new EnumValueInfo (1, "Write");
    public static readonly EnumValueInfo Journalize = new EnumValueInfo (0, "Journalize");
    public static readonly EnumValueInfo Archive = new EnumValueInfo (1, "Archive");
  }
}