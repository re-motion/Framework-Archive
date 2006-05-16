using System;
using System.Reflection;

namespace Rubicon.Security.Metadata
{
  public interface IAbstractRoleReflector
  {
    System.Collections.Generic.List<EnumValueInfo> GetAbstractRoles (Assembly assembly, MetadataCache cache);
  }
}
