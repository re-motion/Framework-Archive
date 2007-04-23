using System;
using System.Collections.Generic;
using System.Reflection;

namespace Mixins.Definitions.Building
{
  public class SpecialMethodSet
  {
    private Dictionary<MethodInfo, MethodInfo> methods = new Dictionary<MethodInfo, MethodInfo>();

    public void Add (MethodInfo method)
    {
      if (method != null && !Contains (method))
      {
        methods.Add (method, method);
      }
    }

    public bool Contains (MethodInfo method)
    {
      return methods.ContainsKey (method);
    }
  }
}
