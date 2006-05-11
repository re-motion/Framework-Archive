using System;

namespace Rubicon.Security.Metadata
{
  public interface ITypeReflector
  {
    SecurableTypeInfo GetMetadata (Type type, MetadataCache cache);
  }
}
