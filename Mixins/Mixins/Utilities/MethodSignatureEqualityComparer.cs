using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Utilities
{
  public class MethodSignatureEqualityComparer : IEqualityComparer<MethodInfo>
  {
    public bool Equals (MethodInfo x, MethodInfo y)
    {
      throw new NotImplementedException ();
    }

    public int GetHashCode (MethodInfo obj)
    {
      throw new NotImplementedException ();
    }
  }
}
