using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rubicon.Security
{
  public interface IPermissionReflector
  {
    Enum[] GetRequiredMethodPermissions (Type type, string methodName);
    Enum[] GetRequiredMethodPermissions (Type type, string methodName, Type[] parameterTypes);

    Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName);
    Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName, Type[] parameterTypes);

    Enum[] GetRequiredConstructorPermissions (Type type);
    Enum[] GetRequiredConstructorPermissions (Type type, Type[] parameterTypes);
  }
}
