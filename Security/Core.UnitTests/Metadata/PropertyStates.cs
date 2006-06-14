using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{
  public static class PropertyStates
  {
    public static readonly EnumValueInfo FileStateNew = new EnumValueInfo ("New", 0, "Rubicon.Security.UnitTests.TestDomain.FileState, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo FileStateNormal = new EnumValueInfo ("Normal", 1, "Rubicon.Security.UnitTests.TestDomain.FileState, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo FileStateArchived = new EnumValueInfo ("Archived", 2, "Rubicon.Security.UnitTests.TestDomain.FileState, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo ConfidentialityNormal = new EnumValueInfo ("Normal", 0, "Rubicon.Security.UnitTests.TestDomain.Confidentiality, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo ConfidentialityConfidential = new EnumValueInfo ("Confidential", 1, "Rubicon.Security.UnitTests.TestDomain.Confidentiality, Rubicon.Security.UnitTests.TestDomain");
    public static readonly EnumValueInfo ConfidentialityPrivate = new EnumValueInfo ("Private", 2, "Rubicon.Security.UnitTests.TestDomain.Confidentiality, Rubicon.Security.UnitTests.TestDomain");
  }
}