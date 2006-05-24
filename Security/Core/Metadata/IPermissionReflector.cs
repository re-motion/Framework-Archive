using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rubicon.Security.Metadata
{
  public interface IPermissionReflector
  {
    Enum[] GetRequiredMethodPermissions (Type type, string methodName);
    Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName);
  }
}
