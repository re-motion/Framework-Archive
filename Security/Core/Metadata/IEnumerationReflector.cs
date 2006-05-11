using System;
using System.Collections.Generic;
using System.Reflection;

namespace Rubicon.Security.Metadata
{

  public interface IEnumerationReflector
  {
    List<EnumValueInfo> GetValues (Type type);
  }

}