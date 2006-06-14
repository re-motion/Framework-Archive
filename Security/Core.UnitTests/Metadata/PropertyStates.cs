using System;
using System.Collections.Generic;
using System.Text;

using Rubicon.Security.Metadata;

namespace Rubicon.Security.UnitTests.Metadata
{
  public static class PropertyStates
  {
    public static readonly EnumValueInfo FileStateNew = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.FileState, Rubicon.Security.UnitTests.TestDomain", "New", 0);
    public static readonly EnumValueInfo FileStateNormal = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.FileState, Rubicon.Security.UnitTests.TestDomain", "Normal", 1);
    public static readonly EnumValueInfo FileStateArchived = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.FileState, Rubicon.Security.UnitTests.TestDomain", "Archived", 2);
    public static readonly EnumValueInfo ConfidentialityNormal = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.Confidentiality, Rubicon.Security.UnitTests.TestDomain", "Normal", 0);
    public static readonly EnumValueInfo ConfidentialityConfidential = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.Confidentiality, Rubicon.Security.UnitTests.TestDomain", "Confidential", 1);
    public static readonly EnumValueInfo ConfidentialityPrivate = new EnumValueInfo ("Rubicon.Security.UnitTests.TestDomain.Confidentiality, Rubicon.Security.UnitTests.TestDomain", "Private", 2);
  }
}