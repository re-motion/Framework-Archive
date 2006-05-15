using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rubicon.Security.Metadata
{

  public interface IEnumerationReflector
  {
    Dictionary<Enum, EnumValueInfo> GetValues (Type type, MetadataCache cache);
  }

}