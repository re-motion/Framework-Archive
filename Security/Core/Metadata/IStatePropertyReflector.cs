using System;
using System.Reflection;

namespace Rubicon.Security.Metadata
{

  public interface IStatePropertyReflector
  {
    StatePropertyInfo GetMetadata (PropertyInfo property, MetadataCache cache);
  }

}