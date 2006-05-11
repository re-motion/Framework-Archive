using System;

namespace Rubicon.Security.Metadata
{
  public interface IAccessTypeReflector
  {
    System.Collections.Generic.List<EnumValueInfo> GetAccessTypes (Type type, MetadataCache cache);
  }
}
