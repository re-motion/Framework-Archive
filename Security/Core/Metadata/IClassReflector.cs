using System;

namespace Rubicon.Security.Metadata
{
  public interface IClassReflector
  {
    SecurableClassInfo GetMetadata (Type type, MetadataCache cache);
  }
}
