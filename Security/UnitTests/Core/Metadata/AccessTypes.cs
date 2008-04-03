using System;
using Remotion.Security.Metadata;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  public static class AccessTypes
  {
    public static readonly EnumValueInfo Read = new EnumValueInfo ("Security.GeneralAccessTypes, Remotion.Core", "Read", 0);
    public static readonly EnumValueInfo Write = new EnumValueInfo ("Security.Security.GeneralAccessTypes, Remotion.Core", "Write", 1);
    public static readonly EnumValueInfo Journalize = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain", "Journalize", 0);
    public static readonly EnumValueInfo Archive = new EnumValueInfo ("Remotion.Security.UnitTests.TestDomain.DomainAccessTypes, Remotion.Security.UnitTests.TestDomain", "Archive", 1);
  }
}