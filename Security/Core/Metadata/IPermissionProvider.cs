using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Rubicon.Security.Metadata
{
  public interface IPermissionProvider
  {
    Enum[] GetRequiredMethodPermissions (Type type, string methodName);
    Enum[] GetRequiredStaticMethodPermissions (Type type, string methodName);

    Enum[] GetRequiredPropertyReadPermissions (Type type, string propertyName);
    Enum[] GetRequiredPropertyWritePermissions (Type type, string propertyName);
  }
}
