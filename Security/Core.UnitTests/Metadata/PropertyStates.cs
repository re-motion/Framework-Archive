using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{
  public static class PropertyStates
  {
    public static readonly EnumValueInfo Normal = new EnumValueInfo (0, "Normal");
    public static readonly EnumValueInfo Confidential = new EnumValueInfo (1, "Confidential");
    public static readonly EnumValueInfo Private = new EnumValueInfo (2, "Private");
  }
}